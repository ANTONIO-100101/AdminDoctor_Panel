Imports Infocare_Project_1.Classes
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace Infocare_Project_1.Object_Models.Interfaces
    Public Interface IPerson
        ' Personal Information
        Property FirstName As String
        Property LastName As String
        Property UserName As String
        Property MiddleName As String
        Property Password As String
        Property ContactNumber As String
        Property Email As String
    End Interface
End Namespace
