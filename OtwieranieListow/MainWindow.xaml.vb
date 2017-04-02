Imports Microsoft.Win32
Imports Microsoft.VisualBasic.FileIO.FileSystem
Imports System.IO
Imports DLL = OtwieranieListowDll
Imports OtwieranieListowDll.KlasyListu.List

Public Class wndOkno

    Private PokazTresc As Boolean = True
    Private ZaznID As Integer = -1

#Region "Zdarzenia"

    Private Sub wndOknoGlowne_Loaded() Handles MyBase.Loaded
        SprawdzZasoby()

        Dim arg As String() = Environment.GetCommandLineArgs()
        If arg.Length > 1 Then
            DaneListu.Otworz(arg(arg.Length - 1), True)
            Title = TYTUL_OKNA & Path.GetFileName(arg(arg.Length - 1))
            PokazList()
        End If
    End Sub

    Private Sub wndOkno_Closed() Handles Me.Closed
        CzyscPliki()
    End Sub

    Private Sub mnuOtworz_Click() Handles mnuOtworz.Click
        Dim dlg As New OpenFileDialog
        dlg.Filter = "Pliki EML|*.eml"

        If dlg.ShowDialog() Then
            DaneListu.Otworz(dlg.FileName, True)
            Title = TYTUL_OKNA & Path.GetFileName(dlg.FileName)
            PokazList()
        End If
    End Sub

    Private Sub mnuPokazNaglowki_Click() Handles mnuPokazNaglowki.Click
        Dim wnd As New wndNaglowki
        wnd.ShowDialog()
    End Sub

    Private Sub PodwojneKlikniecie(sender As Object, e As MouseButtonEventArgs) Handles txtOd.MouseDoubleClick, txtDo.MouseDoubleClick, txtData.MouseDoubleClick, txtTemat.MouseDoubleClick
        Dim txt As TextBox = CType(sender, TextBox)
        txt.SelectAll()
    End Sub

    Private Sub OdznaczTekst(sender As Object, e As RoutedEventArgs) Handles txtOd.LostFocus, txtDo.LostFocus, txtData.LostFocus, txtTemat.LostFocus
        Dim txt As TextBox = CType(sender, TextBox)
        txt.Select(0, 0)
    End Sub

    Private Sub btnPokazOdbiorcow_Click() Handles btnPokazOdbiorcow.Click
        Dim wnd As New wndOdbiorcy
        wnd.ShowDialog()
    End Sub

    Private Sub cboPliki_SelectionChanged() Handles cboPliki.SelectionChanged
        If cboPliki.Items Is Nothing OrElse cboPliki.Items.Count = 0 OrElse Not PokazTresc Then Exit Sub

        Dim zazn As ElementComboBox = CType(cboPliki.SelectedItem, ElementComboBox)
        Dim el As DLL.KlasyListu.ElementListu = DaneListu.ZawartoscListu.Zawartosc(zazn.Id)

        If el.Zalacznik Then

            Dim nazwa As String
            If el.NazwaPliku = "" Then nazwa = el.Nazwa Else nazwa = el.NazwaPliku
            Dim sciezka As String = DLL.Dodatki.PobierzNieistniejacaSciezke(FOLDER_ZAŁĄCZNIKI, nazwa)
            If Not Directory.Exists(FOLDER_ZAŁĄCZNIKI) Then Directory.CreateDirectory(FOLDER_ZAŁĄCZNIKI)
            WriteAllBytes(sciezka, el.Zawartosc, False)
            Interaction.Shell("explorer.exe """ & sciezka & """", AppWinStyle.NormalFocus)

        Else

            ZaznID = zazn.Id

            Select Case el.Typ
                Case MIME_TEXT_HTML
                    PokazTextHtml(el)

                Case MIME_IMAGE_JPEG, MIME_IMAGE_GIF, MIME_IMAGE_PNG
                    PokazObraz(el)

                Case Else
                    PokazTextPlain(el, True)

            End Select

        End If
    End Sub

    Private Sub webList_LoadComplete() Handles webList.LoadCompleted
        If ZaznID = -1 Then Exit Sub

        Dim doc As mshtml.HTMLDocumentClass = CType(webList.Document, mshtml.HTMLDocumentClass)
        Dim obrazki As mshtml.IHTMLElementCollection = doc.getElementsByTagName("img")
        Dim obraz As mshtml.HTMLImgClass
        Dim nazwa As String

        'Adresy obrazkow
        For i As Integer = 0 To obrazki.length - 1
            obraz = CType(obrazki(i), mshtml.HTMLImgClass)

            If obraz.src.StartsWith("cid") Then
                obraz.src = DaneListu.ZawartoscListu.ZnajdzIZapiszElement(APPDATA_HTML, Mid(obraz.src, 5))
            Else
                nazwa = DaneListu.ZawartoscListu.ZnajdzIZapiszElement(APPDATA_HTML, obraz.src)
                If nazwa <> "" Then obraz.src = nazwa
            End If

        Next

    End Sub

#End Region 'Zdarzenia

#Region "Procedury"

    Private Sub PokazList()
        Dim str As String = ""

        With DaneListu.ZawartoscListu
            txtOd.Text = .Od
            txtData.Text = .Data
            txtTemat.Text = .Temat
        End With

        If DaneListu.ZawartoscListu.Do.IndexOf(vbCrLf) = -1 Then
            txtDo.Text = DaneListu.ZawartoscListu.Do
            btnPokazOdbiorcow.IsEnabled = False
        Else
            txtDo.Text = "(wielu odbiorców)"
            btnPokazOdbiorcow.IsEnabled = True
        End If

        cboPliki.Items.Clear()
        webList.Source = Nothing

        If DaneListu.ZawartoscListu.TypZawartosci.TypMime = MIME_TEXT_HTML Then
            PokazTypListu()
            PokazTextHtml(DaneListu.ZawartoscListu.Zawartosc(0))

        ElseIf DaneListu.ZawartoscListu.TypZawartosci.TypMime.StartsWith(MIME_IMAGE) Then
            PokazTypListu()
            PokazObraz(DaneListu.ZawartoscListu.Zawartosc(0))

        ElseIf DaneListu.ZawartoscListu.TypZawartosci.TypMime.StartsWith(MIME_MULTIPART) Then
            cboPliki.IsEnabled = True
            Dim text_id As Integer = -1
            Dim html_id As Integer = -1

            For i As Integer = 0 To DaneListu.ZawartoscListu.Zawartosc.Length - 1

                With DaneListu.ZawartoscListu.Zawartosc(i)
                    str = .Typ
                    If .Zalacznik Then
                        str &= " (Załącznik: "
                        If .NazwaPliku <> "" Then str &= .NazwaPliku Else str &= .Nazwa
                        str &= ")"
                    End If

                    If .Typ = MIME_TEXT_PLAIN Then text_id = i
                    If .Typ = MIME_TEXT_HTML Then html_id = i
                End With

                cboPliki.Items.Add(New ElementComboBox(str, i))
            Next

            If html_id <> -1 Then
                cboPliki.SelectedIndex = html_id
            ElseIf text_id <> -1 Then
                cboPliki.SelectedIndex = text_id
            End If

        Else
            PokazTypListu()
            PokazTextPlain(DaneListu.ZawartoscListu.Zawartosc(0), True)

        End If

    End Sub

    Private Sub PokazTypListu()
        PokazTresc = False
        cboPliki.Items.Add(DaneListu.ZawartoscListu.TypZawartosci.TypMime)
        cboPliki.SelectedIndex = 0
        cboPliki.IsEnabled = False
        PokazTresc = True
    End Sub

    Private Sub PokazTextPlain(element As DLL.KlasyListu.ElementListu, ZamienZnakiSpecjalne As Boolean)
        DLL.Dodatki.ZapiszListDoHTML(element.Zawartosc, element.KodowanieTekstu, APPDATA_HTML & DANE_LIST_TXT_ZAWARTOSC, APPDATA & DANE_LIST_TXT_SZABLON, ZamienZnakiSpecjalne)
        webList.Source = New Uri(APPDATA_HTML & DANE_LIST_TXT_ZAWARTOSC)
    End Sub

    Private Sub PokazTextHtml(element As DLL.KlasyListu.ElementListu)

        If DLL.Dodatki.CzyZaczyna(element.Zawartosc, {60, 72, 84, 77, 76}) OrElse DLL.Dodatki.CzyZaczyna(element.Zawartosc, {60, 104, 116, 109, 108}) Then '<HTML lub <html
            Dim sciezka_html As String = APPDATA_HTML & DANE_LIST_HTML_ZAWARTOSC
            WriteAllBytes(sciezka_html, element.Zawartosc, False)

            webList.Source = New Uri(sciezka_html)
        Else
            PokazTextPlain(element, False)
        End If

    End Sub

    Private Sub PokazObraz(element As DLL.KlasyListu.ElementListu)
        Dim nazwa As String = DLL.Dodatki.PobierzNazweObrazu(element)
        WriteAllBytes(APPDATA_HTML & nazwa, element.Zawartosc, False)
        webList.Source = New Uri(APPDATA_HTML & nazwa)
    End Sub

#End Region 'Procedury

#Region "Klasy"

    Private Class ElementComboBox
        Private Tekst_ As String
        Private Id_ As Integer

        Friend Sub New(Tekst As String, Id As Integer)
            Me.Tekst_ = Tekst
            Me.Id_ = Id
        End Sub

        Public Overrides Function ToString() As String
            Return Tekst_
        End Function

        Friend ReadOnly Property Id As Integer
            Get
                Return Me.Id_
            End Get
        End Property

    End Class

#End Region 'Klasy

End Class