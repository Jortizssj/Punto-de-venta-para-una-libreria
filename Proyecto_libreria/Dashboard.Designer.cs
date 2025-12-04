namespace Proyecto_libreria
{
    partial class Dashboard
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
            this.btnPuntoVenta = new System.Windows.Forms.Button();
            this.btnCRUD_Productos = new System.Windows.Forms.Button();
            this.btnCRUD_Empleados = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnVentasVendedor = new System.Windows.Forms.Button();
            this.btnUnidadesVendidas = new System.Windows.Forms.Button();
            this.btnMonitoreo = new System.Windows.Forms.Button();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.lblRol = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnPuntoVenta
            // 
            this.btnPuntoVenta.Location = new System.Drawing.Point(117, 78);
            this.btnPuntoVenta.Name = "btnPuntoVenta";
            this.btnPuntoVenta.Size = new System.Drawing.Size(145, 48);
            this.btnPuntoVenta.TabIndex = 0;
            this.btnPuntoVenta.Text = "Punto de Venta";
            this.btnPuntoVenta.UseVisualStyleBackColor = true;
            this.btnPuntoVenta.Click += new System.EventHandler(this.btnPuntoVenta_Click);
            // 
            // btnCRUD_Productos
            // 
            this.btnCRUD_Productos.Location = new System.Drawing.Point(117, 201);
            this.btnCRUD_Productos.Name = "btnCRUD_Productos";
            this.btnCRUD_Productos.Size = new System.Drawing.Size(145, 48);
            this.btnCRUD_Productos.TabIndex = 1;
            this.btnCRUD_Productos.Text = "Productos";
            this.btnCRUD_Productos.UseVisualStyleBackColor = true;
            this.btnCRUD_Productos.Click += new System.EventHandler(this.btnCRUD_Productos_Click);
            // 
            // btnCRUD_Empleados
            // 
            this.btnCRUD_Empleados.Location = new System.Drawing.Point(291, 201);
            this.btnCRUD_Empleados.Name = "btnCRUD_Empleados";
            this.btnCRUD_Empleados.Size = new System.Drawing.Size(145, 48);
            this.btnCRUD_Empleados.TabIndex = 2;
            this.btnCRUD_Empleados.Text = "Empleados";
            this.btnCRUD_Empleados.UseVisualStyleBackColor = true;
            this.btnCRUD_Empleados.Click += new System.EventHandler(this.btnCRUD_Empleados_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(114, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Vender";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(114, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Administrar";
            // 
            // btnVentasVendedor
            // 
            this.btnVentasVendedor.Location = new System.Drawing.Point(117, 322);
            this.btnVentasVendedor.Name = "btnVentasVendedor";
            this.btnVentasVendedor.Size = new System.Drawing.Size(145, 48);
            this.btnVentasVendedor.TabIndex = 5;
            this.btnVentasVendedor.Text = "Ventas de un empleado";
            this.btnVentasVendedor.UseVisualStyleBackColor = true;
            this.btnVentasVendedor.Click += new System.EventHandler(this.btnVentasVendedor_Click);
            // 
            // btnUnidadesVendidas
            // 
            this.btnUnidadesVendidas.Location = new System.Drawing.Point(291, 322);
            this.btnUnidadesVendidas.Name = "btnUnidadesVendidas";
            this.btnUnidadesVendidas.Size = new System.Drawing.Size(145, 48);
            this.btnUnidadesVendidas.TabIndex = 6;
            this.btnUnidadesVendidas.Text = "Ventas de un libro";
            this.btnUnidadesVendidas.UseVisualStyleBackColor = true;
            this.btnUnidadesVendidas.Click += new System.EventHandler(this.btnUnidadesVendidas_Click);
            // 
            // btnMonitoreo
            // 
            this.btnMonitoreo.Location = new System.Drawing.Point(117, 424);
            this.btnMonitoreo.Name = "btnMonitoreo";
            this.btnMonitoreo.Size = new System.Drawing.Size(145, 48);
            this.btnMonitoreo.TabIndex = 7;
            this.btnMonitoreo.Text = "Monitoreo de productos";
            this.btnMonitoreo.UseVisualStyleBackColor = true;
            this.btnMonitoreo.Click += new System.EventHandler(this.btnMonitoreo_Click);
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.Location = new System.Drawing.Point(617, 435);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(145, 48);
            this.btnCerrarSesion.TabIndex = 8;
            this.btnCerrarSesion.Text = "Cerrar Sesion";
            this.btnCerrarSesion.UseVisualStyleBackColor = true;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // lblRol
            // 
            this.lblRol.AutoSize = true;
            this.lblRol.Location = new System.Drawing.Point(652, 94);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(28, 16);
            this.lblRol.TabIndex = 9;
            this.lblRol.Text = "Rol";
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 554);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.btnCerrarSesion);
            this.Controls.Add(this.btnMonitoreo);
            this.Controls.Add(this.btnUnidadesVendidas);
            this.Controls.Add(this.btnVentasVendedor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCRUD_Empleados);
            this.Controls.Add(this.btnCRUD_Productos);
            this.Controls.Add(this.btnPuntoVenta);
            this.Name = "Dashboard";
            this.Text = "Dashboard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPuntoVenta;
        private System.Windows.Forms.Button btnCRUD_Productos;
        private System.Windows.Forms.Button btnCRUD_Empleados;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnVentasVendedor;
        private System.Windows.Forms.Button btnUnidadesVendidas;
        private System.Windows.Forms.Button btnMonitoreo;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.Label lblRol;
    }
}