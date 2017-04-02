Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.FileIO.FileSystem
Imports DLL = OtwieranieListowDll

Friend Module App
    Friend ReadOnly APPDATA As String = Environ("APPDATA") & "\marcin1315\OtwieranieListów\"
    Friend ReadOnly APPDATA_HTML As String = APPDATA & "HTML\"
    Friend ReadOnly FOLDER_ZAŁĄCZNIKI As String = SpecialDirectories.MyDocuments & "\Załączniki\"
    Friend Const DANE_NAGLOWKI_SZABLON As String = "naglowki.sth"
    Friend Const DANE_NAGLOWKI_HTML As String = "naglowki.html"
    Friend Const DANE_NAGLOWKI_CSS As String = "naglowki.css"
    Friend Const DANE_LIST_TXT_ZAWARTOSC As String = "pliktxt.html"
    Friend Const DANE_LIST_TXT_SZABLON As String = "pliktxt.sth"
    Friend Const DANE_LIST_HTML_ZAWARTOSC As String = "list.html"
    Friend Const TYTUL_OKNA As String = "Otwieranie listów - "

    Friend WithEvents DaneListu As New DLL.KlasyListu.List

    Friend Sub SprawdzZasoby()
        If Not Directory.Exists(APPDATA) Then Directory.CreateDirectory(APPDATA)
        If Not Directory.Exists(APPDATA_HTML) Then Directory.CreateDirectory(APPDATA_HTML)

        If Not File.Exists(APPDATA & DANE_NAGLOWKI_SZABLON) Then WriteAllBytes(APPDATA & DANE_NAGLOWKI_SZABLON, My.Resources.naglowki_sth, False)
        If Not File.Exists(APPDATA & DANE_NAGLOWKI_CSS) Then WriteAllText(APPDATA & DANE_NAGLOWKI_CSS, My.Resources.naglowki_css, False)
        If Not File.Exists(APPDATA & DANE_LIST_TXT_SZABLON) Then WriteAllBytes(APPDATA & DANE_LIST_TXT_SZABLON, My.Resources.pliktxt_sth, False)
    End Sub

    Friend Sub CzyscPliki()
        If File.Exists(APPDATA & DANE_NAGLOWKI_HTML) Then File.Delete(APPDATA & DANE_NAGLOWKI_HTML)

        Dim pliki As String() = Directory.GetFiles(APPDATA_HTML)

        For i As Integer = 0 To pliki.Length - 1
            File.Delete(pliki(i))
        Next
    End Sub

    Private Sub DaneListu_PokazBlad(Opis As String) Handles DaneListu.PokazBlad
        MessageBox.Show(Opis, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error)
    End Sub
End Module