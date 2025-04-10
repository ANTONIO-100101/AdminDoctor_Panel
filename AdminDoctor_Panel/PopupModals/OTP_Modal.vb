Imports Guna.UI2.WinForms
Imports OtpNet

Public Class OTP_Modal
    Inherits Form

    Private user As UserModel
    Public Event SavePass As Action(Of String) ' Declare the event here
    Private totp As Totp
    Private emailInput As Form

    Private verifyBoxes As Guna2TextBox()
    Private rawOTP As String() = New String(5) {}

    ' 🔹 Updated Constructor to Accept `UserModel` Instead of Just `Email`
    Public Sub New(totp As Totp, emailInput As Form, user As UserModel)
        Me.totp = totp
        Me.emailInput = emailInput
        Me.user = user
        InitializeComponent()
        SubscribeTextChanged()

        Me.verifyBoxes = New Guna2TextBox() {verifyBox1, verifyBox2, verifyBox3, verifyBox4, verifyBox5, verifyBox6}
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

        ' Enable submit button if all OTP fields are filled
        submitBtn.Enabled = rawOTP.All(Function(text) Not String.IsNullOrEmpty(text?.Trim()))
        Debug.WriteLine($"Submit Button Enabled: {submitBtn.Enabled}")
    End Sub

    Private Sub submitBtn_Click(sender As Object, e As EventArgs) Handles submitBtn.Click
        Try
            Dim inputOtp As String = String.Join("", rawOTP)
            Debug.WriteLine($"Input OTP: {inputOtp}")

            If ProcessMethods.ValidateOTP(inputOtp, totp) Then
                'RaiseEvent Raise event with the OTP value
                Dim resetModal As New ResetPassword(emailInput, Me)
                AddHandler resetModal.SavePass, Sub(newPass)
                                                    SavePassHandler(newPass)
                                                    RaiseEvent SavePass(newPass) ' Forward it to main form or wherever it's needed
                                                End Sub
                resetModal.ShowDialog()
            Else
                MessageBox.Show("Invalid OTP. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            Debug.WriteLine($"Exception: {ex.Message}")
            MessageBox.Show("An error occurred. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' 🔹 Updated to Dynamically Get the Email from `UserModel`
    Private Sub OTP_Modal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EmailTextBasis.Text = user.Email
    End Sub

    ' Event handler method
    Private Sub SavePassHandler(newPass As String)
        ' Handle the password reset logic here
        Debug.WriteLine($"New Pass: {newPass}")
    End Sub

    Private Sub closeBtn_Click(sender As Object, e As EventArgs) Handles closeBtn.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to cancel?", "Confirm Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub
End Class
