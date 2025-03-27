﻿Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models.Interfaces
Imports Infocare_Project_1.Classes
Imports Infocare_Project_1.Object_Models.Interfaces

Namespace Infocare_Project_1.Object_Models
    ''' <summary>
    ''' UserModel Class that stores user details.
    ''' </summary>
    Public Class UserModel
        Implements IPerson

        Public Property AccountID As Integer Implements IPerson.AccountID
        Public Property FirstName As String Implements IPerson.FirstName
        Public Property LastName As String Implements IPerson.LastName
        Public Property UserName As String Implements IPerson.UserName
        Public Property MiddleName As String Implements IPerson.MiddleName
        Public Property Password As String Implements IPerson.Password
        Public Property ContactNumber As String Implements IPerson.ContactNumber
        Public Property Email As String Implements IPerson.Email

    End Class
End Namespace
