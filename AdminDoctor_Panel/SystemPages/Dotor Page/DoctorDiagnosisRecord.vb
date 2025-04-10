Imports Microsoft.Data.SqlClient
Partial Public Class DoctorDiagnosisRecord
    Inherits Form

    Private prevForm As DoctorMedicalRecord
    Public Event LoadAppointmentsList As EventHandler
    Public Sub New(prevForm As DoctorMedicalRecord)
        InitializeComponent()
        Me.prevForm = prevForm
    End Sub

    Private Sub doctor_ExitButton_Click(sender As Object, e As EventArgs) Handles doctor_ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to close? Unsaved changes will be lost.", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub doctor_MinimizeButton_Click(sender As Object, e As EventArgs) Handles doctor_MinimizeButton.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Public Sub SetPatientName(firstName As String, lastName As String)
        PatientNameLabel.Text = $"{lastName}, {firstName}"
    End Sub

    Private Sub BackButton_Click(sender As Object, e As EventArgs) Handles BackButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to go back? Unsaved changes will be lost.", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Function GetValueFromFlowLayout(panel As FlowLayoutPanel) As String
        Dim list As New Dictionary(Of String, Decimal)()

        For Each control As Control In panel.Controls
            Dim desc As DescPrice = TryCast(control, DescPrice)
            If desc IsNot Nothing Then
                list.Add(desc.Desc, desc.Price)
            End If
        Next

        Return String.Join(", ", list.Select(Function(vp) $"{vp.Key} {vp.Value}"))
    End Function

    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Dim diagnosis As String = DiagnosisTextBox.Text.Trim()
        Dim doctorOrder As String = GetValueFromFlowLayout(DoctorOrdersFlowLayoutPanel)
        Dim additionalNote As String = AdditionalNoteTextBox.Text.Trim()
        Dim prescription As String = GetValueFromFlowLayout(prescritionFlowLayoutPanel)
        Dim patientName As String = PatientNameLabel.Text
        Dim confinementDays As Integer

        If String.IsNullOrEmpty(patientName) Then
            MessageBox.Show("Patient name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Convert ConfinementDays safely
        If Not Integer.TryParse(ConfineDaysTextbox.Text.Trim(), confinementDays) Then
            confinementDays = 0 ' Default value if parsing fails
        End If

        Dim query As String = "UPDATE tb_AppointmentHistory SET " &
                              "d_diagnosis = @Diagnosis, " &
                              "d_doctororder = @DoctorOrder, " &
                              "d_additionalnotes = @AdditionalNote, " &
                              "d_prescription = @Prescription, " &
                              "ah_Status = @Status, " &
                              "confinement_days = @ConfinementDays " &
                              "WHERE ah_Patient_Name = @PatientName AND ah_status = 'Accepted'"

        Dim parameters As New Dictionary(Of String, Object)() From {
            {"@Diagnosis", If(String.IsNullOrEmpty(diagnosis), DBNull.Value, diagnosis)},
            {"@DoctorOrder", If(String.IsNullOrEmpty(doctorOrder), DBNull.Value, doctorOrder)},
            {"@AdditionalNote", If(String.IsNullOrEmpty(additionalNote), DBNull.Value, additionalNote)},
            {"@ConfinementDays", If(String.IsNullOrEmpty(ConfineDaysTextbox.Text), DBNull.Value, confinementDays)},
            {"@Prescription", If(String.IsNullOrEmpty(prescription), DBNull.Value, prescription)},
            {"@Status", If(String.IsNullOrEmpty(prescription), DBNull.Value, "Completed")},
            {"@PatientName", patientName}
        }

        Database.ExecuteQuery(query, parameters)

        MessageBox.Show("Appointment details saved successfully and marked as completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Me.Close()
        prevForm.Close()

        RaiseEvent LoadAppointmentsList(Me, EventArgs.Empty)
    End Sub

    Private Sub DoctorDiagnosisRecord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadAppointmentDetails()
    End Sub

    Private Sub LoadAppointmentDetails()
        Dim query As String = "SELECT ah_doctor_name, ah_specialization, ah_consfee, ah_time, ah_date " &
                              "FROM tb_appointmenthistory " &
                              "WHERE ah_Patient_Name = @PatientName"
        Try
            Using connection As SqlConnection = Database.GetConnection()
                connection.Open()
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@PatientName", PatientNameLabel.Text)

                    Using reader As SqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Dim doctorName As String = reader("ah_doctor_name").ToString()
                            Dim nameParts() As String = doctorName.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)

                            If nameParts.Length = 2 Then
                                DoctorLastNameLabel.Text = nameParts(0).Trim()
                                DoctorFirstNameLabel.Text = nameParts(1).Trim()
                            Else
                                DoctorLastNameLabel.Text = "Unknown"
                                DoctorFirstNameLabel.Text = "Unknown"
                            End If

                            DoctorSpecializationLabel.Text = reader("ah_specialization").ToString()
                            DoctorConsultationFeeLabel.Text = $"${reader("ah_consfee").ToString()}"
                            DoctorTimeLabel.Text = reader("ah_time").ToString()
                            DoctorDateLabel.Text = reader("ah_date").ToString()
                        Else
                            DoctorFirstNameLabel.Text = "No record found"
                            DoctorLastNameLabel.Text = "No record found"
                            DoctorSpecializationLabel.Text = "No record found"
                            DoctorConsultationFeeLabel.Text = "No record found"
                            DoctorTimeLabel.Text = "No record found"
                            DoctorDateLabel.Text = "No record found"
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub addPrescription_Click(sender As Object, e As EventArgs) Handles addPrescription.Click
        Dim descPriceTile As New DescPrice()
        AddHandler descPriceTile.RemoveTile, AddressOf DeletePrescrip
        prescritionFlowLayoutPanel.Controls.Add(descPriceTile)
    End Sub
    Public Sub DeleteDoctorOrder(sender As DescPrice)
        DoctorOrdersFlowLayoutPanel.Controls.Remove(sender)
    End Sub

    Public Sub DeletePrescrip(sender As DescPrice)
        prescritionFlowLayoutPanel.Controls.Remove(sender)
    End Sub

    Private Sub ConfineDaysTextbox_TextChanged(sender As Object, e As EventArgs) Handles ConfineDaysTextbox.TextChanged
        ' No implementation needed
    End Sub

    Private Sub ConfineDaysTextbox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles ConfineDaysTextbox.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If
    End Sub
    Private Sub addDoctorOrder_Click(sender As Object, e As EventArgs) Handles addDoctorOrder.Click
        Dim descPriceTile As New DescPrice()
        AddHandler descPriceTile.RemoveTile, AddressOf DeleteDoctorOrder
        DoctorOrdersFlowLayoutPanel.Controls.Add(descPriceTile)
    End Sub
End Class
