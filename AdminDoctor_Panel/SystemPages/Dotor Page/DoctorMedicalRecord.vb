Partial Public Class DoctorMedicalRecord
    Inherits Form

    Public Event LoadAppointmentsList As EventHandler
    Public Sub New()
        InitializeComponent()
    End Sub
    Public Sub SetDoctorName(doctorFullName As String)
        DoctorFullNameLabel.Text = doctorFullName
    End Sub

    Public Sub SetPatientDetails(firstName As String, lastName As String, birthday As String, height As String, weight As String,
                                  bmi As String, bloodType As String, allergy As String, medication As String,
                                  prevSurgery As String, preCondition As String, treatment As String)
        FirstNameTextBox.Text = firstName
        LastNameTextBox.Text = lastName
        BirthdayTextBox.Text = birthday
        HeightTextBox.Text = height
        WeightTextBox.Text = weight
        BMITextBox.Text = bmi
        BloodTypeTextBox.Text = bloodType
        AllergyTextBox.Text = allergy
        MedicationTextBox.Text = medication
        PreviousSurgeryTextBox.Text = prevSurgery
        PreConditionTextBox.Text = preCondition
        TreatmentTextBox.Text = treatment
    End Sub

    Private Sub guna2TextBox10_TextChanged(sender As Object, e As EventArgs)
        ' Event logic here
    End Sub

    Private Sub doctor_ExitButton_Click(sender As Object, e As EventArgs) Handles doctor_ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to close?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub doctor_MinimizeButton_Click(sender As Object, e As EventArgs) Handles doctor_MinimizeButton.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub BackButton_Click(sender As Object, e As EventArgs) Handles BackButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to go back?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirm = DialogResult.Yes Then
            Me.Hide()
        End If
    End Sub

    Private Sub ContinueButton_Click(sender As Object, e As EventArgs) Handles ContinueButton.Click
        Dim result As DialogResult = MessageBox.Show("Do you want to save the information?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Try
                Dim firstName As String = FirstNameTextBox.Text.Trim()
                Dim lastName As String = LastNameTextBox.Text.Trim()
                Dim bloodType As String = BloodTypeTextBox.Text.Trim()
                Dim bmi As Double = If(Double.TryParse(BMITextBox.Text.Trim(), bmi), bmi, 0)
                Dim weight As Double = If(Double.TryParse(WeightTextBox.Text.Trim(), weight), weight, 0)
                Dim height As Double = If(Double.TryParse(HeightTextBox.Text.Trim(), height), height, 0)
                Dim allergy As String = AllergyTextBox.Text.Trim()
                Dim previousSurgery As String = PreviousSurgeryTextBox.Text.Trim()
                Dim treatment As String = TreatmentTextBox.Text.Trim()
                Dim medication As String = MedicationTextBox.Text.Trim()
                Dim preCondition As String = PreConditionTextBox.Text.Trim()
                Dim birthday As DateTime = If(DateTime.TryParse(BirthdayTextBox.Text.Trim(), birthday), birthday, DateTime.MinValue)

                If String.IsNullOrEmpty(firstName) OrElse String.IsNullOrEmpty(lastName) Then
                    MessageBox.Show("First Name and Last Name are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                Dim patientName As String = $"{lastName}, {firstName}"

                Dim query As String = "UPDATE tb_AppointmentHistory SET  
                   P_Bdate = @Birthday,  
                   P_Height = @Height,  
                   P_Weight = @Weight,  
                   P_BMI = @BMI,  
                   P_Blood_Type = @BloodType,  
                   P_Precondition = @PreCondition,  
                   P_Treatment = @Treatment,  
                   P_PrevSurgery = @PreviousSurgery,  
                   P_Alergy = @Allergy,  
                   P_Medication = @Medication,  
                   ah_status = 'Accepted'  
                WHERE ah_Patient_Name = @PatientName and ah_status = 'Accepted'"

                Dim parameters As New Dictionary(Of String, Object) From {
                   {"@Birthday", If(birthday = DateTime.MinValue, DBNull.Value, birthday)},
                   {"@Height", If(height > 0, height, DBNull.Value)},
                   {"@Weight", If(weight > 0, weight, DBNull.Value)},
                   {"@BMI", If(bmi > 0, bmi, DBNull.Value)},
                   {"@BloodType", If(String.IsNullOrEmpty(bloodType), DBNull.Value, bloodType)},
                   {"@PreCondition", If(String.IsNullOrEmpty(preCondition), DBNull.Value, preCondition)},
                   {"@Treatment", If(String.IsNullOrEmpty(treatment), DBNull.Value, treatment)},
                   {"@PreviousSurgery", If(String.IsNullOrEmpty(previousSurgery), DBNull.Value, previousSurgery)},
                   {"@Allergy", If(String.IsNullOrEmpty(allergy), DBNull.Value, allergy)},
                   {"@Medication", If(String.IsNullOrEmpty(medication), DBNull.Value, medication)},
                   {"@PatientName", patientName}
               }

                Database.ExecuteQuery(query, parameters)

                MessageBox.Show("Appointment history updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Dim diagnosisRecord As New DoctorDiagnosisRecord(Me)
                AddHandler diagnosisRecord.LoadAppointmentsList, AddressOf LoadAppointmentsListHandler
                diagnosisRecord.SetPatientName(firstName, lastName)
                diagnosisRecord.Show()

            Catch ex As Exception
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub LoadAppointmentsListHandler(sender As Object, e As EventArgs)
        RaiseEvent LoadAppointmentsList(sender, e)
    End Sub
End Class
