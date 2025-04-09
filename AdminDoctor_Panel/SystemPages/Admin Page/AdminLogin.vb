Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Classes

Public Class AdminLogin
    Inherits Form

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub EnterButton_Click(sender As Object, e As EventArgs) Handles EnterButton.Click
        Dim username As String = UsernameTxtbox.Text
        Dim password As String = PasswordTxtbox.Text

        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Dim nullLogin As New LoginEmpty(username, password)
            MessageBox.Show("Username or Password can't be missing", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Hide()
            Return
        End If

        Dim valid As Boolean = Database.RoleLogin(username, password, Role.Admin)

        If valid Then
            MessageBox.Show("Log in Successful", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Dim adminDashboard As New AdminDashboard2()
            adminDashboard.Show()
            Me.Hide()
        Else
            MessageBox.Show("Invalid Username or Password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
    End Sub

    Private Sub admin_showpass_CheckedChanged(sender As Object, e As EventArgs) Handles admin_showpass.CheckedChanged
        If admin_showpass.Checked Then
            PasswordTxtbox.PasswordChar = ControlChars.NullChar
            PasswordTxtbox.UseSystemPasswordChar = False
        Else
            PasswordTxtbox.PasswordChar = "●"c
            PasswordTxtbox.UseSystemPasswordChar = True
        End If
    End Sub

    Private Sub UsernameTxtbox_TextChanged(sender As Object, e As EventArgs) Handles UsernameTxtbox.TextChanged

    End Sub

    Private Sub ad_HomeButton_Click(sender As Object, e As EventArgs) Handles ad_HomeButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to go back?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Dim homeForm As New HomeForm()
            homeForm.Show()
            Me.Hide()
        End If
    End Sub

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to close?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub MinimizeButton_Click(sender As Object, e As EventArgs) Handles MinimizeButton.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub guna2CustomGradientPanel1_Paint(sender As Object, e As PaintEventArgs)

    End Sub
End Class
