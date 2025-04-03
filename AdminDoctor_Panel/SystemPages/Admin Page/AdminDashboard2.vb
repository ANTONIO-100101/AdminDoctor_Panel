Public Class AdminDashboard2

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub ShowDoctorList()
        Try
            Dim DoctorData As DataTable = Database.DoctorList()
            If DoctorData.Rows.Count > 0 Then
                DoctorDataGridViewList2.DataSource = DoctorData
            Else
                MessageBox.Show("No Doctor data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"Error loading doctor data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub pd_HomeButton_Click(sender As Object, e As EventArgs) Handles pd_HomeButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Log Out?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If confirm = DialogResult.Yes Then
            Dim patientLoginForm As New AdminLogin()
            patientLoginForm.Show()
            Me.Hide()
        End If
    End Sub

    Private Sub ad_doctor_Click(sender As Object, e As EventArgs) Handles ad_doctor.Click
        SearchPanel4.Visible = False
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
    End Sub
End Class