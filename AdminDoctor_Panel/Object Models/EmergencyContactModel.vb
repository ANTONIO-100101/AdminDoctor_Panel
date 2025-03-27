Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models.Interfaces
Imports Infocare_Project_1.Object_Models.Interfaces

Namespace Infocare_Project_1.Object_Models
    ''' <summary>
    ''' EmergencyContactModel Class that stores Emergency Contact Details of a Patient.
    ''' </summary>
    Public Class EmergencyContactModel
        Implements IEmergencyContact

        Public Property FirstName As String Implements IEmergencyContact.FirstName
        Public Property MiddleName As String Implements IEmergencyContact.MiddleName
        Public Property LastName As String Implements IEmergencyContact.LastName
        Public Property Suffix As String Implements IEmergencyContact.Suffix

        Public Property Address As AddressModel Implements IEmergencyContact.Address
    End Class
End Namespace
