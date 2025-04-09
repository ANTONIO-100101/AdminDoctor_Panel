Imports Guna.UI2.WinForms
Imports System.Text.RegularExpressions
Imports System.Windows.Forms

Namespace Infocare_Project_1.Classes
    Friend Module InputValidator
        Public Function IsNotEmpty(textBox As Guna2TextBox) As Boolean
            Return Not String.IsNullOrWhiteSpace(textBox.Text)
        End Function

        Public Function IsAlphabetic(textBox As Guna2TextBox) As Boolean
            Dim pattern As String = "^[a-zA-Z\s]+$"
            Return Regex.IsMatch(textBox.Text, pattern)
        End Function

        Public Function IsNumeric(textBox As Guna2TextBox) As Boolean
            Dim pattern As String = "^\d+$"
            Return Regex.IsMatch(textBox.Text, pattern)
        End Function

        Public Function ValidateNotEmpty(textBox As Guna2TextBox, errorMessage As String) As Boolean
            If Not IsNotEmpty(textBox) Then
                textBox.BorderColor = Drawing.Color.Red
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                textBox.Focus()
                Return False
            End If
            ' textBox.BorderColor = Drawing.Color.Green
            Return True
        End Function

        Public Function ValidateAlphabetic(textBox As Guna2TextBox, errorMessage As String) As Boolean
            If Not IsAlphabetic(textBox) Then
                textBox.BorderColor = Drawing.Color.Red
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                textBox.Focus()
                Return False
            End If
            textBox.BorderColor = Drawing.Color.LightBlue
            Return True
        End Function

        Public Function ValidateNumeric(textBox As Guna2TextBox, errorMessage As String) As Boolean
            If Not IsNumeric(textBox) Then
                textBox.BorderColor = Drawing.Color.Red
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                textBox.Focus()
                Return False
            End If
            textBox.BorderColor = Drawing.Color.LightBlue
            Return True
        End Function

        Public Function ValidateAllFieldsFilled(textBoxes() As Guna2TextBox, errorMessage As String) As Boolean
            Dim allFieldsFilled As Boolean = True

            For Each textBox As Guna2TextBox In textBoxes
                If Not IsNotEmpty(textBox) Then
                    textBox.BorderColor = Drawing.Color.Red
                    allFieldsFilled = False
                Else
                    textBox.BorderColor = Drawing.Color.LightBlue
                End If
            Next

            If Not allFieldsFilled Then
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Return allFieldsFilled
        End Function
    End Module
End Namespace
