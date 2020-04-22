#include "Source.h"
#include "MessageSender.h"
#include "StreamLineReader.h"

#include <cinttypes>
#include <regex>
#include <string>

#define LOGHOOKLIB 1
#if _DEBUG && LOGHOOKLIB
#define LOGHOOKLIBPATH TEXT("C:\\Temp\\HookLibMouseHookProc_")
#include "DebugHelper.h"
TimestampLogger Logger = TimestampLogger(LOGHOOKLIBPATH + TimestampLogger::GetTimestampString(TRUE) + TEXT(".log"), TRUE);
#endif

asio::io_context io_context;
MessageSender messagesender(io_context);

BOOL WINAPI DllMain(HINSTANCE hinst, DWORD fdwReason, LPVOID lpReserved)
{
    switch (fdwReason)
    {
    case DLL_PROCESS_ATTACH:
        LogDllMain(hinst, TEXT("DLL_PROCESS_ATTACH"));
        Initialize(hinst);
        break;

    case DLL_THREAD_ATTACH:
        LogDllMain(hinst, TEXT("DLL_THREAD_ATTACH"));
        break;

    case DLL_THREAD_DETACH:
        LogDllMain(hinst, TEXT("DLL_THREAD_DETACH"));
        break;

    case DLL_PROCESS_DETACH:
        LogDllMain(hinst, TEXT("DLL_PROCESS_DETACH"));
        break;
    }

    return TRUE;
}

void Initialize(HINSTANCE hinst)
{
    // Look for initialization file stored in %TEMP%
    const int kPathBufferSize = 1024;
    TCHAR modulepath[kPathBufferSize];
    GetModuleFileName(NULL, modulepath, kPathBufferSize);
    TCHAR temppath[kPathBufferSize];
    GetTempPath(kPathBufferSize, temppath);
    TCHAR configfilepath[kPathBufferSize];
    swprintf(configfilepath, sizeof(configfilepath), TEXT("%ls%ls%lX%d%d"),
        temppath,
        std::regex_replace(modulepath, std::wregex(TEXT("[\\\\]|[/]|[:]")), TEXT("")).c_str(),
        PtrToInt(hinst),
        GetCurrentProcessId(), 
        GetThreadId(GetCurrentThread()));
    WIN32_FIND_DATA findfiledata;
    auto find = FindFirstFile(configfilepath, &findfiledata);
    if (find != INVALID_HANDLE_VALUE)
    {
        FindClose(find);
        StreamLineReader configfile(configfilepath);
        auto portstring = configfile.ReadLine();
        auto port = std::stoi(portstring);
        messagesender.Connect(std::to_string(port));
    }
}

void LogDllMain(HINSTANCE hinst, std::wstring reason)
{
#if _DEBUG && LOGHOOKLIB
    std::wstringstream wss;
    wss << std::setw(16) << std::setfill(L'0') << std::hex << hinst;
    TCHAR procInfo[256];
    swprintf(procInfo, sizeof(procInfo), TEXT("Instance: %lx; Reason: %ls; ProcessId: %d; ThreadId: %d"), 
        PtrToLong(hinst), reason.c_str(), GetCurrentProcessId(), GetThreadId(GetCurrentThread()));
    Logger.WriteLine(procInfo);
#endif
}

LRESULT CALLBACK MouseHookProc(int code, WPARAM wParam, LPARAM lParam) 
{
#if _DEBUG && LOGHOOKLIB
    Logger.WriteLine(DebugHelper::FormatMouseHookMessage(code, wParam, lParam));
#endif
    if (code == HC_ACTION)
    {
        auto pmhs = (PMOUSEHOOKSTRUCT)lParam;
        HookMouseMessage hmm;
        hmm.messageCode = (DWORD)wParam;
        hmm.pointX = pmhs->pt.x;
        hmm.pointY = pmhs->pt.y;
        hmm.hwnd = (DWORD)PtrToInt(pmhs->hwnd);
        hmm.hitTestCode = (DWORD)pmhs->wHitTestCode;
        messagesender.SendMessage(&hmm, sizeof(HookMouseMessage));
    }

    return CallNextHookEx(NULL, code, wParam, lParam);
}