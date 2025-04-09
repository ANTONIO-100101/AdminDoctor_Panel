Partial Public Class DoctorBillingInvoice
    Inherits Form

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub SetDoctorDetails(doctorName As String, specialization As String, doctorDate As String, transactions As DataTable)
        billing_DoctorNameTextbox.Text = doctorName
        billing_Specialization.Text = specialization

        Dim totalConsultationFee As Decimal = 0

        For Each row As DataRow In transactions.Rows
            totalConsultationFee += Convert.ToDecimal(row("ah_Consfee"))
        Next

        TotalLabel.Text = $"{totalConsultationFee:C}"
    End Sub

    Private Sub DoctorBillingInvoice_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim currentDate As String = DateTime.Now.ToString("dd-MM-yyyy")
        billing_DateTextBox.Text = currentDate

        Dim currentTime As String = DateTime.Now.ToString("hh : mm tt")
        billing_TimeTextBox.Text = currentTime

        Dim checkoutTable As DataTable = Database.ChecOutList()
        billing_DataGridView.DataSource = checkoutTable
    End Sub
End Class
