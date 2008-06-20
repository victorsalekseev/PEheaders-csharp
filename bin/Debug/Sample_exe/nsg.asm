.386

.model flat, stdcall

option CaseMap : none

INCLUDE \masm32\include\kernel32.inc
INCLUDE \masm32\include\user32.inc
INCLUDE \masm32\include\windows.inc

INCLUDELIB \masm32\lib\kernel32.lib
INCLUDELIB \masm32\lib\user32.lib

.code
start:
invoke MessageBox, 0h, NULL, NULL, MB_OK
invoke ExitProcess, 0h
end start