Imports System.Globalization
Imports AdminDoctor_Panel
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

End Class
