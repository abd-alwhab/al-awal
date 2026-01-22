Imports System.Data.SqlClient
Imports System.Data
Public Class Form4
    Dim conn As New SqlConnection("Data Source=DESKTOP-A7LUGNG;Initial Catalog=SalesDB;Integrated Security=True")
    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


        If dgvStock.Rows.Count = 0 Then
            MessageBox.Show("⚠️ لا توجد بيانات للإضافة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim addedCount As Integer = 0

        For Each row As DataGridViewRow In dgvStock.Rows
            If row.IsNewRow Then Continue For

            Dim prodNameObj = row.Cells("ProductName").Value
            Dim serialObj = row.Cells("SerialNumber").Value
            Dim stockObj = row.Cells("Stock").Value
            Dim priceObj = row.Cells("Price").Value

            If prodNameObj Is Nothing OrElse serialObj Is Nothing OrElse stockObj Is Nothing OrElse priceObj Is Nothing Then Continue For

            Dim prodName = prodNameObj.ToString().Trim()
            Dim serial = serialObj.ToString().Trim()
            Dim stock As Integer = 0
            Dim price As Decimal = 0

            If prodName = "" OrElse serial = "" Then Continue For
            If Not Integer.TryParse(stockObj.ToString(), stock) Then Continue For
            If Not Decimal.TryParse(priceObj.ToString(), price) Then Continue For

            ' تحقق من وجود المنتج مسبقًا
            Dim checkCmd As New SqlCommand("SELECT COUNT(*) FROM Products WHERE SerialNumber = @s", SalesTools2.conn)
            checkCmd.Parameters.AddWithValue("@s", serial)
            SalesTools2.conn.Open()
            Dim exists As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
            SalesTools2.conn.Close()

            If exists > 0 Then Continue For

            ' تنفيذ الإدخال الكامل
            Dim insertCmd As New SqlCommand("INSERT INTO Products (ProductName, SerialNumber, Stock, Price) VALUES (@n, @s, @stk, @p)", SalesTools2.conn)
            insertCmd.Parameters.AddWithValue("@n", prodName)
            insertCmd.Parameters.AddWithValue("@s", serial)
            insertCmd.Parameters.AddWithValue("@stk", stock)
            insertCmd.Parameters.AddWithValue("@p", price)

            SalesTools2.conn.Open()
            insertCmd.ExecuteNonQuery()
            SalesTools2.conn.Close()

            addedCount += 1
        Next

        If addedCount > 0 Then
            MessageBox.Show($"✅ تم إضافة {addedCount} منتج/منتجات جديدة.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("⚠️ جميع الصفوف مكررة أو غير مكتملة ولم تتم إضافتها.", "ملاحظة", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        LoadStock()


    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If dgvStock.CurrentRow Is Nothing Then
            MessageBox.Show("⚠ يرجى تحديد المنتج من الجدول.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim pid = dgvStock.CurrentRow.Cells("ProductID").Value
        Dim confirm = MessageBox.Show("هل تريد حذف المنتج فعلاً؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If confirm = DialogResult.Yes Then
            Dim cmd As New SqlCommand("DELETE FROM Products WHERE ProductID = @id", SalesTools2.conn)
            cmd.Parameters.AddWithValue("@id", pid)

            SalesTools2.conn.Open()
            cmd.ExecuteNonQuery()
            SalesTools2.conn.Close()

            MessageBox.Show("🗑️ تم حذف المنتج.")
            LoadStock()
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click


        If dgvStock.CurrentRow Is Nothing Then
            MessageBox.Show("⚠ يرجى تحديد صف معدل من الجدول.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim row = dgvStock.CurrentRow

        ' قراءة البيانات من الصف المحدد
        Dim pidObj = row.Cells("ProductID").Value
        Dim nameObj = row.Cells("ProductName").Value
        Dim serialObj = row.Cells("SerialNumber").Value
        Dim stockObj = row.Cells("Stock").Value
        Dim priceObj = row.Cells("Price").Value

        ' التحقق من أن القيم موجودة
        If IsDBNull(pidObj) OrElse IsDBNull(nameObj) OrElse IsDBNull(serialObj) _
           OrElse IsDBNull(stockObj) OrElse IsDBNull(priceObj) Then
            MessageBox.Show("⚠ البيانات غير مكتملة داخل الصف.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim pid As Integer
        If Not Integer.TryParse(pidObj.ToString(), pid) Then
            MessageBox.Show("⚠ رقم المنتج غير صالح.", "خطأ في المعرف", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim name As String = nameObj.ToString().Trim()
        Dim serial As String = serialObj.ToString().Trim()
        Dim stock As Integer
        Dim price As Decimal

        If name = "" OrElse serial = "" Then
            MessageBox.Show("⚠ لا يمكن ترك اسم المنتج أو الرقم التسلسلي فارغًا.", "خطأ في البيانات", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If Not Integer.TryParse(stockObj.ToString(), stock) Then
            MessageBox.Show("⚠ الكمية يجب أن تكون رقمًا صحيحًا.", "خطأ في الكمية", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If Not Decimal.TryParse(priceObj.ToString(), price) Then
            MessageBox.Show("⚠ السعر يجب أن يكون رقمًا صالحًا.", "خطأ في السعر", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' تنفيذ التعديل في قاعدة البيانات
        Dim cmd As New SqlCommand("UPDATE Products SET ProductName = @n, SerialNumber = @s, Stock = @stk, Price = @p WHERE ProductID = @id", SalesTools2.conn)
        cmd.Parameters.AddWithValue("@n", name)
        cmd.Parameters.AddWithValue("@s", serial)
        cmd.Parameters.AddWithValue("@stk", stock)
        cmd.Parameters.AddWithValue("@p", price)
        cmd.Parameters.AddWithValue("@id", pid)

        SalesTools2.conn.Open()
        cmd.ExecuteNonQuery()
        SalesTools2.conn.Close()

        MessageBox.Show("✏️ تم تعديل المنتج بالكامل بنجاح.")
        LoadStock()
        DbHelper.LoadData("SELECT ProductID, ProductName, Price, StockQuantity FROM Products", dgvStock, Nothing)


    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles txtProductName.TextChanged

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim keyword = txtProductName.Text.Trim

        Dim dt As New DataTable()
        Dim cmd As New SqlCommand("SELECT * FROM Products WHERE ProductName LIKE '%' + @kw + '%'", SalesTools2.conn)
        cmd.Parameters.AddWithValue("@kw", keyword)

        Dim da As New SqlDataAdapter(cmd)
        da.Fill(dt)

        dgvStock.DataSource = dt

        lblStatus.Text = $"🔍 عدد النتائج: {dt.Rows.Count}"

    End Sub

    Private Sub LoadStock()
        Dim dt As New DataTable()
        Dim cmd As New SqlCommand("SELECT * FROM Products", SalesTools2.conn)
        Dim da As New SqlDataAdapter(cmd)
        da.Fill(dt)
        dgvStock.DataSource = dt
        dgvStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvStock.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        dgvStock.Font = New Font("Tahoma", 10, FontStyle.Bold)
        dgvStock.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray
    End Sub

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadStock()
        Label1.Text = DateString
        Label2.Text = TimeString
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        FormReview.Show()
        Me.Hide()
    End Sub

    Private Sub dgvStock_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvStock.CellContentClick
        dgvStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvStock.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
    End Sub
End Class