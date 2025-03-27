Namespace Infocare_Project_1.Object_Models.Interfaces
    Friend Interface IAddress
        Property HouseNo As Integer
        Property Street As String
        Property Barangay As String
        Property City As String
        Property ZipCode As Integer
        Property Zone As Integer

        ReadOnly Property FullAddress As String
    End Interface
End Namespace
