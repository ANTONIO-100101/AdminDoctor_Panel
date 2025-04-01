Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient

Namespace Infocare_Project.Classes
    Friend Class Database_DataGridView
        Public Class PatientList
            Private ReadOnly connectionString As String = "Server=.\SQLEXPRESS; Database=InfoCare; Integrated Security=True;"

            Private Function GetConnection() As SqlConnection
                Return New SqlConnection(connectionString)
            End Function

            Public Function GetPatientList() As DataTable
                Using connection As SqlConnection = GetConnection()
                    Try
                        connection.Open()

                        Dim query As String = "SELECT P_Firstname AS Firstname, P_Lastname AS Lastname, P_Middlename AS Middlename, P_Suffix AS Suffix, P_Username AS Username, P_ContactNumber AS 'Contact No.', P_Sex AS Sex FROM tb_patientinfo"

                        Dim adapter As New SqlDataAdapter(query, connection)
                        Dim patientTable As New DataTable()
                        adapter.Fill(patientTable)

                        Return patientTable
                    Catch ex As Exception
                        Throw New Exception($"An error occurred while fetching patient data: {ex.Message}")
                    End Try
                End Using
            End Function

            Public Function GetDoctorList() As DataTable
                Using connection As SqlConnection = GetConnection()
                    Try
                        connection.Open()

                        Dim query As String = "SELECT D_Firstname AS Firstname, D_Lastname AS Lastname, D_Middlename AS Middlename, D_Suffix AS Suffix, D_Username AS Username, D_ContactNumber AS 'Contact No.', D_ConsultationFee AS 'Consultation Fee', D_Sex AS Sex FROM tb_doctorinfo"

                        Dim adapter As New SqlDataAdapter(query, connection)
                        Dim doctorTable As New DataTable()
                        adapter.Fill(doctorTable)

                        Return doctorTable
                    Catch ex As Exception
                        Throw New Exception($"An error occurred while fetching doctor data: {ex.Message}")
                    End Try
                End Using
            End Function

            Public Function GetAllAppointmentList() As DataTable
                Using connection As SqlConnection = GetConnection()
                    Try
                        connection.Open()

                        Dim query As String = "SELECT * FROM tb_appointmenthistory"

                        Dim adapter As New SqlDataAdapter(query, connection)
                        Dim appointmentTable As New DataTable()
                        adapter.Fill(appointmentTable)

                        Return appointmentTable
                    Catch ex As Exception
                        Throw New Exception($"An error occurred while fetching appointment data: {ex.Message}")
                    End Try
                End Using
            End Function
        End Class
    End Class
End Namespace
