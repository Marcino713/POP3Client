;ml Dodatki.asm /c
;link /subsystem:windows /dll /export:DekodujBase64 /export:ZwolnijPamiec /export:JestCMOV Dodatki.obj

.686p
.model flat, stdcall
option casemap :none 
.listall

GetProcessHeap PROTO
HeapAlloc PROTO hHeap:DWORD, dwFlags:DWORD, dwBytes:DWORD
HeapFree PROTO hHeap:DWORD, dwFlags:DWORD, lpMem:DWORD

includelib kernel32.Lib

.data
;Odwzorowanie znakow ASCII na liczby Base64, znakow zakres 43 (+) - 122 (z)
;Base64: liczby 0 - 63, 64 - dla znaku =, 65 - dla znaku niepoprawnego
Znaki 	byte 62, 65, 62, 65, 63 ;od + (kod 43) do /
		byte 52, 53, 54, 55, 56, 57, 58, 59, 60, 61	;cyfry 0-9
		byte 65, 65, 65, 64, 65, 65, 65 ;znaki miedzy cyframi i literami, w tym znak = (liczba 64)
		byte 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 ;wielkie litery
		byte 65, 65, 65, 65, 63, 65 ;znaki po literach i _
		byte 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51 ;male litery

KOD_MIN equ 43
KOD_MAX equ 122
LICZBA_BLAD equ 64

.code
start:

DllEntry proc instance:DWORD, reason:DWORD, reserved:DWORD
    mov     eax, 1
    ret
DllEntry endp


;Zwracane kody bledow (najmlodszy bajt):
;0 - dekodowanie poprawne
;1 - ptrDaneZdekodowane != 0
;2 - dlugosc ciagu wejsciowego po usunieciu bialych znakow nie jest wielokrotnoscia 4
;3 - w dekodowanym ciagu znajduje sie znak nienalezacy do Base64, inny niz spacja/tabulacja/znaki nowej linii; bity 15..8 - kod niepoprawnego znaku

DekodujBase64 proc ptrDane:dword, Dlugosc:dword, ptrDaneZdekodowane:dword
	local sterta:dword, ptrBajtyCzyste:dword, ptrBajtyZdekodowane:dword
	
	;Czy wskaznik poprawny
	push ebx
	mov ebx, ptrDaneZdekodowane
	mov eax, [ebx]
	cmp eax, 0
	pop ebx
	je wskaznik_poprawny
	mov eax, 1
	ret
wskaznik_poprawny:

	push ebx
	push ecx
	push edx
	push esi
	push edi	
	
	invoke GetProcessHeap
	mov sterta, eax

	;----------------Usuwanie bialych znakow---------------
	;EAX - sprawdzany bajt
	;EBX - wynik porownania z bialym znakiem
	;ECX - dekrementowana zmienna petlowa - na poczatku dlugosc tablicy zrodlowej
	;EDX - stala 1
	;ESI - indeks znaku w tablicy zrodlowej
	;EDI - indeks znaku w tablicy docelowej
	
	;Przydziel pamiec
	invoke HeapAlloc, sterta, 0, Dlugosc
	mov ptrBajtyCzyste, eax
	mov esi, ptrDane
	mov edi, eax
	mov ecx, Dlugosc
	mov edx, 1
	
petla_usun:
	sub ecx, 1
	jb petla_usun_koniec

	xor ebx, ebx
	mov al, [esi]
	
	;Czy bialy znak	
	cmp al, 0ah	;LF
	cmove ebx, edx
	cmp al, 0dh	;CR
	cmove ebx, edx
	cmp al, 20h	;spacja
	cmove ebx, edx
	cmp al, 9h		;HT
	cmove ebx, edx
	
	cmp ebx, edx
	je petla_usun_dalej
	
	mov [edi], al
	inc edi
	
petla_usun_dalej:
	inc esi
	jmp petla_usun
	
petla_usun_koniec:
	
	
	;-------------------------Dekodowanie-------------

	;Oblicz dlugosc tablicy i przydziel pamiec
	;EAX - dlugosc tablicy docelowej (dlugosc tablicy zrodlowej * 0.75 + 1)
	;EBX - stala 3
	;ECX - dlugosc tablicy zrodlowej
	
	;Dlugosc
	mov ecx, edi
	sub ecx, ptrBajtyCzyste
	mov eax, ecx
	
	;Czy wielokrotnosc 4
	and eax, 3
	jz dlugosc_poprawna
	invoke HeapFree, sterta, 0, ptrBajtyCzyste
	mov eax, 2
	jmp koniec
	
dlugosc_poprawna:
	mov eax, ecx
	shr eax, 2
	mov ebx, 3
	mul ebx
	inc eax
	
	push ecx
	invoke HeapAlloc, sterta, 0, eax
	pop ecx
	
	
	;Dekoduj
	;EAX - dekodowany bajt AL/zdekodowany bajt AH
	;EBX - dekodowane podwojne slowo/stala 1
	;ECX - dekrementowana zmienna petlowa - na poczatku dlugosc tablicy zrodlowej
	;EDX - zdekodowane bajty/sprawdzanie poprawnosci dekodowanego znaku
	;ESI - indeks bajtu w tablicy zrodlowej
	;EDI - indeks bajtu w tablicy docelowej
	
	mov esi, ptrBajtyCzyste
	mov edi, eax
	mov ptrBajtyZdekodowane, eax
	
	
petla_dekoduj:
	sub ecx, 4
	jb koniec_petli

	xor edx, edx
	
	;wczytaj bajty
	mov ebx, [esi]
	bswap ebx
	add esi, 4

	;pierwszy znak
	mov eax, ebx
	shr eax, 18h
	push ebx
	push edx
	mov ebx, 1
	cmp eax, KOD_MIN
	cmovb edx, ebx
	cmp eax, KOD_MAX
	cmova edx, ebx
	and edx, 1
	pop edx
	pop ebx
	jnz nieprawidlowy_znak
	sub eax, KOD_MIN
	mov ah, Znaki[eax]
	cmp ah, LICZBA_BLAD
	jge nieprawidlowy_znak
	and eax, 0FF00h
	shl eax, 12h
	or edx, eax	
	
	
	;drugi znak
	mov eax, ebx
	shr eax, 10h
	and eax, 0FFh
	push ebx
	push edx
	mov ebx, 1
	xor edx, edx
	cmp eax, KOD_MIN
	cmovb edx, ebx
	cmp eax, KOD_MAX
	cmova edx, ebx
	and edx, 1
	pop edx
	pop ebx
	jnz nieprawidlowy_znak
	sub eax, KOD_MIN
	mov ah, Znaki[eax]
	cmp ah, LICZBA_BLAD
	jge nieprawidlowy_znak
	and eax, 0FF00h
	shl eax, 0Ch
	or edx, eax
	
	
	;trzeci znak
	mov eax, ebx
	shr eax, 8
	and eax, 0FFh
	push ebx
	push edx
	mov ebx, 1
	xor edx, edx
	cmp eax, KOD_MIN
	cmovb edx, ebx
	cmp eax, KOD_MAX
	cmova edx, ebx
	and edx, 1
	pop edx
	pop ebx
	jnz nieprawidlowy_znak
	sub eax, KOD_MIN
	mov ah, Znaki[eax]
	cmp ah, LICZBA_BLAD
	jg nieprawidlowy_znak	;kod 65
	jne	dalej1				;kod 64 (dla znaku =)
	mov eax, 1
	jmp zapisz	
dalej1:
	and eax, 0FF00h
	shl eax, 6
	or edx, eax

	
	;czwarty znak
	mov eax, ebx
	and eax, 0FFh
	push ebx
	push edx
	mov ebx, 1
	xor edx, edx
	cmp eax, KOD_MIN
	cmovb edx, ebx
	cmp eax, KOD_MAX
	cmova edx, ebx
	and edx, 1
	pop edx
	pop ebx
	jnz nieprawidlowy_znak
	sub eax, KOD_MIN
	mov ah, Znaki[eax]
	cmp ah, LICZBA_BLAD
	jg nieprawidlowy_znak	;kod 65
	jne dalej2				;kod 64 (dla znaku =)
	mov eax, 2
	jmp zapisz
dalej2:
	;shr eax, 8
	and eax, 0FF00h
	or edx, eax

	mov eax, 3
	
	
	;Zapisanie zdekodowanych danych do tablicy docelowej
	;Zmiana rejestrow:
	;EAX - ilosc odczytanych bajtow: 1 - 3
	;EBX - stale
	;ECX - o ile bitow przesunac liczbe w prawo
zapisz:
	bswap edx
	mov [edi], edx
	add edi, eax

	jmp petla_dekoduj
	
koniec_petli:
	
	;zwolnij pamiec tablicy bez bialych znakow
	invoke HeapFree, sterta, 0, ptrBajtyCzyste
	
	;przygotuj pamiec dla wyniku
	invoke HeapAlloc, sterta, 0, 8
	mov ebx, eax
	mov edx, ptrBajtyZdekodowane
	sub edi, edx	;Dlugosc tablicy
	mov [ebx], edi
	
	add ebx, 4
	mov [ebx], edx	;Wskaznik na tablice

	mov ebx, ptrDaneZdekodowane
	mov [ebx], eax
	xor eax, eax

	jmp koniec
	
	;Nieprawidlowy znak w tablicy wejsciowej, posprzataj i zakoncz
nieprawidlowy_znak:
	
	;Starszy bajt AX = 0  - bajt nie miesci sie w zakresie
	;Starszy bajt AX != 0 - z tablicy odczytano 65, do kodu znaku nalezy dodac KOD_MIN
	;Przygotowanie zwracanego kodu - bity 15..8 kod niepoprawnego znaku, 8..0 - liczba 3
	mov ebx, KOD_MIN
	xor edx, edx
	mov ecx, eax
	and eax, 0FF00h
	cmovnz edx, ebx
	and ecx, 0FFh
	add ecx, edx
	shl ecx, 8
	mov eax, 3
	or eax, ecx
	push eax
	invoke HeapFree, sterta, 0, ptrBajtyCzyste
	invoke HeapFree, sterta, 0, ptrBajtyZdekodowane
	pop eax

koniec:
	pop edi
	pop esi
	pop edx
	pop ecx
	pop ebx
	
	ret
DekodujBase64 endp

ZwolnijPamiec proc wskaznik:DWORD
	local sterta:dword
	push ebx
	
	invoke GetProcessHeap
	mov sterta, eax
	
	mov ebx, wskaznik
	add ebx, 4
	mov eax, [ebx]
	invoke HeapFree, sterta, 0, eax
	invoke HeapFree, sterta, 0, wskaznik
	
	pop ebx
	xor eax, eax
	ret
ZwolnijPamiec endp

;Zwraca 1, jesli procesor obsluguje instrukcje CMOVcc.
JestCMOV proc
	push ebx
	push ecx
	push edx
	mov eax, 1
	cpuid
	shr edx, 0Eh
	and edx, 1
	mov eax, edx
	pop edx
	pop ecx
	pop ebx
	ret
JestCMOV endp

end DllEntry