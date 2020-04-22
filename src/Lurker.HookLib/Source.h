#pragma once

#define STRICT
#define WIN32_LEAN_AND_MEAN

#include <Windows.h>

#include <string>

struct HookMouseMessage
{
    DWORD messageCode;
    DWORD pointX;
    DWORD pointY;
    DWORD hwnd;
    DWORD hitTestCode;
};

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpReserved);

void Initialize(HINSTANCE hinst);

void LogDllMain(HINSTANCE hinst, std::wstring reason);

extern "C" __declspec(dllexport) LRESULT CALLBACK MouseHookProc(int code, WPARAM wParam, LPARAM lParam);