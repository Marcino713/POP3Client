Imports System.IO
Imports OtwieranieListowDll.KlasyListu

Namespace Dodatki

    Public Module Procedury

        ''' <summary>
        ''' Sprawdza, czy Element znajduje się w TablicaElementow.
        ''' </summary>
        Public Function CzyJest(Element As String, ParamArray TablicaElementow As String()) As Boolean
            For i As Integer = 0 To TablicaElementow.Length - 1
                If Element = TablicaElementow(i) Then Return True
            Next

            Return False
        End Function

        ''' <summary>
        ''' Sprawdza, czy tablica bajtów jest elementem kończącym list. Warunki końca listu:
        ''' długość tablicy = 3,
        ''' pierwsza komórka = 46 (kod ASCII kropki),
        ''' druga i trzecia komórka - znak nowej linii (CRLF)
        ''' </summary>
        Public Function CzyKoniecListu(ByRef bajty As Byte()) As Boolean
            Return bajty.Length = 3 AndAlso bajty(0) = 46 AndAlso bajty(1) = 13 AndAlso bajty(2) = 10
        End Function

        ''' <summary>
        ''' Sprawdza, czy Znak znajduje sie w tablicy Znaki.
        ''' </summary>
        Public Function CzyJestZnak(Znak As Char, ParamArray Znaki() As Char) As Boolean
            For i As Integer = 0 To Znaki.Length - 1
                If Znak = Znaki(i) Then Return True
            Next

            Return False
        End Function

        ''' <summary>
        ''' Sprawdza, czy ciąg bajtów zaczyna się podanym podciągiem.
        ''' </summary>
        Public Function CzyZaczyna(Bajty As Byte(), Poczatek As Byte()) As Boolean
            Dim l As Integer
            If Bajty.Length < Poczatek.Length Then l = Bajty.Length Else l = Poczatek.Length

            For i As Integer = 0 To l - 1
                If Bajty(i) <> Poczatek(i) Then Return False
            Next

            Return True
        End Function

        ''' <summary>
        ''' Zwraca nieistniejącą nazwę pliku w wybranym folderze. Jeśli plik o podanej nazwie istnieje, funkcja dodaje liczbę naturalną w nawiasie między nazwą pliku a rozszerzeniem. 
        ''' </summary>
        Public Function PobierzNieistniejacaSciezke(Sciezka As String, NazwaPliku As String) As String
            If Not File.Exists(Sciezka & NazwaPliku) Then Return Sciezka & NazwaPliku

            Dim nazwa As String = Sciezka & Path.GetFileNameWithoutExtension(NazwaPliku) & " ("
            Dim roz As String = ")" & Path.GetExtension(NazwaPliku)
            Dim i As Integer = 1

            Do While File.Exists(nazwa & i.ToString & roz)
                i += 1
            Loop

            Return nazwa & i.ToString & roz
        End Function

        ''' <summary>
        ''' Zwraca nazwę pliku (bez ścieżki) dla obrazu. Nazwa pobierana jest kolejno z pól Nazwa, NazwaPliku, "obraz" (jeśli pola są puste). Jeśli nazwa ma rozszerzenie inne niż typ obrazu, dodawane jest odpowiednie rozszerzenie.
        ''' </summary>
        Public Function PobierzNazweObrazu(Element As ElementListu) As String
            Dim nazwa As String = "obraz"
            Dim str As String

            If Element.Nazwa <> "" Then
                nazwa = Element.Nazwa
            ElseIf Element.NazwaPliku <> "" Then
                nazwa = Element.NazwaPliku
            End If

            str = nazwa.ToLower

            Select Case Element.Typ
                Case List.MIME_IMAGE_JPEG
                    If Not (str.EndsWith(".jpg") OrElse str.EndsWith(".jpeg")) Then nazwa &= ".jpg"

                Case List.MIME_IMAGE_GIF
                    If Not str.EndsWith(".gif") Then nazwa &= ".gif"

                Case List.MIME_IMAGE_PNG
                    If Not str.EndsWith(".png") Then nazwa &= ".png"

            End Select

            Return nazwa
        End Function

    End Module

End Namespace