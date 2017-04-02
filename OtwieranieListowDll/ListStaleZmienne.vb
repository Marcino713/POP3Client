Imports OtwieranieListowDll.Dodatki

Namespace KlasyListu

    Partial Public Class List

        Public Const MIME_TEXT_HTML As String = "text/html"
        Public Const MIME_TEXT_PLAIN As String = "text/plain"
        'Public Const MIME_MULTIPART_MIXED As String = "multipart/mixed"
        'Public Const MIME_MULTIPART_ALTERNATIVE As String = "multipart/alternative"
        'Public Const MIME_MULTIPART_RELATED As String = "multipart/related"
        Public Const MIME_MULTIPART As String = "multipart/"
        Public Const MIME_IMAGE_JPEG As String = "image/jpeg"
        Public Const MIME_IMAGE_GIF As String = "image/gif"
        Public Const MIME_IMAGE_PNG As String = "image/png"
        Public Const MIME_IMAGE As String = "image/"

        Public Const KODOWANIE_T_7BIT As String = "7bit"
        Public Const KODOWANIE_T_8BIT As String = "8bit"
        Public Const KODOWANIE_T_BASE64 As String = "base64"
        Public Const KODOWANIE_T_QP As String = "quoted-printable"


        Private _ZawartoscListu As New DaneListu
        Private _Naglowki As New Tablica(Of Naglowek)


        Public ReadOnly Property ZawartoscListu As DaneListu
            Get
                Return _ZawartoscListu
            End Get
        End Property

        Public ReadOnly Property Naglowki As Tablica(Of Naglowek)
            Get
                Return _Naglowki
            End Get
        End Property

        Private Plik As New OtwieraniePliku

        ''' <summary>
        ''' Zachodzi, jeśli podczas otwierania lub parsowania listu wystąpi błąd.
        ''' </summary>
        ''' <param name="Opis">Komunikat wyświetlany użytkownikowi programu</param>
        Public Shared Event PokazBlad(Opis As String)

        Public Shared Sub ZglosBlad(Tekst As String)
            RaiseEvent PokazBlad(Tekst)
        End Sub

    End Class

End Namespace