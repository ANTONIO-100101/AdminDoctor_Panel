' VB.NET equivalent of the provided C# code

Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports Guna.UI2.WinForms
Imports Infocare_Project_1.Object_Models
Imports Org.BouncyCastle.Asn1.Mozilla
Imports OtpNet
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Public Class PatientBillingInvoice
    Inherits Form

    Private appointment As Appointment
    Private total As Decimal = 0
    Private memorying As Bitmap

    Public Sub New(appointment As Appointment)
        Me.appointment = appointment
        InitializeComponent()
    End Sub

    Private Shared Function StringToDictionary(input As String) As Dictionary(Of String, Decimal)
        Dim result As New Dictionary(Of String, Decimal)()
        Dim pairs As String() = input.Split(New String() {", "}, StringSplitOptions.RemoveEmptyEntries)

        For Each pair As String In pairs
            Dim keyValue As String() = pair.Split(" "c)

            If keyValue.Length = 2 Then
                Dim key As String = keyValue(0)
                Dim value As Decimal
                If Decimal.TryParse(keyValue(1), value) Then
                    result(key) = value
                End If
            ElseIf keyValue.Length = 3 Then
                Dim key As String = keyValue(0)
                Dim value As Decimal
                If Decimal.TryParse(keyValue(2), value) Then
                    result(key) = value
                End If
            End If
        Next

        Return result
    End Function

    Public Sub SetupAppointmentData()
        Dim data As New DataTable()
        data.Columns.Add("Patient Name")
        data.Columns.Add("Doctor Name")
        data.Columns.Add("Time", GetType(TimeSpan))
        data.Columns.Add("Date", GetType(DateTime))
        data.Columns.Add("Consultation Fee", GetType(Decimal))

        data.Rows.Add(appointment.PatientName, appointment.DoctorName, appointment.Time, appointment.Date, appointment.ConsultationFee)

        pbilling_DataGridView1.DataSource = data
    End Sub

    Private Sub SetupDataGridView(gridview As Guna2DataGridView, valuePairs As Dictionary(Of String, Decimal), category As String)
        Dim data As New DataTable()
        data.Columns.Add(category)
        data.Columns.Add("Price", GetType(Decimal))

        For Each kvp As KeyValuePair(Of String, Decimal) In valuePairs
            total += kvp.Value
            data.Rows.Add(kvp.Key, kvp.Value)
        Next

        gridview.DataSource = data
    End Sub

    Private Sub PatientBillingInvoice_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupAppointmentData()
        SetupDataGridView(pbilling_DataGridView2, StringToDictionary(appointment.Diagnosis.DoctorOrders), "Doctor Order")
        SetupDataGridView(pbilling_DataGridView3, StringToDictionary(appointment.Diagnosis.Prescription), "Prescription")

        pbilling_PatientNameTextbox.Text = appointment.PatientName
        pbilling_DateTextbox.Text = appointment.Date.ToString("d")
        pbilling_TimeTextbox.Text = appointment.Time.ToString("hh\:mm")
        guna2TextBox1.Text = appointment.ConfineDays.ToString()

        total += (appointment.confineDays * 250 + appointment.ConsultationFee)
        pbilling_TotalLabel.Text = $"₱{total}"
    End Sub

    Private Sub pbilling_ExitButton_Click(sender As Object, e As EventArgs) Handles pbilling_ExitButton.Click
        Me.Close()
    End Sub

    Private Sub pbilling_MinimizeButton_Click(sender As Object, e As EventArgs) Handles pbilling_MinimizeButton.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub pbilling_PrintButton_Click(sender As Object, e As EventArgs) Handles pbilling_PrintButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to Print this form?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Print(Me.PrintablePanel)
        End If
    End Sub

    Private Sub pbilling_CreatePDFButton_Click(sender As Object, e As EventArgs) Handles pbilling_CreatePDFButton.Click
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
        AddHandler printDocument.PrintPage,
            Sub(sender As Object, e As PrintPageEventArgs)
                memorying = New Bitmap(printPanel.Width, printPanel.Height)
                printPanel.DrawToBitmap(memorying, New Rectangle(0, 0, printPanel.Width, printPanel.Height))
                e.Graphics.DrawImage(memorying, 0, 0)
            End Sub
        printDocument.PrintController = New StandardPrintController()
        printDocument.Print()
    End Sub

    Private Sub Print(PrintPanel As Panel)
        PrintPanel = PrintablePanel
        getprintarea(PrintPanel)
        printPreviewDialog1.Document = printDocument1
        AddHandler printDocument1.PrintPage, AddressOf printDocument1_PrintPage
        printPreviewDialog1.ShowDialog()
    End Sub

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
End Class