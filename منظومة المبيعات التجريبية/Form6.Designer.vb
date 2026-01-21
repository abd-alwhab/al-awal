<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormReview
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnSearchInvoice = New System.Windows.Forms.Button()
        Me.txtInvoiceID = New System.Windows.Forms.TextBox()
        Me.dgvReview = New System.Windows.Forms.DataGridView()
        Me.lblTotalAmount = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        CType(Me.dgvReview, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnSearchInvoice
        '
        Me.btnSearchInvoice.BackColor = System.Drawing.SystemColors.MenuHighlight
        Me.btnSearchInvoice.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSearchInvoice.Location = New System.Drawing.Point(977, 61)
        Me.btnSearchInvoice.Name = "btnSearchInvoice"
        Me.btnSearchInvoice.Size = New System.Drawing.Size(157, 44)
        Me.btnSearchInvoice.TabIndex = 0
        Me.btnSearchInvoice.Text = "بحث"
        Me.btnSearchInvoice.UseVisualStyleBackColor = False
        '
        'txtInvoiceID
        '
        Me.txtInvoiceID.Location = New System.Drawing.Point(684, 83)
        Me.txtInvoiceID.Name = "txtInvoiceID"
        Me.txtInvoiceID.Size = New System.Drawing.Size(248, 22)
        Me.txtInvoiceID.TabIndex = 1
        '
        'dgvReview
        '
        Me.dgvReview.AllowUserToOrderColumns = True
        Me.dgvReview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvReview.Location = New System.Drawing.Point(12, 13)
        Me.dgvReview.Name = "dgvReview"
        Me.dgvReview.RowHeadersWidth = 51
        Me.dgvReview.RowTemplate.Height = 24
        Me.dgvReview.Size = New System.Drawing.Size(616, 510)
        Me.dgvReview.TabIndex = 2
        '
        'lblTotalAmount
        '
        Me.lblTotalAmount.AutoSize = True
        Me.lblTotalAmount.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalAmount.Location = New System.Drawing.Point(678, 468)
        Me.lblTotalAmount.Name = "lblTotalAmount"
        Me.lblTotalAmount.Size = New System.Drawing.Size(107, 32)
        Me.lblTotalAmount.TabIndex = 3
        Me.lblTotalAmount.Text = "Label1"
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.DarkOrange
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(1037, 347)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(105, 57)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "رجوع"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Red
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.Location = New System.Drawing.Point(1037, 436)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(105, 64)
        Me.Button2.TabIndex = 5
        Me.Button2.Text = "خروج"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'FormReview
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1181, 557)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lblTotalAmount)
        Me.Controls.Add(Me.dgvReview)
        Me.Controls.Add(Me.txtInvoiceID)
        Me.Controls.Add(Me.btnSearchInvoice)
        Me.Name = "FormReview"
        Me.Text = "Form6"
        CType(Me.dgvReview, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnSearchInvoice As Button
    Friend WithEvents txtInvoiceID As TextBox
    Friend WithEvents dgvReview As DataGridView
    Friend WithEvents lblTotalAmount As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
End Class
