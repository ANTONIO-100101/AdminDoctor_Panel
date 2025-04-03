Imports AdminDoctor_Panel.Infocare_Project_1

Public Class LandForm
    Public Sub New()

        InitializeComponent()

    End Sub
    Private Sub EnterButton_Click(sender As Object, e As EventArgs) Handles EnterButton.Click
        Dim homeForm As New HomeForm()
        homeForm.Show()
        Me.Hide()
        Dim hashedPassword As String = ProcessMethods.HashCharacter("admin123")
        MessageBox.Show("Hashed Password: " & hashedPassword)

    End Sub
End Class