Imports System.Net.Security
Imports System.Net.Sockets
Imports System.Security.Authentication
Imports System.Text
Imports System.Security.Cryptography.X509Certificates

Public Class KlientPOP3

    Private Const ODP_BLAD As String = "-ERR"
    Private Const ODP_OK As String = "+OK"

    Private sslStream As SslStream = Nothing

    Public Event PokazBlad(Blad As String)
    Public Event PokazStatus(Status As String)
    Public Event PokazKomunikat(Komunikat As String)
    Public Event PokazPostep(NrWiadomosci As Integer, LiczbaWiadomosci As Integer)

    Private Function ValidateServerCertificate(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) As Boolean
        If sslPolicyErrors = SslPolicyErrors.None Then Return True

        RaiseEvent PokazBlad("Błąd certyfikatu: " & sslPolicyErrors)
        Return False
    End Function

    Private Sub WyslijlWiadomosc(Wiadomosc As String)
        sslStream.Write(Encoding.UTF8.GetBytes(Wiadomosc))
    End Sub

    Private Sub WyslijlLinie(Wiadomosc As String)
        sslStream.Write(Encoding.UTF8.GetBytes(Wiadomosc & vbCrLf))
    End Sub

    Private Function CzytajLinie() As String
        Dim str As New StringBuilder
        Dim b As Char

        Do
            b = Convert.ToChar(sslStream.ReadByte())
            str.Append(b)
        Loop Until b = vbLf

        Return str.ToString
    End Function

    Private Function CzytajKomunikatBledu(Tekst As String) As String
        Return Mid(Tekst, ODP_BLAD.Length + 2)
    End Function

    Private Function UsunPusteZnaki() As Char
        Dim b As Char

        Do
            b = Convert.ToChar(sslStream.ReadByte())
            If Not (b = vbCr OrElse b = vbLf OrElse b = vbNullChar) Then Return b
        Loop
    End Function

    Private Function CzyKoniecListu(ByRef bajty As Byte(), dl As Integer) As Boolean
        Return dl = 3 AndAlso bajty(0) = 46 AndAlso bajty(1) = 13 AndAlso bajty(2) = 10 'kropka, CR, LF
    End Function

    Public Sub PobierzWiadomosci(Serwer As String, Port As UShort, Uzytkownik As String, Haslo As String, Folder As String)
        Dim serwerWiadomosc As String = ""
        Dim tekst As String = ""
        Dim id_wiad As New List(Of String)
        Dim id_wiad_tab As String()
        Dim linia_podzielona As String()
        Dim znak As Char
        Dim wiad As New StringBuilder("")
        Dim ilosc_wiad As Integer = 0
        Dim client As TcpClient

        RaiseEvent PokazStatus("Łączenie z serwerem...")

        'Polacz z serwerem
        Try
            client = New TcpClient(Serwer, Port)
        Catch
            RaiseEvent PokazBlad("Nie można połączyć z serwerem. Sprawdź połączenie internetowe oraz adres serwera i ponów próbę.")
            Exit Sub
        End Try

        sslStream = New SslStream(client.GetStream(), False, New RemoteCertificateValidationCallback(AddressOf ValidateServerCertificate), Nothing)

        Try
            sslStream.AuthenticateAsClient(Serwer)
        Catch e As AuthenticationException
            RaiseEvent PokazBlad("Nie udało się uwierzytelnić serwera.")
            sslStream.Dispose()
            sslStream = Nothing

            Try
                client.Close()
            Catch
            End Try

            Exit Sub
        End Try

        'Powitanie
        CzytajLinie()

        'Uzytkownik
        RaiseEvent PokazStatus("Logowanie...")

        WyslijlLinie("user " & Uzytkownik)
        serwerWiadomosc = CzytajLinie()

        If serwerWiadomosc.StartsWith(ODP_BLAD) Then
            RaiseEvent PokazBlad("Błąd połączenia - niepoprawna nazwa użytkownika." & vbCrLf & CzytajKomunikatBledu(serwerWiadomosc))
            sslStream.Dispose()
            sslStream = Nothing

            client.Close()
            Exit Sub
        End If

        'Haslo
        WyslijlLinie("pass " & Haslo)
        serwerWiadomosc = CzytajLinie()

        If serwerWiadomosc.StartsWith(ODP_BLAD) Then
            RaiseEvent PokazBlad("Błąd połączenia - niepoprawne hasło." & vbCrLf & CzytajKomunikatBledu(serwerWiadomosc))
            sslStream.Dispose()
            sslStream = Nothing
            client.Close()
            Exit Sub
        End If

        'Ilosc wiadomosci
        RaiseEvent PokazStatus("Pobieranie listy wiadomości...")

        WyslijlLinie("list")
        serwerWiadomosc = CzytajLinie()

        If serwerWiadomosc.StartsWith(ODP_BLAD) Then
            RaiseEvent PokazBlad("Błąd połączenia - nie można pobrać liczby wiadomości." & vbCrLf & CzytajKomunikatBledu(serwerWiadomosc))
            sslStream.Dispose()
            sslStream = Nothing
            client.Close()
            Exit Sub
        End If

        'Identyfikatory wiadomosci
        Do
            serwerWiadomosc = CzytajLinie()
            If serwerWiadomosc(0) = "."c Then Exit Do
            linia_podzielona = serwerWiadomosc.Split(" "c)
            id_wiad.Add(linia_podzielona(0))
        Loop

        id_wiad_tab = id_wiad.ToArray()
        ilosc_wiad = id_wiad_tab.Length

        'Utworz folder do pobieranych wiadomosci
        If Not Folder.EndsWith("\") Then Folder &= "\"
        Folder &= Now.ToString("yyyy-MM-dd H-mm-ss") & "\"
        My.Computer.FileSystem.CreateDirectory(Folder)

        RaiseEvent PokazStatus("Pobieranie wiadomości...")

        'Pobierz i zapisz wiadomosci
        For i As Integer = 0 To ilosc_wiad - 1

            'Wyslij zapytanie do pobrania jednej wiadomosci
            WyslijlLinie("retr " & id_wiad_tab(i))
            znak = UsunPusteZnaki()
            serwerWiadomosc = znak & CzytajLinie()

            If serwerWiadomosc.StartsWith(ODP_BLAD) Then
                Continue For
            End If

            wiad.Clear()

            'Pobierz i zapisz wiadomosc
            Try
                Dim plik As New IO.FileStream(Folder & (i + 1).ToString & ".eml", IO.FileMode.Create, IO.FileAccess.Write)
                Dim bajty(4095) As Byte
                Dim b As Byte
                Dim ix As Integer = 0

                Do
                    ix = 0

                    Do  'Pojedyncza linia
                        b = Convert.ToByte(sslStream.ReadByte)
                        bajty(ix) = b
                        ix += 1
                    Loop Until b = 10   'LF - koniec linii

                    plik.Write(bajty, 0, ix)

                Loop Until CzyKoniecListu(bajty, ix)

                plik.Close()

                RaiseEvent PokazPostep(i + 1, ilosc_wiad)

            Catch ex As Exception
                RaiseEvent PokazBlad("Błąd podczas zapisywania wiadomości nr " & (i + 1).ToString & ".")
                sslStream.Dispose()
                sslStream = Nothing
                client.Close()
                Exit Sub
            End Try

        Next

        'Zakoncz polaczenie
        sslStream = Nothing
        client.Close()
        RaiseEvent PokazKomunikat("Zakończono pobieranie " & ilosc_wiad.ToString & " wiadomości.")
    End Sub

End Class