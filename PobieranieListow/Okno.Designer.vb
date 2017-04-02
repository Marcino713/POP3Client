<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class wndOkno
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtSerwer = New System.Windows.Forms.TextBox()
        Me.txtPort = New System.Windows.Forms.TextBox()
        Me.txtUzytkownik = New System.Windows.Forms.TextBox()
        Me.txtHaslo = New System.Windows.Forms.TextBox()
        Me.btnPobierz = New System.Windows.Forms.Button()
        Me.lblStan = New System.Windows.Forms.Label()
        Me.lblIlosc = New System.Windows.Forms.Label()
        Me.prgPobieranie = New System.Windows.Forms.ProgressBar()
        Me.btnPrzegladaj = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtFolder = New System.Windows.Forms.TextBox()
        Me.btnOtworz = New System.Windows.Forms.Button()
        Me.dlgFolder = New System.Windows.Forms.FolderBrowserDialog()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(43, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Serwer:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 41)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(29, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Port:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 67)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(65, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Użytkownik:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 93)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(39, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Hasło:"
        '
        'txtSerwer
        '
        Me.txtSerwer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSerwer.Location = New System.Drawing.Point(83, 12)
        Me.txtSerwer.Name = "txtSerwer"
        Me.txtSerwer.Size = New System.Drawing.Size(189, 20)
        Me.txtSerwer.TabIndex = 1
        '
        'txtPort
        '
        Me.txtPort.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPort.Location = New System.Drawing.Point(83, 38)
        Me.txtPort.Name = "txtPort"
        Me.txtPort.Size = New System.Drawing.Size(189, 20)
        Me.txtPort.TabIndex = 2
        '
        'txtUzytkownik
        '
        Me.txtUzytkownik.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtUzytkownik.Location = New System.Drawing.Point(83, 64)
        Me.txtUzytkownik.Name = "txtUzytkownik"
        Me.txtUzytkownik.Size = New System.Drawing.Size(189, 20)
        Me.txtUzytkownik.TabIndex = 3
        '
        'txtHaslo
        '
        Me.txtHaslo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtHaslo.Location = New System.Drawing.Point(83, 90)
        Me.txtHaslo.Name = "txtHaslo"
        Me.txtHaslo.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtHaslo.Size = New System.Drawing.Size(189, 20)
        Me.txtHaslo.TabIndex = 4
        '
        'btnPobierz
        '
        Me.btnPobierz.Location = New System.Drawing.Point(197, 171)
        Me.btnPobierz.Name = "btnPobierz"
        Me.btnPobierz.Size = New System.Drawing.Size(75, 23)
        Me.btnPobierz.TabIndex = 8
        Me.btnPobierz.Text = "Pobierz"
        Me.btnPobierz.UseVisualStyleBackColor = True
        '
        'lblStan
        '
        Me.lblStan.AutoSize = True
        Me.lblStan.Location = New System.Drawing.Point(12, 176)
        Me.lblStan.Name = "lblStan"
        Me.lblStan.Size = New System.Drawing.Size(50, 13)
        Me.lblStan.TabIndex = 9
        Me.lblStan.Text = "Pobrano:"
        '
        'lblIlosc
        '
        Me.lblIlosc.AutoSize = True
        Me.lblIlosc.Location = New System.Drawing.Point(80, 176)
        Me.lblIlosc.Name = "lblIlosc"
        Me.lblIlosc.Size = New System.Drawing.Size(42, 13)
        Me.lblIlosc.TabIndex = 10
        Me.lblIlosc.Text = "10 z 20"
        '
        'prgPobieranie
        '
        Me.prgPobieranie.Location = New System.Drawing.Point(12, 200)
        Me.prgPobieranie.Name = "prgPobieranie"
        Me.prgPobieranie.Size = New System.Drawing.Size(260, 23)
        Me.prgPobieranie.TabIndex = 11
        '
        'btnPrzegladaj
        '
        Me.btnPrzegladaj.Location = New System.Drawing.Point(197, 142)
        Me.btnPrzegladaj.Name = "btnPrzegladaj"
        Me.btnPrzegladaj.Size = New System.Drawing.Size(75, 23)
        Me.btnPrzegladaj.TabIndex = 7
        Me.btnPrzegladaj.Text = "Przeglądaj..."
        Me.btnPrzegladaj.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 119)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(39, 13)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Folder:"
        '
        'txtFolder
        '
        Me.txtFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFolder.Location = New System.Drawing.Point(83, 116)
        Me.txtFolder.Name = "txtFolder"
        Me.txtFolder.Size = New System.Drawing.Size(189, 20)
        Me.txtFolder.TabIndex = 5
        '
        'btnOtworz
        '
        Me.btnOtworz.Location = New System.Drawing.Point(116, 142)
        Me.btnOtworz.Name = "btnOtworz"
        Me.btnOtworz.Size = New System.Drawing.Size(75, 23)
        Me.btnOtworz.TabIndex = 6
        Me.btnOtworz.Text = "Otwórz"
        Me.btnOtworz.UseVisualStyleBackColor = True
        '
        'wndOkno
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 237)
        Me.Controls.Add(Me.btnOtworz)
        Me.Controls.Add(Me.txtFolder)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.btnPrzegladaj)
        Me.Controls.Add(Me.prgPobieranie)
        Me.Controls.Add(Me.lblIlosc)
        Me.Controls.Add(Me.lblStan)
        Me.Controls.Add(Me.btnPobierz)
        Me.Controls.Add(Me.txtHaslo)
        Me.Controls.Add(Me.txtUzytkownik)
        Me.Controls.Add(Me.txtPort)
        Me.Controls.Add(Me.txtSerwer)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "wndOkno"
        Me.Text = "Pobieranie listów"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents txtSerwer As TextBox
    Friend WithEvents txtPort As TextBox
    Friend WithEvents txtUzytkownik As TextBox
    Friend WithEvents txtHaslo As TextBox
    Friend WithEvents btnPobierz As Button
    Friend WithEvents lblStan As Label
    Friend WithEvents lblIlosc As Label
    Friend WithEvents prgPobieranie As ProgressBar
    Friend WithEvents btnPrzegladaj As Button
    Friend WithEvents Label6 As Label
    Friend WithEvents txtFolder As TextBox
    Friend WithEvents btnOtworz As Button
    Friend WithEvents dlgFolder As FolderBrowserDialog
End Class
