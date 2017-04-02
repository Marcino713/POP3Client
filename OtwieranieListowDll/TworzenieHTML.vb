Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO.FileSystem
Imports OtwieranieListowDll.KlasyListu

Namespace Dodatki

    Public Module TworzenieHTML

        ''' <summary>
        ''' Zapisuje treść listu do nowego pliku utworzonego na podstawie szablonu.
        ''' </summary>
        ''' <param name="Zawartosc">Tablica bajtów do zapisania w pliku</param>
        ''' <param name="Kodowanie">Nazwa systemu kodowania znaków</param>
        ''' <param name="SciezkaPliku">Ściezka pliku, który jest zapisywany. Jeśli plik istnieje, zostanie nadpisany</param>
        ''' <param name="SciezkaSzablonu">Ściezka do szablonu</param>
        ''' <param name="ZamienZnakiSpecjalne">Określa, czy zamienić niektóre znaki (CRLF, tabulcaja pozioma, &lt;, &gt;, dodatkowe spacje) na odpowiednie encje/znaczniki HTML</param>
        Public Sub ZapiszListDoHTML(Zawartosc As Byte(), Kodowanie As String, SciezkaPliku As String, SciezkaSzablonu As String, ZamienZnakiSpecjalne As Boolean)
            If Not FileExists(SciezkaSzablonu) Then
                List.ZglosBlad("Nie można otworzyć pliku " & SciezkaSzablonu)
                Exit Sub
            End If

            Dim fsplik As New FileStream(SciezkaPliku, FileMode.Create, FileAccess.Write)
            Dim fsszablon As New FileStream(SciezkaSzablonu, FileMode.Open, FileAccess.Read)
            Dim b As Byte
            Dim kod(1) As Byte
            Dim bajty_kodowanie As Byte() = Encoding.UTF8.GetBytes(Kodowanie)
            Dim bajty_br As Byte() = {60, 98, 114, 47, 62, 13, 10}  '<br/>CRLF
            Dim bajty_lt As Byte() = {38, 108, 116, 59} '&lt;
            Dim bajty_gt As Byte() = {38, 103, 116, 59} '&gt;
            Dim bajty_tab As Byte() = {38, 35, 120, 48, 48, 48, 57, 59, 9}    '&#x0009;HT
            Dim bajty_nbsp As Byte() = {38, 110, 98, 115, 112, 59}   '&nbsp;
            Dim l As Long = fsszablon.Length
            Dim zapisz_nbsp As Boolean = False

            Do Until fsszablon.Position >= l
                b = Convert.ToByte(fsszablon.ReadByte)

                If b = 35 Then '#

                    If fsszablon.Position + 2 < l Then
                        kod(0) = Convert.ToByte(fsszablon.ReadByte)
                        kod(1) = Convert.ToByte(fsszablon.ReadByte)

                        If kod(0) = 65 AndAlso kod(1) = 49 Then '#A1 - kodowanie
                            fsplik.Write(bajty_kodowanie, 0, bajty_kodowanie.Length)
                            Continue Do

                        ElseIf kod(0) = 65 AndAlso kod(1) = 50 Then '#A2 - zawartość

                            For i As Integer = 0 To Zawartosc.Length - 1
                                b = Zawartosc(i)

                                If ZamienZnakiSpecjalne Then

                                    Select Case b
                                        Case 13 'CR - pomiń
                                        Case 10 'LF
                                            fsplik.Write(bajty_br, 0, bajty_br.Length)
                                            zapisz_nbsp = False
                                        Case 9  'HT
                                            fsplik.Write(bajty_tab, 0, bajty_tab.Length)
                                            zapisz_nbsp = False
                                        Case 60 '<
                                            fsplik.Write(bajty_lt, 0, bajty_lt.Length)
                                            zapisz_nbsp = False
                                        Case 62 '>
                                            fsplik.Write(bajty_gt, 0, bajty_gt.Length)
                                            zapisz_nbsp = False
                                        Case 32 'SP
                                            If zapisz_nbsp Then
                                                fsplik.Write(bajty_nbsp, 0, bajty_nbsp.Length)
                                            Else
                                                zapisz_nbsp = True
                                                fsplik.WriteByte(b)
                                            End If
                                        Case Else
                                            zapisz_nbsp = False
                                            fsplik.WriteByte(b)
                                    End Select

                                Else
                                    fsplik.WriteByte(b)
                                End If

                            Next

                            Continue Do

                        End If

                    End If

                End If

                fsplik.WriteByte(b)

            Loop

            fsplik.Close()
            fsplik.Dispose()
            fsszablon.Close()
            fsszablon.Dispose()
        End Sub

        Public Sub ZapiszNaglowkiDoHTML(Naglowki As Tablica(Of Naglowek), SciezkaPliku As String, SciezkaSzablonu As String)

            If Not FileExists(SciezkaSzablonu) Then
                List.ZglosBlad("Nie można otworzyć pliku " & SciezkaSzablonu)
                Exit Sub
            End If

            Dim plik As String = ReadAllText(SciezkaSzablonu)
            Dim str As New StringBuilder("")
            Dim i As Integer = 0
            Dim l As Integer = plik.Length
            Dim petla_pocz As Integer = 0
            Dim ix As Integer = 0
            Dim liczba_naglowkow As Integer = Naglowki.Length - 1

            Do Until i >= l

                If plik(i) = "#" Then

                    If i + 2 >= l Then
                        str.Append(plik(i))
                        i += 1
                        Continue Do
                    End If

                    Select Case plik(i + 1)
                        Case "a"c, "A"c 'Pętla dla wierszy nagłówków

                            Select Case plik(i + 2)
                                Case "1"c   'Początek pętli
                                    petla_pocz = i + 3
                                    ix = 0

                                Case "2"c   'Koniec pętli
                                    If ix < liczba_naglowkow Then
                                        i = petla_pocz
                                        ix += 1
                                    End If

                            End Select

                        Case "b"c, "B"c 'Zmienne

                            Select Case plik(i + 2)
                                Case "1"c   'Stała Nagłówek
                                    str.Append("Nagłówek")

                                Case "2"c   'Stała Wartość
                                    str.Append("Wartość")

                                Case "3"c   'Nazwa nagłówka
                                    str.Append(Naglowki(ix).Nazwa)
                                    str.Append(":"c)

                                Case "4"c   'Wartość nagłówka
                                    str.Append(Naglowki(ix).Wartosc.Replace("<", "&lt;").Replace(">", "&gt;").Replace(vbCrLf, "<br />"))

                            End Select

                    End Select

                    i += 3

                Else
                    str.Append(plik(i))
                    i += 1
                End If

            Loop

            WriteAllText(SciezkaPliku, str.ToString, False)
        End Sub

    End Module

End Namespace