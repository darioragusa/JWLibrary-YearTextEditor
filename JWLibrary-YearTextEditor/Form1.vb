Imports System
Imports System.IO
Public Class Form1
    Dim JavaScriptPath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Packages"
    Dim JavaScriptText As String
    Const JavaScriptYearText As String = "e.markup"

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ChangeLanguage()
        If IO.Directory.Exists(JavaScriptPath) Then
            For Each Dir As String In Directory.GetDirectories(JavaScriptPath)
                If LCase(Dir).Contains("watchtower") Then
                    JavaScriptPath = Dir & "\LocalState\www\webapp\YearTextDisplay.bundle.js"
                    Exit For
                End If
            Next
        End If
        If Not System.IO.File.Exists(JavaScriptPath) Then
            MsgBox(Strings.Error1Text, MsgBoxStyle.Critical, Strings.Error1Text)
            Return
        End If
        Dim myStream = System.IO.File.OpenText(JavaScriptPath)
        JavaScriptText = myStream.ReadToEnd()
        myStream.Close()

        Dim JavaScriptPart1 = Split(JavaScriptText, "\r\n    font-size: ")(0) & "\r\n    font-size: "
        Dim JavaScriptPart2 = "vw;\r\n    text-align: center;" & Split(Split(JavaScriptText, "\r\n    font-size: ")(1), "vw;\r\n    text-align: center;")(1)
        Dim ActFontSize = JavaScriptText.Replace(JavaScriptPart1, "") : ActFontSize = ActFontSize.Replace(JavaScriptPart2, "")
        NumericUpDown1.Value = Val(ActFontSize)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim PreJavaScript = JavaScriptChangeText(JavaScriptText, TextBox1.Text)
        Dim NewJavaScript = JavaScriptChangeFontSize(PreJavaScript)
        Dim sWriter As New IO.StreamWriter(JavaScriptPath, False)
        sWriter.Write(NewJavaScript)
        sWriter.Close()
    End Sub

    Function JavaScriptChangeText(ByVal JavaScript As String, ByVal Text As String) As String
        Dim VarS As String = "},dangerouslySetInnerHTML:{__html:"
        Dim JavaScriptPart1 As String = Split(JavaScript, VarS)(0) & "},dangerouslySetInnerHTML:{__html:"
        Dim JavaScriptPart2 As String = "}}))}" & Split(Split(JavaScript, VarS)(1), "}}))}")(1)
        If CheckBox1.Checked Then
            JavaScriptChangeText = JavaScriptPart1 & JavaScriptYearText & JavaScriptPart2
        Else
            JavaScriptChangeText = JavaScriptPart1 & """" & CheckText(Text) & """"
            If CheckBox2.Checked Then JavaScriptChangeText &= ".bold()"
            JavaScriptChangeText &= JavaScriptPart2
        End If
    End Function

    Function JavaScriptChangeFontSize(ByVal JavaScript As String) As String
        Dim JavaScriptPart1 = Split(JavaScript, "\r\n    font-size: ")(0) & "\r\n    font-size: "
        Dim JavaScriptPart2 = "vw;\r\n    text-align: center;" & Split(Split(JavaScript, "\r\n    font-size: ")(1), "vw;\r\n    text-align: center;")(1)
        JavaScriptChangeFontSize = JavaScriptPart1 & FormatNumber(NumericUpDown1.Value, 2).Replace(",", ".") & JavaScriptPart2
    End Function

    Function CheckText(ByVal Text As String) As String
        If Text.Contains("""") Then Text = Text.Replace("""", "")
        If Text.Contains(vbCr) Then Text = Text.Replace(vbCr, " <br> ")
        Return Text
    End Function

    Sub ChangeLanguage()
        Dim UserCulture = System.Globalization.CultureInfo.CurrentCulture
        If LCase(UserCulture.ToString).Contains("it") Then
            Strings.Error1Text = "Il file non esiste!"
            Strings.Error1Title = "Errore"
            Strings.CheckBox1Text = "Usa la scrittura dell'anno predefinita"
            Strings.CheckBox2Text = "Grassetto"
            Strings.Label1Text = "Dimensione font:"
            Strings.Button1Text = "Salva"
            Strings.InfoText = "*Per ogni cambiamento è necessario riavviare lo schermo secondario dall'applicazione JW Library"
        End If
        CheckBox1.Text = Strings.CheckBox1Text
        CheckBox2.Text = Strings.CheckBox2Text
        Label1.Text = Strings.Label1Text
        Button1.Text = Strings.Button1Text
        Label2.Text = Strings.InfoText
    End Sub

    Structure Strings
        Shared Error1Text As String = "The file does not exist"
        Shared Error1Title As String = "Error"
        Shared CheckBox1Text As String = "Use default year text"
        Shared CheckBox2Text As String = "Bold"
        Shared Label1Text As String = "Font size:"
        Shared Button1Text As String = "Save"
        Shared InfoText As String = "*For each change it is necessary to restart the secondary monitor from the JW Library application"
    End Structure

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        TextBox1.Enabled = Not CheckBox1.Checked
        CheckBox2.Enabled = Not CheckBox1.Checked
        TextBox1.Text = ""
    End Sub
End Class
