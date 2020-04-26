#pragma once

#define LOGHOOKHELPER 0
#if _DEBUG && LOGHOOKHELPER
#define LOGHOOKHELPERPATH TEXT("C:\\Temp\\HookHelper_")
#include "DebugHelper.h"
#endif

#include "MainWindowFinder.h"
#include "StreamLineWriter.h"

#include <Windows.h>
#include <shellapi.h>

#include <string>
#include <regex>

#if defined(HOOKHELPER64)
#define HOOKLIBNAME TEXT("Lurker.HookLib.x64.dll")
#else
#define HOOKLIBNAME TEXT("Lurker.HookLib.x86.dll")
#endif

class HookHelper
{
public:
    static INT Run(HINSTANCE hInstance, INT nShowCmd);
private:
    static constexpr int kPathBufferSize = 1024;
    HookHelper() { }
};

inline INT HookHelper::Run(HINSTANCE hInstance, INT nShowCmd)
{
#if _DEBUG && LOGHOOKHELPER
    TimestampLogger logger(LOGHOOKHELPERPATH + TimestampLogger::GetTimestampString(TRUE) + TEXT(".log"), TRUE);
#endif

    INT argscount;
    const auto args = CommandLineToArgvW(GetCommandLine(), &argscount);

    auto port = std::wstring(args[1]);
    auto processid = std::stoi(args[2]);
    auto mutexguid = std::wstring(args[3]);

    LocalFree(args);

    // Find process main window thread id

    MainWindowFinder mainwindowfinder(processid);
    auto mainwindowhandle = mainwindowfinder.FindMainWindow();
    auto threadid = GetWindowThreadProcessId(mainwindowhandle, NULL);

    // Get process full path

    auto process = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, FALSE, processid);
    TCHAR processfullpath[kPathBufferSize];
    DWORD processfullpathsize = sizeof(processfullpath);
    QueryFullProcessImageName(process, 0, processfullpath, &processfullpathsize);
    CloseHandle(process);

    // Determine dll path - dll is in same folder as self

    TCHAR modulepath[kPathBufferSize];
    GetModuleFileName(NULL, modulepath, kPathBufferSize);
    auto modulepathtmp = std::wstring(modulepath);
    auto modulefolder = modulepathtmp.substr(0, modulepathtmp.find_last_of(TEXT("\\")) + 1);
    TCHAR hooklibpath[kPathBufferSize];
    swprintf(hooklibpath, sizeof(hooklibpath), TEXT("%ls%ls"),
        modulefolder.c_str(), HOOKLIBNAME);

    // Load dll

    auto hooklib = LoadLibrary(hooklibpath);

    // Build configuration file path

    TCHAR temppath[kPathBufferSize];
    GetTempPath(kPathBufferSize, temppath);
    TCHAR configfilepath[kPathBufferSize];
    swprintf(configfilepath, sizeof(configfilepath), TEXT("%ls%ls%lX%d%d"),
        temppath,
        std::regex_replace(processfullpath, std::wregex(TEXT("[\\\\]|[/]|[:]|[ ]")), TEXT("")).c_str(),
        PtrToInt(hooklib),
        processid,
        threadid);

    // Write configuration file

    StreamLineWriter configfile(configfilepath, false);
    configfile.WriteLine(port);
    configfile.Close();
    
    // Setup hook

    auto hookproc = (HOOKPROC)GetProcAddress(hooklib, "MouseHookProc");
    auto mousehook = SetWindowsHookEx(WH_MOUSE, hookproc, hooklib, threadid);

    // Wait on host mutex

    TCHAR mutexname[256];
    swprintf(mutexname, sizeof(mutexname), TEXT("Global\\%ls"), mutexguid.c_str());
    auto mutex = OpenMutex(SYNCHRONIZE, FALSE, mutexname);
    WaitForSingleObject(mutex, INFINITE);
    CloseHandle(mutex);

    // Unhook

    if (mousehook != NULL)
    {
        UnhookWindowsHookEx(mousehook);
    }

    if (hooklib != NULL)
    {
        FreeLibrary(hooklib);
    }

    return 0;
}