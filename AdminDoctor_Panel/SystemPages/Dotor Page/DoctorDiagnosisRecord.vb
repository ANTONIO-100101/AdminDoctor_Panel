Public Class DoctorDiagnosisRecord
    Inherits Form

    Private prevForm As DoctorMedicalRecord
    Public Event LoadAppointmentsList As EventHandler

    Public Sub New(prevForm As DoctorMedicalRecord)
        InitializeComponent()
        Me.prevForm = prevForm
    End Sub
    Public Sub SetPatientName(firstName As String, lastName As String)
        PatientNameLabel.Text = $"{lastName}, {firstName}"
    End Sub

End Class
