Imports AdminDoctor_Panel.Infocare_Project_1
Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models
Imports MySql.Data.MySqlClient
Imports System
Imports System.Windows.Forms
Imports System.Xml

Partial Public Class PatientBasicInformationForm
    Inherits Form

    Private patient As PatientModel
    Private mode As ModalMode
    Public ReloadResults As Action
    Public DeletePatientAndReload As Action
    Private panelMode As PanelMode

    Public Sub New(ByVal patient As PatientModel, ByVal mode As ModalMode, ByVal panelMode As PanelMode)
        InitializeComponent()
        Me.mode = mode
        Me.patient = patient
        Me.panelMode = panelMode
        AddHandler HeightTextBox.TextChanged, AddressOf HeightOrWeightTextChanged
        AddHandler WeightTextBox.TextChanged, AddressOf HeightOrWeightTextChanged

        Dim bloodtypes As String() = {"Select BloodType", "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"}
        BloodTypeComboBox.DataSource = bloodtypes

        If mode = ModalMode.Edit Then
            DeleteBtn.Visible = (panelMode = PanelMode.AdminDoc)
        End If
    End Sub

    Private Sub LoadPatientName()
        Dim fullName As String = Database.GetPatientName(patient)
        If Not String.IsNullOrEmpty(fullName) Then
            NameLabel.Text = fullName
        Else
            NameLabel.Text = "No data found."
        End If
    End Sub

    Private Sub HeightOrWeightTextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim heightCm As Double = If(String.IsNullOrWhiteSpace(HeightTextBox.Text), 0, Convert.ToDouble(HeightTextBox.Text) / 100)
            Dim weight As Double = If(String.IsNullOrWhiteSpace(WeightTextBox.Text), 0, Convert.ToDouble(WeightTextBox.Text))

            If heightCm > 0 AndAlso weight > 0 Then
                Dim bmi As Double = weight / (heightCm * heightCm)
                BmiTextBox.Text = bmi.ToString("F2")
            Else
                BmiTextBox.Clear()
            End If
        Catch ex As Exception
            MessageBox.Show("Error calculating BMI: " & ex.Message)
        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim confirm As DialogResult = MessageBox.Show($"Are you sure you want to {(If(mode = ModalMode.Add, "cancel registration", "close"))}?", "Cancel registration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

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

    Private Sub RegisterButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        If Not ProcessMethods.IsValidTextInput(AlergyTextbox.Text) Then
            MessageBox.Show("Alergy field must contain only letters, spaces, or 'N/A'.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim height As Double = If(String.IsNullOrWhiteSpace(HeightTextBox.Text), 0, Convert.ToDouble(HeightTextBox.Text))
        Dim weight As Double = If(String.IsNullOrWhiteSpace(WeightTextBox.Text), 0, Convert.ToDouble(WeightTextBox.Text))
        Dim bmi As Double = If(String.IsNullOrWhiteSpace(BmiTextBox.Text), 0, Convert.ToDouble(BmiTextBox.Text))
        Dim bloodType As String = If(BloodTypeComboBox.SelectedItem IsNot Nothing, BloodTypeComboBox.SelectedItem.ToString(), String.Empty)
        Dim preCon As String = If(String.IsNullOrEmpty(preConditionTextBox.Text), String.Empty, preConditionTextBox.Text)
        Dim treatment As String = If(String.IsNullOrEmpty(TreatmentTextBox.Text), String.Empty, TreatmentTextBox.Text)
        Dim prevSurg As String = If(String.IsNullOrEmpty(PreviousSurgeryTextBox.Text), String.Empty, PreviousSurgeryTextBox.Text)
        Dim allergy As String = If(String.IsNullOrEmpty(AlergyTextbox.Text), String.Empty, AlergyTextbox.Text)
        Dim medication As String = If(String.IsNullOrEmpty(MedicationTxtbox.Text), String.Empty, MedicationTxtbox.Text)

        Dim healthInfo As New HealthInfoModel() With {
        .Height = height,
        .Weight = weight,
        .BMI = bmi,
        .BloodType = bloodType
    }

        patient.HealthInfo = healthInfo
        Dim editedInfo As PatientModel = SetupInfo()

        Database.PatientRegFunc(patient, patient.UserName, height, weight, bmi, bloodType, preCon, treatment, prevSurg, allergy, medication, mode)

        Dim emergencyRegistration As New EmergencyRegistration(patient, mode, panelMode)
        emergencyRegistration.ReloadResults = ReloadResults
        emergencyRegistration.DeletePatientAndReload = DeletePatientAndReload
        emergencyRegistration.TopMost = True
        emergencyRegistration.Show()
        Me.Hide()
    End Sub


    Public Function SetupInfo() As PatientModel
        Dim editedInfo As PatientModel = patient
        Dim healthInfo As New HealthInfoModel()

        healthInfo.Weight = Double.Parse(WeightTextBox.Text)
        healthInfo.Height = Double.Parse(HeightTextBox.Text)
        healthInfo.BMI = Double.Parse(BmiTextBox.Text)
        healthInfo.BloodType = BloodTypeComboBox.SelectedItem.ToString()

        editedInfo.HealthInfo = healthInfo
        Return editedInfo
    End Function

    Private Sub MinimizeButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub BackButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim confirm As DialogResult = DialogResult.Cancel

        If mode = ModalMode.Add Then
            confirm = MessageBox.Show("Are you sure you want to go back? Your progress will be lost.", "Back to Page 1", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        End If

        If confirm = DialogResult.Yes OrElse mode = ModalMode.Edit Then
            Try
                If mode = ModalMode.Add Then
                    Database.DeletePatientReg1Data(patient)
                End If

                Dim patientInfoForm As New PatientRegisterForm(mode, patient.AccountID, panelMode)
                patientInfoForm.Show()
                Me.Hide()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Public Sub FillUpFields()
        HeightTextBox.Text = patient.HealthInfo.Height.ToString()
        WeightTextBox.Text = patient.HealthInfo.Weight.ToString()
        BmiTextBox.Text = patient.HealthInfo.BMI.ToString()
        BloodTypeComboBox.SelectedItem = If(String.IsNullOrEmpty(patient.HealthInfo.BloodType), "Select BloodType", patient.HealthInfo.BloodType)
    End Sub

    Private Sub PatientBasicInformationForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        LoadPatientName()
        If mode = ModalMode.Edit Then
            FillUpFields()
        End If
    End Sub

    Private Sub DeleteBtn_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim isDelete As DialogResult = MessageBox.Show("Are you sure you want to delete this information?", "Patient Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If isDelete = DialogResult.No Then Return
        DeletePatientAndReload.Invoke()
        Me.Hide()
    End Sub
End Class