Partial Public Class DescPrice
    Inherits UserControl

    Public Event RemoveTile(ByVal sender As DescPrice)

    Public Sub New()
        InitializeComponent()
    End Sub

    Public ReadOnly Property Desc As String
        Get
            Return descTextbox.Text
        End Get
    End Property

    Private Sub guna2PictureBox1_Click(sender As Object, e As EventArgs) Handles guna2PictureBox1.Click
        RaiseEvent RemoveTile(Me)
    End Sub
End Class
