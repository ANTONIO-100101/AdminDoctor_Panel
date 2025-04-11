Imports System.Data.SqlClient
Imports System.Globalization
Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports Microsoft.Data.SqlClient

Public Class Database
    Private Shared ReadOnly connectionString As String = "Server=JMGENGGENG\SQLEXPRESS;Database=InfoCare;Integrated Security=True;TrustServerCertificate=True;"
    Public Shared Function GetConnection() As SqlConnection
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

                ' Skip hashing for Admins, but hash for other roles
                Dim finalPassword As String = If(role = Role.Admin, password, ProcessMethods.HashCharacter(password))

                command.Parameters.AddWithValue("@Username", username)
                command.Parameters.AddWithValue("@Password", finalPassword)

                connection.Open()

                Dim result As Integer = Convert.ToInt32(command.ExecuteScalar())

                Return result = 1
            Catch ex As Exception
                Throw New Exception("Error validating login: " & ex.Message)
            End Try
        End Using
    End Function
    Public Shared Function DoctorList() As DataTable
        Dim query As String = "SELECT doctor_id as 'Doctor ID', first_name AS 'First Name', last_name AS 'Last Name', email AS 'Email', phone_number AS 'Phone Number', consultation_fee AS 'Consultation Fee', serial_number AS 'Serial Number' FROM tb_doctorinfo"
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


    Public Shared Function GetPatientInfo(username As String, password As String) As PatientModel
        Using connection As SqlConnection = GetConnection()
            Dim user As New PatientModel()
            Dim health As New HealthInfoModel()
            Dim emergency As New EmergencyContactModel()
            Dim user_address As New AddressModel()
            Dim eme_address As New AddressModel()

            connection.Open()
            Dim query As String = "SELECT * FROM tb_patientinfo WHERE P_username = @Username AND P_Password = @Password"

            Using cmd As New SqlCommand(query, connection)
                cmd.Parameters.AddWithValue("@Username", username)
                cmd.Parameters.AddWithValue("@Password", password)

                ' Fetch patient data
                PatientInfoFetcher(cmd, user, health, user_address, emergency, eme_address)
            End Using

            ' Assign related models
            user.HealthInfo = health
            user.Address = user_address
            emergency.Address = eme_address
            user.EmergencyContact = emergency

            Return user
        End Using
    End Function

    Public Shared Sub PatientInfoFetcher(
    ByVal cmd As SqlCommand, ByVal user As PatientModel, ByVal health As HealthInfoModel,
    ByVal user_address As AddressModel, ByVal emergency As EmergencyContactModel, ByVal eme_address As AddressModel)

        Using reader As SqlDataReader = cmd.ExecuteReader()
            While reader.Read()
                user.AccountID = reader.GetInt32(reader.GetOrdinal("id"))
                user.FirstName = reader.GetString(reader.GetOrdinal("P_Firstname"))
                user.LastName = reader.GetString(reader.GetOrdinal("P_Lastname"))
                user.Password = If(reader.IsDBNull(reader.GetOrdinal("P_Password")), "", reader.GetString(reader.GetOrdinal("P_Password")))
                user.MiddleName = reader.GetString(reader.GetOrdinal("P_Middlename"))
                user.UserName = reader.GetString(reader.GetOrdinal("P_username"))
                user.ContactNumber = reader.GetString(reader.GetOrdinal("P_ContactNumber"))
                user.BirthDate = reader.GetDateTime(reader.GetOrdinal("P_Bdate"))
                user.Sex = reader.GetString(reader.GetOrdinal("P_Sex"))
                user.Suffix = If(reader.IsDBNull(reader.GetOrdinal("P_Suffix")), "n/a", reader.GetString(reader.GetOrdinal("P_Suffix")))
                user.Email = If(reader.IsDBNull(reader.GetOrdinal("email")), "", reader.GetString(reader.GetOrdinal("email")))

                health.Height = If(reader.IsDBNull(reader.GetOrdinal("P_Height")), 0, reader.GetDouble(reader.GetOrdinal("P_Height")))
                health.Weight = If(reader.IsDBNull(reader.GetOrdinal("P_Weight")), 0, reader.GetDouble(reader.GetOrdinal("P_Weight")))
                health.BMI = If(reader.IsDBNull(reader.GetOrdinal("P_BMI")), 0, reader.GetDouble(reader.GetOrdinal("P_BMI")))
                health.BloodType = If(reader.IsDBNull(reader.GetOrdinal("P_Blood_Type")), "", reader.GetString(reader.GetOrdinal("P_Blood_Type")))
                health.PreCon = If(reader.IsDBNull(reader.GetOrdinal("P_Precondition")), "", reader.GetString(reader.GetOrdinal("P_Precondition")))
                health.Treatment = If(reader.IsDBNull(reader.GetOrdinal("P_Treatment")), "", reader.GetString(reader.GetOrdinal("P_Treatment")))
                health.PrevSurg = If(reader.IsDBNull(reader.GetOrdinal("P_PrevSurgery")), "", reader.GetString(reader.GetOrdinal("P_PrevSurgery")))

                Dim addressArr() As String = If(reader.IsDBNull(reader.GetOrdinal("P_Address")), ",,,,,", reader.GetString(reader.GetOrdinal("P_Address"))).Split(","c)

                user_address.HouseNo = Integer.Parse(addressArr(0))
                user_address.ZipCode = Integer.Parse(addressArr(1))
                user_address.Zone = Integer.Parse(addressArr(2))
                user_address.Street = addressArr(3)
                user_address.Barangay = addressArr(4)
                user_address.City = addressArr(5)

                health.Alergy = If(reader.IsDBNull(reader.GetOrdinal("P_Alergy")), "", reader.GetString(reader.GetOrdinal("P_Alergy")))
                health.Medication = If(reader.IsDBNull(reader.GetOrdinal("P_Medication")), "", reader.GetString(reader.GetOrdinal("P_Medication")))

                emergency.FirstName = If(reader.IsDBNull(reader.GetOrdinal("Eme_Firstname")), "", reader.GetString(reader.GetOrdinal("Eme_Firstname")))
                emergency.LastName = If(reader.IsDBNull(reader.GetOrdinal("Eme_Lastname")), "", reader.GetString(reader.GetOrdinal("Eme_Lastname")))
                emergency.MiddleName = If(reader.IsDBNull(reader.GetOrdinal("Eme_Middlename")), "", reader.GetString(reader.GetOrdinal("Eme_Middlename")))
                emergency.Suffix = If(reader.IsDBNull(reader.GetOrdinal("Eme_Suffix")), "", reader.GetString(reader.GetOrdinal("Eme_Suffix")))

                Dim eme_addressArr() As String = If(reader.IsDBNull(reader.GetOrdinal("Eme_Address")), "0,0,0,0,0,0", reader.GetString(reader.GetOrdinal("Eme_Address"))).Split(","c)

                eme_address.HouseNo = Integer.Parse(eme_addressArr(0))
                eme_address.ZipCode = Integer.Parse(eme_addressArr(1))
                eme_address.Zone = Integer.Parse(eme_addressArr(2))
                eme_address.Street = eme_addressArr(3)
                eme_address.Barangay = eme_addressArr(4)
                eme_address.City = eme_addressArr(5)
            End While
        End Using
    End Sub

    Public Shared Function GetPatientInfoById(id As Integer) As PatientModel
        Using connection = GetConnection()
            Dim user As New PatientModel()
            Dim health As New HealthInfoModel()
            Dim emergency As New EmergencyContactModel()
            Dim user_address As New AddressModel()
            Dim eme_address As New AddressModel()

            connection.Open()
            Dim query As String = "SELECT * FROM tb_patientinfo WHERE id = @ID"

            Using cmd As New SqlCommand(query, connection)
                cmd.Parameters.AddWithValue("@ID", id)

                PatientInfoFetcher(cmd, user, health, user_address, emergency, eme_address)
            End Using

            user.HealthInfo = health
            user.Address = user_address

            emergency.Address = eme_address
            user.EmergencyContact = emergency

            Return user
        End Using
    End Function

    Public Shared Function UsernameExists(username As String, role As Role) As Boolean
        Dim tblname As String = If(role = Role.Staff, "tb_staffinfo",
                           If(role = Role.Patient, "tb_patientinfo",
                           "tb_doctorinfo"))

        Dim colname As String = If(role = Role.Patient, "P_", "")
        Dim query As String = $"SELECT COUNT(*) FROM {tblname} WHERE {colname}username = @Username"

        Using connection As New SqlConnection(connectionString)
            connection.Open()
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Username", username)
                Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                Return count > 0
            End Using
        End Using
    End Function

    Public Shared Function IsUsernameExists(username As String) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM tb_patientinfo WHERE P_Username = @Username"

        Using connection As New SqlConnection(connectionString)
            connection.Open()
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@Username", username)
                Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                Return count > 0
            End Using
        End Using
    End Function

    Public Shared Sub PatientRegFunc(patient As PatientModel, mode As ModalMode)
        Using connection As SqlConnection = GetConnection()
            Try
                Dim query As String

                If mode = ModalMode.Add Then
                    query = "INSERT INTO tb_patientinfo (p_FirstName, p_LastName, p_MiddleName, p_Suffix, p_Username, P_Password, P_ContactNumber, P_Bdate, P_Sex, P_Address, email) " &
                    "VALUES (@FirstName, @LastName, @MiddleName, @Suffix, @Username, @Password, @ContactNumber, @Bdate, @Sex, @Address, @Email)"
                Else
                    query = "UPDATE tb_patientinfo SET p_FirstName = @FirstName, p_LastName = @LastName, p_MiddleName = @MiddleName, " &
                    "p_Suffix = @Suffix, p_username = @Username, P_ContactNumber = @ContactNumber, P_Bdate = @Bdate, " &
                    "P_Sex = @Sex, P_Address = @Address, email = @Email WHERE P_username = @Username"
                End If

                Dim command As New SqlCommand(query, connection)

                command.Parameters.AddWithValue("@FirstName", patient.FirstName)
                command.Parameters.AddWithValue("@LastName", patient.LastName)
                command.Parameters.AddWithValue("@MiddleName", patient.MiddleName)
                command.Parameters.AddWithValue("@Suffix", patient.Suffix)
                command.Parameters.AddWithValue("@Username", patient.UserName)
                command.Parameters.AddWithValue("@Password", ProcessMethods.HashCharacter(patient.Password))
                command.Parameters.AddWithValue("@ContactNumber", patient.ContactNumber)
                command.Parameters.AddWithValue("@Bdate", patient.BirthDate)
                command.Parameters.AddWithValue("@Sex", patient.Sex.ToString())
                command.Parameters.AddWithValue("@Email", patient.Email)
                command.Parameters.AddWithValue("@Address", patient.Address.FullAddress)

                connection.Open()
                command.ExecuteNonQuery()
            Catch ex As Exception
                Throw New Exception("Error inserting patient data: " & ex.Message)
            End Try
        End Using
    End Sub

    Public Shared Sub PatientRegFunc(patient As PatientModel, username As String, height As Double, weight As Double, bmi As Double, bloodType As String, preCon As String, treatment As String, prevSurg As String, allergy As String, medication As String, mode As ModalMode)
        Dim query As String

        If mode = ModalMode.Add Then
            query = "MERGE INTO tb_patientinfo AS target " &
            "USING (SELECT @Height AS P_Height, @Weight AS P_Weight, @BMI AS P_BMI, @BloodType AS P_Blood_Type, " &
            "@PreCon AS P_Precondition, @Treatment AS P_Treatment, @PrevSurg AS P_PrevSurgery, @Username AS P_Username, " &
            "@Allergy AS P_Alergy, @Medication AS P_Medication) AS source " &
            "ON target.P_Username = source.P_Username " &
            "WHEN MATCHED THEN " &
            "UPDATE SET P_Height = ISNULL(source.P_Height, target.P_Height), P_Weight = ISNULL(source.P_Weight, target.P_Weight), " &
            "P_BMI = ISNULL(source.P_BMI, target.P_BMI), P_Blood_Type = ISNULL(source.P_Blood_Type, target.P_Blood_Type), " &
            "P_Precondition = ISNULL(source.P_Precondition, target.P_Precondition), P_Treatment = ISNULL(source.P_Treatment, target.P_Treatment), " &
            "P_PrevSurgery = ISNULL(source.P_PrevSurgery, target.P_PrevSurgery), P_Alergy = ISNULL(source.P_Alergy, target.P_Alergy), " &
            "P_Medication = ISNULL(source.P_Medication, target.P_Medication) " &
            "WHEN NOT MATCHED THEN " &
            "INSERT (P_Height, P_Weight, P_BMI, P_Blood_Type, P_Precondition, P_Treatment, P_PrevSurgery, P_Username, P_Alergy, P_Medication) " &
            "VALUES (source.P_Height, source.P_Weight, source.P_BMI, source.P_Blood_Type, source.P_Precondition, source.P_Treatment, " &
            "source.P_PrevSurgery, source.P_Username, source.P_Alergy, source.P_Medication);"
        Else
            query = "UPDATE tb_patientinfo SET P_Height = @Height, P_Weight = @Weight, P_BMI = @BMI, P_Blood_Type = @BloodType, " &
            "P_Precondition = @PreCon, P_Treatment = @Treatment, P_PrevSurgery = @PrevSurg, P_Alergy = @Allergy, P_Medication = @Medication " &
            "WHERE P_Username = @Username"
        End If

        Using connection As SqlConnection = GetConnection()
            Dim cmd As New SqlCommand(query, connection)

            cmd.Parameters.AddWithValue("@Height", If(height > 0, height, DBNull.Value))
            cmd.Parameters.AddWithValue("@Weight", If(weight > 0, weight, DBNull.Value))
            cmd.Parameters.AddWithValue("@BMI", If(bmi > 0, bmi, DBNull.Value))
            cmd.Parameters.AddWithValue("@BloodType", If(String.IsNullOrEmpty(bloodType), DBNull.Value, bloodType))
            cmd.Parameters.AddWithValue("@PreCon", If(String.IsNullOrEmpty(preCon), DBNull.Value, preCon))
            cmd.Parameters.AddWithValue("@Treatment", If(String.IsNullOrEmpty(treatment), DBNull.Value, treatment))
            cmd.Parameters.AddWithValue("@PrevSurg", If(String.IsNullOrEmpty(prevSurg), DBNull.Value, prevSurg))
            cmd.Parameters.AddWithValue("@Allergy", If(String.IsNullOrEmpty(allergy), DBNull.Value, allergy))
            cmd.Parameters.AddWithValue("@Medication", If(String.IsNullOrEmpty(medication), DBNull.Value, medication))
            cmd.Parameters.AddWithValue("@Username", username)

            connection.Open()
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Shared Sub PatientRegFunc(Emergency As EmergencyContactModel, username As String, firstName As String, lastName As String, middleName As String, suffix As String, houseNo As Integer, street As String, barangay As String, city As String, zipCode As Integer, zone As Integer, mode As ModalMode)
        Dim query As String

        If mode = ModalMode.Add Then
            query = "MERGE INTO tb_patientinfo AS target " &
            "USING (SELECT @P_username AS P_Username, @Eme_Firstname AS Eme_Firstname, @Eme_Middlename AS Eme_Middlename, " &
            "@Eme_Lastname AS Eme_Lastname, @Eme_Suffix AS Eme_Suffix, @Eme_Address AS Eme_Address) AS source " &
            "ON target.P_Username = source.P_Username " &
            "WHEN MATCHED THEN " &
            "UPDATE SET Eme_Firstname = ISNULL(source.Eme_Firstname, target.Eme_Firstname), Eme_Middlename = ISNULL(source.Eme_Middlename, target.Eme_Middlename), " &
            "Eme_Lastname = ISNULL(source.Eme_Lastname, target.Eme_Lastname), Eme_Suffix = ISNULL(source.Eme_Suffix, target.Eme_Suffix), " &
            "Eme_Address = ISNULL(source.Eme_Address, target.Eme_Address) " &
            "WHEN NOT MATCHED THEN " &
            "INSERT (P_Username, Eme_Firstname, Eme_Middlename, Eme_Lastname, Eme_Suffix, Eme_Address) " &
            "VALUES (source.P_Username, source.Eme_Firstname, source.Eme_Middlename, source.Eme_Lastname, source.Eme_Suffix, source.Eme_Address);"
        Else
            query = "UPDATE tb_patientinfo SET Eme_Firstname = @Eme_Firstname, Eme_Middlename = @Eme_Middlename, " &
            "Eme_Lastname = @Eme_Lastname, Eme_Suffix = @Eme_Suffix, Eme_Address = @Eme_Address WHERE P_Username = @P_username"
        End If

        Using connection As SqlConnection = GetConnection()
            Try
                Dim command As New SqlCommand(query, connection)

                command.Parameters.AddWithValue("@P_username", username)
                command.Parameters.AddWithValue("@Eme_Firstname", If(String.IsNullOrEmpty(firstName), DBNull.Value, firstName))
                command.Parameters.AddWithValue("@Eme_Middlename", If(String.IsNullOrEmpty(middleName), DBNull.Value, middleName))
                command.Parameters.AddWithValue("@Eme_Lastname", If(String.IsNullOrEmpty(lastName), DBNull.Value, lastName))
                command.Parameters.AddWithValue("@Eme_Suffix", If(String.IsNullOrEmpty(suffix), DBNull.Value, suffix))

                Dim fullAddress As String = houseNo.ToString() & "," & zipCode.ToString() & ", " & zone.ToString() & ", " & street & " street, Brgy. " & barangay & ", " & city
                command.Parameters.AddWithValue("@Eme_Address", If(String.IsNullOrEmpty(fullAddress), DBNull.Value, fullAddress))

                connection.Open()
                command.ExecuteNonQuery()
            Catch ex As Exception
                Throw New Exception("Error updating emergency contact data: " & ex.Message)
            End Try
        End Using
    End Sub



    Public Shared Function GetPatientName(ByVal patient As PatientModel) As String
        Using connection = GetConnection()
            Dim query As String = "SELECT P_Firstname, P_Lastname FROM tb_patientinfo WHERE P_Username = @Username"

            Dim command As New SqlCommand(query, connection)
            command.Parameters.AddWithValue("@Username", patient.UserName)

            Try
                connection.Open()

                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Dim firstName As String = reader("P_Firstname").ToString()
                        Dim lastName As String = reader("P_Lastname").ToString()

                        Return $"{lastName}, {firstName}"
                    Else
                        Throw New Exception("No patient found with the given username.")
                    End If
                End Using
            Catch ex As Exception
                Throw New Exception("Error fetching patient name: " & ex.Message)
            End Try
        End Using
    End Function

    Public Shared Sub DeletePatientByUsername(ByVal username As String)
        Dim query As String = "DELETE FROM tb_patientinfo WHERE P_Username = @Username and "

        Using connection = GetConnection()
            Dim cmd As New SqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@Username", username)

            Try
                connection.Open()
                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                If rowsAffected = 0 Then
                    Throw New Exception("No row found to delete.")
                End If
            Catch ex As Exception
                Throw New Exception("Error deleting patient record: " & ex.Message)
            End Try
        End Using
    End Sub

    Public Shared Sub DeletePatientReg1Data(patient As PatientModel)
        Dim query As String = "
        Delete from tb_patientinfo
        WHERE P_Username = @Username"

        Using connection = GetConnection()
            Dim cmd As New SqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@Username", patient.UserName)

            Try
                connection.Open()
                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                If rowsAffected = 0 Then
                    Throw New Exception("No records were found to delete.")
                End If
            Catch ex As Exception
                Throw New Exception("Error deleting patient data: " & ex.Message)
            End Try
        End Using
    End Sub

    Public Shared Sub NullPatientReg2Data(username As String)
        Dim query As String = "
        UPDATE tb_patientinfo
        SET 
            P_Height = NULL,
            P_Weight = NULL,
            P_BMI = NULL,
            P_Blood_Type = NULL,
            P_Precondition = NULL,
            P_Treatment = NULL,
            P_PrevSurgery = NULL,
            P_Alergy = NULL,
            P_Medication = NULL
        WHERE P_Username = @Username"

        Using connection = GetConnection()
            Dim cmd As New SqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@Username", username)

            Try
                connection.Open()
                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                If rowsAffected = 0 Then
                    Throw New Exception("No records were found to update.")
                End If
            Catch ex As Exception
                Throw New Exception("Error deleting patient data: " & ex.Message)
            End Try
        End Using
    End Sub

    Public Shared Function GetSpecialization() As List(Of String)
        Dim specializationSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Dim query As String = "SELECT Specialization FROM tb_doctorinfo"

        Using connection As SqlConnection = GetConnection()
            Dim command As New SqlCommand(query, connection)

            Try
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim specializations As String = If(reader("Specialization")?.ToString(), String.Empty)
                        If Not String.IsNullOrEmpty(specializations) Then
                            ' Split and trim each specialization
                            For Each spec As String In specializations.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
                                specializationSet.Add(spec.Trim())
                            Next
                        End If
                    End While
                End Using
            Catch ex As Exception
                Throw New Exception("Error getting specializations: " & ex.Message)
            End Try
        End Using

        Return specializationSet.ToList()
    End Function

    Public Shared Function GetDoctorNames(specialization As String) As List(Of String)
        Dim doctorNames As New List(Of String)()

        Dim query As String = "
        SELECT DISTINCT CONCAT('Dr. ', last_name, ', ', first_name) AS doctor_name
        FROM tb_doctor_specializations ds
        JOIN tb_doctorinfo di ON ds.doctor_id = di.doctor_id
        WHERE ds.specialization = @Specialization;
        "

        Using connection As SqlConnection = GetConnection()
            Dim command As New SqlCommand(query, connection)
            command.Parameters.AddWithValue("@Specialization", specialization)

            Try
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        doctorNames.Add(reader("doctor_name").ToString())
                    End While
                End Using
            Catch ex As Exception
                Throw New Exception("Error getting doctor names: " & ex.Message)
            End Try
        End Using

        Return doctorNames
    End Function

    Public Shared Function GetDoctorAvailableTimes(doctorName As String, specialization As String) As List(Of String)
        Dim availableTimes As New List(Of String)()

        Dim query As String = "
    SELECT DISTINCT di.start_time, di.end_time, di.day_availability 
    FROM tb_doctorinfo di
    JOIN tb_doctor_specializations ds ON di.doctor_id = ds.doctor_id
    WHERE CONCAT('Dr. ', di.last_name, ', ', di.first_name) = @DoctorName
      AND ds.specialization = @Specialization;
"

        Using connection As SqlConnection = GetConnection()
            Dim cmd As New SqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@DoctorName", doctorName)
            cmd.Parameters.AddWithValue("@Specialization", specialization)

            Try
                connection.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim startTime As TimeSpan = DirectCast(reader("start_time"), TimeSpan)
                        Dim endTime As TimeSpan = DirectCast(reader("end_time"), TimeSpan)

                        Dim totalDuration As TimeSpan = endTime - startTime
                        Dim slotDuration As TimeSpan = TimeSpan.FromTicks(totalDuration.Ticks \ 4)

                        Dim currentTime As TimeSpan = startTime

                        For i As Integer = 0 To 3
                            availableTimes.Add(currentTime.ToString("hh\:mm"))
                            currentTime = currentTime.Add(slotDuration)
                        Next
                    End While
                End Using
            Catch ex As Exception
                Throw New Exception($"Error fetching available times: {ex.Message}")
            End Try
        End Using

        Return availableTimes
    End Function

    Public Shared Function GetDoctorAvailability(doctorName As String) As String
        Dim query As String = "
    SELECT day_availability 
    FROM tb_doctorinfo 
    WHERE CONCAT('Dr. ', last_name, ', ', first_name) = @DoctorName"

        Using connection As SqlConnection = GetConnection()
            Dim command As New SqlCommand(query, connection)
            command.Parameters.AddWithValue("@DoctorName", doctorName)

            Try
                connection.Open()
                Dim result As Object = command.ExecuteScalar()
                If result IsNot Nothing Then
                    Dim dayAvailability As String = result.ToString().Replace("-", ",")
                    Return dayAvailability
                End If
                Return String.Empty
            Catch ex As Exception
                Throw New Exception("Error fetching doctor availability: " & ex.Message)
            End Try
        End Using
    End Function

    Public Shared Function GetConsultationFee(doctorName As String) As Decimal?
        Using connection As SqlConnection = GetConnection()
            Dim query As String = "SELECT consultation_fee 
                           FROM tb_doctorinfo 
                           WHERE CONCAT('Dr. ', last_name, ', ', first_name) = @doctorName"

            Dim command As New SqlCommand(query, connection)
            command.Parameters.AddWithValue("@doctorName", doctorName)

            Try
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        If reader.IsDBNull(reader.GetOrdinal("consultation_fee")) Then
                            Return Nothing
                        Else
                            Return reader.GetDecimal("consultation_fee")
                        End If
                    Else
                        Return Nothing
                    End If
                End Using
            Catch ex As Exception
                Throw New Exception("Error fetching consultation fee: " & ex.Message)
            End Try
        End Using
    End Function

    Public Shared Function IsPatientAppointmentPendingOrAccepted(patientName As String) As Boolean
        Try
            Using connection As New SqlConnection(connectionString)
                connection.Open()
                Dim query As String = "
        SELECT COUNT(*) 
        FROM tb_appointmenthistory 
        WHERE ah_Patient_Name = @PatientName 
          AND ah_status IN ('Pending', 'Accepted')"

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@PatientName", patientName)

                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                    Return count > 5
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error checking appointment status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    Public Shared Function IsDoctorOccupied(doctorName As String, appointmentDate As DateTime) As Boolean
        Try
            Using connection As New SqlConnection(connectionString)
                connection.Open()
                Dim query As String = "
        SELECT COUNT(*) 
        FROM tb_appointmenthistory 
        WHERE ah_Doctor_Name = @DoctorName 
          AND ah_date = @AppointmentDate 
          AND ah_status IN ('Pending', 'Accepted')"

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@DoctorName", doctorName)
                    command.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date)

                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error checking doctor's availability: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    Public Shared Function SaveAppointment(patientName As String, specialization As String, doctorName As String, timeSlot As String, appointmentDate As DateTime, consFee As Decimal) As Boolean
        Try
            Using connection As New SqlConnection(connectionString)
                Dim query As String = "INSERT INTO tb_appointmenthistory (ah_Patient_Name, ah_Specialization, ah_Doctor_Name, ah_time, ah_date, ah_consfee, ah_status) 
                     VALUES (@PatientName, @Specialization, @DoctorName, @TimeSlot, @AppointmentDate, @ConsFee, @Pending)"

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@PatientName", patientName)
                    command.Parameters.AddWithValue("@Specialization", specialization)
                    command.Parameters.AddWithValue("@DoctorName", doctorName)
                    command.Parameters.AddWithValue("@TimeSlot", timeSlot)
                    command.Parameters.AddWithValue("@AppointmentDate", appointmentDate)
                    command.Parameters.AddWithValue("@ConsFee", consFee)
                    command.Parameters.AddWithValue("@Pending", "Pending")

                    connection.Open()

                    Dim rowsAffected As Integer = command.ExecuteNonQuery()

                    Return rowsAffected > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"An error occurred while saving the appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    Public Shared Function ViewAppointments(doctorFullName As String) As DataTable
        Dim query As String = "SELECT ah_Patient_Name AS 'Patient Name', id, ah_Specialization AS 'Doctor Specialization', ah_time AS 'Appointment Time', ah_date AS 'Appointment Date', ah_consfee AS 'Consultation Fee' FROM tb_appointmenthistory " &
                          "WHERE ah_status = 'Accepted' AND ah_Doctor_Name = @DoctorFullName"

        Dim AppointmentTable As New DataTable()

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@DoctorFullName", doctorFullName)
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(AppointmentTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving appointment list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return AppointmentTable
    End Function

    Public Shared Function ViewCompletedAppointments(doctorFullName As String) As DataTable
        Dim query As String = "SELECT id, ah_Patient_Name as 'Patient Name', ah_doctor_name as 'Doctor Name', ah_specialization as 'Specialization', ah_time as 'Appointment Time', ah_date as 'Appointment Date', ah_consfee as 'Consultation Fee' FROM tb_appointmenthistory " &
                          "WHERE ah_status = 'Completed'"

        'aayusin pa yung sa doctor for now completed muna
        Dim AppointmentTable As New DataTable()

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@DoctorFullName", doctorFullName)
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(AppointmentTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving appointment list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return AppointmentTable
    End Function

    Public Shared Function PendingAppointmentList(doctorFullName As String) As DataTable
        Dim query As String = "SELECT ah_Patient_Name AS 'Patient Name', id , ah_Specialization AS 'Doctor Specialization', ah_time AS 'Appointment Time', ah_date AS 'Appointment Date', ah_consfee AS 'Consultation Fee' FROM tb_appointmenthistory " &
                          "WHERE ah_status = 'Pending' AND ah_Doctor_Name = @DoctorFullName"

        Dim appointmentTable As New DataTable()

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@DoctorFullName", doctorFullName)

                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(appointmentTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving appointment list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return appointmentTable
    End Function

    Public Shared Function DeclinedAppointments(doctorFullName As String) As DataTable
        Dim query As String = "SELECT ah_Patient_Name AS 'Patient Name', id AS 'Transaction ID', ah_Specialization AS 'Doctor Specialization', ah_time AS 'Appointment Time', ah_date AS 'Appointment Date', ah_consfee AS 'Consultation Fee'  FROM tb_appointmenthistory " &
                          "WHERE ah_status = 'Declined' AND ah_Doctor_Name = @DoctorFullName"

        Dim AppointmentTable As New DataTable()

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@DoctorFullName", doctorFullName)
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(AppointmentTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving appointment list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return AppointmentTable
    End Function

    Public Shared Sub ReconsiderAppointment(appointmentId As Integer)
        Dim updateQuery As String = "UPDATE tb_appointmenthistory SET ah_status = 'Pending' WHERE id = @id AND ah_status = 'Declined'"

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(updateQuery, conn)
                    cmd.Parameters.AddWithValue("@id", appointmentId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error reconsidering appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Shared Function ChecOutList() As DataTable
        Dim query As String = "SELECT ah_patient_name AS 'Patient Name', ah_Consfee AS 'Consultation Fee', ah_time AS 'Appointment Time', ah_date AS 'Appointment Date' FROM tb_appointmenthistory WHERE ah_status = 'CheckOut'"

        Dim CheckoutTable As New DataTable()

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(CheckoutTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return CheckoutTable
    End Function
    Public Shared Sub CreateDiagnosis(appointmentId As Integer, onSuccess As Action(Of Dictionary(Of String, String)), onFailure As Action(Of String))
        Try
            Using connection As New SqlConnection(connectionString)
                connection.Open()

                Dim getPatientNameQuery As String = "SELECT ah_Patient_Name FROM tb_appointmenthistory WHERE id = @appointmentId AND ah_status = 'Accepted'"
                Dim patientName As String = String.Empty

                Using command As New SqlCommand(getPatientNameQuery, connection)
                    command.Parameters.AddWithValue("@appointmentId", appointmentId)

                    Dim result As Object = command.ExecuteScalar()
                    If result Is Nothing Then
                        onFailure?.Invoke("No patient associated with this appointment.")
                        Return
                    End If

                    patientName = result.ToString()
                End Using

                Dim nameParts As String() = patientName.Split(","c)
                If nameParts.Length < 2 Then
                    onFailure?.Invoke("Invalid patient name format in the database.")
                    Return
                End If

                Dim lastName As String = nameParts(0).Trim()
                Dim firstName As String = nameParts(1).Trim()

                Dim getPatientDetailsQuery As String = "SELECT P_Firstname, P_Lastname, P_Bdate, P_Height, P_Weight, P_BMI, " &
                                                    "P_Blood_Type, P_Alergy, P_Medication, P_PrevSurgery, P_Precondition, P_Treatment " &
                                                    "FROM tb_patientinfo WHERE P_Firstname = @firstName AND P_Lastname = @lastName"

                Using command As New SqlCommand(getPatientDetailsQuery, connection)
                    command.Parameters.AddWithValue("@firstName", firstName)
                    command.Parameters.AddWithValue("@lastName", lastName)

                    Using reader As SqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Dim patientDetails As New Dictionary(Of String, String) From {
                            {"P_Firstname", reader("P_Firstname").ToString()},
                            {"P_Lastname", reader("P_Lastname").ToString()},
                            {"P_Bdate", reader("P_Bdate").ToString()},
                            {"P_Height", reader("P_Height").ToString()},
                            {"P_Weight", reader("P_Weight").ToString()},
                            {"P_BMI", reader("P_BMI").ToString()},
                            {"P_Blood_Type", reader("P_Blood_Type").ToString()},
                            {"P_Alergy", reader("P_Alergy").ToString()},
                            {"P_Medication", reader("P_Medication").ToString()},
                            {"P_PrevSurgery", reader("P_PrevSurgery").ToString()},
                            {"P_Precondition", reader("P_Precondition").ToString()},
                            {"P_Treatment", reader("P_Treatment").ToString()}
                        }

                            onSuccess?.Invoke(patientDetails)
                        Else
                            onFailure?.Invoke("Patient details not found.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            onFailure?.Invoke($"An error occurred: {ex.Message}")
        End Try
    End Sub

    Public Shared Sub AcceptAppointment(appointmentId As Integer)
        Dim updateQuery As String = "UPDATE tb_appointmenthistory SET ah_status = 'Accepted' WHERE id = @id AND ah_status = 'Pending'"

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(updateQuery, conn)
                    cmd.Parameters.AddWithValue("@id", appointmentId)

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error accepting appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Shared Sub DeclineAppointment(appointmentId As Integer)
        Dim query As String = "UPDATE tb_appointmenthistory SET ah_status = 'Declined' WHERE id = @id AND ah_status = 'Pending'"

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", appointmentId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error declining appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Shared Sub viewDocument(appointmentId As Integer, onSuccess As Action(Of Dictionary(Of String, String)), onFailure As Action(Of String))
        Try
            Using connection As New SqlConnection(connectionString)
                connection.Open()

                Dim getPatientNameQuery As String = "SELECT * FROM tb_appointmenthistory WHERE id = @appointmentId"
                Using command As New SqlCommand(getPatientNameQuery, connection)
                    command.Parameters.AddWithValue("@appointmentId", appointmentId)

                    Using reader As SqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Dim patientName As String = reader("ah_Patient_Name").ToString()
                            Dim doctorName As String = reader("ah_Doctor_Name").ToString()
                            Dim nameParts As String() = patientName.Split(","c)
                            Dim doctorParts As String() = doctorName.Split(","c)

                            If nameParts.Length < 2 OrElse doctorParts.Length < 2 Then
                                onFailure?.Invoke("Invalid patient or doctor name format in the database.")
                                Return
                            End If

                            Dim patientFirstName As String = nameParts(1).Trim()
                            Dim patientLastName As String = nameParts(0).Trim()
                            Dim doctorFirstName As String = doctorParts(1).Trim()
                            Dim doctorLastName As String = doctorParts(0).Trim()

                            Dim patientDetails As New Dictionary(Of String, String) From {
                            {"P_Firstname", patientFirstName},
                            {"P_Lastname", patientLastName},
                            {"P_Bdate", reader("P_bdate").ToString()},
                            {"P_Height", reader("P_height").ToString()},
                            {"P_Weight", reader("P_weight").ToString()},
                            {"P_BMI", reader("P_bmi").ToString()},
                            {"P_Blood_Type", reader("P_Blood_type").ToString()},
                            {"P_Alergy", reader("P_alergy").ToString()},
                            {"P_Medication", reader("P_medication").ToString()},
                            {"P_PrevSurgery", reader("P_prevsurgery").ToString()},
                            {"P_Precondition", reader("P_precondition").ToString()},
                            {"P_Treatment", reader("P_treatment").ToString()},
                            {"ah_DoctorFirstName", doctorFirstName},
                            {"ah_DoctorLastName", doctorLastName},
                            {"ah_Time", reader("ah_time").ToString()},
                            {"ah_Date", reader("ah_date").ToString()},
                            {"ah_Consfee", reader("ah_Consfee").ToString()},
                            {"d_diagnosis", reader("d_diagnosis").ToString()},
                            {"d_additionalnotes", reader("d_additionalnotes").ToString()},
                            {"d_doctororder", reader("d_doctororder").ToString()},
                            {"d_prescription", reader("d_prescription").ToString()}
                        }

                            onSuccess?.Invoke(patientDetails)
                        Else
                            onFailure?.Invoke("Appointment details not found.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            onFailure?.Invoke($"An error occurred: {ex.Message}")
        End Try
    End Sub
    Public Sub CheckOutAppointment(appointmentID As Integer)
        Dim updateQuery As String = "UPDATE tb_appointmenthistory SET ah_status = 'CheckOut' WHERE id = @id AND ah_status = 'Completed'"

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(updateQuery, conn)
                    cmd.Parameters.AddWithValue("@id", appointmentID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error CheckOut appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Shared Function GetDoctorSpecialization(doctorFullName As String) As String
        Dim specialization As String = String.Empty
        Dim query As String = "SELECT specialization FROM tb_doctorinfo WHERE CONCAT('Dr. ', last_name, ', ', first_name) = @doctorName"

        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@doctorName", doctorFullName)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    specialization = result.ToString()
                End If
            End Using
        End Using

        Return specialization
    End Function
    Public Shared Function CheckOutAppointmentList(doctorFullName As String) As DataTable
        Dim query As String = "SELECT ah_Patient_Name, id, ah_Specialization, ah_doctor_name, ah_time, ah_date, ah_consfee FROM tb_appointmenthistory " &
                          "WHERE ah_status = 'CheckOut' AND ah_Doctor_Name = @DoctorFullName"

        Dim appointmentTable As New DataTable()
        Try
            Using conn As SqlConnection = GetConnection()
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@DoctorFullName", doctorFullName)

                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(appointmentTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving appointment list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return appointmentTable
    End Function
    Public Shared Function PatientList() As DataTable
        Dim query As String = "SELECT id AS 'Patient ID', p_Firstname AS 'First Name', p_middleName AS 'Middle Name', P_lastname AS 'Last Name', p_suffix AS 'Suffix', p_sex AS 'Sex', P_bdate AS 'Birth Date', p_address AS 'Full Address' FROM tb_patientinfo"

        Dim PatientTable As New DataTable()

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(PatientTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving patient list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return PatientTable
    End Function
    Public Shared Function AppointmentList() As DataTable
        Dim AppointmentTable As New DataTable()

        Try
            Dim query As String = "SELECT id AS 'Transaction ID', ah_patient_name AS 'Patient Name', ah_doctor_name AS 'Doctor Name', ah_Specialization AS 'Specialization', ah_time AS 'Time Slot', ah_date AS 'Date', ah_consfee AS 'Consultation Fee' FROM tb_appointmenthistory"

            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(AppointmentTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving appointment list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return AppointmentTable
    End Function

    Public Shared Function ViewPatientAppointments(patientName As String) As DataTable
        Dim AppointmentTable As New DataTable()
        Dim query As String = "SELECT id, ah_status AS 'Status', ah_doctor_name AS 'Doctor Name', ah_specialization AS 'Specialization', ah_time AS 'Appointment Time', ah_date AS 'Appointment Date', ah_consfee AS 'Consultation Fee' FROM tb_appointmenthistory WHERE ah_Patient_Name = @PatientName AND (ah_status = 'Accepted' OR ah_status = 'Pending' OR ah_status = 'Declined')"

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@PatientName", patientName)
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(AppointmentTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving appointment list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return AppointmentTable
    End Function

    Public Shared Function DeleteAppointmentByPatient(accountId As Integer) As Boolean
        Dim query As String = "DELETE FROM tb_appointmenthistory WHERE id = @ID AND (ah_status = 'Pending' OR ah_status = 'Declined')"

        Using connection As SqlConnection = GetConnection()
            Using cmd As New SqlCommand(query, connection)
                cmd.Parameters.AddWithValue("@ID", accountId)

                Try
                    connection.Open()
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected = 0 Then
                        Console.WriteLine("No record found with the specified ID.")
                        Return False
                    End If

                    Return True
                Catch ex As Exception
                    Console.WriteLine("Error deleting appointment: " & ex.Message)
                    Return False
                End Try
            End Using
        End Using
    End Function

    Public Shared Function GetInvoiceList(fullName As String) As DataTable
        Dim query As String = "SELECT id as 'ID', ah_Patient_Name AS 'Patient Name', ah_specialization AS 'Specialization', ah_Doctor_Name AS 'Doctor Name', ah_Time AS 'Time', ah_date AS 'Date', ah_status AS 'Status' FROM tb_appointmenthistory WHERE ah_Patient_Name = @FullName AND (ah_status = 'Completed' OR ah_status = 'InvoiceChecked')"

        Dim invoiceTable As New DataTable()

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@FullName", fullName)
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(invoiceTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving invoice list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return invoiceTable
    End Function

    Public Shared Function GetAppointmentById(id As Integer) As Appointment
        Dim query As String = "SELECT * FROM tb_appointmenthistory WHERE id = @ID"

        Dim appoint As New Appointment()

        Using con As SqlConnection = GetConnection()
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@ID", id)
                con.Open()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        appoint.PatientName = reader.GetString("ah_Patient_Name")
                        appoint.DoctorName = reader.GetString("ah_Doctor_Name")
                        appoint.ConsultationFee = reader.GetDecimal("ah_consfee")
                        appoint.Time = TimeSpan.Parse(reader.GetString("ah_time"))
                        appoint.Date = reader.GetDateTime("ah_date")
                        appoint.Specialization = reader.GetString("ah_specialization")
                        appoint.ConfineDays = reader.GetInt32("confinement_days")

                        appoint.Diagnosis = New DiagnosisModel() With {
                        .DoctorOrders = reader.GetString("d_doctororder"),
                        .Prescription = reader.GetString("d_prescription")
                    }
                    End While
                End Using
            End Using
        End Using

        Return appoint
    End Function
    Public Shared Function IsEmailExisted(role As Role, user As UserModel) As Boolean
        Using connection As SqlConnection = GetConnection()
            connection.Open()

            ' Get the table name based on the role
            Dim tablename As String = ProcessMethods.GetTablenameByRole(role)

            ' Prepare the query with role-specific column names
            Dim query As String = $"SELECT COUNT(*) FROM {tablename} WHERE email = @Email AND {(If(role = Role.Patient, "P_", ""))}username = @Username"
            Dim command As New SqlCommand(query, connection)

            command.Parameters.AddWithValue("@Email", user.Email)
            command.Parameters.AddWithValue("@Username", user.UserName)

            ' Execute the query and get the result
            Dim result As Integer = Convert.ToInt32(command.ExecuteScalar())

            ' Return true if a matching email and username exist, otherwise false
            Return result <> 0
        End Using
    End Function
    Public Shared Sub UpdateUserPassword(role As Role, user As UserModel)
        Using connection As SqlConnection = GetConnection()
            connection.Open()
            Dim tblName As String = ProcessMethods.GetTablenameByRole(role)

            Try
                Dim query As String = $"UPDATE {tblName} SET {(If(role = Role.Patient, "P_", ""))}password = @Password WHERE email = @Email AND {(If(role = Role.Patient, "P_", ""))}username = @UserName"
                Dim cmd As New SqlCommand(query, connection)

                cmd.Parameters.AddWithValue("@Password", user.Password)
                cmd.Parameters.AddWithValue("@Email", user.Email)
                cmd.Parameters.AddWithValue("@UserName", user.UserName)

                cmd.ExecuteScalar()
                Return
            Catch ex As Exception
                Throw ex
            End Try
        End Using
    End Sub
    Public Shared Sub UpdateStatus(doctorName As String)
        Dim query As String = "UPDATE tb_appointmenthistory " &
                          "SET ah_status = @status " &
                          "WHERE ah_Doctor_Name = @doctorName AND ah_status = 'Checkout'"

        Using conn As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(query, conn)
                Dim status As String = "InvoiceChecked"
                cmd.Parameters.AddWithValue("@status", status)
                cmd.Parameters.AddWithValue("@doctorName", doctorName)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

End Class
