Imports Guna.UI2.WinForms
Imports System
Imports System.Windows.Forms

Namespace Infocare_Project.NewFolder
    Public Class PlaceHolderHandler
        Public Sub HandleTextBoxPlaceholder(textBox As Guna2TextBox, label As Guna2HtmlLabel, placeholderText As String)
            If textBox Is Nothing OrElse label Is Nothing Then
                Throw New ArgumentNullException("TextBox or label cannot be null.")
            End If

            If String.IsNullOrWhiteSpace(textBox.Text) Then
                label.Visible = False
                textBox.PlaceholderText = placeholderText
            Else
                label.Visible = True
                textBox.PlaceholderText = String.Empty
            End If
        End Sub
    End Class
End Namespace
