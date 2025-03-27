Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports Infocare_Project_1.Classes
Imports Infocare_Project_1.Object_Models

Namespace Infocare_Project_1.Object_Models
    ''' <summary>
    ''' DoctorModel Class that is used to store all the details about the doctor.
    ''' </summary>
    Public Class DoctorModel
        Inherits UserModel

        Public Property Specialty As List(Of String)
        Public Property ConsultationFee As Decimal
        Public Property StartTime As TimeSpan
        Public Property EndTime As TimeSpan
        Public Property DayAvailability As String
    End Class
End Namespace
