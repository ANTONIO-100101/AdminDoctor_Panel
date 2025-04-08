Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models

Public Class PatientDashboard
    Inherits Form

    Private patient As PatientModel

    Public Sub New(patient As PatientModel)
        InitializeComponent()
        Me.patient = patient

        NameLabel.Text = $"{patient.FirstName}!"
    End Sub
End Class
