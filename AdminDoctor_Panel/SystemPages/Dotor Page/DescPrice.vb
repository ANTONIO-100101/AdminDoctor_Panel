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

    Public ReadOnly Property Price As Decimal
        Get
            Return Decimal.Parse(priceTextbox.Text)
        End Get
    End Property

    Private Sub DescPrice_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    End Sub

    Private Sub guna2TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles priceTextbox.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then
            e.Handled = True
        End If
    End Sub

    Private Sub guna2PictureBox1_Click(sender As Object, e As EventArgs) Handles guna2PictureBox1.Click
        RaiseEvent RemoveTile(Me)
    End Sub
End Class
