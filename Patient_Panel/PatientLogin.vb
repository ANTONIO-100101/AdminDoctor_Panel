Imports Infocare_Project_1
Imports Infocare_Project_1.Classes
Imports Infocare_Project_1.Object_Models
Imports Patient_Panel
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports Infocare_Project_1.Patient_Panel
Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel
Imports AdminDoctor_Panel.Object_Models
Imports AdminDoctor_Panel.Classes
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models




Partial Public Class PatientLogin
    Inherits Form

    Public Sub New()
        InitializeComponent()
        forgotPassBtn.Cursor = Cursors.Hand
    End Sub

    Private Sub EnterButton_Click(sender As Object, e As EventArgs) Handles EnterButton.Click

        Dim username As String = UsernameTxtbox.Text
        Dim password As String = PasswordTxtbox.Text

        Dim loginEmpty As New LoginEmpty()

        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            MessageBox.Show("Credentials are empty", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim validStaff As Boolean = Database.RoleLogin(username, password, Role.Patient)

        If validStaff Then
            Dim patient As PatientModel = Database.GetPatientInfo(username, ProcessMethods.HashCharacter(password))

            MessageBox.Show("Log in Successful", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Dim patientDashboard As New PatientDashboard(patient)
            patientDashboard.Show()
            Me.Hide()
        Else
            MessageBox.Show("Invalid Username or Password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

    End Sub

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to close?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub staff_HomeButton_Click(sender As Object, e As EventArgs) Handles staff_HomeButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to go back?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If confirm = DialogResult.Yes Then
            Dim homeForm As New LandingPage()
            homeForm.Show()
            Me.Hide()
        End If
    End Sub

    Private Sub staff_showpass_CheckedChanged(sender As Object, e As EventArgs) Handles staff_showpass.CheckedChanged
        If staff_showpass.Checked Then
            PasswordTxtbox.PasswordChar = ChrW(0)  ' Unhides the password
            PasswordTxtbox.UseSystemPasswordChar = False
        Else
            PasswordTxtbox.PasswordChar = "●"c  ' Hides the password
            PasswordTxtbox.UseSystemPasswordChar = True
        End If
    End Sub

    Private Sub forgotPassBtn_Click(sender As Object, e As EventArgs) Handles forgotPassBtn.Click
        ProcessMethods.viewForgotPass(Role.Patient)

    End Sub

    Private Sub RegisterBtn_Click(sender As Object, e As EventArgs) Handles RegisterBtn.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Register a Patient?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If confirm = DialogResult.Yes Then
            Dim patientRegisterForm As New PatientRegisterForm(ModalMode.Add)
            patientRegisterForm.Show()
        End If
    End Sub

    Private Sub MinimizeButton_Click(sender As Object, e As EventArgs) Handles MinimizeButton.Click
        Me.WindowState = FormWindowState.Minimized

    End Sub
End Class