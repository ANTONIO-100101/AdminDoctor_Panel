Imports AdminDoctor_Panel
Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    Partial Friend Class MyApplication
        <Global.System.Diagnostics.DebuggerStepThroughAttribute()>
        Public Sub New()
            MyBase.New(Global.Microsoft.VisualBasic.ApplicationServices.AuthenticationMode.Windows)
            Me.IsSingleInstance = False
            Me.EnableVisualStyles = True
            Me.SaveMySettingsOnExit = True
            Me.ShutdownStyle = Global.Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses
        End Sub

        <Global.System.Diagnostics.DebuggerStepThroughAttribute()>
        Protected Overrides Sub OnCreateMainForm()
            Me.MainForm = LandingPage
        End Sub
    End Class
End Namespace
