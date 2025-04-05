Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models

Public Class DoctorDashboard
    Inherits Form

    Private doctor As DoctorModel

    Public Sub New(doctor As DoctorModel)
        InitializeComponent()

        Me.doctor = doctor

        NameLabel.Text = $"Dr. {doctor.LastName}, {doctor.FirstName}"
    End Sub
End Class