Imports System.Data.SqlClient
Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports Microsoft.Data.SqlClient

Public Class Database
    Private Shared ReadOnly connectionString As String = "Server=.\SQLEXPRESS;Database=InfoCare;Integrated Security=True;TrustServerCertificate=True;"
    Private Shared Function GetConnection() As SqlConnection
        Return New SqlConnection(connectionString)
    End Function

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

                Dim tableColumn As String = If(role = Role.Admin, "A_", If(role = Role.Staff, "S_", If(role = Role.Patient, "P_", "")))

                Dim query As String = If(role = Role.Staff,
                                    $"SELECT COUNT(*) FROM {tableName} WHERE Username = @Username AND Password = @Password;",
                                    $"SELECT COUNT(*) FROM {tableName} WHERE {tableColumn}Username = @Username AND {tableColumn}Password = @Password;")

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


    Public Shared Function DoctorList() As DataTable
        Dim query As String = "SELECT doctor_id as 'Doctor ID', first_name AS 'First Name', last_name AS 'Last Name', email AS 'Email', phone_number AS 'Phone Number', consultation_fee AS 'Consultation Fee' FROM tb_doctorinfo"
        Dim DoctorTable As New DataTable()

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(DoctorTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving Doctor list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return DoctorTable
    End Function

    Public Shared Function GetDoctorInfo(AccountId As Integer) As DoctorModel
        Using connection = GetConnection()
            Dim doctor As New DoctorModel()
            Dim query As String = "SELECT * FROM tb_doctorinfo WHERE doctor_id = @ID"

            Using cmd As New SqlCommand(query, connection)
                cmd.Parameters.AddWithValue("@ID", AccountId)
                connection.Open()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim skills As String() = If(reader.IsDBNull(reader.GetOrdinal("specialization")), "", reader.GetString("specialization")).Split(","c)

                        Dim specializations As New List(Of String)()

                        For Each skill As String In skills
                            specializations.Add(skill)
                        Next
                        doctor = New DoctorModel() With {
                        .AccountID = If(reader.IsDBNull(reader.GetOrdinal("doctor_id")), 0, reader.GetInt32("doctor_id")),
                        .FirstName = If(reader.IsDBNull(reader.GetOrdinal("first_name")), "", reader.GetString("first_name")),
                        .LastName = If(reader.IsDBNull(reader.GetOrdinal("last_name")), "", reader.GetString("last_name")),
                        .MiddleName = If(reader.IsDBNull(reader.GetOrdinal("middlename")), "", reader.GetString("middlename")),
                        .UserName = If(reader.IsDBNull(reader.GetOrdinal("username")), "", reader.GetString("username")),
                        .Password = If(reader.IsDBNull(reader.GetOrdinal("password")), "", reader.GetString("password")),
                        .ContactNumber = If(reader.IsDBNull(reader.GetOrdinal("phone_number")), "", reader.GetString("phone_number")),
                        .Email = If(reader.IsDBNull(reader.GetOrdinal("email")), "", reader.GetString("email")),
                        .ConsultationFee = If(reader.IsDBNull(reader.GetOrdinal("consultation_fee")), 0D, reader.GetDecimal("consultation_fee")),
                        .StartTime = If(reader.IsDBNull(reader.GetOrdinal("start_time")), New TimeSpan(0), reader.GetTimeSpan("start_time")),
                        .EndTime = If(reader.IsDBNull(reader.GetOrdinal("end_time")), New TimeSpan(0), reader.GetTimeSpan("end_time")),
                        .Specialty = specializations,
                        .DayAvailability = If(reader.IsDBNull(reader.GetOrdinal("day_availability")), "", reader.GetString("day_availability"))
                    }
                    End While
                    Return doctor
                End Using
            End Using
        End Using
    End Function

    Public Shared Function UsernameExistsDoctor(username As String) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM tb_doctorinfo WHERE username = @Username"

        Using connection As New SqlConnection(connectionString)
            connection.Open()
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Username", username)
                Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                Return count > 0
            End Using
        End Using
    End Function

    Public Shared Function AddUpdateDoctor1(doctor As DoctorModel, mode As ModalMode) As Integer
        Using connection = GetConnection()
            Dim transaction As SqlTransaction = Nothing

            Try
                connection.Open()
                transaction = connection.BeginTransaction()

                Dim query As String

                If mode = ModalMode.Add Then
                    query = "INSERT INTO tb_doctorinfo " &
                        "(first_name, middlename, last_name, username, password, consultation_fee, start_time, end_time, day_availability, phone_number, email) " &
                        "VALUES (@FirstName, @MiddleName, @LastName, @Username, @Password, @ConsultationFee, @StartTime, @EndTime, @DayAvailability, @ContactNumber, @Email); " &
                        "SELECT SCOPE_IDENTITY();"
                Else
                    query = "UPDATE tb_doctorinfo " &
                        "SET firstname = @FirstName, lastname = @LastName, " &
                        "middlename = @MiddleName, username = @Username, " &
                        "consultationfee = @ConsultationFee, start_time = @StartTime, " &
                        "end_time = @EndTime, day_availability = @DayAvailability, contactnumber = @ContactNumber, " &
                        "email = @Email WHERE doctor_id = @AccountID"
                End If

                Dim command As New SqlCommand(query, connection, transaction)

                Dim hashPassword As String = ProcessMethods.HashCharacter(doctor.Password)
                command.Parameters.AddWithValue("@FirstName", doctor.FirstName)
                command.Parameters.AddWithValue("@MiddleName", doctor.MiddleName)
                command.Parameters.AddWithValue("@LastName", doctor.LastName)
                command.Parameters.AddWithValue("@Username", doctor.UserName)
                command.Parameters.AddWithValue("@Password", hashPassword)
                command.Parameters.AddWithValue("@ConsultationFee", doctor.ConsultationFee)
                command.Parameters.AddWithValue("@StartTime", doctor.StartTime)
                command.Parameters.AddWithValue("@EndTime", doctor.EndTime)
                command.Parameters.AddWithValue("@DayAvailability", doctor.DayAvailability)
                command.Parameters.AddWithValue("@ContactNumber", doctor.ContactNumber)
                command.Parameters.AddWithValue("@Email", doctor.Email)
                command.Parameters.AddWithValue("@AccountID", doctor.AccountID)

                Dim doctorId As Integer
                If mode = ModalMode.Add Then
                    doctorId = Convert.ToInt32(command.ExecuteScalar())
                Else
                    command.ExecuteNonQuery()
                    doctorId = doctor.AccountID
                End If

                ' Handle specializations
                Dim specializationsList As New List(Of String)()

                For Each specialization As String In doctor.Specialty
                    Dim specializationQuery As String = "INSERT INTO tb_doctor_specializations (doctor_id, specialization) " &
                                                         "VALUES (@DoctorId, @Specialization)"
                    Dim specializationCommand As New SqlCommand(specializationQuery, connection, transaction)

                    specializationCommand.Parameters.AddWithValue("@DoctorId", doctorId)
                    specializationCommand.Parameters.AddWithValue("@Specialization", specialization)

                    specializationCommand.ExecuteNonQuery()

                    specializationsList.Add(specialization)
                Next

                Dim joinedSpecializations As String = String.Join(", ", specializationsList)
                Dim updateQuery As String = "UPDATE tb_doctorinfo " &
                                            "SET specialization = @Specialization " &
                                            "WHERE doctor_id = @DoctorId"
                Dim updateCommand As New SqlCommand(updateQuery, connection, transaction)
                updateCommand.Parameters.AddWithValue("@Specialization", joinedSpecializations)
                updateCommand.Parameters.AddWithValue("@DoctorId", doctorId)
                updateCommand.ExecuteNonQuery()

                transaction.Commit()
                Return doctorId
            Catch ex As Exception
                If transaction IsNot Nothing Then
                    transaction.Rollback()
                End If
                Throw New Exception("Error inserting doctor data: " & ex.Message)
            End Try
        End Using
    End Function
    Public Shared Sub AddSpecialization(doctorId As Integer, specialization As String)
        Dim query As String = "INSERT INTO tb_doctor_specializations (doctor_id, specialization) VALUES (@DoctorId, @Specialization)"

        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@DoctorId", doctorId)
                cmd.Parameters.AddWithValue("@Specialization", specialization)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub DeleteDoctorById(AccountID As Integer)
        Dim query As String = "DELETE FROM tb_doctor_specializations WHERE doctor_id = @ID;" &
                          "DELETE FROM tb_doctorinfo WHERE doctor_id = @ID"

        Using connection As SqlConnection = GetConnection()
            Dim cmd As New SqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@ID", AccountID)

            Try
                connection.Open()
                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                If rowsAffected = 0 Then
                    Throw New Exception("No row found to delete.")
                End If
            Catch ex As Exception
                Throw New Exception("Error deleting doctor record: " & ex.Message)
            End Try
        End Using
    End Sub
    Public Shared Function GetDoctorNameDetails(username As String) As DoctorModel
        Using connection = GetConnection()
            Dim query As String = "SELECT * FROM tb_doctorinfo WHERE Username = @Username"
            Dim command As New SqlCommand(query, connection)
            command.Parameters.AddWithValue("@Username", username)

            Try
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Dim firstName As String = reader("first_name").ToString()
                        Dim lastName As String = reader("last_name").ToString()
                        Dim middleName As String = reader("middlename").ToString()
                        Dim contactNumber As String = reader("phone_number").ToString()
                        Dim password As String = reader("password").ToString()
                        Dim specialties As String = reader("specialization").ToString()

                        Dim specialty As New List(Of String)(specialties.Split(","c))

                        Dim consultationFee As Decimal = Decimal.Parse(reader("consultation_fee").ToString())

                        Dim startTime As TimeSpan
                        TimeSpan.TryParse(reader("start_time").ToString(), startTime)

                        Dim endTime As TimeSpan
                        TimeSpan.TryParse(reader("end_time").ToString(), endTime)

                        Dim dayAvailability As String = reader("day_availability").ToString()

                        Return New DoctorModel With {
                        .FirstName = firstName,
                        .LastName = lastName,
                        .MiddleName = middleName,
                        .Password = password,
                        .Specialty = specialty,
                        .ConsultationFee = consultationFee,
                        .StartTime = startTime,
                        .EndTime = endTime,
                        .DayAvailability = dayAvailability
                    }
                    Else
                        Throw New Exception("No Doctor found with the given username.")
                    End If
                End Using
            Catch ex As Exception
                Throw New Exception("Error fetching patient name details: " & ex.Message)
            End Try
        End Using
    End Function


End Class
