Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports Guna.UI2.WinForms
Imports OtpNet
Imports System.Diagnostics
Partial Public Class EmailUsernameInput
    Inherits Form

    Private user As New UserModel()
    Private role As Role

    Public Sub New(role As Role)
        Me.role = role
        InitializeComponent()

        AddHandler usernameTextbox.TextChanged, AddressOf textBoxes_TextChanged
        AddHandler emailTextbox.TextChanged, AddressOf textBoxes_TextChanged
    End Sub

    Private Sub guna2TextBox1_TextChanged(sender As Object, e As EventArgs)
        Dim textBox As Guna2TextBox = TryCast(sender, Guna2TextBox)

        If textBox IsNot Nothing Then
            textBox.BorderColor = SystemColors.ControlDarkDark
            textBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255)
        End If
    End Sub

    Private Sub textBoxes_TextChanged(sender As Object, e As EventArgs)
        ' No implementation provided in the original code
    End Sub

    Public Sub VoidCloseModals()
        Me.Close()
    End Sub

    Private Sub submitBtn_Click(sender As Object, e As EventArgs) Handles submitBtn.Click
        user.UserName = usernameTextbox.Text
        user.Email = emailTextbox.Text
        Me.Cursor = Cursors.WaitCursor
        submitBtn.Enabled = False

        Dim fields As Guna2TextBox() = {usernameTextbox, emailTextbox}

        If Not ProcessMethods.ValidateFields(fields) Then
            MessageBox.Show("These fields cannot be empty", "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error)
            submitBtn.Enabled = True
            Me.Cursor = Cursors.Default
            Return
        End If

        If Not Database.IsEmailExisted(role, user) Then
            MessageBox.Show("Email or Username is not registered yet.", "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error)
            submitBtn.Enabled = True
            Me.Cursor = Cursors.Default
            Return
        End If

        ' Generating an OTP
        Dim secretKey As Byte() = KeyGeneration.GenerateRandomKey(20)
        Dim totp As New Totp(secretKey, step:=1800)

        Dim otp As String = ProcessMethods.GenerateOTP(totp)

        Debug.WriteLine($"Your OTP: {otp}")

        ProcessMethods.SendEmail(user, otp)

        submitBtn.Enabled = True
        Me.Cursor = Cursors.Default

        Dim otpModal As New OTP_Modal(totp, Me, user.Email)
        AddHandler otpModal.SavePass, AddressOf SavePass
        otpModal.ShowDialog()
    End Sub

    Public Sub SavePass(newPass As String)
        Debug.WriteLine($"Username: {user.UserName}")
        Debug.WriteLine($"Email: {user.Email}")
        Debug.WriteLine($"New Pass: {newPass}")

        Dim hashPassword As String = ProcessMethods.HashCharacter(newPass)
        user.Password = hashPassword
        Debug.WriteLine($"Password: {user.Password}")
        Database.UpdateUserPassword(role, user)
        MessageBox.Show("Account Password updated.")
        Me.Close()
    End Sub

    Private Sub closeBtn_Click(sender As Object, e As EventArgs) Handles closeBtn.Click
        Me.Close()
    End Sub
End Class
