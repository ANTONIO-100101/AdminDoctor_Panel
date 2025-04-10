Imports AdminDoctor_Panel.Infocare_Project_1
Imports Guna.UI2.WinForms
Imports Infocare_Project
Imports Infocare_Project_1.Object_Models
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Diagnostics
Imports System.Drawing
Imports System.Linq
Imports System.Text

Public Class AdminDashboard2

    Public Sub New()
        InitializeComponent()

        ' Ensure the panel is visible when the form loads
        Guna2CustomGradientPanel2.Visible = True

        ad_staffpanel.Visible = False
        ad_docpanel.Visible = False
        ad_patientpanel.Visible = False
        ad_AppointmentPanel.Visible = False
    End Sub

    Private Sub AdminDashboard2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Show the panel when the form loads
        Guna2CustomGradientPanel2.Visible = True
    End Sub

    Private Sub ShowPatientTab()
        ' Hide the panel when switching tabs
        Guna2CustomGradientPanel2.Visible = False

        SearchPanel4.Visible = True
        SearchPanel1.Visible = False
        SearchPanel3.Visible = False
        ad_patientpanel.Visible = True
        ad_docpanel.Visible = False
        ad_staffpanel.Visible = False
        ad_AppointmentPanel.Visible = False

        PatientDataGridViewList2.Visible = True
        StaffDataGridViewList2.Visible = False
        DoctorDataGridViewList2.Visible = False
        AppointmentDataGridViewList2.Visible = False

        ShowPatientList()
    End Sub

    Private Sub ad_patientBtn_Click(sender As Object, e As EventArgs) Handles ad_patientBtn.Click
        ShowPatientTab()
    End Sub

    Private Sub ad_appointment_Click(sender As Object, e As EventArgs) Handles ad_appointment.Click
        ' Hide the panel when switching tabs
        Guna2CustomGradientPanel2.Visible = False

        SearchPanel3.Visible = True
        SearchPanel4.Visible = False
        SearchPanel1.Visible = False

        ad_AppointmentPanel.Visible = True
        ad_patientpanel.Visible = False
        ad_docpanel.Visible = False
        ad_staffpanel.Visible = False

        AppointmentDataGridViewList2.Visible = True
        PatientDataGridViewList2.Visible = False
        StaffDataGridViewList2.Visible = False
        DoctorDataGridViewList2.Visible = False

        ShowAppointmentList()
    End Sub

    Private Sub ad_doctor_Click(sender As Object, e As EventArgs) Handles ad_doctor.Click
        ' Hide the panel when switching tabs
        Guna2CustomGradientPanel2.Visible = False

        SearchPanel4.Visible = False
        SearchPanel3.Visible = False
        SearchPanel1.Visible = True

        ad_docpanel.Visible = True
        ad_staffpanel.Visible = False
        ad_patientpanel.Visible = False
        ad_AppointmentPanel.Visible = False

        DoctorDataGridViewList2.Visible = True
        StaffDataGridViewList2.Visible = False
        PatientDataGridViewList2.Visible = False
        AppointmentDataGridViewList2.Visible = False

        ShowDoctorList()
    End Sub
    Private Sub AddDoctor_Click(sender As Object, e As EventArgs) Handles AddDoctor.Click
        Dim adminAddDoctor As New AdminAddDoctor()

        AddHandler adminAddDoctor.ShowDoctorList, AddressOf ShowDoctorList

        adminAddDoctor.Show()
        Debug.WriteLine("AddDoctor button clicked")
    End Sub
    Private Sub ShowDoctorList()
        Try
            Dim DoctorData As DataTable = Database.DoctorList()
            If DoctorData IsNot Nothing AndAlso DoctorData.Rows.Count > 0 Then
                DoctorDataGridViewList2.DataSource = DoctorData
            Else
                MessageBox.Show("No Doctor data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"Error loading doctor data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ShowPatientList()
        Try
            Dim PatientData As DataTable = Database.PatientList()
            If PatientData.Rows.Count > 0 Then
                PatientDataGridViewList2.DataSource = PatientData
            Else
                MessageBox.Show("No patient data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"Error loading doctor data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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

    Private Sub ExitButton_Click(sender As Object, e As EventArgs)
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to close?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub MinimizeButton_Click(sender As Object, e As EventArgs)
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub pd_HomeButton_Click(sender As Object, e As EventArgs) Handles pd_HomeButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Log Out?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If confirm = DialogResult.Yes Then
            Dim patientLoginForm As New AdminLogin()
            patientLoginForm.Show()
            Me.Hide()
        End If
    End Sub

    Private Sub EditBtn_Click(sender As Object, e As EventArgs)
        For Each row As DataGridViewRow In StaffDataGridViewList2.Rows
            Dim checkBoxCell As DataGridViewCheckBoxCell = TryCast(row.Cells("SelectCheckBox"), DataGridViewCheckBoxCell)

            If checkBoxCell IsNot Nothing AndAlso Convert.ToBoolean(checkBoxCell.Value) Then
                For Each cell As DataGridViewCell In row.Cells
                    cell.ReadOnly = False
                Next

                checkBoxCell.Value = False
            End If
        Next
    End Sub

    Private Sub StaffDataGridViewList2_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs)
        If e.ColumnIndex <> 0 Then
            e.Cancel = True
        End If
    End Sub

    Private Sub StaffDataGridViewList2_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
            Dim isChecked As Boolean = CType(StaffDataGridViewList2.Rows(e.RowIndex).Cells(0).Value, Boolean)

            If isChecked Then
                For Each row As DataGridViewRow In StaffDataGridViewList2.Rows
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

    Private Sub StaffDataGridViewList2_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If TypeOf StaffDataGridViewList2.CurrentCell Is DataGridViewCheckBoxCell Then
            StaffDataGridViewList2.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub AppointmentDataGridViewList2_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs)
        If e.ColumnIndex <> 0 Then
            e.Cancel = True
        End If
    End Sub

    Private Sub AppointmentDataGridViewList2_CellBeginEdit_1(sender As Object, e As DataGridViewCellCancelEventArgs)
        If e.ColumnIndex <> 0 Then
            e.Cancel = True
        End If
    End Sub

    Private Sub AppointmentDataGridViewList2_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
            Dim isChecked As Boolean = CType(AppointmentDataGridViewList2.Rows(e.RowIndex).Cells(0).Value, Boolean)

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

    Private Sub AppointmentDataGridViewList2_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If TypeOf AppointmentDataGridViewList2.CurrentCell Is DataGridViewCheckBoxCell Then
            AppointmentDataGridViewList2.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub PatientDataGridViewList2_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs)
        If e.ColumnIndex <> 0 Then
            e.Cancel = True
        End If
    End Sub

    Private Sub PatientDataGridViewList2_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
            Dim isChecked As Boolean = CType(PatientDataGridViewList2.Rows(e.RowIndex).Cells(0).Value, Boolean)

            If isChecked Then
                For Each row As DataGridViewRow In PatientDataGridViewList2.Rows
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

    Private Sub PatientDataGridViewList2_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If TypeOf PatientDataGridViewList2.CurrentCell Is DataGridViewCheckBoxCell Then
            PatientDataGridViewList2.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub DoctorDataGridViewList2_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs)
        If e.ColumnIndex <> 0 Then
            e.Cancel = True
        End If
    End Sub

    Private Sub DoctorDataGridViewList2_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
            Dim isChecked As Boolean = CType(DoctorDataGridViewList2.Rows(e.RowIndex).Cells(0).Value, Boolean)

            If isChecked Then
                For Each row As DataGridViewRow In DoctorDataGridViewList2.Rows
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

    Private Sub DoctorDataGridViewList2_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If TypeOf DoctorDataGridViewList2.CurrentCell Is DataGridViewCheckBoxCell Then
            DoctorDataGridViewList2.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub StaffDataGridViewList2_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        If TypeOf StaffDataGridViewList2.Columns(e.ColumnIndex) Is DataGridViewButtonColumn AndAlso
       StaffDataGridViewList2.Columns(e.ColumnIndex).Name = "EditButton" Then

            Dim selectedRow As DataGridViewRow = StaffDataGridViewList2.Rows(e.RowIndex)

            For Each cell As DataGridViewCell In selectedRow.Cells
                cell.ReadOnly = False
            Next

            selectedRow.Cells("EditButton").Value = "Save"
        End If
    End Sub

    Private Sub StaffDataGridViewList2_RowValidated(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = StaffDataGridViewList2.Rows(e.RowIndex)

            If row.ReadOnly = False Then
                row.Tag = "Modified"
            End If
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
            MessageBox.Show("Patient ID must contain only numeric values.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not String.IsNullOrEmpty(transactionId) OrElse Not String.IsNullOrEmpty(patientName) Then
            Try
                Dim dataSource As DataTable = CType(PatientDataGridViewList2.DataSource, DataTable)

                If dataSource IsNot Nothing Then
                    Dim filter As String = ""

                    If Not String.IsNullOrEmpty(transactionId) Then
                        filter = $"Convert([Patient ID], 'System.String') LIKE '%{transactionId}%'"
                    End If

                    If Not String.IsNullOrEmpty(patientName) Then
                        If Not String.IsNullOrEmpty(filter) Then
                            filter &= " OR "
                        End If

                        Dim nameParts As String() = patientName.Split(","c)

                        If nameParts.Length = 2 Then
                            Dim lastName As String = nameParts(0).Trim()
                            Dim firstName As String = nameParts(1).Trim()

                            filter &= $"[First Name] LIKE '%{firstName}%' OR [Last Name] LIKE '%{lastName}%'"
                        Else
                            filter &= $"[First Name] LIKE '%{patientName}%' OR [Last Name] LIKE '%{patientName}%'"
                        End If
                    End If

                    dataSource.DefaultView.RowFilter = filter
                End If
            Catch ex As Exception
                MessageBox.Show($"Error while filtering data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Please enter either a Patient ID or a patient name to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub ResetTransactionFilterButton_Click(sender As Object, e As EventArgs) Handles ResetTransactionFilterButton.Click
        Try
            Dim dataSource As DataTable = CType(PatientDataGridViewList2.DataSource, DataTable)

            If dataSource IsNot Nothing Then
                dataSource.DefaultView.RowFilter = String.Empty

                TransactionIdTextBox.Clear()
                NameTextBox.Clear()
            End If
        Catch ex As Exception
            MessageBox.Show($"Error while resetting filter: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub SearchDoctorButton_Click(sender As Object, e As EventArgs) Handles SearchDoctorButton.Click
        Dim transactionId As String = SearchDoctorID.Text.Trim()
        Dim patientName As String = SearchDoctorName.Text.Trim()

        If Not String.IsNullOrEmpty(patientName) AndAlso patientName.Any(AddressOf Char.IsDigit) Then
            MessageBox.Show("Doctor name cannot contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not String.IsNullOrEmpty(transactionId) AndAlso Not transactionId.All(AddressOf Char.IsDigit) Then
            MessageBox.Show("Doctor ID must contain only numeric values.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not String.IsNullOrEmpty(transactionId) OrElse Not String.IsNullOrEmpty(patientName) Then
            Try
                Dim dataSource As DataTable = CType(DoctorDataGridViewList2.DataSource, DataTable)

                If dataSource IsNot Nothing Then
                    Dim filter As String = ""

                    If Not String.IsNullOrEmpty(transactionId) Then
                        filter = $"Convert([Doctor ID], 'System.String') LIKE '%{transactionId}%'"
                    End If

                    If Not String.IsNullOrEmpty(patientName) Then
                        If Not String.IsNullOrEmpty(filter) Then
                            filter &= " OR "
                        End If

                        Dim nameParts As String() = patientName.Split(","c)

                        If nameParts.Length = 2 Then
                            Dim lastName As String = nameParts(0).Trim()
                            Dim firstName As String = nameParts(1).Trim()

                            filter &= $"[First Name] LIKE '%{firstName}%' OR [Last Name] LIKE '%{lastName}%'"
                        Else
                            filter &= $"[First Name] LIKE '%{patientName}%' OR [Last Name] LIKE '%{patientName}%'"
                        End If
                    End If

                    dataSource.DefaultView.RowFilter = filter
                End If
            Catch ex As Exception
                MessageBox.Show($"Error while filtering data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Please enter either a Doctor ID or a Doctor name to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub ResetDoctorButton_Click(sender As Object, e As EventArgs) Handles ResetDoctorButton.Click
        Try
            Dim dataSource As DataTable = CType(DoctorDataGridViewList2.DataSource, DataTable)

            If dataSource IsNot Nothing Then
                dataSource.DefaultView.RowFilter = String.Empty

                SearchDoctorName.Clear()
                SearchDoctorID.Clear()
            End If
        Catch ex As Exception
            MessageBox.Show($"Error while resetting filter: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AppointmentSearchButton_Click(sender As Object, e As EventArgs) Handles SearchAppointmentButton.Click
        Dim transactionId As String = SearchAppointmentID.Text.Trim()
        Dim patientName As String = SearchAppointmentName.Text.Trim()

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
                Dim dataSource As DataTable = CType(AppointmentDataGridViewList2.DataSource, DataTable)

                If dataSource IsNot Nothing Then
                    Dim filter As String = ""

                    If Not String.IsNullOrEmpty(transactionId) Then
                        filter = $"Convert([Transaction ID], 'System.String') LIKE '%{transactionId}%'"
                    End If

                    If Not String.IsNullOrEmpty(patientName) Then
                        If Not String.IsNullOrEmpty(filter) Then
                            filter &= " OR "
                        End If

                        Dim nameParts As String() = patientName.Split(","c)

                        If nameParts.Length = 2 Then
                            Dim lastName As String = nameParts(0).Trim()
                            Dim firstName As String = nameParts(1).Trim()

                            filter &= $"[Patient Name] LIKE '%{lastName}%' AND [Patient Name] LIKE '%{firstName}%'"
                        Else
                            filter &= $"[Patient Name] LIKE '%{patientName}%'"
                        End If
                    End If

                    dataSource.DefaultView.RowFilter = filter
                End If
            Catch ex As Exception
                MessageBox.Show($"Error while filtering data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Please enter either a transaction ID or patient name  to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub ResetAppointmentButton_Click(sender As Object, e As EventArgs) Handles ResetAppointmentButton.Click
        Try
            Dim dataSource As DataTable = CType(AppointmentDataGridViewList2.DataSource, DataTable)

            If dataSource IsNot Nothing Then
                dataSource.DefaultView.RowFilter = String.Empty

                SearchAppointmentName.Clear()
                SearchAppointmentID.Clear()
            End If
        Catch ex As Exception
            MessageBox.Show($"Error while resetting filter: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PatientDataGridViewList2_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim selectedRow As DataGridViewRow = PatientDataGridViewList2.Rows(e.RowIndex)

            Dim patientID As Integer = Integer.Parse(selectedRow.Cells(0).Value.ToString())

            Me.Cursor = Cursors.WaitCursor
            Dim regForm As New PatientRegisterForm(ModalMode.Edit, patientID)
            AddHandler regForm.FormClosed, AddressOf ShowPatientTab
            regForm.ShowDialog()

            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub DoctorDataGridViewList2_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim selectedRow As DataGridViewRow = DoctorDataGridViewList2.Rows(e.RowIndex)

            Dim doctorID As Integer = Integer.Parse(selectedRow.Cells(0).Value.ToString())

            Me.Cursor = Cursors.WaitCursor

            Dim doctorForm As New AdminAddDoctor(ModalMode.Edit, doctorID)
            AddHandler doctorForm.ShowDoctorList, AddressOf ShowDoctorList
            doctorForm.ShowDialog()

            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub ExitButton_Click_1(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to exit Infocare?", "Confirm Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub
End Class
