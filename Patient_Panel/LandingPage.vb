﻿Public Class LandingPage
    Private Sub guna2Button1_Click(sender As Object, e As EventArgs) Handles guna2Button1.Click
        Dim patientForm As New PatientLogin()
        patientForm.Show()
        Me.Hide()
    End Sub
End Class
