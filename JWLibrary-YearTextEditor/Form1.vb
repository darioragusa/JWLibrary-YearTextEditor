Imports System
Imports System.IO
Public Class Form1
    Dim JsonPath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Packages"
    Dim JsonText As String
    Const JsonYearText As String = "e.markup"

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ChangeLanguage()
        If IO.Directory.Exists(JsonPath) Then
            For Each Dir As String In Directory.GetDirectories(JsonPath)
                If LCase(Dir).Contains("watchtower") Then
                    JsonPath = Dir & "\LocalState\www\webapp\YearTextDisplay.bundle.js"
                    Exit For
                End If
            Next
        End If
        If Not System.IO.File.Exists(JsonPath) Then
            MsgBox(Strings.Error1Text, MsgBoxStyle.Critical, Strings.Error1Text)
            Return
        End If
        Dim myStream = System.IO.File.OpenText(JsonPath)
        JsonText = myStream.ReadToEnd()
        myStream.Close()

        Dim JsonPart1 = Split(JsonText, "\r\n    font-size: ")(0) & "\r\n    font-size: "
        Dim JsonPart2 = "vw;\r\n    text-align: center;" & Split(Split(JsonText, "\r\n    font-size: ")(1), "vw;\r\n    text-align: center;")(1)
        Dim ActFontSize = JsonText.Replace(JsonPart1, "") : ActFontSize = ActFontSize.Replace(JsonPart2, "")
        NumericUpDown1.Value = Val(ActFontSize)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim PreJson = JsonChangeText(JsonText, TextBox1.Text)
        Dim NewJson = JsonChangeFontSize(PreJson)
        Dim sWriter As New IO.StreamWriter(JsonPath, False)
        sWriter.Write(NewJson)
        sWriter.Close()
    End Sub

    Function JsonChangeText(ByVal Json As String, ByVal Text As String) As String
        Dim VarS As String = "},dangerouslySetInnerHTML:{__html:"
        Dim JsonPart1 As String = Split(Json, VarS)(0) & "},dangerouslySetInnerHTML:{__html:"
        Dim JsonPart2 As String = "}}))}" & Split(Split(Json, VarS)(1), "}}))}")(1)
        If CheckBox1.Checked Then
            JsonChangeText = JsonPart1 & JsonYearText & JsonPart2
        Else
            JsonChangeText = JsonPart1 & """" & CheckText(Text) & """"
            If CheckBox2.Checked Then JsonChangeText &= ".bold()"
            JsonChangeText &= JsonPart2
        End If
    End Function

    Function JsonChangeFontSize(ByVal Json As String) As String
        Dim JsonPart1 = Split(Json, "\r\n    font-size: ")(0) & "\r\n    font-size: "
        Dim JsonPart2 = "vw;\r\n    text-align: center;" & Split(Split(Json, "\r\n    font-size: ")(1), "vw;\r\n    text-align: center;")(1)
        JsonChangeFontSize = JsonPart1 & FormatNumber(NumericUpDown1.Value, 2).Replace(",", ".") & JsonPart2
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
