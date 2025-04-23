Imports System.Drawing.Printing
Imports Guna.UI2.WinForms

Partial Public Class ViewPatientInformation2
    Inherits Form
    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub SetDetails(firstName As String, lastName As String, birthDate As String, height As String, weight As String, bmi As String,
                           bloodType As String, allergy As String, medication As String, prevSurgery As String, precondition As String,
                           treatment As String, doctorFirstName As String, doctorLastName As String, appointmentTime As String,
                           appointmentDate As String, consultationFee As String, diagnosis As String, additionalNotes As String,
                           doctorOrder As String, prescription As String)
        viewinfo_Fname.Text = firstName
        viewinfo_Lname.Text = lastName
        viewinfo_Bdate.Text = birthDate
        viewinfo_Height.Text = height
        viewinfo_Weight.Text = weight
        viewinfo_Bmi.Text = bmi
        viewifo_Btype.Text = bloodType
        viewinfo_Allergy.Text = allergy
        viewinfo_Medic.Text = medication
        viewinfo_Prevsur.Text = prevSurgery
        viewinfo_Precon.Text = precondition
        viewinfo_Treatment.Text = treatment
        diagnosis_Fname.Text = doctorFirstName
        diagnosis_Lname.Text = doctorLastName
        appointmenttimeTextBox.Text = appointmentTime
        appointmentdateTextBox.Text = appointmentDate
        consultationTextBox.Text = consultationFee
        viewinfo_AdditionalNotes.Text = additionalNotes
        viewinfo_diagnosis.Text = diagnosis
        viewinfo_prescription.Text = prescription
        viewinfo_DoctorOrder.Text = doctorOrder
    End Sub

    Private Sub guna2Button1_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub ViewPatientInformation2_Load(sender As Object, e As EventArgs)
    End Sub

    Private Sub Print(PrintPanel As Panel)
        Dim ps As New PrinterSettings()
        PrintPanel = PrintablePanel
        getprintarea(PrintPanel)
        PrintPreviewDialog1.Document = PrintDocument1
        AddHandler PrintDocument1.PrintPage, AddressOf printDocument1_PrintPage
        PrintPreviewDialog1.ShowDialog()
    End Sub

    Private memorying As Bitmap

    Private Sub getprintarea(PrintPanel As Panel)
        memorying = New Bitmap(PrintPanel.Width, PrintPanel.Height)
        PrintPanel.DrawToBitmap(memorying, New Rectangle(0, 0, PrintPanel.Width, PrintPanel.Height))
    End Sub

    Private Sub printDocument1_PrintPage(sender As Object, e As PrintPageEventArgs)
        Dim pagearea As Rectangle = e.PageBounds
        Dim scaleWidth As Single = CSng(pagearea.Width) / PrintablePanel.Width
        Dim scaleHeight As Single = CSng(pagearea.Height) / PrintablePanel.Height
        Dim scale As Single = Math.Min(scaleWidth, scaleHeight)

        e.Graphics.DrawImage(memorying, 0, 0, PrintablePanel.Width * scale, PrintablePanel.Height * scale)
    End Sub


    Private Sub viewinfo_PrintBtn_Click(sender As Object, e As EventArgs) Handles viewinfo_PrintBtn.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Print this form?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Print(Me.PrintablePanel)
        End If
    End Sub

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Me.Hide()
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

    Private Sub Guna2ImageButton1_Click(sender As Object, e As EventArgs) Handles Guna2ImageButton1.Click
        Dim firstName As String = viewinfo_Fname.Text
        Dim lastName As String = viewinfo_Lname.Text
        Dim birthDate As String = viewinfo_Bdate.Text
        Dim doctorFirstName As String = diagnosis_Fname.Text
        Dim doctorLastName As String = diagnosis_Lname.Text
        Dim appointmentDate As String = appointmentdateTextBox.Text
        Dim prescription As String = viewinfo_prescription.Text
        Dim doctorOrder As String = viewinfo_DoctorOrder.Text

        Dim prescriptionForm As New ViewPrescriptionAndDoctorOrders()
        prescriptionForm.SetDetails(
            firstName,
            lastName,
            birthDate,
            doctorFirstName,
            doctorLastName,
            appointmentDate,
            prescription,
            doctorOrder
        )
        prescriptionForm.Show()
    End Sub

End Class
