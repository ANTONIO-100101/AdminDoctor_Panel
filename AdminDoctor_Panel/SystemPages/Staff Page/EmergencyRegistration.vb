Imports AdminDoctor_Panel.Infocare_Project.NewFolder
Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models

Public Class EmergencyRegistration
    Inherits Form

    Private _placeHolderHandler As PlaceHolderHandler
    Public ReloadResults As Action
    Public DeletePatientAndReload As Action
    Private patient As PatientModel
    Private mode As ModalMode
    Private panelMode As PanelMode

    Public Sub New(patient As PatientModel, mode As ModalMode, panelMode As PanelMode)
        InitializeComponent()
        Me.mode = mode
        Me.panelMode = panelMode
        _placeHolderHandler = New PlaceHolderHandler()
        NameLabel.Text = $"{patient.LastName}, {patient.FirstName}"
        Me.patient = patient

        If mode = ModalMode.Edit Then
            DeleteBtn.Visible = panelMode = PanelMode.AdminDoc
            RegisterButton.Text = "Update"
        Else
            DeleteBtn.Visible = False
            RegisterButton.Text = "Register"
        End If
    End Sub

    Private Sub EmergencyRegistration_Load(sender As Object, e As EventArgs)
        LoadPatientName()
    End Sub
    Private Sub LoadPatientName()
        Dim fullName As String = Database.GetPatientName(patient)

        If Not String.IsNullOrEmpty(fullName) Then
            NameLabel.Text = fullName
        Else
            NameLabel.Text = "No data found."
        End If
    End Sub

    Public Function SetupInfo() As EmergencyContactModel
        Dim addressInfo As New AddressModel() With {
            .HouseNo = Integer.Parse(HouseNoTxtbox.Text),
            .ZipCode = Integer.Parse(ZipCodeTxtbox.Text),
            .Zone = Integer.Parse(ZoneTxtbox.Text),
            .Barangay = BarangayTxtbox.Text,
            .City = CityTxtbox.Text,
            .Street = StreetTxtbox.Text
        }

        Return New EmergencyContactModel() With {
            .FirstName = FirstnameTxtbox.Text,
            .LastName = LastNameTxtbox.Text,
            .MiddleName = MiddleNameTxtbox.Text,
            .Suffix = SuffixTxtbox.Text,
            .Address = addressInfo
        }
    End Function

    Private Sub RegisterButton_Click(sender As Object, e As EventArgs) Handles RegisterButton.Click
        'VALIDATION

        If Not FirstnameTxtbox.Text.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c)) AndAlso Not String.IsNullOrEmpty(FirstnameTxtbox.Text) Then
            MessageBox.Show("First name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not LastNameTxtbox.Text.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c)) AndAlso Not String.IsNullOrEmpty(LastNameTxtbox.Text) Then
            MessageBox.Show("Last name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not MiddleNameTxtbox.Text.All(Function(c) Char.IsLetter(c) OrElse Char.IsWhiteSpace(c) OrElse c = "/" AndAlso Not String.IsNullOrEmpty(MiddleNameTxtbox.Text)) Then
            MessageBox.Show("Middle name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim validSuffixes As String() = {"Jr.", "Sr.", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "Jr", "Sr", "N/A"}
        Dim enteredText As String = SuffixTxtbox.Text.Trim()

        ' Check if the entered text is one of the valid suffixes (case-insensitive)
        If Not String.IsNullOrEmpty(enteredText) AndAlso Not validSuffixes.Any(Function(suffix) String.Equals(suffix, enteredText, StringComparison.OrdinalIgnoreCase)) Then
            MessageBox.Show("Please enter a valid suffix (e.g., Jr., Sr., I, II, III, IV, etc.).", "Invalid Suffix", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Declare ZipCode and Zone outside the block to ensure they are accessible later
        Dim zipCode As Integer
        Dim zone As Integer

        If Not Integer.TryParse(ZipCodeTxtbox.Text, zipCode) Then
            MessageBox.Show("Please enter a valid number for Zip Code.")
            Return
        End If

        If Not Integer.TryParse(ZoneTxtbox.Text, zone) Then
            MessageBox.Show("Please enter a valid number for Zone.")
            Return
        End If

        Try
            Dim firstname As String = FirstnameTxtbox.Text.Trim()
            Dim middlename As String = MiddleNameTxtbox.Text.Trim()
            Dim lastname As String = LastNameTxtbox.Text.Trim()
            Dim suffix As String = SuffixTxtbox.Text.Trim()

            Dim housenum As Integer = If(String.IsNullOrWhiteSpace(HouseNoTxtbox.Text), 0, Convert.ToInt32(HouseNoTxtbox.Text))
            Dim street As String = If(String.IsNullOrWhiteSpace(StreetTxtbox.Text), String.Empty, StreetTxtbox.Text.Trim())
            Dim barangay As String = If(String.IsNullOrWhiteSpace(BarangayTxtbox.Text), String.Empty, BarangayTxtbox.Text.Trim())
            Dim city As String = If(String.IsNullOrWhiteSpace(CityTxtbox.Text), String.Empty, CityTxtbox.Text.Trim())
            Dim address As New AddressModel() With {
            .HouseNo = housenum,
            .Street = street,
            .Barangay = barangay,
            .City = city,
            .ZipCode = If(String.IsNullOrWhiteSpace(ZipCodeTxtbox.Text), 0, Convert.ToInt32(ZipCodeTxtbox.Text)),
            .Zone = If(String.IsNullOrWhiteSpace(ZoneTxtbox.Text), 0, Convert.ToInt32(ZoneTxtbox.Text))
        }
            Dim emergencyContact As New EmergencyContactModel() With {
            .FirstName = firstname,
            .MiddleName = middlename,
            .LastName = lastname,
            .Suffix = suffix,
            .Address = address
        }

            Dim category As String = If(mode = ModalMode.Add, "submit", "update")
            Dim YesNO As DialogResult = MessageBox.Show($"Are you sure to {category}?", $"{category} information", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If YesNO = DialogResult.Yes Then
                Try
                    Database.PatientRegFunc(emergencyContact, patient.UserName, firstname, lastname, middlename, suffix, housenum, street, barangay, city, If(String.IsNullOrWhiteSpace(ZipCodeTxtbox.Text), 0, Convert.ToInt32(ZipCodeTxtbox.Text)), If(String.IsNullOrWhiteSpace(ZoneTxtbox.Text), 0, Convert.ToInt32(ZoneTxtbox.Text)), mode)

                    MessageBox.Show(If(mode = Global.AdminDoctor_Panel.Infocare_Project_1.ModalMode.Add, "Submit Successfully", "Information Updated"))
                    Me.Hide()

                    ' Refresh the patient list
                    If mode = ModalMode.Edit Then
                        ReloadResults.Invoke()
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error deleting data: " & ex.Message)
                Finally
                    Me.Close()
                End Try
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub


    Private Sub FirstnameTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(FirstnameTxtbox, FirstNameLabel, "Firstname")
    End Sub

    Private Sub LastNameTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(LastNameTxtbox, LastNameLabel, "Lastname")
    End Sub

    Private Sub MiddleNameTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(MiddleNameTxtbox, MiddleNameLabel, "Middlename")
    End Sub

    Private Sub SuffixTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(SuffixTxtbox, SuffixLabel, "Suffix")
    End Sub

    Private Sub HouseNoTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(HouseNoTxtbox, HouseLabel, "House No.")
    End Sub

    Private Sub ZipCodeTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(ZipCodeTxtbox, ZipCodeLabel, "Zip code")
    End Sub

    Private Sub ZoneTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(ZoneTxtbox, ZoneLabel, "Zone")
    End Sub

    Private Sub StreetTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(StreetTxtbox, StreetLabel, "Street")
    End Sub

    Private Sub BarangayTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(BarangayTxtbox, BarangayLabel, "Barangay")
    End Sub

    Private Sub CityTxtbox_TextChanged(sender As Object, e As EventArgs)
        _placeHolderHandler.HandleTextBoxPlaceholder(CityTxtbox, CityLabel, "City")
    End Sub

    Private Sub MinimizeButton_Click(sender As Object, e As EventArgs)
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub BackButton_Click(sender As Object, e As EventArgs) Handles BackButton.Click
        Dim confirm As DialogResult = DialogResult.Cancel
        If mode = ModalMode.Add Then
            confirm = MessageBox.Show("Are you sure you want to go back? Your progress will be lost.", "Back to Page 2", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        End If

        If confirm = DialogResult.Yes OrElse mode = ModalMode.Edit Then
            Try
                If mode = ModalMode.Add Then
                    Database.NullPatientReg2Data(patient.UserName)
                End If

                Dim patientInfoForm As New PatientBasicInformationForm(patient, mode, panelMode)
                patientInfoForm.Show()
                Me.Hide()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim confirm As DialogResult = MessageBox.Show("Are you sure to cancel " & If(mode = ModalMode.Add, "registration", "close") & "?", "Cancel registration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirm = DialogResult.Yes Then
            Try
                If mode = ModalMode.Add Then
                    Database.DeletePatientByUsername(patient.UserName)
                    MessageBox.Show("Your data has been deleted.")
                End If
            Catch ex As Exception
                MessageBox.Show("Error deleting data: " & ex.Message)
            Finally
                Me.Close()
            End Try
        End If
    End Sub

    Private Sub FillUpFields()
        FirstnameTxtbox.Text = patient.EmergencyContact.FirstName
        LastNameTxtbox.Text = patient.EmergencyContact.LastName
        MiddleNameTxtbox.Text = patient.EmergencyContact.MiddleName
        SuffixTxtbox.Text = patient.EmergencyContact.Suffix

        'Address
        HouseNoTxtbox.Text = patient.EmergencyContact.Address.HouseNo.ToString()
        ZipCodeTxtbox.Text = patient.EmergencyContact.Address.ZipCode.ToString()
        ZoneTxtbox.Text = patient.EmergencyContact.Address.Zone.ToString()
        BarangayTxtbox.Text = patient.EmergencyContact.Address.Barangay
        CityTxtbox.Text = patient.EmergencyContact.Address.City
        StreetTxtbox.Text = patient.EmergencyContact.Address.Street
    End Sub

    Private Sub ExitButton_Click_1(sender As Object, e As EventArgs) Handles ExitButton.Click
        Dim YesNO As DialogResult = MessageBox.Show("Are you sure to cancel registration?", "Cancel registration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If YesNO = DialogResult.Yes Then
            Try
                Database.DeletePatientByUsername(patient.UserName)
                MessageBox.Show("Your data has been deleted.")
            Catch ex As Exception
                MessageBox.Show("Error deleting data: " & ex.Message)
            Finally
                Me.Close()
            End Try
        End If
    End Sub
End Class