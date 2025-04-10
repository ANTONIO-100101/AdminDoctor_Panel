Public Class HomeForm
    Private Sub DoctorEnterButton_Click(sender As Object, e As EventArgs) Handles DoctorEnterButton.Click
        Dim doctorLogin As New DoctorLogin()
        doctorLogin.Show()
        Me.Hide()
    End Sub

    Private Sub AdminEnterButton_Click(sender As Object, e As EventArgs) Handles AdminEnterButton.Click
        Dim adminLogin As New AdminLogin()
        adminLogin.Show()
        Me.Hide()
    End Sub

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to close Infocare?", "Confirm Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub
End Class