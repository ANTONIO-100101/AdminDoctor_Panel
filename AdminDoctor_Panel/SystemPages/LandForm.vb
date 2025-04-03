Public Class LandForm
    Public Sub New()

        InitializeComponent()

    End Sub
    Private Sub EnterButton_Click(sender As Object, e As EventArgs) Handles EnterButton.Click
        Dim homeForm As New HomeForm()
        homeForm.Show()
        Me.Hide()
    End Sub
End Class