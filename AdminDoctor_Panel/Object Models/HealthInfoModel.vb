Imports AdminDoctor_Panel.Infocare_Project_1.Object_Models.Interfaces
Imports Infocare_Project_1.Object_Models.Interfaces

Namespace Infocare_Project_1.Object_Models
    ''' <summary>
    ''' HealthInfoModel Class that stores Health-Related Details of a Patient.
    ''' </summary>
    Public Class HealthInfoModel
        Implements IHealthInfo

        Public Property Height As Double Implements IHealthInfo.Height
        Public Property Weight As Double Implements IHealthInfo.Weight
        Public Property BMI As Double Implements IHealthInfo.BMI
        Public Property BloodType As String Implements IHealthInfo.BloodType
        Public Property PreCon As String Implements IHealthInfo.PreCon
        Public Property Treatment As String Implements IHealthInfo.Treatment
        Public Property PrevSurg As String Implements IHealthInfo.PrevSurg
        Public Property Alergy As String Implements IHealthInfo.Alergy
        Public Property Medication As String Implements IHealthInfo.Medication

    End Class
End Namespace
