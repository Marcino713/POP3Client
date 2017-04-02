Imports System.Text
Imports OtwieranieListowDll.Dodatki

Namespace KlasyListu

    Partial Public Class List

        ''' <summary>
        ''' Otwiera i parsuje plik zawierający wiadomość internetową.
        ''' </summary>
        ''' <param name="Sciezka">Ścieżka do pliku, który ma zostać odczytany</param>
        ''' <param name="CzytajTresc">Jeśli True, odczytuje całą wiadomość; jeśli False, odczytuje tylko nagłówki listu</param>
        Public Sub Otworz(Sciezka As String, CzytajTresc As Boolean)
            Dim elem As ElementListu

            _ZawartoscListu = New DaneListu
            _Naglowki = New Tablica(Of Naglowek)

            'Otwarcie pliku
            Try
                Plik.OtworzPlik(Sciezka)
            Catch
                RaiseEvent PokazBlad("Błąd podczas otwierania pliku: " & Sciezka)
                Exit Sub
            End Try


            'Nagłówki
            Naglowki.Tab = CzytajNaglowki()

            For i As Integer = 0 To Naglowki.Tab.Length - 1
                SprawdzNaglowek(Naglowki.Tab(i))
            Next


            'Treść
            If CzytajTresc Then

                If CzyJest(ZawartoscListu.TypZawartosci.TypMime, MIME_TEXT_PLAIN, MIME_TEXT_HTML) OrElse ZawartoscListu.TypZawartosci.TypMime.StartsWith(MIME_IMAGE) Then

                    elem = New ElementListu
                    elem.Zawartosc = Plik.CzytajBajty(vbCrLf & "." & vbCrLf)
                    elem.KodowanieTekstu = ZawartoscListu.TypZawartosci.Kodowanie
                    elem.KodowanieTransportowe = ZawartoscListu.KodowanieTransportowe

                    If CzyJest(ZawartoscListu.KodowanieTransportowe, KODOWANIE_T_7BIT, KODOWANIE_T_8BIT, "") Then
                        elem.Zakodowany = False
                    End If

                    ReDim ZawartoscListu.Zawartosc.Tab(0)
                    ZawartoscListu.Zawartosc.Tab(0) = elem

                ElseIf ZawartoscListu.TypZawartosci.TypMime.StartsWith(MIME_MULTIPART) Then
                    ZawartoscListu.Zawartosc.Tab = CzytajCzesciListu(ZawartoscListu.TypZawartosci.Rozdzielacz)

                Else
                    RaiseEvent PokazBlad("Nieznany typ wiadomości:" & vbCrLf & ZawartoscListu.TypZawartosci.TypMime)

                End If

            End If

            Plik.ZamknijPlik()
        End Sub

        ''' <summary>
        ''' Funkcja odczytuje nagłówki wiadomości lub części wiadomości.
        ''' </summary>
        Private Function CzytajNaglowki() As Naglowek()
            Dim naglowki As New List(Of Naglowek)
            Dim linia As String
            Dim stb As New StringBuilder("")
            Dim nazwa As String = ""
            Dim nagl As Naglowek = Nothing
            Dim pozycja As Integer = 0

            linia = Plik.CzytajLinieTekstu()

            Do Until linia = ""

                'Czy ciąg dalszy nagłówka
                If linia(0) = " " Or linia(0) = vbTab Then
                    stb.AppendLine()
                    stb.Append(linia.Trim())
                    linia = Plik.CzytajLinieTekstu
                    Continue Do
                End If

                If nazwa <> "" Then
                    nagl = New Naglowek(nazwa, stb.ToString)
                    naglowki.Add(nagl)
                    stb.Clear()
                End If

                'Nagłówek
                pozycja = linia.IndexOf(":")
                nazwa = Mid(linia, 1, pozycja)
                stb.Append(Mid(linia, pozycja + 2).Trim)
                linia = Plik.CzytajLinieTekstu
            Loop

            'Ostatni nagłówek
            nagl = New Naglowek(nazwa, stb.ToString)
            naglowki.Add(nagl)

            Return naglowki.ToArray()
        End Function

        ''' <summary>
        ''' Odczytuje elementy (oraz ich ewentualne podelementy) wiadomości wieloczęściowej.
        ''' </summary>
        ''' <param name="Rozdzielacz">Ciąg znaków oddzielający części listu</param>
        Private Function CzytajCzesciListu(Rozdzielacz As String) As ElementListu()
            Dim czesci As New List(Of ElementListu)
            Dim naglowki As Naglowek()
            Dim elem_listu As ElementListu
            Dim rozdzielacz_zagniezdzony As String = ""
            Dim rozdz As String = ""
            Dim czesci_tab As ElementListu()

            'Pomiń separator
            Plik.CzytajBajty("--" & Rozdzielacz)

            Do
                'Pomiń białe znaki
                Plik.CzytajLinieTekstu()

                elem_listu = New ElementListu
                rozdzielacz_zagniezdzony = ""


                'Nagłówki
                naglowki = CzytajNaglowki()

                For i As Integer = 0 To naglowki.Length - 1
                    rozdz = SprawdzNaglowekCzesci(naglowki(i), elem_listu)
                    If rozdz <> "" Then rozdzielacz_zagniezdzony = rozdz
                Next


                'Zawartość
                If rozdzielacz_zagniezdzony <> "" Then  'Zawartość zagnieżdżona

                    czesci_tab = CzytajCzesciListu(rozdzielacz_zagniezdzony)

                    For i As Integer = 0 To czesci_tab.Length - 1
                        czesci.Add(czesci_tab(i))
                    Next

                    Plik.CzytajBajty("--" & Rozdzielacz)

                Else    'Zawartość zwykła

                    elem_listu.Zawartosc = Plik.CzytajBajty("--" & Rozdzielacz)
                    If CzyJest(elem_listu.KodowanieTransportowe, KODOWANIE_T_7BIT, KODOWANIE_T_8BIT, "") Then
                        elem_listu.Zakodowany = False
                    End If
                    czesci.Add(elem_listu)

                End If

                'Czy koniec
                If Plik.PeeknijBajt = 45 Then  '-
                    Plik.CzytajLinieTekstu()
                    Exit Do
                End If

            Loop

            Return czesci.ToArray()
        End Function

        ''' <summary>
        ''' Sprawdza nagłówek listu i uzupełnia odpowiednie pole przekazanego obiektu.
        ''' </summary>
        Private Sub SprawdzNaglowek(ByRef nagl As Naglowek)
            Select Case nagl.Nazwa.ToLower
                Case "from"
                    ZawartoscListu.Od = DekodujAdresyEmail(nagl.Wartosc)

                Case "to"
                    ZawartoscListu.Do &= DekodujAdresyEmail(nagl.Wartosc)

                Case "cc"
                    ZawartoscListu.Do &= vbCrLf & vbCrLf & "Do wiadomości:" & vbCrLf & DekodujAdresyEmail(nagl.Wartosc)

                Case "bcc"
                    ZawartoscListu.Do &= vbCrLf & vbCrLf & "Ukryte do wiadomości:" & vbCrLf & DekodujAdresyEmail(nagl.Wartosc)

                Case "subject"
                    ZawartoscListu.Temat = DekodujWartoscNaglowka(nagl.Wartosc)

                Case "date"
                    Try
                        ZawartoscListu.DataLiczba = Date.Parse(nagl.Wartosc)
                        ZawartoscListu.Data = ZawartoscListu.DataLiczba.ToString("d MMMM yyyy H:mm:ss, dddd")
                    Catch
                        ZawartoscListu.Data = nagl.Wartosc
                    End Try

                Case "content-transfer-encoding"
                    ZawartoscListu.KodowanieTransportowe = nagl.Wartosc.ToLower

                Case "content-type"
                    ZawartoscListu.TypZawartosci = CzytajTypZawartosci(nagl.Wartosc)

            End Select
        End Sub

        ''' <summary>
        ''' Sprawdza nagłówek części listu i uzupełnia odpowiednie pole przekazanego obiektu. Jeśli nagłówek zawiera pole boundary, funkcja zwraca jego wartość
        ''' </summary>
        Private Function SprawdzNaglowekCzesci(ByRef nagl As Naglowek, ByRef elem As ElementListu) As String
            Dim rozdzielacz As String = ""
            Dim param As New ParametryNaglowka
            Dim typ As TypWiadomosci

            Select Case nagl.Nazwa.ToLower
                Case "content-transfer-encoding"
                    elem.KodowanieTransportowe = nagl.Wartosc.ToLower

                Case "content-type"
                    typ = CzytajTypZawartosci(nagl.Wartosc)

                    elem.Typ = typ.TypMime
                    elem.Nazwa = typ.Nazwa
                    elem.KodowanieTekstu = typ.Kodowanie
                    rozdzielacz = typ.Rozdzielacz

                Case "content-disposition"
                    param = ParsujNaglowek(nagl.Wartosc)
                    If param.Wartosc.ToLower = "attachment" Then elem.Zalacznik = True

                    For i As Integer = 0 To param.Parametry.Length - 1

                        With param.Parametry(i)
                            Select Case .Nazwa.ToLower
                                Case "filename"
                                    elem.NazwaPliku = DekodujWartoscNaglowka(.Wartosc)

                            End Select
                        End With

                    Next

                Case "content-id"
                    Dim stb As New StringBuilder("")
                    Dim i As Integer = 0

                    'Dekoduje wartość zapisaną w formacie <wartosc>
                    Do Until i >= nagl.Wartosc.Length OrElse nagl.Wartosc(i) = "<"c
                        i += 1
                    Loop
                    i += 1

                    Do Until i >= nagl.Wartosc.Length OrElse nagl.Wartosc(i) = ">"c
                        stb.Append(nagl.Wartosc(i))
                        i += 1
                    Loop

                    elem.Id = stb.ToString

                Case "content-location"
                    elem.Lokalizacja = nagl.Wartosc

            End Select

            Return rozdzielacz

        End Function

        ''' <summary>
        ''' Odczytuje informacje z nagłówka Content-Type
        ''' </summary>
        Private Function CzytajTypZawartosci(WartoscContentType As String) As TypWiadomosci
            Dim typ As New TypWiadomosci

            Dim param As ParametryNaglowka = ParsujNaglowek(WartoscContentType)
            typ.TypMime = param.Wartosc

            For i As Integer = 0 To param.Parametry.Length - 1

                With param.Parametry(i)
                    Select Case .Nazwa
                        Case "charset" : typ.Kodowanie = .Wartosc.ToLower
                        Case "boundary" : typ.Rozdzielacz = .Wartosc
                        Case "name" : typ.Nazwa = .Wartosc
                    End Select
                End With

            Next

            Return typ
        End Function

        ''' <summary>
        ''' Parsuje wartość nagłówka, która składa się z listy elementów rozdzielonych przecinkami lub średnikami
        ''' (nazwa; par1=wart1; par2=wart2)
        ''' </summary>
        Private Function ParsujNaglowek(Wartosc As String) As ParametryNaglowka
            Dim str As New StringBuilder("")
            Dim param As New ParametryNaglowka
            Dim par As Naglowek
            Dim nazwa As String
            Dim i As Integer = 0
            Dim l As Integer = Wartosc.Length
            Dim lista As New List(Of Naglowek)

            'Typ
            Do Until i >= l OrElse CzyJestZnak(Wartosc(i), ";"c, ","c, " "c, vbTab(0), vbCr(0))
                str.Append(Wartosc(i))
                i += 1
            Loop
            i += 1

            param.Wartosc = str.ToString.ToLower
            str.Clear()

            'Parametry
            Do Until i >= l
                If CzyJestZnak(Wartosc(i), " "c, ";"c, ","c, vbTab(0), vbCr(0), vbLf(0)) Then
                    i += 1
                    Continue Do
                End If

                'Nazwa parametru
                Do Until i >= l OrElse CzyJestZnak(Wartosc(i), "="c, " "c, vbTab(0), vbCr(0), vbLf(0))
                    str.Append(Wartosc(i))
                    i += 1
                Loop
                i += 1

                nazwa = str.ToString
                str.Clear()

                'Pomiń białe znaki
                Do Until i >= l OrElse (Not CzyJestZnak(Wartosc(i), "="c, " "c, vbTab(0), vbCr(0), vbLf(0)))
                    i += 1
                Loop

                'Wartosc
                If Wartosc(i) = """" Then

                    i += 1  'pomiń "

                    Do Until i >= l OrElse Wartosc(i) = """"
                        str.Append(Wartosc(i))
                        i += 1
                    Loop

                    i += 1 'pomiń "

                Else

                    Do Until i >= l OrElse CzyJestZnak(Wartosc(i), ";"c, ","c, " "c, vbTab(0), vbCr(0), vbLf(0))
                        str.Append(Wartosc(i))
                        i += 1
                    Loop

                End If

                'Zapisz wartosc
                par = New Naglowek(nazwa.ToLower, str.ToString)
                lista.Add(par)

                str.Clear()
            Loop

            param.Parametry.Tab = lista.ToArray

            Return param
        End Function

        ''' <summary>
        ''' Odczytuje, dekoduje i łączy w jedną linię tekst, który może składać się z wielu lini, każda linia jest zwykłym tekstem lub zakodowanym słowem w postaci
        ''' =?kodowanie?kodowanie_t?tekst?=
        ''' </summary>
        Private Function DekodujWartoscNaglowka(Wartosc As String) As String
            Dim str As String() = Wartosc.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            Dim sb As New StringBuilder("")

            For i As Integer = 0 To str.Length - 1

                If str(i).Contains("=?") Then
                    sb.Append(DekodujSlowo(str(i)))
                Else
                    sb.Append(str(i))
                End If

            Next

            Return sb.ToString
        End Function

        ''' <summary>
        ''' Dekoduje linię tekstu zapisaną w postaci =?kodowanie?kodowanie_t?tekst?=
        ''' </summary>
        Private Function DekodujSlowo(Tekst As String) As String
            Dim str As New StringBuilder("")
            Dim i As Integer = 0
            Dim l As Integer = Tekst.Length
            Dim KodowanieTekstu As String = ""
            Dim KodowanieTransportowe As String = ""
            Dim Zawartosc As String = ""
            Dim b As Byte()

            'Znaki na poczatku
            Do Until i >= l OrElse Tekst(i) = "?"
                i += 1
            Loop
            i += 1

            'Kodowanie tekstu
            Do Until i >= l OrElse Tekst(i) = "?"
                str.Append(Tekst(i))
                i += 1
            Loop
            i += 1

            KodowanieTekstu = str.ToString.ToLower
            str.Clear()

            'Kodowanie transportowe
            Do Until i >= l OrElse Tekst(i) = "?"
                str.Append(Tekst(i))
                i += 1
            Loop
            i += 1

            KodowanieTransportowe = str.ToString.ToLower
            str.Clear()

            'Zawartosc
            Do Until i >= l OrElse Tekst(i) = "?"
                str.Append(Tekst(i))
                i += 1
            Loop

            Select Case KodowanieTransportowe
                Case "q"
                    b = Encoding.UTF8.GetBytes(str.Replace("_"c, " "c).ToString)
                    b = Dekoduj_QuotedPrintable(b)

                Case "b"
                    b = Encoding.UTF8.GetBytes(str.ToString)
                    b = Dekoduj_Base64(b)

                Case Else
                    b = Encoding.UTF8.GetBytes(str.ToString)

            End Select

            If b IsNot Nothing Then Zawartosc = Encoding.GetEncoding(KodowanieTekstu).GetString(b)

            Return Zawartosc
        End Function

        Private Function DekodujAdresyEmail(Adresy As String) As String
            Dim i As Integer = 0
            Dim l As Integer = Adresy.Length
            Dim lista As New StringBuilder("")
            Dim poz As Integer

            Do

                'Pomin biale znaki
                Do While i <l AndAlso CzyJestZnak(Adresy(i), {" "c, ","c, vbTab(0), vbCr(0), vbLf(0)})
                    If Adresy(i) = ","c Then lista.Append(vbCrLf)
                    i += 1
                Loop

                Select Case Adresy(i)
                    Case """"c  'Nazwa w cudzyslowiu

                        i += 1

                        Do Until i >= l OrElse Adresy(i) = """"c
                            lista.Append(Adresy(i))
                            i += 1
                        Loop

                        i += 1
                        lista.Append(" "c)

                    Case "="c   'Nazwa zakodowana
                        poz = Adresy.IndexOf("?=", i)
                        lista.Append(DekodujSlowo(Mid(Adresy, i + 1, poz - i)))
                        lista.Append(" "c)
                        i = poz + 2

                    Case "<"c   'Adres

                        Do Until i >= l OrElse Adresy(i) = ">"c
                            lista.Append(Adresy(i))
                            i += 1
                        Loop

                        i += 1
                        lista.Append(">"c)

                    Case Else   'Nazwa bez cudzyslowia

                        Do Until i >= l OrElse CzyJestZnak(Adresy(i), "<"c, ","c)
                            lista.Append(Adresy(i))
                            i += 1
                        Loop


                End Select


            Loop Until i >= l

            Return lista.ToString.Trim
        End Function

    End Class

End Namespace