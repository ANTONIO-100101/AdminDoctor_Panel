Imports Guna.UI2.WinForms
Imports Infocare_Project
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports System.Windows.Forms
Partial Public Class ResetPassword
    Inherits Form

    Public user As UserModel
    Public Event SavePass As Action(Of String)
    Public textBoxes As Guna2TextBox()
    Private emailInput As Form
    Private otpModal As Form

    Public Sub New(emailInput As Form, otpModal As Form)
        InitializeComponent()
        Me.emailInput = emailInput
        Me.otpModal = otpModal

        textBoxes = New Guna2TextBox() {newpassTextbox, confirmpassTextbox}

        AddHandler newpassTextbox.TextChanged, AddressOf textBox_textChanged
        AddHandler confirmpassTextbox.TextChanged, AddressOf textBox_textChanged
    End Sub

    Private Sub textBox_textChanged(sender As Object, e As EventArgs)
        Dim textBox As Guna2TextBox = TryCast(sender, Guna2TextBox)

        textBox.BorderColor = SystemColors.ControlDarkDark
        textBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255)
    End Sub

    Private Sub savePassBtn_Click(sender As Object, e As EventArgs)
        If Not ProcessMethods.ValidateFields(textBoxes) Then
            MessageBox.Show("These Fields Cannot be empty", "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If newpassTextbox.Text.Trim() <> confirmpassTextbox.Text.Trim() Then
            MessageBox.Show("Passwords Didn't Match", "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If Not ProcessMethods.ValidatePassword(newpassTextbox.Text) Then
            Return
        End If

        Me.Cursor = Cursors.WaitCursor
        RaiseEvent SavePass(newpassTextbox.Text)

        Me.Close()
        emailInput.Close()
        otpModal.Close()
    End Sub

    Private Sub showPass_CheckedChanged(sender As Object, e As EventArgs)
        If TypeOf sender Is Guna2CheckBox Then
            Dim checkBox As Guna2CheckBox = CType(sender, Guna2CheckBox)
            If checkBox.Checked Then
                newpassTextbox.PasswordChar = ChrW(0)
                newpassTextbox.UseSystemPasswordChar = False

                confirmpassTextbox.PasswordChar = ChrW(0)
                confirmpassTextbox.UseSystemPasswordChar = False
            Else
                newpassTextbox.PasswordChar = "●"c
                newpassTextbox.UseSystemPasswordChar = True

                confirmpassTextbox.PasswordChar = "●"c
                confirmpassTextbox.UseSystemPasswordChar = True
            End If
        End If
    End Sub

    Private Sub newpassTextbox_TextChanged(sender As Object, e As EventArgs)
        If newpassTextbox.Text.Trim() = "" Then
            passValidatorMsg.Visible = False
        Else
            passValidatorMsg.Visible = True
            Dim msg As String = If(Not Regex.IsMatch(newpassTextbox.Text, "[A-Z]"), "Add at least one uppercase letter",
                                       If(Not Regex.IsMatch(newpassTextbox.Text, "[^a-zA-Z0-9\s]"), "Add At least one special character",
                                       If(Not Regex.IsMatch(newpassTextbox.Text, "[\d]"), "Add At least one number",
                                       If(Not Regex.IsMatch(newpassTextbox.Text, ".{8,}"), "Must have at least 8 characters long", ""))))

            If msg = "" Then
                passValidatorMsg.Text = "*Strong Enough"
                passValidatorMsg.ForeColor = Color.Green
            Else
                passValidatorMsg.Text = "*" & msg
                passValidatorMsg.ForeColor = Color.Red
            End If
        End If
    End Sub
End Class
