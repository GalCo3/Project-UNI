#include <windows.h>
#include <tchar.h>
#include <iostream>
#include <tlhelp32.h>
#include <vector>
#include <psapi.h>
#include <string>

TCHAR dllPath[MAX_PATH];

void inject(DWORD pid) {
    std::wcout << L"Injecting into PID: " << pid << std::endl;

    HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pid);
    if (!hProcess) {
		std::wcerr << L"Failed to open process "<< L"\n";
        return;
    }

	// allocate space in other process for dll path
    size_t pathSize = (_tcslen(dllPath) + 1) * sizeof(TCHAR);
    LPVOID pRemoteMemory = VirtualAllocEx(hProcess, nullptr, pathSize,MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
    if (!pRemoteMemory) {
        std::wcerr << L"Failed to allocate memory in target process.\n";
        CloseHandle(hProcess);
        return;
    }

	// write to allocated memory in other process
    if (!WriteProcessMemory(hProcess, pRemoteMemory, dllPath, pathSize, nullptr)) {
        std::wcerr << L"Failed to write to process memory.\n";
        VirtualFreeEx(hProcess, pRemoteMemory, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return;
    }

	// get LoadLibraryW address from Kernel32.dll, we know that in every process Kernel32.dll is loaded at the same address
    HMODULE hKernel32 = GetModuleHandle(_T("Kernel32"));
    if (!hKernel32) {
        std::wcerr << L"Failed to get handle of Kernel32.\n";
        VirtualFreeEx(hProcess, pRemoteMemory, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return;
    }
	// get address of LoadLibraryW from Kernel32.dll
    LPVOID pLoadLibraryW = GetProcAddress(hKernel32, "LoadLibraryW");
    if (!pLoadLibraryW) {
        std::wcerr << L"Failed to get address of LoadLibraryW.\n";
        VirtualFreeEx(hProcess, pRemoteMemory, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return;
    }

	// call remote LoadLibraryW for dll
    HANDLE hRemoteThread = CreateRemoteThread(
        hProcess,
        nullptr,
        0,
        reinterpret_cast<LPTHREAD_START_ROUTINE>(pLoadLibraryW),
        pRemoteMemory,
        0,
        nullptr
    );

    if (!hRemoteThread) {
        std::wcerr << L"Failed to create remote thread.\n";
        VirtualFreeEx(hProcess, pRemoteMemory, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return;
    }

    // wait for remote thread finish loads
    WaitForSingleObject(hRemoteThread, INFINITE);

    VirtualFreeEx(hProcess, pRemoteMemory, 0, MEM_RELEASE);
    CloseHandle(hRemoteThread);
    CloseHandle(hProcess);

    std::wcout << L"Injection succeeded for PID: " << pid << L".\n";
}


void FindProcessAndInject() {
	LPCTSTR processName = _T("mspaint.exe");
    DWORD processes[1024], need;
    if (!EnumProcesses(processes, sizeof(processes), &need)) {
		return;
    }

	for (unsigned int i = 0; i < need / sizeof(DWORD); i++) {
        if (processes[i] == 0) continue;

        HANDLE hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, processes[i]);
        if (hProcess) {
            TCHAR exeName[MAX_PATH] = _T("");
            if (GetModuleBaseName(hProcess, NULL, exeName, MAX_PATH)) {
                if (_tcsicmp(exeName, processName) == 0) {
					std::wcout << L"Found Notepad process with PID: " << processes[i] << std::endl;
					inject(processes[i]);
                }
            }
            CloseHandle(hProcess);
        }
    }
}



int _tmain(int argc, TCHAR* argv[])
{
    if (!GetFullPathName(_T("APIHooking.dll"), MAX_PATH, dllPath, NULL)) {
        std::wcerr << L"Failed to get full path of injected.dll.\n";
        return 1;
    }

	std::wcout << L"Path of DLL: " << dllPath << std::endl;

    if (argc < 2) {
        std::wcout << L"No PID provided. Scanning all processes for MSPAINT...\n";
		FindProcessAndInject();
    }
    else {
        DWORD pid = _tstoi(argv[1]);
        inject(pid);
    }

    return 0;
}
