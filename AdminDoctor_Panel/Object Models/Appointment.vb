Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace Infocare_Project_1.Object_Models
    Public Class Appointment
        Public Property PatientName As String
        Private Property Status As AppointmentStatus
        Public Property DoctorName As String
        Public Property Specialization As String
        Public Property Time As TimeSpan
        Public Property [Date] As DateTime
        Public Property ConsultationFee As Decimal
        Public Property Birthdate As DateTime
        Public Property HealthInfo As HealthInfoModel
        Public Property Diagnosis As DiagnosisModel
        Public Property ConfineDays As Integer
    End Class
End Namespace
