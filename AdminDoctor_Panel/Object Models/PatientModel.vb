Imports Infocare_Project_1.Classes

Namespace Infocare_Project_1.Object_Models
    ''' <summary>
    ''' PatientModel Class that stores all the details of a Patient.
    ''' </summary>
    Public Class PatientModel
        Inherits UserModel

        Public Property AccountID As Integer
        Public Property HealthInfo As HealthInfoModel
        Public Property EmergencyContact As EmergencyContactModel
        Public Property BirthDate As Date
        Public Property Sex As String
        Public Property Suffix As String
        Public Property Address As AddressModel

    End Class
End Namespace
