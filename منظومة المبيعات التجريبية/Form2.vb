Imports System.Data.SqlClient
Imports System.Data
Public Class Form2
    Dim conn As New SqlConnection("Data Source=DESKTOP-A7LUGNG;Initial Catalog=SalesDB;Integrated Security=True")
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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not SalesTools2.IsValidProductRow(TextBox2.Tag, numQty.Value) Then Exit Sub

        Dim pid = TextBox2.Tag
        Dim price = Convert.ToDecimal(numQty.Tag)
        Dim qty = numQty.Value
        Dim subtotal = qty * price

        dgvInvoice.Rows.Add(pid, TextBox2.Text, price, qty, subtotal)


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


    End Sub

    Private Sub dgvInvoice_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvInvoice.CellContentClick

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

        ' التحديث فقط عند تعديل عمود "Quantity"
        If dgvInvoice.Columns(e.ColumnIndex).Name = "Quantity" Then
            Dim row = dgvInvoice.Rows(e.RowIndex)

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

                ' تحديث الإجمالي الجزئي
                row.Cells("SubTotal").Value = qty * price

            Catch
                MessageBox.Show("⚠ خطأ في صيغة الكمية أو السعر.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form3.Show()
        Me.Hide()
    End Sub



End Class