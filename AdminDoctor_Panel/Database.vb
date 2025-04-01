Imports System.Data.SqlClient
Imports AdminDoctor_Panel.Infocare_Project_1
Imports Microsoft.Data.SqlClient

Public Class Database
    Private Shared ReadOnly connectionString As String = "Server=.\SQLEXPRESS;Database=InfoCare;Integrated Security=True;"

    Public Shared Sub ExecuteQuery(query As String, parameters As Dictionary(Of String, Object))
        Using connection As New SqlConnection(connectionString)
            connection.Open()
            Using command As New SqlCommand(query, connection)
                For Each param In parameters
                    command.Parameters.AddWithValue(param.Key, If(param.Value, DBNull.Value))
                Next
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub
    Public Shared Function RoleLogin(username As String, password As String, role As Role) As Boolean
        Using connection As New SqlConnection(connectionString)
            Try
                Dim tableName As String = ProcessMethods.GetTablenameByRole(role)

                Dim tableColumn As String = If(role = role.Admin, "A_", If(role = role.Staff, "S_", If(role = role.Patient, "P_", "")))

                Dim query As String = $"SELECT COUNT(*) FROM {tableName} WHERE {(If(role = role.Staff, "", tableColumn))}Username = @Username AND {tableColumn}Password = @Password;"

                Dim command As New SqlCommand(query, connection)
                Dim hashedPassword As String = ProcessMethods.HashCharacter(password)
                command.Parameters.AddWithValue("@Username", username)
                command.Parameters.AddWithValue("@Password", hashedPassword)

                connection.Open()
                Dim result As Integer = Convert.ToInt32(command.ExecuteScalar())

                Return result = 1
            Catch ex As Exception
                Throw New Exception("Error validating login: " & ex.Message)
            End Try
        End Using
    End Function

End Class
