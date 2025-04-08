Imports AdminDoctor_Panel.Infocare_Project_1

Public Class LandForm
    Public Sub New()

        InitializeComponent()

    End Sub
    Private Sub EnterButton_Click(sender As Object, e As EventArgs) Handles EnterButton.Click
        Dim homeForm As New HomeForm()
        homeForm.Show()
        Me.Hide()
    End Sub

    Private Sub Guna2CustomGradientPanel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2CustomGradientPanel1.Paint

    End Sub
End Class