Namespace Infocare_Project_1.Classes
    Public Class LoginEmpty
        Public Property Username As String
        Public Property Password As String

        Public Sub New()
            ' Default constructor
        End Sub

        Public Sub New(username As String, password As String)
            Me.Username = username
            Me.Password = password
        End Sub
    End Class
End Namespace
