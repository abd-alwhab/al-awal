Imports System.Data.SqlClient

Module DbHelper





    ' دالة عامة لتحميل أي بيانات في جريدفيو
    Public Sub LoadData(query As String, dgv As DataGridView, params As Dictionary(Of String, Object))
        Try
            Using conn As New SqlConnection(SalesTools2.conn.ConnectionString)
                Using cmd As New SqlCommand(query, conn)

                    ' إضافة الباراميترات إذا موجودة
                    If params IsNot Nothing Then
                        For Each kvp In params
                            cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                        Next
                    End If

                    Dim adapter As New SqlDataAdapter(cmd)
                    Dim table As New DataTable()
                    adapter.Fill(table)

                    dgv.DataSource = table
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("❌ خطأ أثناء تحميل البيانات: " & ex.Message)
        End Try
    End Sub

    ' دالة عامة لتنفيذ أوامر SQL (إضافة/حذف/تعديل)
    Public Sub ExecuteNonQuery(query As String, params As Dictionary(Of String, Object))
        Try
            Using conn As New SqlConnection(SalesTools2.conn.ConnectionString)
                Using cmd As New SqlCommand(query, conn)

                    For Each kvp In params
                        cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                    Next

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("❌ خطأ أثناء التنفيذ: " & ex.Message)
        End Try
    End Sub

End Module
