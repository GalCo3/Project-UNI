#include <windows.h>
#include <stdio.h>
#include <stdlib.h>

typedef LONG NTSTATUS;
#define STATUS_SUCCESS ((NTSTATUS)0x00000000L)

typedef enum _PROCESSINFOCLASS {
    ProcessBasicInformation = 0
} PROCESSINFOCLASS;

typedef struct _PROCESS_BASIC_INFORMATION {
    PVOID Reserved1;
    PVOID PebBaseAddress;
    PVOID Reserved2[2];
    ULONG_PTR UniqueProcessId;
    PVOID Reserved3;
} PROCESS_BASIC_INFORMATION;

typedef NTSTATUS(NTAPI* PFN_NTQUERYINFORMATIONPROCESS)(
    HANDLE ProcessHandle,
    PROCESSINFOCLASS ProcessInformationClass,
    PVOID ProcessInformation,
    ULONG ProcessInformationLength,
    PULONG ReturnLength
    );

int main(int argc, char* argv[])
{
    if (argc < 3) {
        printf("Usage: %s <path_to_ex4b-2.exe> <argiment_file_path_for_ex4b-2.exe>\n", argv[0]);
        return 1;
    }

    char cmdLine[1024] = { 0 };
    sprintf_s(cmdLine, sizeof(cmdLine), "\"%s\" \"%s\"", argv[1], argv[2]);

	// Create the target process in suspended mode and give it the argument.
    STARTUPINFOA si = { 0 };
    si.cb = sizeof(si);
    PROCESS_INFORMATION pi = { 0 };
    CreateProcessA(argv[1], cmdLine, NULL, NULL, FALSE, CREATE_SUSPENDED, NULL, NULL, &si, &pi);

    HMODULE hNtdll = LoadLibraryA("ntdll.dll");
    PFN_NTQUERYINFORMATIONPROCESS NtQueryInformationProcess =
        (PFN_NTQUERYINFORMATIONPROCESS)GetProcAddress(hNtdll, "NtQueryInformationProcess");

    PROCESS_BASIC_INFORMATION pbi = { 0 };
    ULONG retLen = 0;
    NtQueryInformationProcess(pi.hProcess, ProcessBasicInformation, &pbi, sizeof(pbi), &retLen);

    // Get ImageBaseAddress from the PEB (at offset 0x10).
    PVOID imageBaseAddress = NULL;
    SIZE_T bytesRead = 0;
    ReadProcessMemory(pi.hProcess, (PBYTE)pbi.PebBaseAddress + 0x10,
        &imageBaseAddress, sizeof(imageBaseAddress), &bytesRead);

    // --- Patch 1 ---
    // Instruction: cmp [rsp+28h+arg_0], 3  -> Expected bytes: 83 7C 24 30 03
    //Change immediate (5th byte) from 03 to 02.
    SIZE_T patch1_offset = 0x111D;
    LPVOID patch1Addr = (LPVOID)((PBYTE)imageBaseAddress + patch1_offset);
    LPVOID patch1Imm = (LPVOID)((PBYTE)patch1Addr + 4);
    DWORD oldProtect = 0;
    VirtualProtectEx(pi.hProcess, patch1Imm, sizeof(BYTE), PAGE_EXECUTE_READWRITE, &oldProtect);
    {
        BYTE val = 2;
        SIZE_T bytesWritten = 0;
        WriteProcessMemory(pi.hProcess, patch1Imm, &val, sizeof(BYTE), &bytesWritten);
    }
    VirtualProtectEx(pi.hProcess, patch1Imm, sizeof(BYTE), oldProtect, &oldProtect);

    // --- Patch 2 ---
    // Instruction: cmp eax, 1 -> Expected bytes 83 F8 01 ..
	// Change the 1-byte immediate (at offset +2) from 1 to 0.
    SIZE_T patch2_offset = 0x1038;
    LPVOID patch2Addr = (LPVOID)((PBYTE)imageBaseAddress + patch2_offset);
    LPVOID patch2Imm = (LPVOID)((PBYTE)patch2Addr + 2);
    VirtualProtectEx(pi.hProcess, patch2Imm, sizeof(BYTE), PAGE_EXECUTE_READWRITE, &oldProtect);
    {
        BYTE val = 0;
        SIZE_T bytesWritten = 0;
        WriteProcessMemory(pi.hProcess, patch2Imm, &val, sizeof(BYTE), &bytesWritten);
    }
    VirtualProtectEx(pi.hProcess, patch2Imm, sizeof(BYTE), oldProtect, &oldProtect);

    // --- Patch 3 ---
    // Instruction: mov edx, 8 ->  BA 08 00 00 00.
    // Change the 4-byte from 8 to 1024 (0x400).
    SIZE_T patch3_offset = 0x104C;
    LPVOID patch3Addr = (LPVOID)((PBYTE)imageBaseAddress + patch3_offset);
    LPVOID patch3Imm = (LPVOID)((PBYTE)patch3Addr + 1);
    VirtualProtectEx(pi.hProcess, patch3Imm, sizeof(DWORD), PAGE_EXECUTE_READWRITE, &oldProtect);
    {
        DWORD val = 0x00000400;
        SIZE_T bytesWritten = 0;
        WriteProcessMemory(pi.hProcess, patch3Imm, &val, sizeof(DWORD), &bytesWritten);
    }
    VirtualProtectEx(pi.hProcess, patch3Imm, sizeof(DWORD), oldProtect, &oldProtect);

    // --- Patch 4 ---
	// Instruction: dec eax at offset 0x1071 -> Expected bytes: FF C8
	// Change the 2-byte instruction to NOPs (0x90 0x90).
    SIZE_T patch4_offset = 0x1071;
    LPVOID patch4Addr = (LPVOID)((PBYTE)imageBaseAddress + patch4_offset);
    VirtualProtectEx(pi.hProcess, patch4Addr, 2, PAGE_EXECUTE_READWRITE, &oldProtect);
    {
        BYTE nops[2] = { 0x90, 0x90 };
        SIZE_T bytesWritten = 0;
        WriteProcessMemory(pi.hProcess, patch4Addr, nops, 2, &bytesWritten);
    }
    VirtualProtectEx(pi.hProcess, patch4Addr, 2, oldProtect, &oldProtect);

    // Resume the target process.
    ResumeThread(pi.hThread);
    WaitForSingleObject(pi.hProcess, INFINITE);

    CloseHandle(pi.hThread);
    CloseHandle(pi.hProcess);
    FreeLibrary(hNtdll);

    return 0;
}
