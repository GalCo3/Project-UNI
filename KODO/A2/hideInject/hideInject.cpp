#include <windows.h>
#include <tchar.h>
#include <psapi.h>
#include <iostream>
#include <string>

int _tmain(int argc, TCHAR* argv[])
{
    if (argc < 2) {
        std::wcerr << L"Usage: hideInject.exe <PID>" << std::endl;
        return 1;
    }

    // Parse PID from arguments
    DWORD pid = _tstoi(argv[1]);

    // Open target process with enough privileges
    HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pid);
    if (!hProcess) {
        std::wcerr << L"[-] Failed to open process. Error: " << GetLastError() << std::endl;
        return 1;
    }

    // Enumerate modules in the target process
    HMODULE hMods[1024];
    DWORD cbNeeded;
    if (!EnumProcessModules(hProcess, hMods, sizeof(hMods), &cbNeeded)) {
        std::wcerr << L"[-] EnumProcessModules failed. Error: " << GetLastError() << std::endl;
        CloseHandle(hProcess);
        return 1;
    }

    // Find the base address of APIHooking.dll in the target process
    TCHAR modName[MAX_PATH];
    HMODULE hTargetDll = NULL;
    for (unsigned int i = 0; i < (cbNeeded / sizeof(HMODULE)); i++) {
        if (GetModuleBaseName(hProcess, hMods[i], modName, MAX_PATH)) {
            if (_tcsicmp(modName, _T("APIHooking.dll")) == 0) {
                hTargetDll = hMods[i];
                break;
            }
        }
    }

    if (!hTargetDll) {
        std::wcerr << L"[-] APIHooking.dll not found in target process (PID: " << pid << L")." << std::endl;
        CloseHandle(hProcess);
        return 1;
    }

    // Get the address of FreeLibrary in local process
    // (same address of FreeLibrary in remote process in this context)
    HMODULE hKernel32 = GetModuleHandle(_T("kernel32.dll"));
    if (!hKernel32) {
        std::wcerr << L"[-] Failed to get handle of kernel32.dll in local process." << std::endl;
        CloseHandle(hProcess);
        return 1;
    }

    LPVOID pFreeLibrary = GetProcAddress(hKernel32, "FreeLibrary");
    if (!pFreeLibrary) {
        std::wcerr << L"[-] GetProcAddress(FreeLibrary) failed." << std::endl;
        CloseHandle(hProcess);
        return 1;
    }

    // Create remote thread in target process that calls FreeLibrary(hTargetDll)
    HANDLE hRemoteThread = CreateRemoteThread(
        hProcess,
        NULL,
        0,
        reinterpret_cast<LPTHREAD_START_ROUTINE>(pFreeLibrary),
        hTargetDll,
        0,
        NULL
    );
    if (!hRemoteThread) {
        std::wcerr << L"[-] Failed to create remote thread. Error: " << GetLastError() << std::endl;
        CloseHandle(hProcess);
        return 1;
    }
    WaitForSingleObject(hRemoteThread, INFINITE);

    CloseHandle(hRemoteThread);
    CloseHandle(hProcess);

    std::wcout << L"[+] Successfully detached APIHooking.dll from process (PID: " << pid << L")." << std::endl;
    return 0;
}
