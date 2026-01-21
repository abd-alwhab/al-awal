'Public Class FormReview

'End Class


Imports System.Data.SqlClient

Public Class FormReview

    ' 🔍 البحث عن فاتورة
    Private Sub btnSearchInvoice_Click(sender As Object, e As EventArgs) Handles btnSearchInvoice.Click
        If txtInvoiceID.Text.Trim = "" Then
            MessageBox.Show("يرجى إدخال رقم الفاتورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim invoiceID As Integer
        If Not Integer.TryParse(txtInvoiceID.Text, invoiceID) Then
            MessageBox.Show("رقم الفاتورة يجب أن يكون رقماً صحيحًا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 🧾 استعلام لتفاصيل الفاتورة
        Dim query As String = "
            SELECT P.ProductName AS [المنتج], D.Quantity AS [الكمية], D.SubTotal AS [الإجمالي الجزئي]
            FROM InvoiceDetails D
            JOIN Products P ON D.ProductID = P.ProductID
            WHERE D.InvoiceID = @id
        "

        Dim dt As New DataTable()
        Dim cmd As New SqlCommand(query, SalesTools2.conn)
        cmd.Parameters.AddWithValue("@id", invoiceID)
        Dim da As New SqlDataAdapter(cmd)
        da.Fill(dt)

        dgvReview.DataSource = dt

        ' SalesTools2.conn.Open()
        ' Dim total = totalCmd.ExecuteScalar()
        'SalesTools2.conn.Close()
        ' 💰 جلب الإجمالي الكامل للفاتورة
        'Dim totalCmd As New SqlCommand("SELECT TotalAmount FROM Invoices WHERE InvoiceID = @id", SalesTools2.conn)
        'totalCmd.Parameters.AddWithValue("@id", invoiceID)
        'Dim total = totalCmd.ExecuteScalar()

        'If total IsNot Nothing Then
        'lblTotalAmount.Text = "إجمالي الفاتورة: " & FormatCurrency(total)
        'Else
        'MessageBox.Show("⚠️ لم يتم العثور على الفاتورة.", "غير موجود", MessageBoxButtons.OK, MessageBoxIcon.Information)
        'dgvReview.DataSource = Nothing
        'lblTotalAmount.Text = ""
        'End If

        Dim totalCmd As New SqlCommand("SELECT TotalAmount FROM Invoices WHERE InvoiceID = @id", SalesTools2.conn)
        totalCmd.Parameters.AddWithValue("@id", invoiceID)

        SalesTools2.conn.Open() ' ✅ فتح الاتصال
        Dim total = totalCmd.ExecuteScalar()
        SalesTools2.conn.Close() ' ✅ إغلاق الاتصال بعد التنفيذ

        If total IsNot Nothing Then
            lblTotalAmount.Text = "الفاتورة"
        Else
            MessageBox.Show("في الفاتورة لا يوجد")
            dgvReview.DataSource = Nothing
            lblTotalAmount.Text = ""
        End If





    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form4.Show()
        Me.Hide()

    End Sub


    Private Sub dgvReview_SelectionChanged(sender As Object, e As EventArgs) Handles dgvReview.SelectionChanged
        If dgvReview.CurrentRow Is Nothing Then Exit Sub

        ' الحصول على معرف الفاتورة
        Dim invoiceObj = dgvReview.CurrentRow.Cells("InvoiceID").Value

        If invoiceObj Is Nothing OrElse IsDBNull(invoiceObj) Then
            lblTotalAmount.Text = "⚠ رقم الفاتورة غير متوفر"
            Exit Sub
        End If

        Dim invoiceID As Integer
        If Not Integer.TryParse(invoiceObj.ToString(), invoiceID) Then
            lblTotalAmount.Text = "⚠ رقم الفاتورة غير صالح"
            Exit Sub
        End If

        ' جلب الإجمالي من قاعدة البيانات
        Dim cmd As New SqlCommand("SELECT TotalAmount FROM Invoices WHERE InvoiceID = @id", SalesTools2.conn)
        cmd.Parameters.AddWithValue("@id", invoiceID)

        SalesTools2.conn.Open()
        Dim totalObj = cmd.ExecuteScalar()
        SalesTools2.conn.Close()

        If totalObj IsNot Nothing AndAlso Not IsDBNull(totalObj) Then
            Dim total As Decimal = Convert.ToDecimal(totalObj)
            lblTotalAmount.Text = $"إجمالي الفاتورة: {total:N2} دينار"
        Else
            lblTotalAmount.Text = "⚠ لا يوجد إجمالي لهذه الفاتورة"
        End If
    End Sub



End Class