Imports Microsoft.Win32

Friend Class wndOkno

    Const REJESTR_SCIEZKA As String = "Software\Marcin1315\PobieranieMaili"
    Const REJESTR_SERWER As String = "Serwer"
    Const REJESTR_PORT As String = "Port"
    Const REJESTR_UZYTKOWNIK As String = "Użytkownik"
    Const REJESTR_FOLDER As String = "Folder"

    Const REJESTR_D_SERWER As String = ""
    Const REJESTR_D_PORT As String = "995"
    Const REJESTR_D_UZYTKOWNIK As String = ""
    ReadOnly REJESTR_D_FOLDER As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\Listy"

    Private dane_zmienione As Boolean = False
    Private WithEvents klient As PobieranieListowDll.KlientPOP3


#Region "Obsługa formularza"
    Private Sub wndOkno_Load() Handles Me.Load
        CzytajUstawienia()
        CzyscKontrolki()
        klient = New PobieranieListowDll.KlientPOP3
    End Sub

    Private Sub txt_TextChanged() Handles txtSerwer.TextChanged, txtPort.TextChanged, txtUzytkownik.TextChanged, txtFolder.TextChanged
        dane_zmienione = True
    End Sub

    Private Sub btnOtworz_Click() Handles btnOtworz.Click
        Shell("explorer.exe " & txtFolder.Text, AppWinStyle.NormalFocus)
    End Sub

    Private Sub btnPrzegladaj_Click() Handles btnPrzegladaj.Click
        dlgFolder.SelectedPath = txtFolder.Text
        If dlgFolder.ShowDialog = DialogResult.OK Then txtFolder.Text = dlgFolder.SelectedPath
    End Sub

    Private Sub wndOkno_FormClosed() Handles Me.FormClosed
        If dane_zmienione Then ZapiszUstawienia()
    End Sub

    Private Sub btnPobierz_Click() Handles btnPobierz.Click
        Dim serwer As String = txtSerwer.Text
        Dim port As UShort
        Dim uzytkownik As String = txtUzytkownik.Text
        Dim haslo As String = txtHaslo.Text
        Dim folder As String = txtFolder.Text

        Try
            port = UShort.Parse(txtPort.Text)
        Catch ex As Exception
            klient_PokazBlad("Podano nieprawidłowy numer portu." & vbCrLf & "Wartość musi być liczbą całkowitą z zakresu 0 - 65535.")
            Exit Sub
        End Try

        If dane_zmienione Then ZapiszUstawienia()
        klient.PobierzWiadomosci(serwer, port, uzytkownik, haslo, folder)
        CzyscKontrolki()
    End Sub
#End Region 'Obsługa formualrza


#Region "Dodatkowe procedury"
    Private Sub CzyscKontrolki()
        lblStan.Text = ""
        lblIlosc.Text = ""
        lblIlosc.Visible = False
        prgPobieranie.Value = 0
    End Sub

    Private Sub CzytajUstawienia()
        Dim r As RegistryKey = My.Computer.Registry.CurrentUser.CreateSubKey(REJESTR_SCIEZKA)
        txtSerwer.Text = CType(r.GetValue(REJESTR_SERWER, REJESTR_D_SERWER), String)
        txtPort.Text = CType(r.GetValue(REJESTR_PORT, REJESTR_D_PORT), String)
        txtUzytkownik.Text = CType(r.GetValue(REJESTR_UZYTKOWNIK, REJESTR_D_UZYTKOWNIK), String)
        txtFolder.Text = CType(r.GetValue(REJESTR_FOLDER, REJESTR_D_FOLDER), String)
        r.Close()
        dane_zmienione = False
    End Sub

    Private Sub ZapiszUstawienia()
        Dim r As RegistryKey = My.Computer.Registry.CurrentUser.CreateSubKey(REJESTR_SCIEZKA)
        r.SetValue(REJESTR_SERWER, txtSerwer.Text, RegistryValueKind.String)
        r.SetValue(REJESTR_PORT, txtPort.Text, RegistryValueKind.String)
        r.SetValue(REJESTR_UZYTKOWNIK, txtUzytkownik.Text, RegistryValueKind.String)
        r.SetValue(REJESTR_FOLDER, txtFolder.Text, RegistryValueKind.String)
        r.Close()
        dane_zmienione = False
    End Sub
#End Region 'Dodatkowe procedury


#Region "Obsługa zdarzeń"
    Private Sub klient_PokazStatus(Status As String) Handles klient.PokazStatus
        lblStan.Text = Status
        Application.DoEvents()
    End Sub

    Private Sub klient_PokazPostep(NrWiadomosci As Integer, LiczbaWiadomosci As Integer) Handles klient.PokazPostep
        prgPobieranie.Maximum = LiczbaWiadomosci
        prgPobieranie.Value = NrWiadomosci
        lblIlosc.Visible = True
        lblIlosc.Text = NrWiadomosci.ToString & " z " & LiczbaWiadomosci.ToString
        lblStan.Text = "Pobrano:"
        Application.DoEvents()

        If NrWiadomosci = LiczbaWiadomosci Then CzyscKontrolki()
    End Sub

    Private Sub klient_PokazKomunikat(Komunikat As String) Handles klient.PokazKomunikat
        CzyscKontrolki()
        MessageBox.Show(Komunikat, "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub klient_PokazBlad(Blad As String) Handles klient.PokazBlad
        CzyscKontrolki()
        MessageBox.Show(Blad, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

#End Region 'Obsługa zdarzeń

End Class