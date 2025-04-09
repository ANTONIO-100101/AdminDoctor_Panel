Imports Guna.UI2.WinForms
Imports OtpNet

Public Class OTP_Modal
    Inherits Form

    Private user As New UserModel()
    Public Event SavePass As Action(Of String) ' Declare the event here
    Private otp As String
    Private totp As Totp
    Private email As String

    Private verifyBoxes As Guna2TextBox()
    Private rawOTP As String() = New String(5) {}
    Private emailInput As Form

    Public Sub New(totp As Totp, emailInput As Form, email As String)
        Me.totp = totp
        Me.emailInput = emailInput
        InitializeComponent()
        SubscribeTextChanged()

        Me.verifyBoxes = New Guna2TextBox() {verifyBox1, verifyBox2, verifyBox3, verifyBox4, verifyBox5, verifyBox6}
        Me.email = email
    End Sub

    Private Sub SubscribeTextChanged()
        AddHandler verifyBox1.TextChanged, AddressOf VerifyBoxesTextChanged
        AddHandler verifyBox2.TextChanged, AddressOf VerifyBoxesTextChanged
        AddHandler verifyBox3.TextChanged, AddressOf VerifyBoxesTextChanged
        AddHandler verifyBox4.TextChanged, AddressOf VerifyBoxesTextChanged
        AddHandler verifyBox5.TextChanged, AddressOf VerifyBoxesTextChanged
        AddHandler verifyBox6.TextChanged, AddressOf VerifyBoxesTextChanged
    End Sub

    Private Sub VerifyBoxesTextChanged(sender As Object, e As EventArgs)
        Dim box As Control = CType(sender, Control)
        Dim index As Integer = Integer.Parse(box.Tag.ToString())
        rawOTP(index) = box.Text

        If box.Text <> "" AndAlso index < 5 Then
            verifyBoxes(index + 1).Focus()
        End If
        submitBtn.Enabled = rawOTP.All(Function(text) Not String.IsNullOrEmpty(text?.Trim()))
    End Sub

    Private Sub submitBtn_Click(sender As Object, e As EventArgs)
        Dim inputOtp As String = String.Join("", rawOTP)
        Debug.WriteLine($"Input OTP: {inputOtp}")

        If ProcessMethods.ValidateOTP(inputOtp, totp) Then
            ' Raise event with the OTP value
            RaiseEvent SavePass(inputOtp)
            Dim resetModal As New ResetPassword(emailInput, Me)
            AddHandler resetModal.SavePass, AddressOf SavePassHandler ' Corrected here
            resetModal.ShowDialog()
        End If
    End Sub

    Private Sub OTP_Modal_Load(sender As Object, e As EventArgs)
        EmailTextBasis.Text = email
    End Sub

    ' Event handler method
    Private Sub SavePassHandler(newPass As String)
        ' Handle the password reset logic here
        Debug.WriteLine($"New Pass: {newPass}")
    End Sub
End Class
