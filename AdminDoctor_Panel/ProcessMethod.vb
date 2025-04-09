Imports System.Text
Imports System.Security.Cryptography
Imports System.Text.RegularExpressions
Imports Guna.UI2.WinForms
Imports OtpNet
Imports System.Net.Mail
Imports RazorLight
Imports System.IO
Imports System.Threading.Tasks
Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports FluentEmail.Core
Imports FluentEmail.Smtp

Public Class ProcessMethods

    Public Shared Function ValidatePassword(password As String) As Boolean
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

    Public Shared Function IsValidTextInput(input As String) As Boolean
        If String.IsNullOrEmpty(input) Then
            Return True
        End If

        If input.Equals("N/A", StringComparison.OrdinalIgnoreCase) Then
            Return True
        End If

        Return input.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c))
    End Function

    Public Shared Function HashCharacter(input As String) As String
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

    Public Shared Sub SendEmail()
        ' Email sending functionality (empty in the original code)
    End Sub

    Public Shared Sub ViewForgotPass(role As Role)
        Dim inputForm As New EmailUsernameInput(role)
        inputForm.ShowDialog()
    End Sub

    Public Shared Function GetTablenameByRole(role As Role) As String
        Dim tableName As String =
            If(role = Role.Staff, "tb_staffinfo",
                If(role = Role.Admin, "tb_adminlogin",
                    If(role = Role.Doctor, "tb_doctorinfo", "tb_patientinfo")))

        Return tableName
    End Function

    Public Shared Function ValidateFields(fields As Guna2TextBox()) As Boolean
        For Each textBox As Guna2TextBox In fields
            If String.IsNullOrWhiteSpace(textBox.Text) Then
                textBox.FocusedState.BorderColor = Color.Red
                textBox.BorderColor = Color.Red
                Return False
            End If
        Next

        Return True
    End Function

    Public Shared Function GenerateOTP(totp As Totp) As String
        Return totp.ComputeTotp()
    End Function

    Public Shared Function ValidateOTP(inputOTP As String, totp As Totp) As Boolean
        Return totp.VerifyTotp(inputOTP, Nothing, New VerificationWindow(previous:=1, future:=1))
    End Function

    Public Shared Async Sub SendEmail(user As UserModel, otp As String)
        ' Setup SMTP client
        Dim client As New SmtpClient("smtp.gmail.com") With {
            .Port = 587,
            .Credentials = New System.Net.NetworkCredential("infocare004@gmail.com", "scde knkt wrfy qfzt"),
            .EnableSsl = True
        }

        ' Setup FluentEmail sender
        FluentEmail.Core.Email.DefaultSender = New SmtpSender(client)

        ' Create RazorLight engine
        Dim templatesPath As String = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "EmailTemplates")

        Dim engine As IRazorLightEngine = New RazorLightEngineBuilder().
        UseFileSystemProject(templatesPath).
        UseMemoryCachingProvider().
        Build()
        ' Data to be passed to Razor template
        Dim data = New With {
            Key .Name = user.UserName,
            Key .OTP = otp
        }

        ' Razor template file name
        Dim razorTemplate As String = "otp.cshtml"

        ' Render email body from Razor template
        Dim body As String = Await engine.CompileRenderAsync(razorTemplate, data)

        ' Create email and send
        Dim email As FluentEmail.Core.Email = FluentEmail.Core.Email _
            .From("infocare004@gmail.com", "InfoCare") _
            .To(user.Email) _
            .Subject("Your One-Time Password (OTP)") _
            .Body(body, True)

        ' Send email asynchronously
        Dim response = Await email.SendAsync()

        If response.Successful Then
            Debug.WriteLine("OTP email sent successfully!")
        Else
            Debug.WriteLine("Failed to send OTP email: " & String.Join(", ", response.ErrorMessages))
        End If
    End Sub


    Public Shared Function ValidateEmail(inputEmail As String) As Boolean
        Dim pattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
        Return Regex.IsMatch(inputEmail, pattern)
    End Function
End Class
