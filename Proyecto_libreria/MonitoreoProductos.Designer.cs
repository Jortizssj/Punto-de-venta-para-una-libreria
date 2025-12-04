namespace Proyecto_libreria
{
    partial class MonitoreoProductos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.dataGridViewAuditoria = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAuditoria)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRefrescar
            // 
            this.btnRefrescar.Location = new System.Drawing.Point(658, 63);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(117, 48);
            this.btnRefrescar.TabIndex = 0;
            this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.UseVisualStyleBackColor = true;
            this.btnRefrescar.Click += new System.EventHandler(this.btnRefrescar_Click);
            // 
            // dataGridViewAuditoria
            // 
            this.dataGridViewAuditoria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAuditoria.Location = new System.Drawing.Point(50, 149);
            this.dataGridViewAuditoria.Name = "dataGridViewAuditoria";
            this.dataGridViewAuditoria.RowHeadersWidth = 51;
            this.dataGridViewAuditoria.RowTemplate.Height = 24;
            this.dataGridViewAuditoria.Size = new System.Drawing.Size(779, 324);
            this.dataGridViewAuditoria.TabIndex = 1;
            // 
            // MonitoreoProductos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 508);
            this.Controls.Add(this.dataGridViewAuditoria);
            this.Controls.Add(this.btnRefrescar);
            this.Name = "MonitoreoProductos";
            this.Text = "MonitoreoProductos";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAuditoria)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.DataGridView dataGridViewAuditoria;
    }
}