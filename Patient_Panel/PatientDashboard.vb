Imports System.Globalization
Imports AdminDoctor_Panel
Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models

Public Class PatientDashboard
    Inherits Form

    Private patient As PatientModel

    Public Sub New(patient As PatientModel)
        InitializeComponent()
        Me.patient = patient

        NameLabel.Text = $"{patient.FirstName}!"
    End Sub

    Private Sub PatientDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Guna2CustomGradientPanel3.Visible = True

        LoadSpecializations()
        AppointmentDatePicker.MinDate = DateTime.Today
        AppointmentDatePicker.MaxDate = DateTime.Today.AddMonths(5)
        InvoicePanel.Visible = False
    End Sub


    Private Sub HomeDisplay()
        SelectPatientPanel.Visible = False
        SpecPanel.Visible = True
        BookAppPanel.Visible = True
        pd_DoctorPanel.Visible = False
        BookingPanel.Visible = False

        LoadSpecializations()
    End Sub

    Private Sub pd_BookAppointment_Click(sender As Object, e As EventArgs) Handles pd_BookAppointment.Click
        HomeDisplay()
        Guna2CustomGradientPanel3.Visible = False
    End Sub


    Private Sub pd_SpecBtn_Click(sender As Object, e As EventArgs) Handles pd_SpecBtn.Click
        If pd_SpecBox.SelectedItem Is Nothing OrElse pd_SpecBox.SelectedItem.ToString() = "Select" Then
            MessageBox.Show("Please select a valid doctor's specialization.")
            Return
        End If

        Dim selectedSpecialization As String = pd_SpecBox.SelectedItem.ToString()
        Dim result As DialogResult = MessageBox.Show($"You selected '{selectedSpecialization}' as the specialization. Would you like to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            SelectPatientPanel.Visible = False
            SpecPanel.Visible = False
            pd_DoctorPanel.Visible = True
            BookAppPanel.Visible = True

            Dim doctorNames As List(Of String) = Database.GetDoctorNames(selectedSpecialization)
            pd_DocBox.Items.Clear()
            pd_DocBox.Items.Add("Select")

            For Each doctorName As String In doctorNames
                pd_DocBox.Items.Add(doctorName)
            Next

            pd_DocBox.SelectedIndex = 0
        ElseIf result = DialogResult.No Then
            SpecPanel.Visible = True
            BookAppPanel.Visible = True
        End If

        LoadConsFee()
    End Sub

    Private Sub pd_DocBtn_Click(sender As Object, e As EventArgs) Handles pd_DocBtn.Click
        If pd_DocBox.SelectedItem Is Nothing OrElse pd_DocBox.SelectedItem.ToString() = "Select" Then
            Return
        End If

        Dim selectedDoctor As String = pd_DocBox.SelectedItem.ToString()
        Dim selectedSpecialization As String = pd_SpecBox.SelectedItem.ToString()
        Dim timeSlots As List(Of String) = Database.GetDoctorAvailableTimes(selectedDoctor, selectedSpecialization)

        SelectPatientPanel.Visible = False
        SpecPanel.Visible = False
        pd_DoctorPanel.Visible = False
        BookAppPanel.Visible = True
        BookingPanel.Visible = True

        If timeSlots.Count > 0 Then
            TimeCombobox.Items.Clear()
            TimeCombobox.Items.Add("Select a Time Slot")

            For Each timeSlot As String In timeSlots
                TimeCombobox.Items.Add(timeSlot)
            Next

            TimeCombobox.SelectedIndex = 0
        Else
            MessageBox.Show("No time slots found for this doctor and specialization.", "No Availability", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub pd_DocBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles pd_DocBox.SelectedIndexChanged
        If pd_DocBox.SelectedItem IsNot Nothing AndAlso pd_DocBox.SelectedItem.ToString() <> "Select" Then
            Dim selectedDoctor As String = pd_DocBox.SelectedItem.ToString()
            Dim availability As String = Database.GetDoctorAvailability(selectedDoctor)

            If Not String.IsNullOrEmpty(availability) Then
                Dim availableDays As List(Of DayOfWeek) = ParseDayAvailability(availability)
                ConfigureMonthCalendar(AppointmentDatePicker, availableDays)
            Else
                MessageBox.Show("No date availability data found for the selected doctor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            Try
                Dim consultationFee As Decimal? = Database.GetConsultationFee(selectedDoctor)
                If consultationFee.HasValue Then
                    ConsFeeLbl.Text = $"{consultationFee:C}"
                Else
                    ConsFeeLbl.Text = "Not Available"
                End If
            Catch ex As Exception
                MessageBox.Show($"An error occurred while loading the consultation fee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Function ParseDayAvailability(availability As String) As List(Of DayOfWeek)
        Dim days As New List(Of DayOfWeek)()
        Dim dayNames As String() = availability.Split(New Char() {"-"c, ","c}, StringSplitOptions.RemoveEmptyEntries)

        For Each day As String In dayNames
            Dim dayOfWeek As DayOfWeek
            If [Enum].TryParse(day.Trim(), True, dayOfWeek) Then
                days.Add(dayOfWeek)
            End If
        Next

        Return days
    End Function

    Private Sub ConfigureMonthCalendar(calendar As MonthCalendar, availableDays As List(Of DayOfWeek))
        Dim today As DateTime = DateTime.Today
        Dim minDate As DateTime = today.AddDays(4)
        Dim maxDate As DateTime = today.AddMonths(5)

        calendar.MinDate = minDate
        calendar.MaxDate = maxDate
        calendar.MaxSelectionCount = 1

        AddHandler calendar.DateChanged, Sub(s, e)
                                             If e.Start < minDate OrElse e.Start > maxDate OrElse Not availableDays.Contains(e.Start.DayOfWeek) Then
                                                 MessageBox.Show("The selected date is unavailable. Please select a valid day.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                 calendar.SetDate(FindNearestValidDate(e.Start, availableDays, minDate, maxDate))
                                             End If
                                         End Sub
    End Sub

    Private Function FindNearestValidDate(selectedDate As DateTime, availableDays As List(Of DayOfWeek), minDate As DateTime, maxDate As DateTime) As DateTime
        Dim currentDate As DateTime = selectedDate.Date

        While currentDate <= maxDate
            If availableDays.Contains(currentDate.DayOfWeek) Then
                Return currentDate
            End If

            currentDate = currentDate.AddDays(1)
        End While

        Return minDate
    End Function

    Private Sub LoadSpecializations()
        Try
            Dim specializations As List(Of String) = Database.GetSpecialization()

            pd_SpecBox.Items.Clear()
            pd_SpecBox.Items.Add("Select")

            pd_SpecBox.Items.AddRange(specializations.ToArray())

            pd_SpecBox.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading specializations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadConsFee()
        Try
            If pd_DocBox.SelectedItem Is Nothing OrElse pd_DocBox.SelectedItem.ToString() = "Select" Then
                Return
            End If

            Dim selectedDoctor As String = pd_DocBox.SelectedItem.ToString()

            Dim consultationFee As Nullable(Of Decimal) = Database.GetConsultationFee(selectedDoctor)

            If consultationFee.HasValue Then
                ConsFeeLbl.Text = $"{consultationFee:C}"
            Else
                ConsFeeLbl.Text = "Not Available"
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading consultation fee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ConfirmBookBtn_Click(sender As Object, e As EventArgs) Handles ConfirmBookBtn.Click
        Dim result As DialogResult = MessageBox.Show(
            "Are you sure you want to confirm this booking?",
            "Confirm Booking",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        )

        If result = DialogResult.Yes Then
            Try
                Dim selectedPatient As String = $"{patient.LastName}, {patient.FirstName}"
                Dim selectedDoctor As String = If(pd_DocBox.SelectedItem?.ToString(), String.Empty)
                Dim selectedTimeSlot As String = If(TimeCombobox.SelectedItem?.ToString(), String.Empty)
                Dim appointmentDate As DateTime = AppointmentDatePicker.SelectionStart
                Dim specialization As String = If(pd_SpecBox.SelectedItem?.ToString(), String.Empty)

                If String.IsNullOrWhiteSpace(selectedPatient) OrElse
                   String.IsNullOrWhiteSpace(selectedDoctor) OrElse
                   String.IsNullOrWhiteSpace(selectedTimeSlot) OrElse
                   String.IsNullOrWhiteSpace(specialization) Then
                    MessageBox.Show("Please ensure all fields are filled out correctly.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                If selectedTimeSlot = "Select a Time Slot" Then
                    MessageBox.Show("Please select a valid time slot.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                If Database.IsPatientAppointmentPendingOrAccepted(selectedPatient) Then
                    MessageBox.Show("This patient already has a pending or accepted appointment. They cannot book another appointment until the current one is completed.", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                If Database.IsDoctorOccupied(selectedDoctor, appointmentDate) Then
                    MessageBox.Show("This doctor is already occupied for the selected date. Please choose another doctor or date.", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                Dim feeText As String = ConsFeeLbl.Text.Trim().Replace("$", "").Replace("€", "").Replace("£", "").Trim()
                feeText = New String(feeText.Where(Function(c) Char.IsDigit(c) OrElse c = "."c).ToArray())
                Dim consultationFee As Decimal = 0
                If Not Decimal.TryParse(feeText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, consultationFee) Then
                    MessageBox.Show("Invalid consultation fee format. Please check the fee and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim appointmentSaved As Boolean = Database.SaveAppointment(selectedPatient, specialization, selectedDoctor, selectedTimeSlot, appointmentDate, consultationFee)

                If appointmentSaved Then
                    MessageBox.Show("Appointment saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    HomeDisplay()
                Else
                    MessageBox.Show("Failed to save appointment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub ShowAppointmentList()
        Try
            Dim AppointmentData As DataTable = Database.AppointmentList()
            If AppointmentData.Rows.Count > 0 Then
                AppointmentDataGridViewList2.DataSource = AppointmentData
            Else
                MessageBox.Show("No Appointment History data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"Error loading Appointment History data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Staff_ExitButton_Click(sender As Object, e As EventArgs) Handles Staff_ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to close? Unsaved changes will be lost.", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub Staff_MinimizeButton_Click(sender As Object, e As EventArgs) Handles Staff_MinimizeButton.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub
    Private Sub pd_HomeButton_Click(sender As Object, e As EventArgs) Handles pd_HomeButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Log Out?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Dim patientLoginForm As New PatientLogin()
            patientLoginForm.Show()
            Me.Hide()
        End If
    End Sub
    Private Sub guna2Button1_Click(sender As Object, e As EventArgs) Handles guna2Button1.Click
        InvoicePanel.Visible = False
        SearchPanel.Visible = True
        ViewButton.Visible = True
        DeleteButton.Visible = True
        AppointmentLabel.Text = "My Appointments"
        SelectPatientPanel.Visible = False
        SpecPanel.Visible = False
        pd_DoctorPanel.Visible = False
        BookingPanel.Visible = False
        BookAppPanel.Visible = False
        ViewAppointmentPanel.Visible = True

        Dim patientName As String = $"{patient.LastName}, {patient.FirstName}"
        Dim viewcompletedappoointment As DataTable = Database.ViewPatientAppointments(patientName)

        If viewcompletedappoointment.Rows.Count <> 0 Then
            AppointmentDataGridViewList2.Visible = True
            AppointmentDataGridViewList2.DataSource = viewcompletedappoointment
            Return
        End If

        MessageBox.Show("You have no Appointments.", "Appoinment Msg")
        AppointmentDataGridViewList2.Visible = False
    End Sub

    Private Sub AppointmentDataGridViewList2_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles AppointmentDataGridViewList2.CellBeginEdit
        If e.ColumnIndex <> 0 Then
            e.Cancel = True
        End If
    End Sub

    Private Sub AppointmentDataGridViewList2_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles AppointmentDataGridViewList2.CellValueChanged
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
            Dim isChecked As Boolean = CBool(AppointmentDataGridViewList2.Rows(e.RowIndex).Cells(0).Value)

            If isChecked Then
                For Each row As DataGridViewRow In AppointmentDataGridViewList2.Rows
                    If row.Index <> e.RowIndex Then
                        Dim checkBoxCell As DataGridViewCheckBoxCell = TryCast(row.Cells(0), DataGridViewCheckBoxCell)
                        If checkBoxCell IsNot Nothing Then
                            checkBoxCell.Value = False
                        End If
                    End If
                Next
            End If
        End If

        Console.WriteLine("After CellValueChanged:")
        For Each row As DataGridViewRow In AppointmentDataGridViewList2.Rows
            Console.WriteLine($"Row {row.Index}: Visible={row.Visible}, Checked={(If(row.Cells(0).Value IsNot Nothing, row.Cells(0).Value.ToString(), "null"))}")
        Next
    End Sub

    Private Sub AppointmentDataGridViewList2_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles AppointmentDataGridViewList2.CurrentCellDirtyStateChanged
        If TypeOf AppointmentDataGridViewList2.CurrentCell Is DataGridViewCheckBoxCell Then
            AppointmentDataGridViewList2.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub ViewButton_Click(sender As Object, e As EventArgs) Handles ViewButton.Click
        If AppointmentDataGridViewList2.SelectedRows.Count > 0 Then
            Dim appointmentId As Integer = Convert.ToInt32(AppointmentDataGridViewList2.SelectedRows(0).Cells("ID").Value)

            Database.viewDocument(
            appointmentId,
            Sub(patientDetails)
                Dim viewpatientinfo As New ViewPatientInformation2()

                viewpatientinfo.SetDetails(
                    patientDetails("P_Firstname"),
                    patientDetails("P_Lastname"),
                    patientDetails("P_Bdate"),
                    patientDetails("P_Height"),
                    patientDetails("P_Weight"),
                    patientDetails("P_BMI"),
                    patientDetails("P_Blood_Type"),
                    patientDetails("P_Alergy"),
                    patientDetails("P_Medication"),
                    patientDetails("P_PrevSurgery"),
                    patientDetails("P_Precondition"),
                    patientDetails("P_Treatment"),
                    patientDetails("ah_DoctorFirstName"),
                    patientDetails("ah_DoctorLastName"),
                    patientDetails("ah_Time"),
                    patientDetails("ah_Date"),
                    patientDetails("ah_Consfee"),
                    patientDetails("d_diagnosis"),
                    patientDetails("d_additionalnotes"),
                    patientDetails("d_doctoroder"),
                    patientDetails("d_prescription")
                )

                viewpatientinfo.Show()
            End Sub,
            Sub(errorMessage)
                MessageBox.Show(errorMessage)
            End Sub
        )
        Else
            MessageBox.Show("Please select an appointment.")
        End If
    End Sub

    Private Sub SearchTransactionButton_Click(sender As Object, e As EventArgs) Handles SearchTransactionButton.Click
        Dim transactionId As String = TransactionIdTextBox.Text.Trim()
        Dim patientName As String = NameTextBox.Text.Trim()

        If Not String.IsNullOrEmpty(patientName) AndAlso patientName.Any(AddressOf Char.IsDigit) Then
            MessageBox.Show("Patient name cannot contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not String.IsNullOrEmpty(transactionId) AndAlso Not transactionId.All(AddressOf Char.IsDigit) Then
            MessageBox.Show("Transaction ID must contain only numeric values.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not String.IsNullOrEmpty(transactionId) OrElse Not String.IsNullOrEmpty(patientName) Then
            Try
                Dim dataSource As DataTable = TryCast(AppointmentDataGridViewList2.DataSource, DataTable)

                If dataSource IsNot Nothing Then
                    Dim filter As String = ""

                    If Not String.IsNullOrEmpty(transactionId) Then
                        filter = $"Convert([Transaction ID], 'System.String') LIKE '%{transactionId}%'"
                    End If

                    If Not String.IsNullOrEmpty(patientName) Then
                        If Not String.IsNullOrEmpty(filter) Then
                            filter += " OR "
                        End If

                        Dim nameParts As String() = patientName.Split(","c)

                        If nameParts.Length = 2 Then
                            Dim lastName As String = nameParts(0).Trim()
                            Dim firstName As String = nameParts(1).Trim()

                            filter += $"[Patient Name] LIKE '%{lastName}%' AND [Patient Name] LIKE '%{firstName}%'"
                        Else
                            filter += $"[Patient Name] LIKE '%{patientName}%'"
                        End If
                    End If

                    dataSource.DefaultView.RowFilter = filter
                End If
            Catch ex As Exception
                MessageBox.Show($"Error while filtering data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Please enter either a transaction ID or a patient name to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub


    Private Sub ResetTransactionFilterButton_Click(sender As Object, e As EventArgs) Handles ResetTransactionFilterButton.Click
        Try
            Dim dataSource As DataTable = TryCast(AppointmentDataGridViewList2.DataSource, DataTable)

            If dataSource IsNot Nothing Then
                dataSource.DefaultView.RowFilter = String.Empty
                TransactionIdTextBox.Clear()
                NameTextBox.Clear()
            End If
        Catch ex As Exception
            MessageBox.Show($"Error while resetting filter: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub MyProfileTabBtn_Click(sender As Object, e As EventArgs) Handles MyProfileTabBtn.Click
        Dim form As New PatientRegisterForm(ModalMode.Edit, patient.AccountID, PanelMode.Patient)
        form.ShowDialog()
    End Sub


    Private Sub DeleteButton_Click(sender As Object, e As EventArgs) Handles DeleteButton.Click
        If AppointmentDataGridViewList2.SelectedRows.Count > 0 Then
            Dim appointmentId As Integer = Convert.ToInt32(AppointmentDataGridViewList2.SelectedRows(0).Cells("Transaction ID").Value)

            Dim result As DialogResult = MessageBox.Show(
            "Are you sure you want to delete this appointment?",
            "Delete Appointment",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        )

            If result = DialogResult.Yes Then
                Dim isDeleted As Boolean = Database.DeleteAppointmentByPatient(appointmentId)

                If isDeleted Then
                    MessageBox.Show("Appointment successfully deleted.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    RefreshAppointmentList()
                Else
                    MessageBox.Show("You can delete Pending and Declined appointment only. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Else
            MessageBox.Show("Please select an appointment to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


    Private Sub RefreshAppointmentList()
        Dim patientName As String = $"{patient.LastName}, {patient.FirstName}"
        AppointmentDataGridViewList2.DataSource = Database.ViewPatientAppointments(patientName)
    End Sub

    Private Sub LoadInvoiceData()
        Dim fullName As String = $"{patient.LastName}, {patient.FirstName}"
        Dim data As DataTable = Database.GetInvoiceList(fullName)
        InvoiceDataGridView.DataSource = data
    End Sub


    Private Sub guna2Button5_Click(sender As Object, e As EventArgs) Handles guna2Button5.Click
        SearchPanel.Visible = True
        ViewButton.Visible = False
        DeleteButton.Visible = False
        SpecPanel.Visible = False
        BookAppPanel.Visible = False

        InvoicePanel.Visible = True
        InvoiceBtn.Visible = True
        InvoiceDataGridView.Visible = True
        InvoiceHeadTitle.Visible = True

        ViewAppointmentPanel.Visible = False
        LoadInvoiceData()
    End Sub
    Private Sub InvoiceDataGridView_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles InvoiceDataGridView.CellValueChanged
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
            Dim isChecked As Boolean = Convert.ToBoolean(InvoiceDataGridView.Rows(e.RowIndex).Cells(0).Value)

            If isChecked Then
                For Each row As DataGridViewRow In InvoiceDataGridView.Rows
                    If row.Index <> e.RowIndex Then
                        Dim checkBoxCell As DataGridViewCheckBoxCell = TryCast(row.Cells(0), DataGridViewCheckBoxCell)
                        If checkBoxCell IsNot Nothing Then
                            checkBoxCell.Value = False
                        End If
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub InvoiceBtn_Click(sender As Object, e As EventArgs) Handles InvoiceBtn.Click
        If InvoiceDataGridView.SelectedRows.Count > 0 Then
            Dim appointmentId As Integer = Convert.ToInt32(InvoiceDataGridView.SelectedRows(0).Cells("ID").Value)

            Dim appointment As Appointment = Database.GetAppointmentById(appointmentId)

            Dim bill As New PatientBillingInvoice(appointment)
            bill.ShowDialog()
        Else
            MessageBox.Show("Please select an invoice record first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub


End Class
