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

End Class
