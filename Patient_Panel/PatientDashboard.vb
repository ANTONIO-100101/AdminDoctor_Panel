﻿Imports System.Globalization
Imports AdminDoctor_Panel
Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models

Public Class PatientDashboard
    Inherits Form

    Private patient As PatientModel
    Private Property CurrentAccessiblePanel As Panel = Nothing
    Private suppressAvailabilityCheck As Boolean = False


    Public Sub New(patient As PatientModel)
        InitializeComponent()
        Me.patient = patient
        NameLabel.Text = $"{patient.FirstName}!"
    End Sub

    Private Sub SetPanelAccessibility(panelToEnable As Panel)

        SpecialPanel.Enabled = False
        DoccPanel.Enabled = False
        TimeePanel.Enabled = False
        ConfirmBookBtn.Enabled = False

        ' Manage ConfBack button state based on current panel
        If panelToEnable Is SpecialPanel Then
            ConfBack.Enabled = False ' Disable when at first panel
        Else
            ConfBack.Enabled = True ' Enable when in other panels
        End If

        ' Enable the specified panel
        panelToEnable.Enabled = True
        CurrentAccessiblePanel = panelToEnable

        ' Update panel colors
        UpdatePanelColors(panelToEnable)

        ' Additional logic for when returning to previous panels
        If panelToEnable Is SpecialPanel Then
            ' When going back to SpecialPanel, reset other panels if needed
            pd_DocBox.SelectedIndex = 0
            TimeCombobox.Items.Clear()
            TimeCombobox.Enabled = True
        ElseIf panelToEnable Is DoccPanel Then
            ' When going back to DoccPanel, reset time selection
            TimeCombobox.Items.Clear()
            TimeCombobox.Enabled = True
        End If
    End Sub

    Private Sub PatientDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Guna2CustomGradientPanel3.Visible = True

        ' Initialize panel states
        SpecialPanel.Visible = False
        DoccPanel.Visible = False
        TimeePanel.Visible = False
        ConfirmBookBtn.Visible = False

        LoadSpecializations()
        ' Set date range - minimum 3 days from today, maximum 5 months from today
        AppointmentDatePicker.MinDate = DateTime.Today.AddDays(3)
        AppointmentDatePicker.MaxDate = DateTime.Today.AddMonths(5)
        InvoicePanel.Visible = False

        ' Initialize panel colors
        UpdatePanelColors(Nothing)
    End Sub

    Private Sub HomeDisplay()
        SpecPanel.Visible = True
        BookAppPanel.Visible = True
        LoadSpecializations()
    End Sub

    Private Sub pd_BookAppointment_Click(sender As Object, e As EventArgs) Handles pd_BookAppointment.Click
        suppressAvailabilityCheck = True
        HomeDisplay()
        Guna2CustomGradientPanel3.Visible = False

        ' Clear any previous selections
        ClearAllSelections()

        ' Set date range to start from 3 days after today
        AppointmentDatePicker.MinDate = DateTime.Today.AddDays(3)
        AppointmentDatePicker.MaxDate = DateTime.Today.AddMonths(5)

        ' Show all panels but only enable SpecialPanel initially
        SpecPanel.Visible = True
        SpecialPanel.Visible = True
        DoccPanel.Visible = True
        TimeePanel.Visible = True
        ConfirmBookBtn.Visible = True

        SetPanelAccessibility(SpecialPanel)
        suppressAvailabilityCheck = False
    End Sub


    Private Sub pd_SpecBtn_Click(sender As Object, e As EventArgs)
        If pd_SpecBox.SelectedItem Is Nothing OrElse pd_SpecBox.SelectedItem.ToString = "Select" Then
            MessageBox.Show("Please select a valid doctor's specialization.")
            Return
        End If

        Dim selectedSpecialization = pd_SpecBox.SelectedItem.ToString
        Dim result = MessageBox.Show($"You selected '{selectedSpecialization}' as the specialization. Would you like to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            ' Move to next panel (DoccPanel)
            SetPanelAccessibility(DoccPanel)

            ' Load doctors for the selected specialization
            Dim doctorNames = Database.GetDoctorNames(selectedSpecialization)
            pd_DocBox.Items.Clear()
            pd_DocBox.Items.Add("Select")

            For Each doctorName In doctorNames
                pd_DocBox.Items.Add(doctorName)
            Next

            pd_DocBox.SelectedIndex = 0
        End If

        LoadConsFee()
    End Sub

    Private Sub pd_DocBtn_Click(sender As Object, e As EventArgs)
        If pd_DocBox.SelectedItem Is Nothing OrElse pd_DocBox.SelectedItem.ToString = "Select" Then
            MessageBox.Show("Please select a valid doctor.")
            Return
        End If

        Dim selectedDoctor = pd_DocBox.SelectedItem.ToString
        Dim selectedSpecialization = pd_SpecBox.SelectedItem.ToString

        ' Add confirmation dialog
        Dim result = MessageBox.Show($"You selected Dr. {selectedDoctor} ({selectedSpecialization}). Would you like to proceed?",
                               "Confirmation",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Dim timeSlots = Database.GetDoctorAvailableTimes(selectedDoctor, selectedSpecialization)

            ' Move to next panel (TimeePanel)
            SetPanelAccessibility(TimeePanel)

            ' Load available time slots
            TimeCombobox.Items.Clear()
            TimeCombobox.Items.Add("Select a Time Slot")

            If timeSlots.Count > 0 Then
                For Each timeSlot In timeSlots
                    TimeCombobox.Items.Add(timeSlot)
                Next
                TimeCombobox.SelectedIndex = 0
            Else
                MessageBox.Show("No time slots found for this doctor and specialization.", "No Availability", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ' Make the button unclickable after proceeding
            ' pd_DocBtn.Enabled = False
        End If
    End Sub

    Private Sub DateTimeBtn_Click(sender As Object, e As EventArgs)
        If TimeCombobox.SelectedItem Is Nothing OrElse TimeCombobox.SelectedItem.ToString = "Select a Time Slot" Then
            MessageBox.Show("Please select a valid time slot.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Show confirmation dialog
        Dim selectedTime = TimeCombobox.SelectedItem.ToString
        Dim result = MessageBox.Show($"You selected the time slot: {selectedTime}. Would you like to proceed?",
                          "Confirm Time Selection",
                          MessageBoxButtons.YesNo,
                          MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            ' Enable the confirmation button
            ConfirmBookBtn.Enabled = True

            ' Disable components to lock in selection
            TimeCombobox.Enabled = False
            AppointmentDatePicker.Enabled = False ' 🔒 This disables the date picker
            ' Optionally disable this button too if needed
            ' DateTimeBtn1.Enabled = False 
        End If
    End Sub

    Private Sub pd_DocBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles pd_DocBox.SelectedIndexChanged
        If pd_DocBox.SelectedItem IsNot Nothing AndAlso pd_DocBox.SelectedItem.ToString <> "Select" Then
            Dim selectedDoctor = pd_DocBox.SelectedItem.ToString
            Dim availability = Database.GetDoctorAvailability(selectedDoctor)

            If Not String.IsNullOrEmpty(availability) Then
                Dim availableDays = ParseDayAvailability(availability)
                ConfigureMonthCalendar(AppointmentDatePicker, availableDays)
            ElseIf Not suppressAvailabilityCheck Then
                MessageBox.Show("No date availability data found for the selected doctor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If


            Try
                Dim consultationFee = Database.GetConsultationFee(selectedDoctor)
                If consultationFee.HasValue Then
                    ConsFeeLbl1.Text = $"{consultationFee:C}"
                Else
                    ConsFeeLbl1.Text = "Not Available"
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
        Dim maxDate As DateTime = today.AddMonths(5)

        calendar.MaxDate = maxDate
        calendar.MaxSelectionCount = 1

        AddHandler calendar.DateChanged, Sub(s, e)
                                             If e.Start > maxDate OrElse Not availableDays.Contains(e.Start.DayOfWeek) Then
                                                 MessageBox.Show("The selected date is unavailable. Please select a valid day.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                 calendar.SetDate(FindNearestValidDate(e.Start, availableDays, maxDate))
                                             End If
                                         End Sub
    End Sub

    Private Function FindNearestValidDate(selectedDate As DateTime, availableDays As List(Of DayOfWeek), maxDate As DateTime) As DateTime
        Dim currentDate As DateTime = selectedDate.Date

        While currentDate <= maxDate
            If availableDays.Contains(currentDate.DayOfWeek) Then
                Return currentDate
            End If

            currentDate = currentDate.AddDays(1)
        End While

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
                ConsFeeLbl1.Text = $"{consultationFee:C}"
            Else
                ConsFeeLbl1.Text = "Not Available"
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading consultation fee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ConfirmBookBtn_Click(sender As Object, e As EventArgs) Handles ConfirmBookBtn.Click
        Dim result = MessageBox.Show(
        "Are you sure you want to confirm this booking?",
        "Confirm Booking",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    )

        If result = DialogResult.Yes Then
            Try
                Dim selectedPatient = $"{patient.LastName}, {patient.FirstName}"
                Dim selectedDoctor = If(pd_DocBox.SelectedItem?.ToString, String.Empty)
                Dim selectedTimeSlot = If(TimeCombobox.SelectedItem?.ToString, String.Empty)
                Dim appointmentDate = AppointmentDatePicker.SelectionStart
                Dim specialization = If(pd_SpecBox.SelectedItem?.ToString, String.Empty)

                ' Check if patient already has pending/accepted appointment
                If Database.IsPatientAppointmentPendingOrAccepted(selectedPatient) Then
                    MessageBox.Show("This patient already has a pending or accepted appointment. They cannot book another appointment until the current one is completed.",
                    "Booking Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)
                    ConfBack.Enabled = False
                    ResetFormToInitialState()
                    Return
                End If

                ' Check if doctor is already booked for this exact time slot
                If Database.IsDoctorTimeSlotOccupied(selectedDoctor, appointmentDate, selectedTimeSlot) Then
                    MessageBox.Show("This doctor is already booked for the selected date and time slot. Please choose another time or date.",
                    "Booking Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)
                    ConfBack.Enabled = False
                    pd_SpecBox.Enabled = True
                    pd_SpecBtn1.Enabled = True
                    ConfirmBookBtn.Enabled = False
                    Return
                End If

                ' Rest of your booking logic...
                Dim feeText = ConsFeeLbl1.Text.Trim.Replace("$", "").Replace("€", "").Replace("£", "").Trim
                feeText = New String(feeText.Where(Function(c) Char.IsDigit(c) OrElse c = "."c).ToArray)
                Dim consultationFee As Decimal = 0
                If Not Decimal.TryParse(feeText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, consultationFee) Then
                    MessageBox.Show("Invalid consultation fee format. Please check the fee and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim appointmentSaved = Database.SaveAppointment(selectedPatient, specialization, selectedDoctor, selectedTimeSlot, appointmentDate, consultationFee)

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

        If IsAppointmentFilled() Then
            Dim result = MessageBox.Show("You have unsaved appointment details. Are you sure you want to exit?",
                                     "Confirmation",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question)

            If result = DialogResult.No Then
                Return
            End If
        End If

        InvoicePanel.Visible = False
        SearchPanel.Visible = True
        ViewButton.Visible = True
        DeleteButton.Visible = True
        AppointmentLabel.Text = "My Appointments"
        SpecPanel.Visible = False
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
                    patientDetails("d_doctororder"),
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
        If IsAppointmentFilled() Then
            Dim result = MessageBox.Show("You have unsaved appointment details. Are you sure you want to exit?",
                                     "Confirmation",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question)

            If result = DialogResult.No Then
                Return
            End If
        End If

        ' Open the profile if the user confirms
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
        If IsAppointmentFilled() Then
            Dim result = MessageBox.Show("You have unsaved appointment details. Are you sure you want to exit?",
                                     "Confirmation",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question)

            If result = DialogResult.No Then
                Return
            End If
        End If

        ' Proceed with showing the invoice panel if the user confirms
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

    Private Sub TimeCombobox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TimeCombobox.SelectedIndexChanged
        ' Optional: Add any time selection change logic here
    End Sub

    Private Sub pd_SpecBtn_Click_1(sender As Object, e As EventArgs)

    End Sub

    Private Sub pd_SpecBtn1_Click(sender As Object, e As EventArgs) Handles pd_SpecBtn1.Click
        If pd_SpecBox.SelectedItem Is Nothing OrElse pd_SpecBox.SelectedItem.ToString = "Select" Then
            MessageBox.Show("Please select a valid doctor's specialization.")
            Return
        End If

        Dim selectedSpecialization = pd_SpecBox.SelectedItem.ToString
        Dim result = MessageBox.Show($"You selected '{selectedSpecialization}' as the specialization. Would you like to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            ' Move to next panel (DoccPanel)
            SetPanelAccessibility(DoccPanel)

            ' Load doctors for the selected specialization
            Dim doctorNames = Database.GetDoctorNames(selectedSpecialization)
            pd_DocBox.Items.Clear()
            pd_DocBox.Items.Add("Select")

            For Each doctorName In doctorNames
                pd_DocBox.Items.Add(doctorName)
            Next

            pd_DocBox.SelectedIndex = 0
        End If
    End Sub

    Private Sub pd_DocBtn1_Click(sender As Object, e As EventArgs) Handles pd_DocBtn1.Click
        If pd_DocBox.SelectedItem Is Nothing OrElse pd_DocBox.SelectedItem.ToString = "Select" Then
            MessageBox.Show("Please select a valid doctor.")
            Return
        End If

        Dim selectedDoctor = pd_DocBox.SelectedItem.ToString
        Dim selectedSpecialization = pd_SpecBox.SelectedItem.ToString

        ' Add confirmation dialog
        Dim result = MessageBox.Show($"You selected Dr. {selectedDoctor} ({selectedSpecialization}). Would you like to proceed?",
                               "Confirmation",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Dim timeSlots = Database.GetDoctorAvailableTimes(selectedDoctor, selectedSpecialization)

            ' Move to next panel (TimeePanel)
            SetPanelAccessibility(TimeePanel)

            ' Load available time slots
            TimeCombobox.Items.Clear()
            TimeCombobox.Items.Add("Select a Time Slot")

            If timeSlots.Count > 0 Then
                For Each timeSlot In timeSlots
                    TimeCombobox.Items.Add(timeSlot)
                Next
                TimeCombobox.SelectedIndex = 0
            Else
                MessageBox.Show("No time slots found for this doctor and specialization.", "No Availability", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            AppointmentDatePicker.Enabled = True
            DateTimeBtn1.Enabled = True
            ' Make the button unclickable after proceeding
            'pd_DocBtn.Enabled = False
        End If
    End Sub


    Private Sub DateTimeBtn1_Click(sender As Object, e As EventArgs) Handles DateTimeBtn1.Click
        If TimeCombobox.SelectedItem Is Nothing OrElse TimeCombobox.SelectedItem.ToString() = "Select a Time Slot" Then
            MessageBox.Show("Please select a valid time slot.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedTime = TimeCombobox.SelectedItem.ToString()
        Dim result = MessageBox.Show($"You selected the time slot: {selectedTime}. Would you like to proceed?",
                                 "Confirm Time Selection",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            ConfirmBookBtn.Enabled = True

            DateTimeBtn1.Enabled = False


            TimeCombobox.Enabled = False
            AppointmentDatePicker.Enabled = False

            CurrentAccessiblePanel = Nothing
        End If
    End Sub


    Private Sub DocBack_Click(sender As Object, e As EventArgs) Handles DocBack.Click
        Dim result = MessageBox.Show(
        "Are you sure you want to go back? Any selected doctor will be cleared.",
        "Confirm Navigation",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    )

        If result = DialogResult.Yes Then
            SetPanelAccessibility(SpecialPanel)
            ConfBack.Enabled = False ' Disable when going back to first panel
        End If
    End Sub

    Private Sub TimeBack_Click(sender As Object, e As EventArgs) Handles TimeBack.Click
        Dim result = MessageBox.Show(
        "Are you sure you want to go back? Selected time slot will be lost.",
        "Confirm Navigation",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    )

        If result = DialogResult.Yes Then
            SetPanelAccessibility(DoccPanel)
            ConfBack.Enabled = True ' Keep enabled when going back to doctor panel
        End If
    End Sub

    Private Sub ConfBack_Click(sender As Object, e As EventArgs) Handles ConfBack.Click
        Dim result = MessageBox.Show(
            "Are you sure you want to go back? Your current selections might be lost.",
            "Confirm Navigation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        )

        If result = DialogResult.Yes Then
            ' Return to TimeePanel
            SetPanelAccessibility(TimeePanel)

            ' Re-enable the time selection controls
            TimeCombobox.Enabled = True
            ConfirmBookBtn.Enabled = False
            AppointmentDatePicker.Enabled = True
            DateTimeBtn1.Enabled = True
        End If
    End Sub

    Private Sub pd_SpecBox_SelectedIndexChanged(sender As Object, e As EventArgs)

    End Sub

    Private Function IsAppointmentFilled() As Boolean
        Return Not (String.IsNullOrEmpty(pd_SpecBox.SelectedItem?.ToString()) OrElse
                pd_SpecBox.SelectedItem.ToString = "Select" OrElse
                String.IsNullOrEmpty(pd_DocBox.SelectedItem?.ToString()) OrElse
                pd_DocBox.SelectedItem.ToString = "Select" OrElse
                String.IsNullOrEmpty(TimeCombobox.SelectedItem?.ToString()) OrElse
                TimeCombobox.SelectedItem.ToString = "Select a Time Slot")
    End Function

    Private Sub ResetFormToInitialState()
        pd_SpecBox.SelectedIndex = 0
        pd_DocBox.Items.Clear()
        pd_DocBox.Items.Add("Select")
        pd_DocBox.SelectedIndex = 0
        TimeCombobox.Items.Clear()
        TimeCombobox.Items.Add("Select a Time Slot")
        TimeCombobox.SelectedIndex = 0

        ' Reset the date picker with 3-day minimum
        Try
            Dim minDate As DateTime = DateTime.Today.AddDays(3)
            AppointmentDatePicker.MinDate = minDate
            AppointmentDatePicker.MaxDate = DateTime.Today.AddMonths(5)
            AppointmentDatePicker.SelectionStart = minDate
            AppointmentDatePicker.SelectionEnd = minDate
        Catch ex As Exception
            MessageBox.Show("Error resetting date picker: " & ex.Message)
        End Try

        ' Reset fee display
        ConsFeeLbl1.Text = "$0.00"

        ' Reset panel states
        SetPanelAccessibility(SpecialPanel)

        ' Enable all necessary buttons
        pd_SpecBtn1.Enabled = True
        pd_DocBtn1.Enabled = True
        DateTimeBtn1.Enabled = True

        ' Reset the accessible panel
        CurrentAccessiblePanel = SpecialPanel
        ConfBack.Enabled = True

    End Sub

    Private Sub ClearAllSelections()
        pd_SpecBox.SelectedIndex = 0
        pd_DocBox.Items.Clear()
        pd_DocBox.Items.Add("Select")
        pd_DocBox.SelectedIndex = 0
        TimeCombobox.Items.Clear()
        TimeCombobox.Items.Add("Select a Time Slot")
        TimeCombobox.SelectedIndex = 0

        ' Safely reset the date picker with 3-day minimum
        Try
            AppointmentDatePicker.MinDate = DateTime.Today.AddDays(3)
            AppointmentDatePicker.MaxDate = DateTime.Today.AddMonths(5)
        Catch ex As Exception
            MessageBox.Show("Error resetting date picker: " & ex.Message)
        End Try

        ConsFeeLbl1.Text = "$0.00"
    End Sub

    Private Sub UpdatePanelColors(activePanel As Panel)
        ' Reset all panels to default color (13, 41, 80)
        SpecialPanel.BackColor = Color.FromArgb(13, 41, 80)
        DoccPanel.BackColor = Color.FromArgb(13, 41, 80)
        TimeePanel.BackColor = Color.FromArgb(13, 41, 80)

        ' Reset button color (assuming ConfirmBookBtn is a Guna2Button)
        ConfirmBookBtn.FillColor = Color.FromArgb(13, 41, 80)

        ' Set the active panel to green (66, 136, 62)
        If activePanel IsNot Nothing Then
            activePanel.BackColor = Color.FromArgb(66, 136, 62)
        End If

        ' Special case for ConfirmBookBtn when it's active
        If CurrentAccessiblePanel Is Nothing AndAlso ConfirmBookBtn.Enabled Then
            ConfirmBookBtn.FillColor = Color.FromArgb(66, 136, 62)
        End If
    End Sub
End Class
