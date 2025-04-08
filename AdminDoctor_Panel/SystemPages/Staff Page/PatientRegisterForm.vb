Imports System.Text.RegularExpressions
Imports AdminDoctor_Panel.Infocare_Project.NewFolder
Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Classes
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports Guna.UI2.WinForms

Public Class PatientRegisterForm
    Inherits Form

    Private passShow As Boolean = True
    Private _placeHolderHandler As PlaceHolderHandler
    Public Property ReloadResults As Action
    Private houseNo As Integer
    Private zipCode As Integer
    Private zone As Integer
    Private mode As ModalMode
    Private AccountID As Integer
    Private panelMode As PanelMode
    Public Property username As String
    Public Property password As String

    Private extractedInfo As PatientModel

    Public Sub New(mode As ModalMode, Optional AccountId As Integer = 0, Optional panelMode As PanelMode = PanelMode.AdminDoc)
        InitializeComponent()
        Me.mode = mode
        Me.panelMode = panelMode
        Me.AccountID = AccountId
        _placeHolderHandler = New PlaceHolderHandler()
        PageTitle.Text = If(mode = ModalMode.Edit, "Patient Information", "Patient Registration")

        If mode = ModalMode.Edit Then
            DeleteBtn.Visible = (panelMode = PanelMode.AdminDoc)
            PasswordTextBox.Visible = False
            passValidatorMsg.Visible = False
        End If
    End Sub

    Private Sub PatientRegisterForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If mode = ModalMode.Add Then
            BdayDateTimePicker.MaxDate = DateTime.Today
        Else
            ' Edit
            extractedInfo = Database.GetPatientInfoById(AccountID)
            FillUpFields(extractedInfo)
        End If
    End Sub

    Public Sub FillUpFields(info As PatientModel)
        FirstnameTxtbox.Text = info.FirstName
        LastNameTxtbox.Text = info.LastName
        UsernameTextbox.Text = info.UserName
        MiddleNameTxtbox.Text = info.MiddleName
        SuffixTxtbox.Text = info.Suffix
        EmailTxtbox.Text = info.Email
        ContactNumberTxtbox.Text = info.ContactNumber
        PasswordTextBox.Text = info.Password
        BdayDateTimePicker.Value = info.BirthDate
        SexCombobox.SelectedItem = info.Sex

        HouseNoTxtbox.Text = info.Address.HouseNo.ToString()
        ZipCodeTxtbox.Text = info.Address.ZipCode.ToString()
        ZoneTxtbox.Text = info.Address.Zone.ToString()
        StreetTxtbox.Text = info.Address.Street
        BarangayTxtbox.Text = info.Address.Barangay
        CityTxtbox.Text = info.Address.City
    End Sub

    Public Function SetupObj() As PatientModel
        Dim patient As New PatientModel() With {
            .FirstName = FirstnameTxtbox.Text,
            .LastName = LastNameTxtbox.Text,
            .UserName = UsernameTextbox.Text,
            .Password = PasswordTextBox.Text,
            .MiddleName = MiddleNameTxtbox.Text,
            .BirthDate = BdayDateTimePicker.Value,
            .Suffix = SuffixTxtbox.Text,
            .Email = EmailTxtbox.Text,
            .ContactNumber = ContactNumberTxtbox.Text,
            .Sex = SexCombobox.SelectedItem.ToString()
        }

        Dim address As New AddressModel() With {
            .HouseNo = Integer.Parse(HouseNoTxtbox.Text),
            .ZipCode = Integer.Parse(ZipCodeTxtbox.Text),
            .City = CityTxtbox.Text,
            .Zone = Integer.Parse(ZoneTxtbox.Text),
            .Street = StreetTxtbox.Text,
            .Barangay = BarangayTxtbox.Text
        }

        patient.Address = address

        Return patient
    End Function

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show($"Are you sure to cancel {(If(mode = ModalMode.Add, "cancel registration", "close"))}?", "Cancel registration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirm = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub EnterButton_Click(sender As Object, e As EventArgs) Handles EnterButton.Click
        Dim contactNumber As String = ContactNumberTxtbox.Text

        If contactNumber.Length > 0 AndAlso (contactNumber.Length <> 11 OrElse Not contactNumber.StartsWith("09") OrElse Not contactNumber.All(AddressOf Char.IsDigit)) Then
            MessageBox.Show("Invalid number. The contact number must start with '09' and be exactly 11 digits.", "Invalid Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not FirstnameTxtbox.Text.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c)) AndAlso Not String.IsNullOrEmpty(FirstnameTxtbox.Text) Then
            MessageBox.Show("First name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not LastNameTxtbox.Text.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c)) AndAlso Not String.IsNullOrEmpty(LastNameTxtbox.Text) Then
            MessageBox.Show("Last name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not MiddleNameTxtbox.Text.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c) OrElse c = "/"c) AndAlso Not String.IsNullOrEmpty(MiddleNameTxtbox.Text) Then
            MessageBox.Show("Middle name must contain only letters, spaces, and the '/' character.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Database.UsernameExists(UsernameTextbox.Text, Role.Patient) AndAlso mode = ModalMode.Add Then
            MessageBox.Show("The username is already in use. Please choose a different username.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not ProcessMethods.ValidateEmail(EmailTxtbox.Text) Then
            MessageBox.Show("Please enter a valid email.", "Invalid error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim password As String = PasswordTextBox.Text

        If mode = ModalMode.Add Then
            If Not ProcessMethods.ValidatePassword(password) Then
                Return
            End If
        End If

        If Not InputValidator.ValidateAlphabetic(FirstnameTxtbox, "First name must contain only letters. ex. (Juan)") OrElse
            Not InputValidator.ValidateAlphabetic(LastNameTxtbox, "Last name must contain only letters. ex. (Dela Cruz)") OrElse
            Not InputValidator.ValidateAlphabetic(CityTxtbox, "City must contain only letters. ex. (Caloocan)") Then
            Return
        End If

        If Not UsernameTextbox.Text.All(Function(c) Char.IsLetter(c)) AndAlso Not String.IsNullOrEmpty(UsernameTextbox.Text) Then
            MessageBox.Show("Username must contain only letters.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not InputValidator.ValidateNumeric(ContactNumberTxtbox, "Contact number must contain only numbers. ex.(09777864220)") OrElse
            Not InputValidator.ValidateNumeric(ZipCodeTxtbox, "Zip Code must contain only numbers. ex. (1400)") OrElse
            Not InputValidator.ValidateNumeric(ZoneTxtbox, "Zone must contain only numbers. ex. (1)") OrElse
            Not InputValidator.ValidateNumeric(HouseNoTxtbox, "House No must contain only numbers. ex. (14)") Then
            Return
        End If

        Dim validSuffixes As String() = {"Jr.", "Sr.", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "Jr", "Sr", "N/A"}
        Dim enteredText As String = SuffixTxtbox.Text.Trim()

        If Not String.IsNullOrEmpty(enteredText) AndAlso Not validSuffixes.Any(Function(suffix) String.Equals(suffix, enteredText, StringComparison.OrdinalIgnoreCase)) Then
            MessageBox.Show("Please enter a valid suffix (e.g., Jr., Sr., I, II, III, IV, etc.).", "Invalid Suffix", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim ZipCode As Integer
        Dim Zone As Integer

        If Not Integer.TryParse(ZipCodeTxtbox.Text, ZipCode) Then
            MessageBox.Show("Please enter a valid number for Zip Code.")
            Return
        End If

        If Not Integer.TryParse(ZoneTxtbox.Text, Zone) Then
            MessageBox.Show("Please enter a valid number for Zone.")
            Return
        End If

        If Database.IsUsernameExists(EmailTxtbox.Text) Then
            MessageBox.Show("The username is already in use. Please choose a different username.")
            Return
        End If

        If SexCombobox.SelectedItem Is Nothing Then
            MessageBox.Show("Please select your gender")
            Return
        End If

        Dim requiredTextBoxes As Guna2TextBox() = {FirstnameTxtbox, LastNameTxtbox, MiddleNameTxtbox, SuffixTxtbox, CityTxtbox,
                                                  ContactNumberTxtbox, ZipCodeTxtbox, ZoneTxtbox, StreetTxtbox, BarangayTxtbox, EmailTxtbox}

        If Not InputValidator.ValidateAllFieldsFilled(requiredTextBoxes, "Please fill out all fields.") Then
            Return
        End If

        Dim address As New AddressModel() With {
            .HouseNo = houseNo,
            .Street = StreetTxtbox.Text,
            .Barangay = BarangayTxtbox.Text,
            .City = CityTxtbox.Text,
            .ZipCode = ZipCode,
            .Zone = Zone
        }

        Dim newPatient As New PatientModel() With {
            .UserName = UsernameTextbox.Text,
            .FirstName = FirstnameTxtbox.Text,
            .LastName = LastNameTxtbox.Text,
            .MiddleName = MiddleNameTxtbox.Text,
            .Password = PasswordTextBox.Text,
            .ContactNumber = ContactNumberTxtbox.Text,
            .Email = EmailTxtbox.Text,
            .Address = address,
            .Sex = SexCombobox.SelectedItem.ToString()
        }

        Me.Cursor = Cursors.WaitCursor
        Dim editedInfo As PatientModel = SetupObj()

        Dim ProperModel As PatientModel = If(mode = ModalMode.Add, newPatient, extractedInfo)

        Database.PatientRegFunc(editedInfo, mode)

        Me.Cursor = Cursors.Default
        Dim patientInfoForm As New PatientBasicInformationForm(ProperModel, mode, panelMode)
        patientInfoForm.ReloadResults += ReloadResults
        patientInfoForm.DeletePatientAndReload += DeletePatientAndReload
        patientInfoForm.TopMost = True
        patientInfoForm.Show()
        Me.Hide()
    End Sub

    Private Sub UsernameTxtbox_TextChanged(sender As Object, e As EventArgs) Handles UsernameTextbox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(EmailTxtbox, EmailLabel, "Email")
    End Sub

    Private Sub ContactNumberTxtbox_TextChanged(sender As Object, e As EventArgs) Handles ContactNumberTxtbox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(ContactNumberTxtbox, ContactNumberLabel, "Contact Number")
    End Sub

    Private Sub FirstnameTxtbox_TextChanged(sender As Object, e As EventArgs) Handles FirstnameTxtbox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(FirstnameTxtbox, FirstNameLabel, "First name")
    End Sub

    Private Sub LastNameTxtbox_TextChanged(sender As Object, e As EventArgs) Handles LastNameTxtbox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(LastNameTxtbox, LastNameLabel, "Last name")
    End Sub

    Private Sub HouseNoTxtbox_TextChanged(sender As Object, e As EventArgs) Handles HouseNoTxtbox.TextChanged
        _placeHolderHandler.HandleTextBoxPlaceholder(HouseNoTxtbox, HouseLabel, "House No")
    End Sub

    Private Sub PasswordTextBox_IconRightClick(sender As Object, e As EventArgs) Handles PasswordTextBox.IconRightClick
        If passShow Then
            PasswordTextBox.PasswordChar = ChrW(0)
            PasswordTextBox.IconRight = AdminDoctor_Panel.Resources.hide_password_logo
            passShow = False
        Else
            PasswordTextBox.PasswordChar = "*"
            PasswordTextBox.IconRight = AdminDoctor_Panel.Properties.Resources.show_password_logo
            passShow = True
        End If
    End Sub

End Class
