<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ResetPassword
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        components = New ComponentModel.Container()
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        guna2BorderlessForm1 = New Guna.UI2.WinForms.Guna2BorderlessForm(components)
        passValidatorMsg = New Guna.UI2.WinForms.Guna2HtmlLabel()
        showPass = New Guna.UI2.WinForms.Guna2CheckBox()
        savePassBtn = New Guna.UI2.WinForms.Guna2Button()
        confirmpassTextbox = New Guna.UI2.WinForms.Guna2TextBox()
        newpassTextbox = New Guna.UI2.WinForms.Guna2TextBox()
        guna2HtmlLabel2 = New Guna.UI2.WinForms.Guna2HtmlLabel()
        guna2HtmlLabel1 = New Guna.UI2.WinForms.Guna2HtmlLabel()
        SuspendLayout()
        ' 
        ' guna2BorderlessForm1
        ' 
        guna2BorderlessForm1.BorderRadius = 10
        guna2BorderlessForm1.ContainerControl = Me
        guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6R
        guna2BorderlessForm1.TransparentWhileDrag = True
        ' 
        ' passValidatorMsg
        ' 
        passValidatorMsg.BackColor = Color.Transparent
        passValidatorMsg.Font = New Font("Segoe UI", 8.25F, FontStyle.Bold)
        passValidatorMsg.ForeColor = Color.Red
        passValidatorMsg.Location = New Point(95, 175)
        passValidatorMsg.Margin = New Padding(3, 2, 3, 2)
        passValidatorMsg.Name = "passValidatorMsg"
        passValidatorMsg.Size = New Size(138, 15)
        passValidatorMsg.TabIndex = 202
        passValidatorMsg.Text = "*At least 8 characters long"
        passValidatorMsg.Visible = False
        ' 
        ' showPass
        ' 
        showPass.AutoSize = True
        showPass.CheckedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        showPass.CheckedState.BorderRadius = 0
        showPass.CheckedState.BorderThickness = 0
        showPass.CheckedState.FillColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        showPass.Location = New Point(95, 269)
        showPass.Name = "showPass"
        showPass.Size = New Size(113, 19)
        showPass.TabIndex = 201
        showPass.Text = "Show Passwords"
        showPass.UncheckedState.BorderColor = Color.FromArgb(CByte(125), CByte(137), CByte(149))
        showPass.UncheckedState.BorderRadius = 0
        showPass.UncheckedState.BorderThickness = 0
        showPass.UncheckedState.FillColor = Color.FromArgb(CByte(125), CByte(137), CByte(149))
        ' 
        ' savePassBtn
        ' 
        savePassBtn.BackColor = Color.Transparent
        savePassBtn.BorderRadius = 5
        savePassBtn.CustomizableEdges = CustomizableEdges1
        savePassBtn.DisabledState.BorderColor = Color.DarkGray
        savePassBtn.DisabledState.CustomBorderColor = Color.DarkGray
        savePassBtn.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        savePassBtn.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        savePassBtn.FillColor = Color.MidnightBlue
        savePassBtn.Font = New Font("Segoe UI", 9F)
        savePassBtn.ForeColor = Color.White
        savePassBtn.Location = New Point(167, 309)
        savePassBtn.Name = "savePassBtn"
        savePassBtn.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        savePassBtn.Size = New Size(180, 45)
        savePassBtn.TabIndex = 200
        savePassBtn.Text = "Save Password"
        ' 
        ' confirmpassTextbox
        ' 
        confirmpassTextbox.BorderColor = SystemColors.ControlDarkDark
        confirmpassTextbox.BorderRadius = 5
        confirmpassTextbox.CustomizableEdges = CustomizableEdges3
        confirmpassTextbox.DefaultText = ""
        confirmpassTextbox.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        confirmpassTextbox.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        confirmpassTextbox.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        confirmpassTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        confirmpassTextbox.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        confirmpassTextbox.Font = New Font("Segoe UI", 9F)
        confirmpassTextbox.ForeColor = SystemColors.ActiveCaptionText
        confirmpassTextbox.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        confirmpassTextbox.Location = New Point(95, 205)
        confirmpassTextbox.Margin = New Padding(3, 4, 3, 4)
        confirmpassTextbox.Name = "confirmpassTextbox"
        confirmpassTextbox.PasswordChar = "●"c
        confirmpassTextbox.PlaceholderForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        confirmpassTextbox.PlaceholderText = "Confirm Password"
        confirmpassTextbox.SelectedText = ""
        confirmpassTextbox.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        confirmpassTextbox.Size = New Size(336, 42)
        confirmpassTextbox.TabIndex = 199
        ' 
        ' newpassTextbox
        ' 
        newpassTextbox.BorderColor = SystemColors.ControlDarkDark
        newpassTextbox.BorderRadius = 5
        newpassTextbox.CustomizableEdges = CustomizableEdges5
        newpassTextbox.DefaultText = ""
        newpassTextbox.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        newpassTextbox.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        newpassTextbox.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        newpassTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        newpassTextbox.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        newpassTextbox.Font = New Font("Segoe UI", 9F)
        newpassTextbox.ForeColor = SystemColors.ActiveCaptionText
        newpassTextbox.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        newpassTextbox.Location = New Point(95, 128)
        newpassTextbox.Margin = New Padding(3, 4, 3, 4)
        newpassTextbox.Name = "newpassTextbox"
        newpassTextbox.PasswordChar = "●"c
        newpassTextbox.PlaceholderForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        newpassTextbox.PlaceholderText = "New Password"
        newpassTextbox.SelectedText = ""
        newpassTextbox.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        newpassTextbox.Size = New Size(336, 42)
        newpassTextbox.TabIndex = 198
        ' 
        ' guna2HtmlLabel2
        ' 
        guna2HtmlLabel2.BackColor = Color.Transparent
        guna2HtmlLabel2.Font = New Font("Segoe UI", 12F)
        guna2HtmlLabel2.Location = New Point(130, 69)
        guna2HtmlLabel2.Name = "guna2HtmlLabel2"
        guna2HtmlLabel2.Size = New Size(267, 23)
        guna2HtmlLabel2.TabIndex = 197
        guna2HtmlLabel2.Text = "Set the new password of your account"
        ' 
        ' guna2HtmlLabel1
        ' 
        guna2HtmlLabel1.BackColor = Color.Transparent
        guna2HtmlLabel1.Font = New Font("Segoe UI", 18F, FontStyle.Bold)
        guna2HtmlLabel1.Location = New Point(143, 29)
        guna2HtmlLabel1.Name = "guna2HtmlLabel1"
        guna2HtmlLabel1.Size = New Size(243, 34)
        guna2HtmlLabel1.TabIndex = 196
        guna2HtmlLabel1.Text = "Reset Your Password"
        ' 
        ' ResetPassword
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(519, 394)
        Controls.Add(passValidatorMsg)
        Controls.Add(showPass)
        Controls.Add(savePassBtn)
        Controls.Add(confirmpassTextbox)
        Controls.Add(newpassTextbox)
        Controls.Add(guna2HtmlLabel2)
        Controls.Add(guna2HtmlLabel1)
        FormBorderStyle = FormBorderStyle.None
        Margin = New Padding(3, 2, 3, 2)
        Name = "ResetPassword"
        StartPosition = FormStartPosition.CenterScreen
        Text = "ResetPassword"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private WithEvents guna2BorderlessForm1 As Guna.UI2.WinForms.Guna2BorderlessForm
    Private WithEvents passValidatorMsg As Guna.UI2.WinForms.Guna2HtmlLabel
    Private WithEvents showPass As Guna.UI2.WinForms.Guna2CheckBox
    Private WithEvents savePassBtn As Guna.UI2.WinForms.Guna2Button
    Private WithEvents confirmpassTextbox As Guna.UI2.WinForms.Guna2TextBox
    Private WithEvents newpassTextbox As Guna.UI2.WinForms.Guna2TextBox
    Private WithEvents guna2HtmlLabel2 As Guna.UI2.WinForms.Guna2HtmlLabel
    Private WithEvents guna2HtmlLabel1 As Guna.UI2.WinForms.Guna2HtmlLabel
End Class
