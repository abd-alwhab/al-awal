
Imports System.Data.SqlClient

Module SalesTools2




    ' 🔌 الاتصال بقاعدة البيانات
    Public conn As New SqlConnection("Data Source=DESKTOP-A7LUGNG;Initial Catalog=SalesDB;Integrated Security=True")

    ' 🔄 توليد رقم وهمي للفاتورة (اختياري - لأغراض العرض أو التجربة)
    Public Function GenerateFakeInvoiceNumber() As String
            Return "INV-" & DateTime.Now.ToString("yyyyMMddHHmmss")
        End Function

        ' 📊 حساب إجمالي الفاتورة من DataGridView
        Public Function CalculateInvoiceTotal(dgv As DataGridView) As Decimal
            Dim total As Decimal = 0
            For Each row As DataGridViewRow In dgv.Rows
                If Not row.IsNewRow Then
                    total += Convert.ToDecimal(row.Cells("SubTotal").Value)
                End If
            Next
            Return total
        End Function

        ' ✅ التحقق من صحة المنتج والكمية قبل الإضافة
        Public Function IsValidProductRow(productID As Object, quantity As Integer) As Boolean
            If productID Is Nothing OrElse quantity <= 0 Then
                MessageBox.Show("يرجى تحديد منتج صالح وكمية أكبر من 0.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
            Return True
        End Function

        ' 🧾 عرض رسالة تأكيد بعد حفظ الفاتورة
        Public Sub ShowInvoiceSavedMessage(invoiceID As Integer)
            MessageBox.Show("✅ تم حفظ الفاتورة بنجاح. رقم الفاتورة: " & invoiceID, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        ' 🔎 استعلام منتج واحد حسب الاسم
        Public Function GetProductByName(name As String) As DataRow
            Dim dt As New DataTable()
            Dim cmd As New SqlCommand("SELECT TOP 1 * FROM Products WHERE ProductName LIKE @name", conn)
            cmd.Parameters.AddWithValue("@name", "%" & name & "%")
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count > 0 Then
                Return dt.Rows(0)
            Else
                Return Nothing
            End If
        End Function

        ' 📦 تحميل المخزون في DataGridView
        Public Sub LoadStock(dgv As DataGridView)
            Dim dt As New DataTable()
            Dim cmd As New SqlCommand("SELECT ProductID AS [رقم المنتج], ProductName AS [اسم المنتج], Price AS [السعر], Stock AS [المخزون] FROM Products", conn)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
            dgv.DataSource = dt
        End Sub


End Module
