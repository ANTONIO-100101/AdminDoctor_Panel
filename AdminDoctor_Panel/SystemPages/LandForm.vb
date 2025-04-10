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

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to close Infocare?", "Confirm Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub
End Class