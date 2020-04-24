#pragma once

#include <Windows.h>

class MainWindowFinder
{
public:
    MainWindowFinder(DWORD processid) : processid_(processid) { }
    HWND FindMainWindow()
    {
        EnumWindows(EnumWindowsCallback, (LONG_PTR)this);

        return besthandle_;
    }
private:
    static BOOL CALLBACK EnumWindowsCallback(HWND handle, LONG_PTR mwf)
    {
        auto self = reinterpret_cast<MainWindowFinder*>(mwf);
        DWORD processid{};
        GetWindowThreadProcessId(handle, &processid);
        if (processid == self->processid_ && MainWindowFinder::IsMainWindow(handle))
        {
            self->besthandle_ = handle;
            return FALSE;
        }

        return TRUE;
    }
    static BOOL IsMainWindow(HWND handle)
    {
        return GetWindow(handle, GW_OWNER) == (HWND)0 && IsWindowVisible(handle);
    }
private:
    DWORD processid_{};
    HWND besthandle_{};
};