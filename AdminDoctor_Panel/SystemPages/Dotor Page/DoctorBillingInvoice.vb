Imports System.Drawing.Printing

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

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Try
                Dim checkoutTable As DataTable = CType(billing_DataGridView.DataSource, DataTable)

                Dim doctorName As String = billing_DoctorNameTextbox.Text.Trim()
                Dim specialization As String = billing_Specialization.Text.Trim()

                If String.IsNullOrEmpty(doctorName) OrElse String.IsNullOrEmpty(specialization) Then
                    MessageBox.Show("Doctor Name or Specialization cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                For Each row As DataRow In checkoutTable.Rows
                    Dim [date] As String = Convert.ToDateTime(row("ah_date")).ToString("dd-MM-yyyy")
                    Database.UpdateStatus(doctorName)
                Next

                MessageBox.Show("Statuses updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show($"Error updating statuses: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            Me.Hide()
        End If
    End Sub

    Private Sub viewinfo_PrintBtn_Click(sender As Object, e As EventArgs) Handles viewinfo_PrintBtn.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Print this form?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Print(Me.PrintablePanel)
        End If
    End Sub

    Private Sub CreatePDFButton_Click(sender As Object, e As EventArgs) Handles CreatePDFButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to make PDF of form?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Dim saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf"
            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                Dim filePath As String = saveFileDialog.FileName
                PrintToPDF(Me.PrintablePanel, filePath)
            End If
        End If
    End Sub
    Private Sub PrintToPDF(printPanel As Panel, filePath As String)
        Dim printDocument As New PrintDocument()
        printDocument.PrinterSettings.PrinterName = "Microsoft Print to PDF"
        AddHandler printDocument.PrintPage, Sub(sender, e)
                                                Dim memorying As New Bitmap(printPanel.Width, printPanel.Height)
                                                printPanel.DrawToBitmap(memorying, New Rectangle(0, 0, printPanel.Width, printPanel.Height))
                                                e.Graphics.DrawImage(memorying, 0, 0)
                                            End Sub
        printDocument.PrintController = New StandardPrintController()
        printDocument.Print()
    End Sub

    Private Sub Print(PrintPanel As Panel)
        Dim ps As New PrinterSettings()
        Me.PrintablePanel = PrintPanel
        getprintarea(PrintPanel)
        printPreviewDialog1.Document = printDocument1
        AddHandler printDocument1.PrintPage, AddressOf printDocument1_PrintPage
        printPreviewDialog1.ShowDialog()
    End Sub

    Private memorying As Bitmap

    Private Sub getprintarea(PrintPanel As Panel)
        memorying = New Bitmap(PrintPanel.Width, PrintPanel.Height)
        PrintPanel.DrawToBitmap(memorying, New Rectangle(0, 0, PrintPanel.Width, PrintPanel.Height))
    End Sub

    Private Sub printDocument1_PrintPage(sender As Object, e As PrintPageEventArgs)
        Dim pagearea As Rectangle = e.PageBounds
        Dim scaleWidth As Single = pagearea.Width / PrintablePanel.Width
        Dim scaleHeight As Single = pagearea.Height / PrintablePanel.Height
        Dim scale As Single = Math.Min(scaleWidth, scaleHeight)
        e.Graphics.DrawImage(memorying, 0, 0, PrintablePanel.Width * scale, PrintablePanel.Height * scale)
    End Sub

    Private Sub guna2Button1_Click(sender As Object, e As EventArgs) Handles guna2Button1.Click
        Me.Close()
    End Sub

End Class
