#ifndef UNICODE
#define UNICODE
#endif
#ifndef _UNICODE
#define _UNICODE
#endif

#define STRICT
#define WIN32_LEAN_AND_MEAN

#include "HookHelper.h"

#include <Windows.h>

#if _DEBUG
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#endif

#if _MSC_VER
#pragma warning(suppress: 28251)
#ifndef _iob_defined
#define _iob_defined
FILE _iob[] = { *stdin, *stdout, *stderr };
extern "C" FILE * __cdecl __iob_func(void) { return _iob; }
#endif
#endif

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, PWSTR lpCmdLine, int nShowCmd)
{
	return HookHelper::Run(hInstance, nShowCmd);
}
