Imports System.Drawing.Printing

Partial Public Class ViewPrescriptionAndDoctorOrders
    Inherits Form

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub SetDetails(firstName As String, lastName As String, birthDate As String,
                          doctorFirstName As String, doctorLastName As String,
                          appointmentDate As String, prescription As String, doctorOrder As String)

        viewinfo_Fname.Text = firstName
        viewinfo_Lname.Text = lastName
        viewinfo_Bdate.Text = birthDate
        diagnosis_Fname.Text = doctorFirstName
        diagnosis_Lname.Text = doctorLastName
        appointmentdateTextBox.Text = appointmentDate
        viewinfo_prescription.Text = prescription
        viewinfo_DoctorOrder.Text = doctorOrder
    End Sub

    ' Print Button Click
    Private Sub viewinfo_PrintBtn_Click(sender As Object, e As EventArgs) Handles viewinfo_PrintBtn.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Print this form?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Print(Me.PrintablePanel)
        End If
    End Sub

    ' Exit Button
    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Me.Hide()
        End If
    End Sub

    ' Create PDF Button Click
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

    ' Print Function
    Private Sub Print(PrintPanel As Panel)
        Dim ps As New PrinterSettings()
        getprintarea(PrintPanel)
        PrintPreviewDialog1.Document = PrintDocument1
        AddHandler PrintDocument1.PrintPage, AddressOf printDocument1_PrintPage
        PrintPreviewDialog1.ShowDialog()
    End Sub

    ' Capture Area
    Private memorying As Bitmap
    Private Sub getprintarea(PrintPanel As Panel)
        memorying = New Bitmap(PrintPanel.Width, PrintPanel.Height)
        PrintPanel.DrawToBitmap(memorying, New Rectangle(0, 0, PrintPanel.Width, PrintPanel.Height))
    End Sub

    ' Print Page Event
    Private Sub printDocument1_PrintPage(sender As Object, e As PrintPageEventArgs)
        Dim pagearea As Rectangle = e.PageBounds
        Dim scaleWidth As Single = CSng(pagearea.Width) / PrintablePanel.Width
        Dim scaleHeight As Single = CSng(pagearea.Height) / PrintablePanel.Height
        Dim scale As Single = Math.Min(scaleWidth, scaleHeight)

        e.Graphics.DrawImage(memorying, 0, 0, PrintablePanel.Width * scale, PrintablePanel.Height * scale)
    End Sub

    ' PDF Function
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

End Class
