// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "fstream"
#include "iostream"
#include "time.h"
#include "windows.h"

using namespace std;

// 导入静态库  
#pragma comment(lib, "Comctl32.lib")
// 开启视觉效果 Copy from MSDN  
#pragma comment(linker, "\"/manifestdependency:type='win32' \
 name = 'Microsoft.Windows.Common-Controls' version = '6.0.0.0' \
 processorArchitecture = '*' publicKeyToken = '6595b64144ccf1df' language = '*'\"")

int   count1 = 0;

VOID CALLBACK TimerProc(HWND hwnd, UINT uMsg, UINT_PTR idEvent, DWORD dwTime)
{
	count1++;

	TCHAR szBuffer[1024];
	LPCTSTR str = TEXT("适配器的显示模式的个数: %i");
	wsprintf(szBuffer, str, count1);

	MessageBox(NULL, szBuffer, TEXT("适配器"), MB_OK);
}

DWORD CALLBACK   Thread(PVOID   pvoid)
{
	MSG  msg;
	PeekMessage(&msg, NULL, WM_USER, WM_USER, PM_NOREMOVE);
	UINT  timerid = SetTimer(NULL, 111, 1000, TimerProc);
	BOOL  bRet;

	while ((bRet = GetMessage(&msg, NULL, 0, 0)) != 0)
	{
		if (bRet == -1)
		{
			//   handle   the   error   and   possibly   exit   
		}
		else
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}
	KillTimer(NULL, timerid);
	MessageBox(NULL, TEXT("Thread end"), TEXT("适配器"), MB_OK);
	return   0;
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	const char filename[] = "d://dllLog.txt";
	ofstream o_file;

	SYSTEMTIME sys;
	GetLocalTime(&sys);

	DWORD   dwThreadId;
	MessageBox(NULL, TEXT("use   timer   in   workthread   of   console   application"), TEXT("适配器"), MB_OK);
	HANDLE   hThread;

	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		o_file.open(filename,ios::app);
		o_file << "[" << sys.wHour << ":" << sys.wMinute << ":" << sys.wSecond <<"]\r\n" << "DLL_PROCESS_ATTACH\r\n";
		o_file.close();
		::MessageBoxA(NULL, "dll PROCESS ATTACHED", "MESSAGE", MB_ICONERROR);
		hThread = CreateThread(NULL, 0, Thread, 0, 0, &dwThreadId);
		
	case DLL_THREAD_ATTACH:
		o_file.open(filename, ios::app);
		o_file << "DLL_THREAD_ATTACH\r\n";
		o_file.close();
		//::MessageBoxA(NULL, "dll THREAD ATTACHED", "MESSAGE", MB_ICONERROR);
	case DLL_THREAD_DETACH:
		o_file.open(filename, ios::app);
		o_file << "DLL_THREAD_DETACH\r\n";
		o_file.close();
		//::MessageBoxA(NULL, "dll THREAD DETACHED", "MESSAGE", MB_ICONERROR);
	case DLL_PROCESS_DETACH:
		o_file.open(filename, ios::app);
		o_file << "DLL_PROCESS_DETACH\r\n";
		o_file.close();
		//::MessageBoxA(NULL, "dll PROCESS DETACHED", "MESSAGE", MB_ICONERROR);
		break;
	}
	return TRUE;
}

