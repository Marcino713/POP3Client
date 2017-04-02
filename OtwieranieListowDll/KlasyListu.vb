Imports System.IO
Imports OtwieranieListowDll.Dodatki
Imports Microsoft.VisualBasic.FileIO.FileSystem

Namespace KlasyListu

    Public Class Naglowek
        Private _Nazwa As String
        Private _Wartosc As String

        Public ReadOnly Property Nazwa As String
            Get
                Return _Nazwa
            End Get
        End Property

        Public ReadOnly Property Wartosc As String
            Get
                Return _Wartosc
            End Get
        End Property


        Public Sub New()
            Czysc()
        End Sub

        Public Sub New(Nazwa As String, Wartosc As String)
            _Nazwa = Nazwa
            _Wartosc = Wartosc
        End Sub

        Friend Sub Czysc()
            _Nazwa = ""
            _Wartosc = ""
        End Sub

        Public Overrides Function ToString() As String
            Return _Nazwa & "=" & _Wartosc
        End Function
    End Class



    Public Class DaneListu
        Private _Od As String
        Private _Do As String
        Private _Temat As String
        Private _Data As String
        Private _DataLiczba As Date
        Private _KodowanieTransportowe As String
        Private _TypZawartosci As TypWiadomosci
        Private _Zawartosc As New Tablica(Of ElementListu)

        Public Property Od As String
            Get
                Return _Od
            End Get
            Friend Set(value As String)
                _Od = value
            End Set
        End Property

        Public Property [Do] As String
            Get
                Return _Do
            End Get
            Friend Set(value As String)
                _Do = value
            End Set
        End Property

        Public Property Temat As String
            Get
                Return _Temat
            End Get
            Friend Set(value As String)
                _Temat = value
            End Set
        End Property

        Public Property Data As String
            Get
                Return _Data
            End Get
            Friend Set(value As String)
                _Data = value
            End Set
        End Property

        Public Property DataLiczba As Date
            Get
                Return _DataLiczba
            End Get
            Friend Set(value As Date)
                _DataLiczba = value
            End Set
        End Property

        Public Property KodowanieTransportowe As String
            Get
                Return _KodowanieTransportowe
            End Get
            Friend Set(value As String)
                _KodowanieTransportowe = value
            End Set
        End Property

        Public Property TypZawartosci As TypWiadomosci
            Get
                Return _TypZawartosci
            End Get
            Friend Set(value As TypWiadomosci)
                _TypZawartosci = value
            End Set
        End Property

        Public ReadOnly Property Zawartosc As Tablica(Of ElementListu)
            Get
                Return _Zawartosc
            End Get
        End Property


        Public Sub New()
            Czysc()
        End Sub

        Friend Sub Czysc()
            _Od = ""
            _Do = ""
            _Temat = ""
            _Data = ""
            _DataLiczba = Date.MinValue
            _KodowanieTransportowe = ""
            _TypZawartosci = Nothing
            _Zawartosc.Tab = Nothing
        End Sub

        ''' <summary>
        ''' Wyszukuje element, którego ID lub lokalizacja jest równa parametrowi NazwaElementu oraz zapisuje go w pliku.
        ''' </summary>
        ''' <param name="Sciezka">Scieżka do folderu, w którym zostanie zapisany element</param>
        ''' <param name="NazwaElementu">Lokalizacja lub ID szukanego elementu</param>
        ''' <returns>Ściezka do zapisanego pliku lub pusty ciąg znaków, jeśli element nie został odnaleziony</returns>
        Public Function ZnajdzIZapiszElement(Sciezka As String, NazwaElementu As String) As String
            Dim nazwa As String

            If Not Sciezka.EndsWith(Path.DirectorySeparatorChar) Then Sciezka &= Path.DirectorySeparatorChar

            For i As Integer = 0 To _Zawartosc.Tab.Length - 1

                With _Zawartosc.Tab(i)

                    If .Id = NazwaElementu OrElse .Lokalizacja = NazwaElementu Then
                        nazwa = PobierzNazweObrazu(_Zawartosc.Tab(i))

                        If File.Exists(Sciezka & nazwa) Then nazwa = Path.GetFileName(PobierzNieistniejacaSciezke(Sciezka, nazwa))

                        WriteAllBytes(Sciezka & nazwa, .Zawartosc, False)
                        Return nazwa
                    End If

                End With

            Next

            Return ""
        End Function

    End Class



    Public Class ParametryNaglowka
        Private _Wartosc As String
        Private _Parametry As New Tablica(Of Naglowek)

        Public Property Wartosc As String
            Get
                Return _Wartosc
            End Get
            Friend Set(value As String)
                _Wartosc = value
            End Set
        End Property

        Public ReadOnly Property Parametry As Tablica(Of Naglowek)
            Get
                Return _Parametry
            End Get
        End Property


        Public Sub New()
            Czysc()
        End Sub

        Friend Sub Czysc()
            _Wartosc = ""
            _Parametry.Tab = Nothing
        End Sub

        Public Overrides Function ToString() As String
            Return _Wartosc & ", długość=" & _Parametry.Length.ToString
        End Function
    End Class



    Public Class TypWiadomosci
        Private _TypMime As String
        Private _Kodowanie As String
        Private _Rozdzielacz As String
        Private _Nazwa As String

        Public Property TypMime As String
            Get
                Return _TypMime
            End Get
            Friend Set(value As String)
                _TypMime = value
            End Set
        End Property

        Public Property Kodowanie As String
            Get
                Return _Kodowanie
            End Get
            Friend Set(value As String)
                _Kodowanie = value
            End Set
        End Property

        Public Property Rozdzielacz As String
            Get
                Return _Rozdzielacz
            End Get
            Friend Set(value As String)
                _Rozdzielacz = value
            End Set
        End Property

        Public Property Nazwa As String
            Get
                Return _Nazwa
            End Get
            Friend Set(value As String)
                _Nazwa = value
            End Set
        End Property


        Public Sub New()
            Czysc()
        End Sub

        Friend Sub Czysc()
            _TypMime = ""
            _Kodowanie = ""
            _Rozdzielacz = ""
            _Nazwa = ""
        End Sub

        Public Overrides Function ToString() As String
            Return _TypMime
        End Function
    End Class



    Public Class ElementListu
        Private _Typ As String
        Private _KodowanieTekstu As String
        Private _KodowanieTransportowe As String
        Private _Id As String
        Private _Nazwa As String
        Private _NazwaPliku As String
        Private _Lokalizacja As String
        Private _Zakodowany As Boolean
        Private _Zalacznik As Boolean
        Private _Zawartosc As Byte()

        Public Property Typ As String
            Get
                Return _Typ
            End Get
            Friend Set(value As String)
                _Typ = value
            End Set
        End Property

        Public Property KodowanieTekstu As String
            Get
                Return _KodowanieTekstu
            End Get
            Friend Set(value As String)
                _KodowanieTekstu = value
            End Set
        End Property

        Public Property KodowanieTransportowe As String
            Get
                Return _KodowanieTransportowe
            End Get
            Friend Set(value As String)
                _KodowanieTransportowe = value
            End Set
        End Property

        Public Property Id As String
            Get
                Return _Id
            End Get
            Friend Set(value As String)
                _Id = value
            End Set
        End Property

        Public Property Nazwa As String
            Get
                Return _Nazwa
            End Get
            Friend Set(value As String)
                _Nazwa = value
            End Set
        End Property

        Public Property NazwaPliku As String
            Get
                Return _NazwaPliku
            End Get
            Friend Set(value As String)
                _NazwaPliku = value
            End Set
        End Property

        Public Property Lokalizacja As String
            Get
                Return _Lokalizacja
            End Get
            Friend Set(value As String)
                _Lokalizacja = value
            End Set
        End Property

        Friend Property Zakodowany As Boolean
            Get
                Return _Zakodowany
            End Get
            Set(value As Boolean)
                _Zakodowany = value
            End Set
        End Property

        Public Property Zalacznik As Boolean
            Get
                Return _Zalacznik
            End Get
            Friend Set(value As Boolean)
                _Zalacznik = value
            End Set
        End Property

        Public Property Zawartosc As Byte()
            Get

                If _Zakodowany Then
                    Select Case _KodowanieTransportowe
                        Case List.KODOWANIE_T_BASE64
                            _Zawartosc = Dekoduj_Base64(_Zawartosc)

                        Case List.KODOWANIE_T_QP
                            _Zawartosc = Dekoduj_QuotedPrintable(_Zawartosc)

                        Case Else
                            List.ZglosBlad("Nieznane kodowanie transportowe: " & _KodowanieTransportowe)

                    End Select

                    _Zakodowany = False
                End If

                Return _Zawartosc
            End Get
            Friend Set(value As Byte())
                _Zawartosc = value
            End Set
        End Property


        Public Sub New()
            Czysc()
        End Sub

        Friend Sub Czysc()
            _Typ = ""
            _KodowanieTekstu = ""
            _KodowanieTransportowe = ""
            _Id = ""
            _Nazwa = ""
            _NazwaPliku = ""
            _Lokalizacja = ""
            _Zakodowany = True
            _Zalacznik = False
            _Zawartosc = Nothing
        End Sub

        Public Overrides Function ToString() As String
            Return _Typ & ", nazwa=" & _NazwaPliku
        End Function
    End Class



    Public Class Tablica(Of T)
        Friend Tab As T()

        Default Public ReadOnly Property Item(ix As Integer) As T
            Get
                Return Tab(ix)
            End Get
        End Property

        Public ReadOnly Property Length As Integer
            Get
                If Tab Is Nothing Then Return 0 Else Return Tab.Length
            End Get
        End Property
    End Class

End Namespace