<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DescPrice
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DescPrice))
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        priceTextbox = New Guna.UI2.WinForms.Guna2TextBox()
        guna2ContextMenuStrip1 = New Guna.UI2.WinForms.Guna2ContextMenuStrip()
        guna2PictureBox1 = New Guna.UI2.WinForms.Guna2PictureBox()
        descTextbox = New Guna.UI2.WinForms.Guna2TextBox()
        CType(guna2PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' priceTextbox
        ' 
        priceTextbox.BorderRadius = 5
        priceTextbox.CustomizableEdges = CustomizableEdges1
        priceTextbox.DefaultText = ""
        priceTextbox.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        priceTextbox.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        priceTextbox.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        priceTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        priceTextbox.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        priceTextbox.Font = New Font("Segoe UI", 9F)
        priceTextbox.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        priceTextbox.Location = New Point(140, 4)
        priceTextbox.Name = "priceTextbox"
        priceTextbox.PlaceholderText = ""
        priceTextbox.SelectedText = ""
        priceTextbox.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        priceTextbox.Size = New Size(105, 36)
        priceTextbox.TabIndex = 5
        ' 
        ' guna2ContextMenuStrip1
        ' 
        guna2ContextMenuStrip1.Name = "guna2ContextMenuStrip1"
        guna2ContextMenuStrip1.RenderStyle.ArrowColor = Color.FromArgb(CByte(151), CByte(143), CByte(255))
        guna2ContextMenuStrip1.RenderStyle.BorderColor = Color.Gainsboro
        guna2ContextMenuStrip1.RenderStyle.ColorTable = Nothing
        guna2ContextMenuStrip1.RenderStyle.RoundedEdges = True
        guna2ContextMenuStrip1.RenderStyle.SelectionArrowColor = Color.White
        guna2ContextMenuStrip1.RenderStyle.SelectionBackColor = Color.FromArgb(CByte(100), CByte(88), CByte(255))
        guna2ContextMenuStrip1.RenderStyle.SelectionForeColor = Color.White
        guna2ContextMenuStrip1.RenderStyle.SeparatorColor = Color.Gainsboro
        guna2ContextMenuStrip1.RenderStyle.TextRenderingHint = Drawing.Text.TextRenderingHint.SystemDefault
        guna2ContextMenuStrip1.Size = New Size(61, 4)
        ' 
        ' guna2PictureBox1
        ' 
        guna2PictureBox1.BackgroundImage = CType(resources.GetObject("guna2PictureBox1.BackgroundImage"), Image)
        guna2PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
        guna2PictureBox1.CustomizableEdges = CustomizableEdges3
        guna2PictureBox1.FillColor = Color.Transparent
        guna2PictureBox1.ImageRotate = 0F
        guna2PictureBox1.Location = New Point(252, 11)
        guna2PictureBox1.Name = "guna2PictureBox1"
        guna2PictureBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        guna2PictureBox1.Size = New Size(20, 20)
        guna2PictureBox1.TabIndex = 6
        guna2PictureBox1.TabStop = False
        ' 
        ' descTextbox
        ' 
        descTextbox.BorderRadius = 5
        descTextbox.CustomizableEdges = CustomizableEdges5
        descTextbox.DefaultText = ""
        descTextbox.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        descTextbox.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        descTextbox.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        descTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        descTextbox.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        descTextbox.Font = New Font("Segoe UI", 9F)
        descTextbox.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        descTextbox.Location = New Point(3, 3)
        descTextbox.Name = "descTextbox"
        descTextbox.PlaceholderText = ""
        descTextbox.SelectedText = ""
        descTextbox.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        descTextbox.Size = New Size(130, 36)
        descTextbox.TabIndex = 4
        ' 
        ' DescPrice
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Transparent
        Controls.Add(priceTextbox)
        Controls.Add(guna2PictureBox1)
        Controls.Add(descTextbox)
        Name = "DescPrice"
        Size = New Size(283, 52)
        CType(guna2PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Private WithEvents priceTextbox As Guna.UI2.WinForms.Guna2TextBox
    Private WithEvents guna2ContextMenuStrip1 As Guna.UI2.WinForms.Guna2ContextMenuStrip
    Private WithEvents guna2PictureBox1 As Guna.UI2.WinForms.Guna2PictureBox
    Private WithEvents descTextbox As Guna.UI2.WinForms.Guna2TextBox

End Class
