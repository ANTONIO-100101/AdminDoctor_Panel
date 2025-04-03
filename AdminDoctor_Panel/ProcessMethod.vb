Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports Guna.UI2.WinForms
Imports System.Net.Mail
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Namespace Infocare_Project_1
    Public Module ProcessMethods

        Public Function ValidatePassword(password As String) As Boolean
            Dim regex As New System.Text.RegularExpressions.Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$")

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
            If String.IsNullOrEmpty(input) Then
                Return True
            End If

            If input.Equals("N/A", StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If

            Return input.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c))
        End Function

        Public Function HashCharacter(input As String) As String
            Using sha256 As SHA256 = SHA256.Create()
                Dim bytes As Byte() = Encoding.UTF8.GetBytes(input)
                Dim hashBytes As Byte() = sha256.ComputeHash(bytes)
                Dim sb As New StringBuilder()

                For Each b As Byte In bytes
                    sb.Append(b.ToString("x2"))
                Next

                Return sb.ToString()
            End Using
        End Function

        Public Sub SendEmail()
            ' Implement your email sending logic here if needed
        End Sub

        Public Sub ViewForgotPass(role As Role)
            Dim inputForm As New EmailUsernameInput(role)
            inputForm.ShowDialog()
        End Sub

        Public Function GetTablenameByRole(role As Role) As String
            Dim tableName As String =
                      If(role = Role.Staff, "tb_staffinfo", If(role = Role.Admin, "tb_adminlogin", If(role = Role.Doctor, "tb_doctorinfo", "tb_patientinfo")))
            Return tableName
        End Function

        Public Function ValidateFields(fields As Guna2TextBox()) As Boolean
            For Each textBox As Guna2TextBox In fields
                If textBox.Text.Trim() = "" Then
                    textBox.FocusedState.BorderColor = Color.Red
                    textBox.BorderColor = Color.Red
                    Return False
                End If
            Next

            Return True
        End Function

        Public Function GenerateOTP(totp As Totp) As String
            Return totp.ComputeTotp()
        End Function

        Public Function ValidateOTP(inputOTP As String, totp As Totp) As Boolean
            Return totp.VerifyTotp(inputOTP, Nothing, New VerificationWindow(previous:=1, future:=1))
        End Function

        Public Async Sub SendEmail(user As UserModel, otp As String)
            Dim client As New SmtpClient("smtp.gmail.com") With {
                .Port = 587,
                .Credentials = New System.Net.NetworkCredential("infocare004@gmail.com", "scde knkt wrfy qfzt"),
                .EnableSsl = True
            }

            email.DefaultSender = New SmtpSender(client)

            Dim engine As New RazorLightEngineBuilder() _
                .UseFileSystemProject(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName, "EmailTemplates")) _
                .UseMemoryCachingProvider() _
                .Build()

            Dim data = New With {
                Key .Name = user.UserName,
                Key .OTP = otp
            }

            Dim razorTemplate As String = "otp.cshtml"
            Dim body As String = Await engine.CompileRenderAsync(razorTemplate, data)

            Dim email = email _
                .From("infocare004@gmail.com", "InfoCare") _
                .To(user.Email) _
                .Subject("Your One-Time Password (OTP)") _
                .Body(body, True)

            Dim response = Await email.SendAsync()

            If response.Successful Then
                Debug.WriteLine("OTP email sent successfully!")
            Else
                Debug.WriteLine("Failed to send OTP email: " & String.Join(", ", response.ErrorMessages))
            End If
        End Sub

        Public Function ValidateEmail(inputEmail As String) As Boolean
            Dim pattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
            Return Regex.IsMatch(inputEmail, pattern)
        End Function

    End Module
End Namespace
