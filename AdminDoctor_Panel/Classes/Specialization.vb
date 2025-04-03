Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Linq

Namespace Infocare_Project.NewFolder
    Friend Class Specialization
        Public Class DoctorSpecialization
            Public Property Specializations As List(Of String)

            Public Sub New()
                Specializations = New List(Of String) From {
                    "Specialization", "Cardiology", "Dermatology", "Neurology", "Orthopedics", "Pediatrics", "Psychiatry",
                    "Ophthalmology", "Obstetrics and Gynecology", "Endocrinology", "Gastroenterology",
                    "Hematology", "Oncology", "Pulmonology", "Nephrology", "Urology", "General Surgery",
                    "Plastic Surgery", "Radiology", "Anesthesiology", "Internal Medicine", "Family Medicine",
                    "Emergency Medicine", "Rheumatology", "Pathology", "Vascular Surgery", "Rehabilitation Medicine",
                    "Geriatrics", "Sports Medicine", "Infectious Disease", "Palliative Care"
                }
            End Sub

            Public Sub ConfigureSearchableComboBox(comboBox As ComboBox)
                comboBox.DropDownStyle = ComboBoxStyle.DropDown
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend
                comboBox.AutoCompleteSource = AutoCompleteSource.ListItems

                comboBox.ForeColor = Color.FromArgb(47, 89, 114)
                comboBox.Font = New Font("Segoe UI", 9.0F)
            End Sub
        End Class
    End Class
End Namespace
