Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models.Interfaces
Imports Infocare_Project_1.Object_Models.Interfaces

Namespace Infocare_Project_1.Object_Models
    Public Class AddressModel
        Implements IAddress

        Public Property HouseNo As Integer Implements IAddress.HouseNo
        Public Property Street As String Implements IAddress.Street
        Public Property Barangay As String Implements IAddress.Barangay
        Public Property City As String Implements IAddress.City
        Public Property ZipCode As Integer Implements IAddress.ZipCode
        Public Property Zone As Integer Implements IAddress.Zone

        Public ReadOnly Property FullAddress As String Implements IAddress.FullAddress
            Get
                Return $"{HouseNo}, {ZipCode}, {Zone}, {Street} street, Brgy. {Barangay}, {City}"
            End Get
        End Property
    End Class
End Namespace
