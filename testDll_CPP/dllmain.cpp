// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		::MessageBoxA(NULL, "Hi", "dll PROCESS ATTACHED", MB_ICONERROR);
	case DLL_THREAD_ATTACH:
		::MessageBoxA(NULL, "Hi", "dll THREAD ATTACHED", MB_ICONERROR);
	case DLL_THREAD_DETACH:
		::MessageBoxA(NULL, "Hi", "dll THREAD DETACHED", MB_ICONERROR);
	case DLL_PROCESS_DETACH:
		::MessageBoxA(NULL, "Hi", "dll PROCESS DETACHED", MB_ICONERROR);
		break;
	}
	return TRUE;
}

