Imports System

Namespace Infocare_Project_1.Object_Models.Interfaces
    Friend Interface IEmergencyContact
        Property FirstName As String
        Property MiddleName As String
        Property LastName As String
        Property Suffix As String

        Property Address As AddressModel
    End Interface
End Namespace
