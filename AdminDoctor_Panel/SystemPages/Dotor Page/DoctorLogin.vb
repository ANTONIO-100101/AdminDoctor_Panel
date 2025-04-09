Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Partial Public Class DoctorLogin
    Inherits Form
    Public Sub New()
        InitializeComponent()
        forgotPassBtn.Cursor = Cursors.Hand
    End Sub

    Private Sub DoctorLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    End Sub

    Private Sub doctor_EnterButton_Click(sender As Object, e As EventArgs) Handles doctor_EnterButton.Click
        Dim username As String = doctor_UsernameTxtbox.Text
        Dim password As String = doctor_PasswordTxtbox.Text

        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            MessageBox.Show("Credentials are empty", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Hide()
            Return
        End If

        Dim validDoctor As Boolean = Database.RoleLogin(username, password, Role.Doctor)

        If validDoctor Then
            Dim doctor As DoctorModel = Database.GetDoctorNameDetails(username)
            MessageBox.Show("Log in Successful", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Dim dashboard As New DoctorDashboard(doctor)
            dashboard.Show()
            Me.Hide()
        Else
            MessageBox.Show("Invalid Username or Password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
    End Sub

    Private Sub staff_HomeButton_Click(sender As Object, e As EventArgs) Handles staff_HomeButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to go back?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Dim homeForm As New HomeForm()
            homeForm.Show()
            Me.Hide()
        End If
    End Sub

    Private Sub doctor_showpass_CheckedChanged(sender As Object, e As EventArgs) Handles doctor_showpass.CheckedChanged
        If doctor_showpass.Checked Then
            doctor_PasswordTxtbox.PasswordChar = Chr(0)
            doctor_PasswordTxtbox.UseSystemPasswordChar = False
        Else
            doctor_PasswordTxtbox.PasswordChar = "●"c
            doctor_PasswordTxtbox.UseSystemPasswordChar = True
        End If
    End Sub

    Private Sub forgotPassBtn_Click(sender As Object, e As EventArgs) Handles forgotPassBtn.Click
        ProcessMethods.ViewForgotPass(Role.Doctor)

    End Sub

    Private Sub doctor_MinimizeButton_Click(sender As Object, e As EventArgs) Handles doctor_MinimizeButton.Click
        Me.WindowState = FormWindowState.Minimized

    End Sub

    Private Sub doctor_ExitButton_Click(sender As Object, e As EventArgs) Handles doctor_ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to close?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub PasswordLabel_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub guna2CustomGradientPanel1_Paint(sender As Object, e As PaintEventArgs)

    End Sub
End Class