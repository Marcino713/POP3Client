Imports System.Runtime.InteropServices
Imports OtwieranieListowDll.KlasyListu

Namespace Dodatki

    Public Module Kodowanie

#Region "Base64"
        Private Declare Auto Function DekodujBase64 Lib "Dodatki.dll" (ptrDane As Byte(), Dlugosc As Integer, ByRef ptrDaneZdekodowane As IntPtr) As Integer
        Private Declare Auto Sub ZwolnijPamiec Lib "Dodatki.dll" (Wskaznik As IntPtr)
        Private Declare Auto Function JestCMOV Lib "Dodatki.dll" () As Integer

        Private Function DekodujBase64Asm(Dane As Byte()) As Byte()
            Dim wsk As IntPtr = IntPtr.Zero
            Dim blad As Integer = DekodujBase64(Dane, Dane.Length, wsk)

            If blad = 0 Then

                Dim tab As TablicaBajtow = Marshal.PtrToStructure(Of TablicaBajtow)(wsk)
                Dim b(tab.Dlugosc - 1) As Byte
                Marshal.Copy(tab.Bajty, b, 0, tab.Dlugosc)
                ZwolnijPamiec(wsk)
                Return b

            Else

                If blad = 1 Then
                    List.ZglosBlad("Błąd dekodowania Base64:" & vbCrLf & "Wskaźnik przekazany do funkcji DekodujBase64 nie miał wartości 0.")
                ElseIf blad = 2 Then
                    List.ZglosBlad("Błąd dekodowania Base64:" & vbCrLf & "Długość ciągu wejściowego nie jest wielokrotnością 4.")
                ElseIf (blad And 255) = 3 Then
                    Dim kod As Integer = (blad And &HFF00) >> 8
                    List.ZglosBlad("Błąd dekodowania Base64:" & vbCrLf & "W ciągu wejściowym znalazł się niepoprawny znak." & vbCrLf & "Kod znaku: " & kod.ToString & " (" & Chr(kod).ToString & ")")
                Else
                    List.ZglosBlad("Błąd dekodowania Base64:" & vbCrLf & "Wystąpił nieznany błąd w funkcji DekodujBase64, kod błędu: " & blad.ToString)
                End If

                Return Nothing

            End If
        End Function

        Public Function Dekoduj_Base64(Dane As Byte()) As Byte()

            If JestCMOV = 1 Then
                Return DekodujBase64Asm(Dane)
            Else
                Return Convert.FromBase64String(Text.Encoding.UTF8.GetString(Dane))
            End If

        End Function

        Private Structure TablicaBajtow
            Friend Dlugosc As Integer
            Friend Bajty As IntPtr
        End Structure
#End Region 'Base64


#Region "QuotedPrintable"
        Public Function Dekoduj_QuotedPrintable(Dane As Byte()) As Byte()
            Dim l As Integer = Dane.Length - 1
            Dim bajty(l) As Byte
            Dim i As Integer = 0    'Indeks w tablicy zakodowanej
            Dim ix As Integer = 0   'Indeks w tablicy odkodowanej
            Dim b As Byte

            Do Until i > l
                b = Dane(i)

                If b = 61 AndAlso (i + 2 <= l) Then  'Znak rownosci

                    If Dane(i + 1) = 13 AndAlso Dane(i + 2) = 10 Then   'CRLF - koniec linii
                        ix -= 1
                    Else
                        Dim int As Integer = (KonwertujZnakNaLiczbe(Dane(i + 1)) << 4) + KonwertujZnakNaLiczbe(Dane(i + 2))
                        bajty(ix) = Convert.ToByte(int)
                    End If

                    i += 3

                Else
                    bajty(ix) = b
                    i += 1
                End If

                ix += 1
            Loop

            ReDim Preserve bajty(ix - 1)

            Return bajty
        End Function

        'Konwertuje znak ASCII na cyfre szesnastkowa
        Private Function KonwertujZnakNaLiczbe(Liczba As Integer) As Integer
            If Liczba < 58 Then 'Cyfra
                Liczba -= 48
            ElseIf Liczba < 71 Then  'Wielka litera
                Liczba -= 55
            Else    'Mała litera
                Liczba -= 87
            End If

            Return Liczba
        End Function
#End Region 'QuotedPrintable

    End Module

End Namespace