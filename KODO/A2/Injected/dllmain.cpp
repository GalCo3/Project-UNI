#include "pch.h"
#include <windows.h>
#include <tchar.h>
#include <commctrl.h>
#include <stdio.h>
#include <stdlib.h>



static DWORD g_CurrentProcessId = 0;


BOOL CALLBACK TestAndChange(HWND hwnd, LPARAM lParam) {
    DWORD pid = 0;
    GetWindowThreadProcessId(hwnd, &pid);

    if (pid == g_CurrentProcessId) {
        SetWindowText(hwnd, _T("This MSPAINT was HACKED by Gal Cohen"));
    }
    return TRUE;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call) {
    case DLL_PROCESS_ATTACH:
        g_CurrentProcessId = GetCurrentProcessId();
        EnumWindows(TestAndChange, 0);
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
