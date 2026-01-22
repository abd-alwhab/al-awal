Imports System.Data.SqlClient
Imports System.Data
Public Class Form2
    Dim conn As New SqlConnection("Data Source=DESKTOP-A7LUGNG;Initial Catalog=SalesDB;Integrated Security=True")


    Private Sub Add1(ctrl As Control, msg As String)
        ToolTip1.SetToolTip(ctrl, msg)
        ToolTip1.AutoPopDelay = 10000
        ToolTip1.InitialDelay = 500
        ToolTip1.ReshowDelay = 200
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click



        If dgvInvoice.CurrentRow Is Nothing Then
            MessageBox.Show("⚠ يرجى تحديد صف داخل الجدول.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim row = dgvInvoice.CurrentRow

        Try
            Dim qty = Convert.ToInt32(row.Cells("Quantity").Value)
            Dim price = Convert.ToDecimal(row.Cells("Price").Value)
            row.Cells("SubTotal").Value = qty * price

            MessageBox.Show("✅ تم تحديث الإجمالي بنجاح.", "تحديث", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("⚠ تأكد من إدخال قيم صحيحة في الكمية والسعر.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click




        Dim productId As Integer
        Dim productName As String
        Dim productPrice As Decimal
        Dim qty As Integer

        ' الحالة الأولى: البحث بالاسم في TextBox2
        If Not String.IsNullOrWhiteSpace(TextBox2.Text) Then
            Using conn As New SqlConnection("Data Source=DESKTOP-A7LUGNG;Initial Catalog=SalesDB;Integrated Security=True")
                conn.Open()
                Dim cmd As New SqlCommand("SELECT TOP 1 * FROM Products WHERE ProductName LIKE @name", conn)
                cmd.Parameters.AddWithValue("@name", "%" & TextBox2.Text & "%")
                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        productId = Convert.ToInt32(reader("ProductID"))
                        productName = reader("ProductName").ToString()
                        productPrice = Convert.ToDecimal(reader("Price"))
                    Else
                        MessageBox.Show("⚠ المنتج غير موجود بالبحث النصي.")
                        Exit Sub
                    End If
                End Using
            End Using

            ' الكمية باستخدام NumericUpDown
            qty = Convert.ToInt32(numQty.Value)
            If qty <= 0 Then
                MessageBox.Show("⚠ أدخل كمية صحيحة أكبر من صفر.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

        Else
            ' الحالة الثانية: اختيار المنتج من DataGridView1
            If DataGridView1.CurrentRow Is Nothing Then
                MessageBox.Show("⚠ يرجى اختيار منتج من القائمة أو كتابة اسمه.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            productId = Convert.ToInt32(DataGridView1.CurrentRow.Cells("رقم المنتج").Value)
            productName = DataGridView1.CurrentRow.Cells("اسم المنتج").Value.ToString()
            productPrice = Convert.ToDecimal(DataGridView1.CurrentRow.Cells("السعر").Value)

            ' الكمية باستخدام InputBox
            Dim qtyInput As String = InputBox("أدخل الكمية للمنتج: " & productName, "إدخال الكمية")
            If Not Integer.TryParse(qtyInput, qty) OrElse qty <= 0 Then
                MessageBox.Show("⚠ الكمية غير صحيحة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        End If

        ' حساب الإجمالي الجزئي
        Dim subtotal As Decimal = qty * productPrice

        ' إضافة إلى الفاتورة
        dgvInvoice.Rows.Add(productId, productName, productPrice, qty, subtotal)

        ' تحديث الإجمالي الكلي مباشرة
        Dim total As Decimal = 0
        For Each row As DataGridViewRow In dgvInvoice.Rows
            If row.IsNewRow Then Continue For
            total += Convert.ToDecimal(row.Cells("SubTotal").Value)
        Next
        Label3.Text = "الإجمالي الكلي: " & FormatCurrency(total)

        MessageBox.Show("✅ تمت إضافة المنتج '" & productName & "' بالكمية " & qty & " إلى الفاتورة.")

        TextBox2.Text = ""

    End Sub




    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles numQty.ValueChanged

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        conn.Open()
        Dim cmd As New SqlCommand("SELECT TOP 1 * FROM Products WHERE ProductName LIKE @name", conn)
        cmd.Parameters.AddWithValue("@name", "%" & TextBox2.Text & "%")
        Dim reader = cmd.ExecuteReader()
        If reader.Read() Then
            TextBox2.Tag = reader("ProductID")
            numQty.Tag = reader("Price")
            MessageBox.Show("تم العثور: السعر " & reader("Price"))
        Else
            MessageBox.Show("المنتج غير موجود")
        End If
        reader.Close()
        conn.Close()

    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click




        Try
            ' فتح الاتصال
            If SalesTools2.conn.State <> ConnectionState.Open Then SalesTools2.conn.Open()

            ' حفظ الفاتورة الأساسية
            Dim total As Decimal = SalesTools2.CalculateInvoiceTotal(dgvInvoice)
            Dim cmd As New SqlCommand("INSERT INTO Invoices (InvoiceDate, TotalAmount) VALUES (@d, @t); SELECT SCOPE_IDENTITY()", SalesTools2.conn)
            cmd.Parameters.AddWithValue("@d", DateTime.Now)
            cmd.Parameters.AddWithValue("@t", total)
            Dim invoiceID As Integer = Convert.ToInt32(cmd.ExecuteScalar())

            ' معالجة كل صف داخل الفاتورة
            For Each row As DataGridViewRow In dgvInvoice.Rows
                If row.IsNewRow Then Continue For

                ' التحقق من وجود القيم
                If row.Cells("ProductID").Value Is Nothing OrElse
               row.Cells("Quantity").Value Is Nothing OrElse
               row.Cells("SubTotal").Value Is Nothing Then
                    MessageBox.Show("⚠ صف يحتوي على بيانات ناقصة. لن يتم حفظه.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Continue For
                End If

                ' تحويل القيم بشكل آمن
                Dim pid As Integer = Convert.ToInt32(row.Cells("ProductID").Value)
                Dim qty As Integer = Convert.ToInt32(row.Cells("Quantity").Value)
                Dim subtotal As Decimal = Convert.ToDecimal(row.Cells("SubTotal").Value)

                ' جلب الكمية المتوفرة من قاعدة البيانات
                Dim checkCmd As New SqlCommand("SELECT Stock FROM Products WHERE ProductID = @pid", SalesTools2.conn)
                checkCmd.Parameters.AddWithValue("@pid", pid)
                Dim availableStock As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                ' تحقق من توفر الكمية
                If qty > availableStock Then
                    MessageBox.Show($"❌ لا يمكن خصم الكمية المطلوبة للمنتج رقم {pid}. الكمية المتوفرة هي {availableStock}.", "نقص في المخزون", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Continue For
                End If

                ' حفظ تفاصيل الفاتورة
                Dim cmdDetail As New SqlCommand("INSERT INTO InvoiceDetails (InvoiceID, ProductID, Quantity, SubTotal) VALUES (@i, @p, @q, @s)", SalesTools2.conn)
                cmdDetail.Parameters.AddWithValue("@i", invoiceID)
                cmdDetail.Parameters.AddWithValue("@p", pid)
                cmdDetail.Parameters.AddWithValue("@q", qty)
                cmdDetail.Parameters.AddWithValue("@s", subtotal)
                cmdDetail.ExecuteNonQuery()

                ' تحديث المخزون
                Dim cmdStock As New SqlCommand("UPDATE Products SET Stock = Stock - @qty WHERE ProductID = @pid", SalesTools2.conn)
                cmdStock.Parameters.AddWithValue("@qty", qty)
                cmdStock.Parameters.AddWithValue("@pid", pid)
                cmdStock.ExecuteNonQuery()

                ' رسالة لكل بند ناجح
                MessageBox.Show($"✔ تم حفظ المنتج رقم {pid} وخصم {qty} من المخزون.", "نجاح جزئي", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Next

            ' تأكيد حفظ الفاتورة كاملة
            SalesTools2.ShowInvoiceSavedMessage(invoiceID)

            ' إغلاق الاتصال
            SalesTools2.conn.Close()

        Catch ex As Exception
            MessageBox.Show("⚠ حدث خطأ أثناء حفظ الفاتورة: " & ex.Message, "خطأ عام", MessageBoxButtons.OK, MessageBoxIcon.Error)
            If SalesTools2.conn.State = ConnectionState.Open Then SalesTools2.conn.Close()
        End Try



        Dim totpl As Decimal = 0

        For Each row As DataGridViewRow In dgvInvoice.Rows
            If row.IsNewRow Then Continue For

            Dim subtotalValue = row.Cells("SubTotal").Value
            If subtotalValue IsNot Nothing Then
                Try
                    totpl += Convert.ToDecimal(subtotalValue)
                Catch
                    ' تجاهل القيم غير الصالحة
                End Try
            End If
        Next

        ' عرض الناتج في Label أو TextBox
        Label3.Text = "الإجمالي الكلي: " & FormatCurrency(totpl)







    End Sub


    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label1.Text = DateString
        Label2.Text = TimeString
        Add1(btnAdd, "إضغط على هذا الزر لإضافة المنتج المكتوب
في مربع النص الخاص بإسم
المنتج إلى الفاتورة")
        Add1(Button2, "إضغط هذا الزر لحفظ التغييرات 
في الفاتورة بعد تغييرها 
مباشرة في شاشة العرض و لتحديث بيانات العرض")

        Add1(Button7, "إضغط هذا الزر لحفظ الفاتورة 
و إضافتها إلى قاعدة البيانات ")
        Add1(Button6, "   إضغط على هذا الزر للبحث عن 
المنتج المكتوب في مربع النص الخاص 
بإسم المنتج للتحقق من
و جوده أو عدمه و بيانات المنتج")
        Add1(Button3, " إضغط هذا الزر لحذف المنتج المحدد من الفاتورة")
        Add1(Button4, " إضغط هذا الزر لإلغاء الفاتورة بالكامل")
        Add1(Button5, "الضغط على هذا الزر يقود لإعدادات 
المنظومة بالكامل و هو مخصص
للمسؤل حيث يوجد كلمة مرور")
        Add1(numQty, "لإختار الكمية المطلوبة")
        Add1(TextBox2, "لإذخال إسم المنتج المراد إضافته
إلى الفاتورة أو البحث عنه")

        If dgvInvoice.Columns.Count = 0 Then
            dgvInvoice.Columns.Add("ProductID", "رقم المنتج")
            dgvInvoice.Columns.Add("ProductName", "اسم المنتج")
            dgvInvoice.Columns.Add("Price", "السعر")
            dgvInvoice.Columns.Add("Quantity", "الكمية")
            dgvInvoice.Columns.Add("SubTotal", "الإجمالي الجزئي")
        End If


        Dim dt As New DataTable()
        Dim query As String = "SELECT ProductID AS [رقم المنتج], ProductName AS [اسم المنتج], Price AS [السعر], Stock AS [المخزون] FROM Products"
        Dim da As New SqlDataAdapter(query, conn)
        da.Fill(dt)
        DataGridView1.DataSource = dt
        dgvInvoice.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray
        DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

        dgvInvoice.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvInvoice.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        dgvInvoice.Font = New Font("Tahoma", 12, FontStyle.Bold)
        dgvInvoice.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 11, FontStyle.Bold)

        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        DataGridView1.Font = New Font("Tahoma", 12, FontStyle.Bold)
        DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 11, FontStyle.Bold)




    End Sub

    Private Sub dgvInvoice_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvInvoice.CellContentClick
        dgvInvoice.Font = New Font("Tahoma", 10, FontStyle.Bold)










    End Sub

    'Private Sub dgvInvoice_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvInvoice.CellEndEdit
    'Dim row = dgvInvoice.Rows(e.RowIndex)

    ' تحقق من أن الصف صالح
    'If row.IsNewRow Then Exit Sub

    ' حساب الإجمالي بعد التعديل
    'Try
    'Dim qty = Convert.ToInt32(row.Cells("Quantity").Value)
    'Dim price = Convert.ToDecimal(row.Cells("Price").Value)
    '       row.Cells("SubTotal").Value = qty * price
    'Catch
    '       MessageBox.Show("⚠ تأكد من إدخال قيم صحيحة للكمية والسعر.", "خطأ في التعديل", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    'End Try
    'End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If dgvInvoice.SelectedRows.Count = 0 Then
            MessageBox.Show("يرجى تحديد صف للحذف.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim confirm = MessageBox.Show("هل تريد حذف هذا البند فعلاً؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            dgvInvoice.Rows.RemoveAt(dgvInvoice.SelectedRows(0).Index)
        End If

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim confirm = MessageBox.Show("هل أنت متأكد من إلغاء الفاتورة؟ سيتم مسح جميع البنود.", "تأكيد الإلغاء", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If confirm = DialogResult.Yes Then
            dgvInvoice.Rows.Clear()
            MessageBox.Show("✅ تم إلغاء الفاتورة بنجاح.", "إلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub


    Private Sub dgvInvoice_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvInvoice.CellValueChanged



        ' التحقق من أن الصف صالح وليس صف جديد
        If e.RowIndex < 0 OrElse dgvInvoice.Rows(e.RowIndex).IsNewRow Then Exit Sub

        Dim row = dgvInvoice.Rows(e.RowIndex)

        ' إذا تم تعديل عمود الكمية أو السعر
        If dgvInvoice.Columns(e.ColumnIndex).Name = "Quantity" OrElse dgvInvoice.Columns(e.ColumnIndex).Name = "Price" Then
            Try
                Dim qtyText = row.Cells("Quantity").Value?.ToString()
                Dim priceText = row.Cells("Price").Value?.ToString()

                ' التحقق من أن القيم غير فارغة وصحيحة
                If String.IsNullOrWhiteSpace(qtyText) OrElse String.IsNullOrWhiteSpace(priceText) Then Exit Sub

                Dim qty = Convert.ToInt32(qtyText)
                Dim price = Convert.ToDecimal(priceText)

                If qty <= 0 Then
                    MessageBox.Show("⚠ الكمية يجب أن تكون أكبر من صفر.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                ' تحديث الإجمالي الجزئي للصف
                row.Cells("SubTotal").Value = qty * price

                ' تحديث الإجمالي الكلي لكل الفاتورة
                Dim total As Decimal = 0
                For Each r As DataGridViewRow In dgvInvoice.Rows
                    If r.IsNewRow Then Continue For
                    If r.Cells("SubTotal").Value IsNot Nothing Then
                        total += Convert.ToDecimal(r.Cells("SubTotal").Value)
                    End If
                Next
                Label3.Text = "الإجمالي الكلي: " & FormatCurrency(total)

            Catch
                MessageBox.Show("⚠ خطأ في صيغة الكمية أو السعر.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End If
        DataGridView1.Font = New Font("Tahoma", 10, FontStyle.Bold)



    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form3.Show()
        Me.Hide()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        DataGridView1.Font = New Font("Tahoma", 10, FontStyle.Bold)
    End Sub
    Private Sub LoadStock()



        Dim dt As New DataTable()
        Dim cmd As New SqlCommand("SELECT * FROM Products", SalesTools2.conn)
        Dim da As New SqlDataAdapter(cmd)
        da.Fill(dt)

        ' ربط البيانات
        dgvInvoice.DataSource = dt
        DataGridView1.DataSource = dt

        ' مرونة الأعمدة والصفوف + تكبير الخط

        ' تلوين الصفوف بالتبادل


    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub
End Class