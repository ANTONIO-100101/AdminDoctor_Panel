Imports AdminDoctor_Panel.Infocare_Project.NewFolder
Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports Guna.UI2.WinForms
Imports System.Globalization
Imports System.Text.RegularExpressions

Partial Public Class AdminAddDoctor
    Inherits Form

    Private _placeHolderHandler As PlaceHolderHandler
    Public Event ShowDoctorList As Action
    Private passShow As Boolean
    Private mode As ModalMode
    Private AccountId As Integer
    Private doctor As DoctorModel

    Public Sub New(Optional mode As ModalMode = ModalMode.Add, Optional AccountID As Integer = 0)
        InitializeComponent()
        _placeHolderHandler = New PlaceHolderHandler()

        Me.mode = mode
        Me.AccountId = AccountID

        If mode = ModalMode.Edit Then
            RegisterButton.Text = "Update"
            guna2HtmlLabel2.Text = "Update Doctor"
            PasswordTextBox.Visible = False
            ConfirmPasswordTextBox.Visible = False
            removeDoctor.Visible = True
            passValidatorMsg.Visible = False
        End If
    End Sub

    Public Function ConcatenateTimeSpan(startTime As TimeSpan, endTime As TimeSpan) As String
        Dim start As DateTime = DateTime.Today.Add(startTime)
        Dim [end] As DateTime = DateTime.Today.Add(endTime)

        Return String.Format("{0} - {1}", start.ToString("hh:mm tt"), [end].ToString("hh:mm tt"))
    End Function

    Public Sub FillUpFields(doctor As DoctorModel)
        FirstNameTextBox.Text = doctor.FirstName
        ConsultationFeeTextBox.Text = doctor.ConsultationFee.ToString()
        LastNameTextbox.Text = doctor.LastName
        MiddleNameTextbox.Text = doctor.MiddleName
        DayAvailabilityCombobox.SelectedItem = doctor.DayAvailability

        UserNameTextBox.Text = doctor.UserName
        emailTextBox.Text = doctor.Email
        ContactNumberTextbox.Text = doctor.ContactNumber

        PopulateTimeCombobox()
        TimeComboBox.SelectedItem = ConcatenateTimeSpan(doctor.StartTime, doctor.EndTime)

        flowLayoutPanel1.Controls.Clear()
        For Each skill As String In doctor.Specialty
            Dim label As New Guna2TextBox()
            label.Text = skill

            flowLayoutPanel1.Controls.Add(label)
        Next
    End Sub

    Private Sub AdminAddDoctor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateTimeCombobox()
        DayAvComboBox()

        If mode = ModalMode.Edit Then
            doctor = Database.GetDoctorInfo(AccountId)
            FillUpFields(doctor)
        End If
    End Sub

    Private Sub BackButton_Click(sender As Object, e As EventArgs) Handles BackButton.Click
        Me.Hide()
    End Sub

    Private Sub RegisterButton_Click(sender As Object, e As EventArgs) Handles RegisterButton.Click

        Dim contactNumber As String = ContactNumberTextbox.Text

        If contactNumber.Length > 0 AndAlso (contactNumber.Length <> 11 OrElse Not contactNumber.StartsWith("09") OrElse Not contactNumber.All(AddressOf Char.IsDigit)) Then
            MessageBox.Show("Invalid number. The contact number must start with '09' and be exactly 11 digits.", "Invalid Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not FirstNameTextBox.Text.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c)) AndAlso Not String.IsNullOrEmpty(FirstNameTextBox.Text) Then
            MessageBox.Show("First name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not LastNameTextbox.Text.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c)) AndAlso Not String.IsNullOrEmpty(LastNameTextbox.Text) Then
            MessageBox.Show("First name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not MiddleNameTextbox.Text.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c) OrElse c = "/") AndAlso Not String.IsNullOrEmpty(MiddleNameTextbox.Text) Then
            MessageBox.Show("Middle name must contain only letters, spaces, and the '/' character.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Database.UsernameExistsDoctor(UserNameTextBox.Text) AndAlso mode = ModalMode.Add Then
            MessageBox.Show("The username is already in use. Please choose a different username.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim password As String = PasswordTextBox.Text

        If mode = ModalMode.Add Then
            If Not ProcessMethods.ValidatePassword(password) Then
                Return
            End If
        End If

        If Not ProcessMethods.ValidateEmail(emailTextBox.Text) Then
            MessageBox.Show("Please enter a valid email.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(FirstNameTextBox.Text.Trim()) OrElse
           String.IsNullOrWhiteSpace(LastNameTextbox.Text.Trim()) OrElse
           String.IsNullOrWhiteSpace(UserNameTextBox.Text.Trim()) OrElse
           (String.IsNullOrWhiteSpace(PasswordTextBox.Text.Trim()) AndAlso mode = ModalMode.Add) OrElse
           (String.IsNullOrWhiteSpace(ConfirmPasswordTextBox.Text.Trim()) AndAlso mode = ModalMode.Add) OrElse
           String.IsNullOrWhiteSpace(ConsultationFeeTextBox.Text.Trim()) OrElse
           TimeComboBox.SelectedIndex = 0 OrElse
           DayAvailabilityCombobox.SelectedIndex = 0 Then
            MessageBox.Show("Please fill out all fields and select valid options.")
            Return
        End If

        Dim specializations As New List(Of String)()
        For Each control As Object In flowLayoutPanel1.Controls
            If TypeOf control Is Guna.UI2.WinForms.Guna2TextBox Then
                Dim specializationTextBox As Guna.UI2.WinForms.Guna2TextBox = CType(control, Guna.UI2.WinForms.Guna2TextBox)
                If Not String.IsNullOrWhiteSpace(specializationTextBox.Text.Trim()) Then
                    specializations.Add(specializationTextBox.Text.Trim())
                End If
            End If
        Next

        If specializations.Count = 0 Then
            MessageBox.Show("Please enter at least one specialization.")
            Return
        End If

        Dim contact As Decimal

        If Not Decimal.TryParse(ConsultationFeeTextBox.Text, contact) Then
            MessageBox.Show("Please enter a valid Consultation Fee.")
            Return
        End If

        Dim newDoctorInfo As New DoctorModel With {
            .AccountID = If(mode = ModalMode.Edit, doctor.AccountID, 0),
            .FirstName = FirstNameTextBox.Text.Trim(),
            .LastName = LastNameTextbox.Text.Trim(),
            .MiddleName = MiddleNameTextbox.Text.Trim(),
            .ContactNumber = ContactNumberTextbox.Text.Trim(),
            .UserName = UserNameTextBox.Text.Trim(),
            .Password = If(mode = ModalMode.Edit, doctor.Password, PasswordTextBox.Text.Trim()),
            .Email = emailTextBox.Text,
            .ConsultationFee = If(Decimal.TryParse(ConsultationFeeTextBox.Text, Nothing), Convert.ToDecimal(ConsultationFeeTextBox.Text), 0),
            .Specialty = specializations
        }

        Dim selectedTimeSlot As String = TimeComboBox.SelectedItem.ToString()
        If Not String.IsNullOrEmpty(selectedTimeSlot) AndAlso selectedTimeSlot <> "Select a time slot" Then
            Dim timeSlots As String() = selectedTimeSlot.Split("-"c)
            If timeSlots.Length = 2 Then
                Dim startTimeString As String = timeSlots(0).Trim()
                Dim endTimeString As String = timeSlots(1).Trim()

                Dim startTime As DateTime
                Dim endTime As DateTime
                If DateTime.TryParseExact(startTimeString, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, startTime) AndAlso
           DateTime.TryParseExact(endTimeString, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, endTime) Then
                    newDoctorInfo.StartTime = startTime.TimeOfDay
                    newDoctorInfo.EndTime = endTime.TimeOfDay
                Else
                    MessageBox.Show("Please select a valid time.")
                    Return
                End If
            Else
                MessageBox.Show("Please select a valid time slot.")
                Return
            End If
        End If

        newDoctorInfo.DayAvailability = If(DayAvailabilityCombobox.SelectedItem IsNot Nothing, DayAvailabilityCombobox.SelectedItem.ToString(), String.Empty)

        If PasswordTextBox.Text.Trim() <> ConfirmPasswordTextBox.Text.Trim() Then
            MessageBox.Show("Passwords do not match.")
            Return
        End If

        Try
            Dim doctorId As Integer = Database.AddUpdateDoctor1(newDoctorInfo, mode)

            For Each specialization As String In specializations
                Database.AddSpecialization(doctorId, specialization)
            Next

            MessageBox.Show($"Doctor {(If(mode = ModalMode.Add, "Added", "Info Updated"))} successfully!")
            Me.Hide()
            RaiseEvent ShowDoctorList()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub PopulateTimeCombobox(Optional mode As ModalMode = ModalMode.Add)
        TimeComboBox.Items.Clear()
        TimeComboBox.Items.Add("Select a time slot")

        Dim startTime As TimeSpan = If(mode = ModalMode.Add, TimeSpan.FromHours(8), doctor.StartTime)
        Dim endTime As TimeSpan = If(mode = ModalMode.Add, TimeSpan.FromHours(20), doctor.EndTime)
        Dim interval As TimeSpan = TimeSpan.FromHours(4)

        For time As TimeSpan = startTime To endTime Step interval
            Dim slotEndTime As TimeSpan = time + interval
            Dim formattedTime As String = $"{(DateTime.Today + time):hh:mm tt} - {(DateTime.Today + slotEndTime):hh:mm tt}"
            TimeComboBox.Items.Add(formattedTime)
        Next

        TimeComboBox.SelectedIndex = 0
    End Sub


    Private Sub DayAvComboBox()
        DayAvailabilityCombobox.Items.Clear()
        DayAvailabilityCombobox.Items.Add("Select weekly schedule")

        DayAvailabilityCombobox.Items.Add("Monday-Wednesday-Friday")
        DayAvailabilityCombobox.Items.Add("Tuesday-Thursday-Saturday")

        DayAvailabilityCombobox.SelectedIndex = 0
    End Sub

    Private Sub FirstNameTextBox_TextChanged(sender As Object, e As EventArgs) Handles FirstNameTextBox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(FirstNameTextBox, FNLabel, "First name")
    End Sub

    Private Sub LastNameTextBox_TextChanged(sender As Object, e As EventArgs) Handles LastNameTextbox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(LastNameTextbox, LNLabel, "Last name")
    End Sub

    Private Sub UserNameTextBox_TextChanged(sender As Object, e As EventArgs) Handles UserNameTextBox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(UserNameTextBox, UNlabel, "User name")
    End Sub

    Private Sub PasswordTextBox_TextChanged(sender As Object, e As EventArgs) Handles PasswordTextBox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(PasswordTextBox, PLabel, "Password")

        If PasswordTextBox.Text.Trim() = "" Then
            passValidatorMsg.Visible = False
        Else
            passValidatorMsg.Visible = True
            Dim msg As String =
                If(Not Regex.IsMatch(PasswordTextBox.Text, "[A-Z]"), "Add at least one uppercase letter",
                If(Not Regex.IsMatch(PasswordTextBox.Text, "[^a-zA-Z0-9\s]"), "Add At least one special character",
                If(Not Regex.IsMatch(PasswordTextBox.Text, "[\d]"), "Add At least one number",
                If(Not Regex.IsMatch(PasswordTextBox.Text, ".{8,}"), "Must have at least 8 characters long", ""))))

            If msg = "" Then
                passValidatorMsg.Text = "*Strong Enough"
                passValidatorMsg.ForeColor = Color.Green
            Else
                passValidatorMsg.Text = "*" & msg
                passValidatorMsg.ForeColor = Color.Red
            End If
        End If
    End Sub

    Private Sub ConfirmPasswordTextBox_TextChanged(sender As Object, e As EventArgs) Handles ConfirmPasswordTextBox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(ConfirmPasswordTextBox, CPLabel, "Confirm Password")
    End Sub

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        If mode = ModalMode.Add Then
            Dim confirm As DialogResult = MessageBox.Show("Are you sure to cancel registration?", "Cancel registration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If confirm = DialogResult.Yes Then
                Me.Close()
            End If

            Return
        End If

        Me.Close()
    End Sub

    Private Sub MinimizeButton_Click(sender As Object, e As EventArgs) Handles MinimizeButton.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub BackButton_Click_1(sender As Object, e As EventArgs) Handles BackButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to go back? Unsaved changes will be lost.", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub AddSpecialization_Click(sender As Object, e As EventArgs)
        Dim newSpecializationTextBox As New Guna2TextBox With {
        .Width = 194,
        .Height = 38,
        .Margin = New Padding(3, 3, 3, 3),
        .PlaceholderText = "Specialization",
        .PlaceholderForeColor = Color.FromArgb(47, 89, 114),
        .Font = New Font("Segoe UI", 9),
        .ForeColor = Color.FromArgb(47, 89, 114),
        .BorderColor = Color.FromArgb(93, 202, 209),
        .BorderThickness = 1,
        .BorderRadius = 8,
        .BackColor = Color.FromArgb(110, 177, 247),
        .Cursor = Cursors.Default,
        .Padding = New Padding(0),
        .IconLeftSize = New Size(20, 20),
        .IconRightSize = New Size(20, 20),
        .WordWrap = True,
        .Multiline = False,
        .TextAlign = HorizontalAlignment.Left
    }

        flowLayoutPanel1.Controls.Add(newSpecializationTextBox)
    End Sub

    Private Sub RemoveDoctor_Click(sender As Object, e As EventArgs) Handles removeDoctor.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure you want to remove this doctor?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If confirm = DialogResult.Yes Then
            Database.DeleteDoctorById(doctor.AccountID)
            MessageBox.Show("Doctor removed successfully!")
            Me.Close()
            RaiseEvent ShowDoctorList()
        End If
    End Sub
End Class