<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class EmailUsernameInput
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges7 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EmailUsernameInput))
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        guna2BorderlessForm1 = New Guna.UI2.WinForms.Guna2BorderlessForm(components)
        submitBtn = New Guna.UI2.WinForms.Guna2Button()
        emailTextbox = New Guna.UI2.WinForms.Guna2TextBox()
        usernameTextbox = New Guna.UI2.WinForms.Guna2TextBox()
        guna2HtmlLabel1 = New Guna.UI2.WinForms.Guna2HtmlLabel()
        closeBtn = New Guna.UI2.WinForms.Guna2ImageButton()
        SuspendLayout()
        ' 
        ' guna2BorderlessForm1
        ' 
        guna2BorderlessForm1.BorderRadius = 10
        guna2BorderlessForm1.ContainerControl = Me
        guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6R
        guna2BorderlessForm1.TransparentWhileDrag = True
        ' 
        ' submitBtn
        ' 
        submitBtn.BorderRadius = 5
        submitBtn.CustomizableEdges = CustomizableEdges2
        submitBtn.DisabledState.BorderColor = Color.DarkGray
        submitBtn.DisabledState.CustomBorderColor = Color.DarkGray
        submitBtn.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        submitBtn.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        submitBtn.FillColor = Color.MidnightBlue
        submitBtn.Font = New Font("Segoe UI Semibold", 9.75F, FontStyle.Bold)
        submitBtn.ForeColor = Color.White
        submitBtn.Location = New Point(128, 333)
        submitBtn.Margin = New Padding(3, 4, 3, 4)
        submitBtn.Name = "submitBtn"
        submitBtn.ShadowDecoration.CustomizableEdges = CustomizableEdges3
        submitBtn.Size = New Size(206, 60)
        submitBtn.TabIndex = 8
        submitBtn.Text = "Submit"
        ' 
        ' emailTextbox
        ' 
        emailTextbox.BorderColor = SystemColors.ControlDarkDark
        emailTextbox.BorderRadius = 5
        emailTextbox.CustomizableEdges = CustomizableEdges4
        emailTextbox.DefaultText = ""
        emailTextbox.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        emailTextbox.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        emailTextbox.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        emailTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        emailTextbox.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        emailTextbox.Font = New Font("Segoe UI", 9F)
        emailTextbox.ForeColor = SystemColors.ActiveCaptionText
        emailTextbox.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        emailTextbox.Location = New Point(57, 233)
        emailTextbox.Margin = New Padding(3, 5, 3, 5)
        emailTextbox.Name = "emailTextbox"
        emailTextbox.PlaceholderForeColor = SystemColors.ControlDarkDark
        emailTextbox.PlaceholderText = "Enter Your Email"
        emailTextbox.SelectedText = ""
        emailTextbox.ShadowDecoration.CustomizableEdges = CustomizableEdges5
        emailTextbox.Size = New Size(361, 59)
        emailTextbox.TabIndex = 7
        ' 
        ' usernameTextbox
        ' 
        usernameTextbox.BorderColor = SystemColors.ControlDarkDark
        usernameTextbox.BorderRadius = 5
        usernameTextbox.CustomizableEdges = CustomizableEdges6
        usernameTextbox.DefaultText = ""
        usernameTextbox.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        usernameTextbox.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        usernameTextbox.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        usernameTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        usernameTextbox.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        usernameTextbox.Font = New Font("Segoe UI", 9F)
        usernameTextbox.ForeColor = SystemColors.ActiveCaptionText
        usernameTextbox.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        usernameTextbox.Location = New Point(57, 146)
        usernameTextbox.Margin = New Padding(3, 5, 3, 5)
        usernameTextbox.Name = "usernameTextbox"
        usernameTextbox.PlaceholderForeColor = SystemColors.ControlDarkDark
        usernameTextbox.PlaceholderText = "Enter Your Username"
        usernameTextbox.SelectedText = ""
        usernameTextbox.ShadowDecoration.CustomizableEdges = CustomizableEdges7
        usernameTextbox.Size = New Size(361, 59)
        usernameTextbox.TabIndex = 6
        ' 
        ' guna2HtmlLabel1
        ' 
        guna2HtmlLabel1.BackColor = Color.Transparent
        guna2HtmlLabel1.Font = New Font("Segoe UI Semibold", 18F, FontStyle.Bold)
        guna2HtmlLabel1.Location = New Point(116, 65)
        guna2HtmlLabel1.Margin = New Padding(3, 4, 3, 4)
        guna2HtmlLabel1.Name = "guna2HtmlLabel1"
        guna2HtmlLabel1.Size = New Size(251, 43)
        guna2HtmlLabel1.TabIndex = 5
        guna2HtmlLabel1.Text = "Input Your Details"
        ' 
        ' closeBtn
        ' 
        closeBtn.CheckedState.ImageSize = New Size(64, 64)
        closeBtn.HoverState.ImageSize = New Size(64, 64)
        closeBtn.Image = CType(resources.GetObject("closeBtn.Image"), Image)
        closeBtn.ImageOffset = New Point(0, 0)
        closeBtn.ImageRotate = 0F
        closeBtn.ImageSize = New Size(40, 40)
        closeBtn.Location = New Point(421, 12)
        closeBtn.Name = "closeBtn"
        closeBtn.PressedState.ImageSize = New Size(64, 64)
        closeBtn.ShadowDecoration.CustomizableEdges = CustomizableEdges1
        closeBtn.Size = New Size(57, 54)
        closeBtn.TabIndex = 9
        ' 
        ' EmailUsernameInput
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(490, 417)
        Controls.Add(closeBtn)
        Controls.Add(submitBtn)
        Controls.Add(emailTextbox)
        Controls.Add(usernameTextbox)
        Controls.Add(guna2HtmlLabel1)
        FormBorderStyle = FormBorderStyle.None
        Name = "EmailUsernameInput"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Form1"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private WithEvents guna2BorderlessForm1 As Guna.UI2.WinForms.Guna2BorderlessForm
    Private WithEvents submitBtn As Guna.UI2.WinForms.Guna2Button
    Private WithEvents emailTextbox As Guna.UI2.WinForms.Guna2TextBox
    Private WithEvents usernameTextbox As Guna.UI2.WinForms.Guna2TextBox
    Private WithEvents guna2HtmlLabel1 As Guna.UI2.WinForms.Guna2HtmlLabel
    Friend WithEvents closeBtn As Guna.UI2.WinForms.Guna2ImageButton

End Class
