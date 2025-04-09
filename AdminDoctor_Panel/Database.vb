Imports System.Data.SqlClient
Imports System.Globalization
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
                user.BirthDate = DateTime.ParseExact(reader.GetString(reader.GetOrdinal("P_Bdate")), "dd-MM-yyyy", CultureInfo.InvariantCulture)
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

End Class
