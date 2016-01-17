// Win32DLL.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "Win32DLL.h"
#include "iostream"

using namespace std;

////////////////////////////////////////////////
////////////////////////////////////////////////

#include "stdafx.h"
#include <stdio.h>
#undef   UNICODE 
#include <Windows.h>
#include <TlHelp32.h>
#include <io.h>

#define INJECT_PROCESS_NAME    "explorer.exe" //目标进程
typedef WCHAR WPATH[MAX_PATH];
typedef DWORD64(WINAPI *PFNTCREATETHREADEX)
(
PHANDLE                 ThreadHandle,
ACCESS_MASK             DesiredAccess,
LPVOID                  ObjectAttributes,
HANDLE                  ProcessHandle,
LPTHREAD_START_ROUTINE  lpStartAddress,
LPVOID                  lpParameter,
BOOL                    CreateSuspended,
DWORD64                   dwStackSize,
DWORD64                   dw1,
DWORD64                   dw2,
LPVOID                  Unknown
);

//函数前置声明 
BOOL   CreateRemoteThreadLoadDll(LPCWSTR   lpwLibFile, DWORD64   dwProcessId);
BOOL   CreateRemoteThreadUnloadDll(LPCWSTR   lpwLibFile, DWORD64   dwProcessId);
HANDLE MyCreateRemoteThread(HANDLE hProcess, LPTHREAD_START_ROUTINE pThreadProc, LPVOID pRemoteBuf);

BOOL   EnableDebugPrivilege(VOID);
int   AddPrivilege(LPCWSTR   *Name);
void   GetWorkPath(TCHAR   szPath[], int   nSize);

//全局变量声明 
HANDLE   hProcessSnap = NULL;     //进程快照句柄 
DWORD64   dwRemoteProcessId;       //目标进程ID 

//--------------------------------------------------------------------- 
//注入函数，调用该函数即可
int injectDll()
{
	BOOL result = FALSE;
	//提升权限
	result = EnableDebugPrivilege();
	if (result != TRUE)
	{
		printf("add privilege failed!\n");
		return -1;
	}
	PROCESSENTRY32   pe32 = { 0 };
	//打开进程快照
	hProcessSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

	if (hProcessSnap == (HANDLE)-1)
	{
		return   -1;
	}

	pe32.dwSize = sizeof(PROCESSENTRY32);

	//获取目标进程的ID
	if (Process32First(hProcessSnap, &pe32))   //获取第一个进程 
	{
		do{
			char te[MAX_PATH];
			strcpy_s(te, pe32.szExeFile);
			if (strcmp(te, INJECT_PROCESS_NAME) == 0)
			{
				dwRemoteProcessId = pe32.th32ProcessID;
				printf("%d\n", dwRemoteProcessId);
				break;
			}
		} while (Process32Next(hProcessSnap, &pe32));//获取下一个进程 
	}
	else
	{
		return   -1;
	}


	WCHAR   wsz[MAX_PATH];
	swprintf(wsz, 100 ,L"%S ", "c:\\dll.dll"); //dll地址

	LPCWSTR   p = wsz;
	//在目标进程中创建线程并注入dll
	if (CreateRemoteThreadLoadDll(p, dwRemoteProcessId))
		return 1;
}

//--------------------------------------------------------------------- 
//在目标进程中创建线程并注入dll
BOOL   CreateRemoteThreadLoadDll(LPCWSTR   lpwLibFile, DWORD64   dwProcessId)
{
	BOOL   bRet = FALSE;
	HANDLE   hProcess = NULL, hThread = NULL;
	LPVOID pszLibRemoteFile = NULL;
	SIZE_T dwWritten = 0;
	__try
	{
		//1.打开进程，同时申请权限，这里申请了PROCESS_ALL_ACCESS
		hProcess = OpenProcess(PROCESS_ALL_ACCESS, TRUE, dwProcessId);
		if (hProcess == NULL)
			__leave;

		int   cch = 1 + lstrlenW(lpwLibFile);
		int   cb = cch   *   sizeof(WCHAR);
		printf("cb:%d\n", cb);
		printf("cb1:%d\n", sizeof(lpwLibFile));
		//2.申请足够的空间，以便把我们的dll写入目标进程中这个空间里
		pszLibRemoteFile = VirtualAllocEx(hProcess, NULL, cb, MEM_COMMIT, PAGE_READWRITE);

		if (pszLibRemoteFile == NULL)
			__leave;
		//3.正式把我们的dll写入上面申请的空间
		BOOL   bw = WriteProcessMemory(hProcess, pszLibRemoteFile, (PVOID)lpwLibFile, cb, &dwWritten);
		if (dwWritten != cb)
		{
			printf("write error!\n");
		}
		if (!bw)
			__leave;
		//4.获得关键函数LoadLibraryW地址
		PTHREAD_START_ROUTINE   pfnThreadRnt = (LPTHREAD_START_ROUTINE)GetProcAddress(GetModuleHandle(L"Kernel32"), "LoadLibraryW");
		if (pfnThreadRnt == NULL)
			__leave;
		//5.创建线程并把LoadLibraryW作为线程起始函数，传给LoadLibraryW的参数是我们的dll
		hThread = MyCreateRemoteThread(hProcess, pfnThreadRnt, pszLibRemoteFile);
		if (hThread == NULL)
			__leave;
		//6.等待上一步完成
		WaitForSingleObject(hThread, INFINITE);

		bRet = TRUE;
	}
	__finally
	{
		if (pszLibRemoteFile != NULL)
			VirtualFreeEx(hProcess, pszLibRemoteFile, 0, MEM_RELEASE);

		if (hThread != NULL)
			CloseHandle(hThread);

		if (hProcess != NULL)
			CloseHandle(hProcess);
	}

	return   bRet;
}
//--------------------------------------------------------------------- 
//卸载dll
BOOL   CreateRemoteThreadUnloadDll(LPCWSTR   lpwLibFile, DWORD64   dwProcessId)
{
	BOOL   bRet = FALSE;
	HANDLE   hProcess = NULL, hThread = NULL;
	HANDLE   hSnapshot = NULL;

	__try
	{
		hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, dwProcessId);
		if (hSnapshot == NULL)
			__leave;
		MODULEENTRY32W   me = { sizeof(MODULEENTRY32W) };
		BOOL   bFound = FALSE;
		BOOL   bMoreMods = Module32FirstW(hSnapshot, &me);
		for (; bMoreMods; bMoreMods = Module32NextW(hSnapshot, &me))
		{
			bFound = (lstrcmpiW(me.szModule, lpwLibFile) == 0) ||
				(lstrcmpiW(me.szExePath, lpwLibFile) == 0);
			if (bFound)
				break;
		}

		if (!bFound)
			__leave;

		hProcess = OpenProcess(
			PROCESS_CREATE_THREAD |
			PROCESS_VM_OPERATION,
			FALSE, dwProcessId);

		if (hProcess == NULL)
			__leave;

		PTHREAD_START_ROUTINE   pfnThreadRnt = (PTHREAD_START_ROUTINE)
			GetProcAddress(GetModuleHandle(TEXT("Kernel32 ")), "FreeLibrary ");
		if (pfnThreadRnt == NULL)
			__leave;

		//hThread   =   CreateRemoteThread(hProcess,   NULL,   0,pfnThreadRnt,   me.modBaseAddr,   0,   NULL); 
		hThread = MyCreateRemoteThread(hProcess, pfnThreadRnt, me.modBaseAddr);

		if (hThread == NULL)
			__leave;

		WaitForSingleObject(hThread, INFINITE);

		bRet = TRUE;
	}
	__finally
	{
		if (hSnapshot != NULL)
			CloseHandle(hSnapshot);

		if (hThread != NULL)
			CloseHandle(hThread);

		if (hProcess != NULL)
			CloseHandle(hProcess);
	}

	return   bRet;
}
//--------------------------------------------------------------------- 
//提升程序权限
BOOL   EnableDebugPrivilege()
{
	HANDLE   hToken;
	BOOL   fOk = FALSE;
	if (OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, &hToken))
	{
		TOKEN_PRIVILEGES   tp;
		tp.PrivilegeCount = 1;
		if (!LookupPrivilegeValue(NULL, SE_DEBUG_NAME, &tp.Privileges[0].Luid));
		tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
		if (!AdjustTokenPrivileges(hToken, FALSE, &tp, sizeof(tp), NULL, NULL));
		else
			fOk = TRUE;
		CloseHandle(hToken);
	}
	return   fOk;
}

//--------------------------------------------------------------------- 
//为当前进程增加指定的特权 
int   AddPrivilege(LPCWSTR Name)
{
	HANDLE   hToken;
	TOKEN_PRIVILEGES   tp;
	LUID   Luid;

	if (!OpenProcessToken(GetCurrentProcess(),
		TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,
		&hToken))
	{
#ifdef   _DEBUG 
		printf("OpenProcessToken   error.\n ");
#endif 
		return   1;
	}

	if (!LookupPrivilegeValue(NULL, Name, &Luid))
	{
#ifdef   _DEBUG 
		printf("LookupPrivilegeValue   error.\n ");
#endif 
		return   1;
	}

	tp.PrivilegeCount = 1;
	tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
	tp.Privileges[0].Luid = Luid;

	if (!AdjustTokenPrivileges(hToken,
		0,
		&tp,
		sizeof(TOKEN_PRIVILEGES),
		NULL,
		NULL))
	{
#ifdef   _DEBUG 
		printf("AdjustTokenPrivileges   error.\n ");
#endif 
		return   1;
	}

	return   0;
}
//--------------------------------------------------------------------- 
//操作系统版本判断
BOOL IsVistaOrLater()
{
	OSVERSIONINFO osvi;
	ZeroMemory(&osvi, sizeof(OSVERSIONINFO));
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	//GetVersionEx(&osvi);
	//if (osvi.dwMajorVersion >= 6)
		return TRUE;
	//return FALSE;
}
//--------------------------------------------------------------------- 
//OD跟踪，发现最后调用的是NtCreateThreadEx,所以这里手动调用
HANDLE MyCreateRemoteThread(HANDLE hProcess, LPTHREAD_START_ROUTINE pThreadProc, LPVOID pRemoteBuf)
{
	HANDLE      hThread = NULL;
	FARPROC     pFunc = NULL;
	if (IsVistaOrLater())    // Vista, 7, Server2008  
	{
		pFunc = GetProcAddress(GetModuleHandle(L"ntdll.dll"), "NtCreateThreadEx");
		if (pFunc == NULL)
		{
			printf("MyCreateRemoteThread() : GetProcAddress(\"NtCreateThreadEx\") 调用失败！错误代码: [%d]/n",
				GetLastError());
			return FALSE;
		}
		((PFNTCREATETHREADEX)pFunc)(
			&hThread,
			0x1FFFFF,
			NULL,
			hProcess,
			pThreadProc,
			pRemoteBuf,
			FALSE,
			NULL,
			NULL,
			NULL,
			NULL);
		if (hThread == NULL)
		{
			printf("MyCreateRemoteThread() : NtCreateThreadEx() 调用失败！错误代码: [%d]/n", GetLastError());
			return FALSE;
		}
	}
	else                    // 2000, XP, Server2003  
	{
		hThread = CreateRemoteThread(hProcess,
			NULL,
			0,
			pThreadProc,
			pRemoteBuf,
			0,
			NULL);
		if (hThread == NULL)
		{
			printf("MyCreateRemoteThread() : CreateRemoteThread() 调用失败！错误代码: [%d]/n", GetLastError());
			return FALSE;
		}
	}
	if (WAIT_FAILED == WaitForSingleObject(hThread, INFINITE))
	{
		printf("MyCreateRemoteThread() : WaitForSingleObject() 调用失败！错误代码: [%d]/n", GetLastError());
		return FALSE;
	}
	return hThread;
}


////////////////////////////////////////////////
////////////////////////////////////////////////

// 导入静态库  
#pragma comment(lib, "Comctl32.lib")
// 开启视觉效果 Copy from MSDN  
#pragma comment(linker, "\"/manifestdependency:type='win32' \
 name = 'Microsoft.Windows.Common-Controls' version = '6.0.0.0' \
 processorArchitecture = '*' publicKeyToken = '6595b64144ccf1df' language = '*'\"")

#define MAX_LOADSTRING 100
// Global Variables:
HINSTANCE hInst;								// current instance
TCHAR szTitle[MAX_LOADSTRING];					// The title bar text
TCHAR szWindowClass[MAX_LOADSTRING];			// the main window class name

// Forward declarations of functions included in this code module:
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPTSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

 	// TODO: Place code here.
	MSG msg;
	HACCEL hAccelTable;

	// Initialize global strings
	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadString(hInstance, IDC_WIN32DLL, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance);

	// Perform application initialization:
	if (!InitInstance (hInstance, nCmdShow))
	{
		return FALSE;
	}

	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_WIN32DLL));
	injectDll();
	// Main message loop:
	while (GetMessage(&msg, NULL, 0, 0))
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	return (int) msg.wParam;
}



//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
	WNDCLASSEX wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style			= CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc	= WndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hInstance;
	wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_WIN32DLL));
	wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
	wcex.lpszMenuName	= MAKEINTRESOURCE(IDC_WIN32DLL);
	wcex.lpszClassName	= szWindowClass;
	wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassEx(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   HWND hWnd;

   hInst = hInstance; // Store instance handle in our global variable

   hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   return TRUE;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the main window.
//
//  WM_COMMAND	- process the application menu
//  WM_PAINT	- Paint the main window
//  WM_DESTROY	- post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	int wmId, wmEvent;
	PAINTSTRUCT ps;
	HDC hdc;

	switch (message)
	{
	case WM_COMMAND:
		wmId    = LOWORD(wParam);
		wmEvent = HIWORD(wParam);
		// Parse the menu selections:
		switch (wmId)
		{
		case IDM_ABOUT:
			//injectDll();
			DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
			break;
		case IDM_EXIT:
			DestroyWindow(hWnd);
			break;
		default:
			return DefWindowProc(hWnd, message, wParam, lParam);
		}
		break;
	case WM_PAINT:
		hdc = BeginPaint(hWnd, &ps);
		// TODO: Add any drawing code here...
		EndPaint(hWnd, &ps);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(lParam);
	switch (message)
	{
	case WM_INITDIALOG:
		return (INT_PTR)TRUE;

	case WM_COMMAND:
		if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
		{
			EndDialog(hDlg, LOWORD(wParam));
			return (INT_PTR)TRUE;
		}
		break;
	}
	return (INT_PTR)FALSE;
}
