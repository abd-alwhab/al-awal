Public Class Form3

    Private ghk As Integer = 0
    Private mxghk As Integer = 4


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click






        If TextBox1.Text = "عبد الوهاب" And TextBox2.Text = 123456 Then
            Form4.Show()
            Me.Hide()
        ElseIf TextBox1.Text <> "عبد الوهاب" And TextBox2.Text = 123456 Then
            MsgBox("الرجاء التأكد من إسم المشرف")
            TextBox1.BackColor = Color.Red
        ElseIf TextBox1.Text = "عبد الوهاب" And TextBox2.Text <> 123456 Then
            MsgBox("الرجاء التأكد من كلمة المرور")
            TextBox2.BackColor = Color.Red
        ElseIf TextBox1.Text <> "عبد الوهاب" And TextBox2.Text <> 123456 Then
            MsgBox("الرجاء الأكد من البيانات")
            TextBox1.BackColor = Color.Red
            TextBox2.BackColor = Color.Red

        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        TextBox1.Text = ""
        TextBox2.Text = ""

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form2.Show()
        Me.Hide()

    End Sub
End Class