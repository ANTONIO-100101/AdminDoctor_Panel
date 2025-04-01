Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports FluentEmail.Core
Imports FluentEmail.Smtp
Imports Guna.UI2.WinForms
Imports Infocare_Project_1.Object_Models
Imports Infocare_Project_1.PopupModals
Imports OtpNet
Imports RazorLight
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Net.Mail
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace Infocare_Project_1
    Public Module ProcessMethods

        Public Function ValidatePassword(password As String) As Boolean
            Dim regex As New Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$")

            If Not regex.IsMatch(password) Then
                MessageBox.Show("Password must be at least 8 characters long and include:" & vbCrLf &
                                "- At least one uppercase letter" & vbCrLf &
                                "- At least one lowercase letter" & vbCrLf &
                                "- At least one number" & vbCrLf &
                                "- At least one special character (e.g., @, !, etc.)",
                                "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            Return True
        End Function

        Public Function IsValidTextInput(input As String) As Boolean
            If String.IsNullOrEmpty(input) OrElse input.Equals("N/A", StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If

            Return input.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c))
        End Function

        Public Function HashCharacter(input As String) As String
            Using sha256 As SHA256 = SHA256.Create()
                Dim bytes As Byte() = Encoding.UTF8.GetBytes(input)
                Dim hashBytes As Byte() = sha256.ComputeHash(bytes)

                Dim sb As New StringBuilder()
                For Each b As Byte In hashBytes
                    sb.Append(b.ToString("x2"))
                Next

                Return sb.ToString()
            End Using
        End Function

        Public Sub SendEmail()
            ' Placeholder for sending email functionality
        End Sub

        Public Sub ViewForgotPass(role As Role)
            Dim inputForm As New EmailUsernameInput(role)
            inputForm.ShowDialog()
        End Sub

        Public Function GetTablenameByRole(role As Role) As String
            Dim tableName As String = If(role = Role.Staff, "tb_staffinfo",
                                   If(role = Role.Admin, "tb_adminlogin",
                                   If(role = Role.Doctor, "tb_doctorinfo", "tb_patientinfo")))
            Return tableName
        End Function

        Public Function ValidateFields(fields As Guna2TextBox()) As Boolean
            For Each textBox As Guna2TextBox In fields
                If String.IsNullOrWhiteSpace(textBox.Text) Then
                    textBox.FocusedState.BorderColor = Color.Red
                    textBox.BorderColor = Color.Red
                    Return False
                End If
            Next
            Return True
        End Function
    End Module
End Namespace
