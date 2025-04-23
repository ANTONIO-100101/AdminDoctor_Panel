Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models

Public Class DoctorDashboard
    Inherits Form

    Private doctor As DoctorModel

    Public Sub New(doctor As DoctorModel)
        InitializeComponent()
        Me.doctor = doctor
        NameLabel.Text = $"Dr. {doctor.LastName}, {doctor.FirstName}"
    End Sub

    Private Sub DoctorDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Guna2CustomGradientPanel2.Visible = True
        ad_docpanel.Visible = False
    End Sub

    Private Sub LoadPendingApprovals(haveAnError As Boolean)
        DataGridViewList.DataSource = Nothing
        ad_docpanel.Visible = True
        Guna2CustomGradientPanel2.Visible = False

        ReconsiderButton.Visible = False
        CreateDiagnosisButton.Visible = False
        AcceptButton.Visible = True
        DeclineButton.Visible = True
        ViewButton.Visible = False
        CheckOutButton.Visible = False
        InvoiceButton.Visible = False

        Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"

        Try
            Dim pendingAppointments As DataTable = Database.PendingAppointmentList(doctorFullName)

            DataGridViewList.AutoGenerateColumns = True
            DataGridViewList.AllowUserToAddRows = False
            DataGridViewList.Visible = True

            If pendingAppointments IsNot Nothing AndAlso pendingAppointments.Rows.Count > 0 Then
                DataGridViewList.DataSource = pendingAppointments
            ElseIf haveAnError Then
                MessageBox.Show("No appointments found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ApprovalPendingButton_Click(sender As Object, e As EventArgs) Handles ApprovalPendingButton.Click
        LoadPendingApprovals(True)
    End Sub

    Public Function LoadAppointmentsList() As DataTable
        Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"
        Dim viewAppointment As DataTable = Database.ViewAppointments(doctorFullName)
        DataGridViewList.DataSource = viewAppointment

        Return viewAppointment
    End Function

    Private Sub AppointmentListButton_Click(sender As Object, e As EventArgs) Handles AppointmentListButton.Click
        DataGridViewList.DataSource = Nothing
        ad_docpanel.Visible = True
        Guna2CustomGradientPanel2.Visible = False

        ReconsiderButton.Visible = False
        AcceptButton.Visible = False
        DeclineButton.Visible = False
        CreateDiagnosisButton.Visible = True
        ViewButton.Visible = False
        CheckOutButton.Visible = False
        InvoiceButton.Visible = False

        Dim viewAppointment As DataTable = LoadAppointmentsList()

        Try
            If viewAppointment IsNot Nothing AndAlso viewAppointment.Rows.Count > 0 Then
                DataGridViewList.AutoGenerateColumns = True
                DataGridViewList.AllowUserToAddRows = False
                DataGridViewList.Visible = True
            Else
                MessageBox.Show("No appointments found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridViewList_CurrentCellChanged(sender As Object, e As EventArgs) Handles DataGridViewList.CurrentCellChanged
        ' Add functionality as needed
    End Sub

    Private Sub DataGridViewList_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DataGridViewList.CurrentCellDirtyStateChanged
        If TypeOf DataGridViewList.CurrentCell Is DataGridViewCheckBoxCell Then
            ' Commit the edit when a checkbox is clicked
            DataGridViewList.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub DataGridViewList_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles DataGridViewList.CellBeginEdit
        If e.ColumnIndex <> 0 Then ' Replace with your checkbox column index if different
            e.Cancel = True ' Prevent editing for other columns
        End If
    End Sub

    Private Sub DataGridViewList_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewList.CellValueChanged
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then ' Assuming the checkbox column is the first column
            ' Check if any checkbox is checked
            Dim isAnyChecked As Boolean = False
            For Each row As DataGridViewRow In DataGridViewList.Rows
                If TypeOf row.Cells(0) Is DataGridViewCheckBoxCell AndAlso row.Cells(0).Value IsNot Nothing AndAlso CType(row.Cells(0).Value, Boolean) Then
                    isAnyChecked = True
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub LoadCompletedAppointments(Optional includeAnErrorValidation As Boolean = True)
        ad_docpanel.Visible = True
        Guna2CustomGradientPanel2.Visible = False

        Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"
        Dim completedAppointments As DataTable = Database.ViewCompletedAppointments(doctorFullName)
        DataGridViewList.DataSource = completedAppointments

        Try
            If completedAppointments IsNot Nothing AndAlso completedAppointments.Rows.Count > 0 Then
                DataGridViewList.AutoGenerateColumns = True
                DataGridViewList.AllowUserToAddRows = False
                DataGridViewList.Visible = True
            ElseIf includeAnErrorValidation Then
                MessageBox.Show("No completed appointments found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading completed appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadPendingAppointments()
        ad_docpanel.Visible = True
        Guna2CustomGradientPanel2.Visible = False

        Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"
        Dim pendingAppointments As DataTable = Database.PendingAppointmentList(doctorFullName)
        DataGridViewList.DataSource = pendingAppointments

        Try
            If pendingAppointments IsNot Nothing AndAlso pendingAppointments.Rows.Count > 0 Then
                DataGridViewList.AutoGenerateColumns = True
                DataGridViewList.AllowUserToAddRows = False
                DataGridViewList.Visible = True
            Else
                MessageBox.Show("No pending appointments found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading pending appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadRejectedAppointments()
        ad_docpanel.Visible = True
        Guna2CustomGradientPanel2.Visible = False

        Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"
        Dim rejectedAppointments As DataTable = Database.DeclinedAppointments(doctorFullName)
        DataGridViewList.DataSource = rejectedAppointments

        Try
            If rejectedAppointments IsNot Nothing AndAlso rejectedAppointments.Rows.Count > 0 Then
                DataGridViewList.AutoGenerateColumns = True
                DataGridViewList.AllowUserToAddRows = False
                DataGridViewList.Visible = True
            Else
                MessageBox.Show("No rejected appointments found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading rejected appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub pd_HomeButton_Click(sender As Object, e As EventArgs) Handles pd_HomeButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Log Out?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Dim DoctorLoginForm As New DoctorLogin()
            DoctorLoginForm.Show()
            Me.Hide()
        End If
    End Sub

    Private Sub ReconsiderButton_Click(sender As Object, e As EventArgs) Handles ReconsiderButton.Click
        Try
            ' Reconsider the selected appointments
            For Each row As DataGridViewRow In DataGridViewList.Rows
                If TypeOf row.Cells("checkboxcolumn") Is DataGridViewCheckBoxCell Then
                    Dim checkBoxCell As DataGridViewCheckBoxCell = DirectCast(row.Cells("checkboxcolumn"), DataGridViewCheckBoxCell)
                    If checkBoxCell.Value IsNot Nothing AndAlso CBool(checkBoxCell.Value) Then
                        Dim appointmentId As Integer = Convert.ToInt32(row.Cells("id").Value)
                        Database.ReconsiderAppointment(appointmentId) ' Update status to 'Pending' for declined appointments
                    End If
                End If
            Next

            ' Reload the data after reconsidering appointments
            Dim doctorName As String = NameLabel.Text.Replace("!", "").Trim()
            Dim pendingAppointments As DataTable = Database.PendingAppointmentList(doctorName) ' Ensure this retrieves updated data
            DataGridViewList.DataSource = pendingAppointments

            MessageBox.Show("Selected appointments have been reconsidered successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"An error occurred while reconsidering the appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    Private Sub CreateDiagnosisButton_Click(sender As Object, e As EventArgs) Handles CreateDiagnosisButton.Click
        Dim selectedRows = DataGridViewList.Rows.Cast(Of DataGridViewRow)().
                       Where(Function(r)
                                 Dim cellValue = r.Cells("checkboxcolumn").Value
                                 Return cellValue IsNot Nothing AndAlso cellValue.ToString().ToLower() = "true"
                             End Function).
                       ToList()

        If selectedRows.Count = 1 Then
            Dim appointmentId As Integer = Convert.ToInt32(selectedRows(0).Cells("id").Value)

            Database.CreateDiagnosis(
            appointmentId,
            Sub(patientDetails)
                Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"

                Dim doctorMedicalRecord As New DoctorMedicalRecord()
                doctorMedicalRecord.SetDoctorName(doctorFullName)

                doctorMedicalRecord.SetPatientDetails(
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
                    patientDetails("P_Treatment")
                )

                AddHandler doctorMedicalRecord.LoadAppointmentsList, AddressOf LoadAppointmentsList

                doctorMedicalRecord.Show()
            End Sub,
            Sub(errorMessage)
                MessageBox.Show(errorMessage)
            End Sub
        )
        Else
            MessageBox.Show("Please select only one appointment using the checkbox.")
        End If
    End Sub

    Private Sub AcceptButton_Click(sender As Object, e As EventArgs) Handles AcceptButton.Click
        Try
            For Each row As DataGridViewRow In DataGridViewList.Rows
                If TypeOf row.Cells("checkboxcolumn") Is DataGridViewCheckBoxCell Then
                    Dim checkBoxCell As DataGridViewCheckBoxCell = CType(row.Cells("checkboxcolumn"), DataGridViewCheckBoxCell)
                    If checkBoxCell.Value IsNot Nothing AndAlso CBool(checkBoxCell.Value) Then
                        Dim appointmentId As Integer = Convert.ToInt32(row.Cells("id").Value)
                        Database.AcceptAppointment(appointmentId)
                    End If
                End If
            Next

            Dim doctorName As String = NameLabel.Text.Replace("!", "").Trim()
            Dim pendingAppointments As DataTable = Database.PendingAppointmentList(doctorName)
            DataGridViewList.DataSource = pendingAppointments

            MessageBox.Show("Selected appointments have been accepted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"An error occurred while accepting the appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DeclineButton_Click(sender As Object, e As EventArgs) Handles DeclineButton.Click
        Try
            For Each row As DataGridViewRow In DataGridViewList.Rows
                If TypeOf row.Cells("checkboxcolumn") Is DataGridViewCheckBoxCell Then
                    Dim checkBoxCell As DataGridViewCheckBoxCell = CType(row.Cells("checkboxcolumn"), DataGridViewCheckBoxCell)
                    If checkBoxCell.Value IsNot Nothing AndAlso CBool(checkBoxCell.Value) Then
                        Dim appointmentId As Integer = Convert.ToInt32(row.Cells("id").Value)
                        Database.DeclineAppointment(appointmentId)
                    End If
                End If
            Next

            Dim doctorName As String = NameLabel.Text.Replace("!", "").Trim()
            Dim pendingAppointments As DataTable = Database.PendingAppointmentList(doctorName)
            DataGridViewList.DataSource = pendingAppointments

            MessageBox.Show("Selected appointments have been declined.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"An error occurred while declining the appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub RejectedRequestsButton_Click(sender As Object, e As EventArgs) Handles RejectedRequestsButton.Click
        DataGridViewList.DataSource = Nothing

        ReconsiderButton.Visible = True
        AcceptButton.Visible = False
        DeclineButton.Visible = False
        CreateDiagnosisButton.Visible = False
        ViewButton.Visible = False

        Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"
        Dim declinedappointment As DataTable = Database.DeclinedAppointments(doctorFullName)
        DataGridViewList.DataSource = declinedappointment

        Try
            If declinedappointment IsNot Nothing AndAlso declinedappointment.Rows.Count > 0 Then
                DataGridViewList.AutoGenerateColumns = True
                DataGridViewList.AllowUserToAddRows = False
                DataGridViewList.Visible = True
            Else
                MessageBox.Show("No appointments found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CompletedAppointmentsButton_Click(sender As Object, e As EventArgs) Handles CompletedAppointmentsButton.Click
        Guna2CustomGradientPanel2.Visible = False

        DataGridViewList.DataSource = Nothing
        ReconsiderButton.Visible = False
        AcceptButton.Visible = False
        DeclineButton.Visible = False
        CreateDiagnosisButton.Visible = False
        ViewButton.Visible = True
        CheckOutButton.Visible = True
        InvoiceButton.Visible = True

        Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"
        Dim viewcompletedappoointment As DataTable = Database.ViewCompletedAppointments(doctorFullName)
        DataGridViewList.DataSource = viewcompletedappoointment

        Try
            If viewcompletedappoointment IsNot Nothing AndAlso viewcompletedappoointment.Rows.Count > 0 Then
                DataGridViewList.AutoGenerateColumns = True
                DataGridViewList.AllowUserToAddRows = False
                DataGridViewList.Visible = True
            Else
                MessageBox.Show("No appointments found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ViewButton_Click(sender As Object, e As EventArgs) Handles ViewButton.Click
        Guna2CustomGradientPanel2.Visible = False

        If DataGridViewList.SelectedRows.Count > 0 Then
            Dim appointmentId As Integer = Convert.ToInt32(DataGridViewList.SelectedRows(0).Cells("id").Value)

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

    Private Sub CheckOutButton_Click(sender As Object, e As EventArgs) Handles CheckOutButton.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to check out this appointment?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Try
                Dim db As New Database() ' Create an instance of the Database class

                For Each row As DataGridViewRow In DataGridViewList.Rows
                    If TypeOf row.Cells("checkboxcolumn") Is DataGridViewCheckBoxCell Then
                        Dim checkBoxCell As DataGridViewCheckBoxCell = CType(row.Cells("checkboxcolumn"), DataGridViewCheckBoxCell)
                        If checkBoxCell.Value IsNot Nothing AndAlso CBool(checkBoxCell.Value) Then
                            Dim appointmentId As Integer = Convert.ToInt32(row.Cells("id").Value)
                            db.CheckOutAppointment(appointmentId) ' Use the instance of Database to call the method
                        End If
                    End If
                Next

                Dim doctorName As String = NameLabel.Text.Replace("!", "").Trim()

                Dim specialization As String = db.GetDoctorSpecialization(doctorName) ' Also using the instance here

                Dim invoiceForm As New DoctorBillingInvoice()

                Dim checkoutAppointments As DataTable = db.CheckOutAppointmentList(doctorName) ' Same here
                Dim currentDate As String = DateTime.Now.ToString("yyyy-MM-dd")
                invoiceForm.SetDoctorDetails(doctorName, specialization, currentDate, checkoutAppointments)
                invoiceForm.Show()

                'Reload the whole DataGridView
                LoadCompletedAppointments(False)
            Catch ex As Exception
                MessageBox.Show($"An error occurred while checking out the appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private _doctorBillingInvoice As DoctorBillingInvoice

    Private Sub InvoiceButton_Click(sender As Object, e As EventArgs) Handles InvoiceButton.Click
        If _doctorBillingInvoice Is Nothing OrElse _doctorBillingInvoice.IsDisposed Then
            Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"

            Dim specialization As String = Database.GetDoctorSpecialization(doctorFullName)

            Dim checkoutAppointments As DataTable = Database.CheckOutAppointmentList(doctorFullName)

            _doctorBillingInvoice = New DoctorBillingInvoice()

            AddHandler _doctorBillingInvoice.FormClosed, Sub()
                                                             _doctorBillingInvoice = Nothing
                                                         End Sub

            Dim currentDate As String = DateTime.Now.ToString("yyyy-MM-dd")

            _doctorBillingInvoice.SetDoctorDetails(doctorFullName, specialization, currentDate, checkoutAppointments)

            _doctorBillingInvoice.Show()
        Else
            _doctorBillingInvoice.Show()
            _doctorBillingInvoice.BringToFront()
        End If
    End Sub

    Private Sub SfCalendar1_SelectionChanged(sender As Object, e As EventArgs) Handles SfCalendar1.SelectionChanged
        If SfCalendar1.SelectedDate IsNot Nothing Then
            Dim selectedDate As Date = SfCalendar1.SelectedDate.Value.Date
            LoadAppointmentsByDate(selectedDate)
        End If
    End Sub

    Private Sub LoadAppointmentsByDate(selectedDate As Date)
        Dim doctorFullName As String = $"Dr. {doctor.LastName}, {doctor.FirstName}"

        ' Show loading indicator if you have one
        ad_docpanel.Visible = True
        Guna2CustomGradientPanel2.Visible = False

        ' Set button visibility
        ReconsiderButton.Visible = False
        AcceptButton.Visible = False
        DeclineButton.Visible = False
        CreateDiagnosisButton.Visible = True
        ViewButton.Visible = False
        CheckOutButton.Visible = False
        InvoiceButton.Visible = False

        Try
            ' Get appointments for the selected date
            Dim appointments As DataTable = Database.ViewAppointmentsByDate(doctorFullName, selectedDate)

            If appointments IsNot Nothing AndAlso appointments.Rows.Count > 0 Then
                DataGridViewList.DataSource = appointments
                DataGridViewList.AutoGenerateColumns = True
                DataGridViewList.AllowUserToAddRows = False
                DataGridViewList.Visible = True
            Else
                DataGridViewList.DataSource = Nothing
                MessageBox.Show($"No appointments found for {selectedDate.ToString("dd/MM/yyyy")}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while loading appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class