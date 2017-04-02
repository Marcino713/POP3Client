Public Class wndNaglowki

    Private Sub wndNaglowki_Loaded() Handles wndNaglowki.Loaded
        OtwieranieListowDll.Dodatki.ZapiszNaglowkiDoHTML(DaneListu.Naglowki, APPDATA & DANE_NAGLOWKI_HTML, APPDATA & DANE_NAGLOWKI_SZABLON)
        webNaglowki.Source = New Uri(APPDATA & DANE_NAGLOWKI_HTML)
    End Sub

End Class