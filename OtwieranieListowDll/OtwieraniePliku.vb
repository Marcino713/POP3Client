Imports System.IO
Imports System.Text

Namespace Dodatki

    Public Class OtwieraniePliku
        Private strumien As FileStream = Nothing

        Public Sub OtworzPlik(Sciezka As String)
            strumien = New FileStream(Sciezka, FileMode.Open, FileAccess.Read, FileShare.Read)
        End Sub

        Public Function CzytajLinieTekstu() As String
            Dim str As New StringBuilder("")
            Dim b As Integer
            Dim l As Long = strumien.Length - 1

            Do
                b = strumien.ReadByte

                If b = 13 Then 'vbCr
                    Exit Do
                Else
                    str.Append(Convert.ToChar(b))
                End If

            Loop Until strumien.Position = l

            If strumien.Position < l Then   'vbLf
                strumien.ReadByte()
            End If

            Return str.ToString
        End Function

        Public Function CzytajLinieBajtow() As Byte()
            Dim lista As New List(Of Byte)
            Dim b As Integer
            Dim l As Long = strumien.Length - 1

            Do
                b = strumien.ReadByte

                If b = 13 Then 'vbCr
                    Exit Do
                Else
                    lista.Add(Convert.ToByte(b))
                End If

            Loop Until strumien.Position = l

            If strumien.Position < l Then   'vbLf
                strumien.ReadByte()
            End If

            Return lista.ToArray()
        End Function

        Public Function CzytajBajty(Koniec As String) As Byte()
            Dim bajty_koniec As Byte() = Encoding.UTF8.GetBytes(Koniec)
            Dim zwieksz_rozmiar As Integer = 5000
            Dim lkoniec As Integer = bajty_koniec.Length - 1
            Dim lstr As Long = strumien.Length
            Dim ix As Integer = 1
            Dim pojemnosc As Integer = zwieksz_rozmiar
            Dim bajty(pojemnosc) As Byte
            Dim bajt As Byte
            Dim ix_bajty As Integer
            Dim ix_koniec As Integer
            Dim rowne As Boolean

            'Pomin biale znaki
            Do Until strumien.Position >= lstr

                bajt = Convert.ToByte(strumien.ReadByte)
                If Not CzyJestBajt(bajt, 32, 13, 10, 9) Then 'spacja, cr, lf, tab
                    bajty(0) = bajt
                    Exit Do
                End If

            Loop

            'Zapisz bajt
            Do Until strumien.Position >= lstr
                bajt = Convert.ToByte(strumien.ReadByte)
                bajty(ix) = bajt
                ix += 1

                'Czy powiekszyc tablice
                If ix = pojemnosc Then
                    pojemnosc += zwieksz_rozmiar
                    ReDim Preserve bajty(pojemnosc)
                End If

                'Czy odczytano separator
                ix_bajty = ix - 1
                ix_koniec = lkoniec
                rowne = True

                Do Until ix_bajty <= 0 OrElse ix_koniec <= 0

                    If bajty(ix_bajty) <> bajty_koniec(ix_koniec) Then
                        rowne = False
                        Exit Do
                    End If

                    ix_bajty -= 1
                    ix_koniec -= 1

                Loop

                If rowne AndAlso ix >= lkoniec Then
                    ix -= (lkoniec + 2) 'Usun separator
                    Exit Do
                End If
            Loop

            'Usun biale znaki
            Do Until ix < 0
                If CzyJestBajt(bajty(ix), 32, 13, 10, 9) Then 'spacja, cr, lf, tab
                    ix -= 1
                Else
                    Exit Do
                End If
            Loop

            'Zwroc bajty
            If ix < 0 Then
                Return Nothing
                'ReDim bajty_nowe(-1)
            Else
                ReDim Preserve bajty(ix)
                Return bajty
            End If

        End Function

        'Public Function CzytajBajty(Ilosc As Integer) As String
        '    Dim str As New StringBuilder
        '    Dim i As Integer = 0

        '    Do Until i = Ilosc OrElse Koniec()
        '        str.Append(Convert.ToChar(strumien.ReadByte))
        '        i += 1
        '    Loop

        '    Return str.ToString
        'End Function

        Public Function CzytajBajt() As Byte
            Return Convert.ToByte(strumien.ReadByte)
        End Function

        Public Function PeeknijBajt() As Byte
            Dim b As Byte = Convert.ToByte(strumien.ReadByte)
            strumien.Position -= 1
            Return b
        End Function

        Public Function Koniec() As Boolean
            Return strumien.Position >= strumien.Length
        End Function

        Public Sub ZamknijPlik()
            strumien.Close()
            strumien.Dispose()
            strumien = Nothing
        End Sub

        Private Function CzyJestBajt(Bajt As Byte, ParamArray Bajty() As Byte) As Boolean
            For i As Integer = 0 To Bajty.Length - 1
                If Bajt = Bajty(i) Then Return True
            Next

            Return False
        End Function

    End Class

End Namespace