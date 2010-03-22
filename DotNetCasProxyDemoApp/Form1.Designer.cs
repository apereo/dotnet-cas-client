namespace DotNetCasProxyDemoApp
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ServiceField = new System.Windows.Forms.TextBox();
            this.TicketField = new System.Windows.Forms.TextBox();
            this.VerifyUrlField = new System.Windows.Forms.TextBox();
            this.ServerResponseField = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.FinalValidationUrlField = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Proxy Validate URL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Target Service Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Proxy Ticket";
            // 
            // ServiceField
            // 
            this.ServiceField.Location = new System.Drawing.Point(133, 48);
            this.ServiceField.Name = "ServiceField";
            this.ServiceField.Size = new System.Drawing.Size(511, 20);
            this.ServiceField.TabIndex = 3;
            // 
            // TicketField
            // 
            this.TicketField.Location = new System.Drawing.Point(133, 78);
            this.TicketField.Name = "TicketField";
            this.TicketField.Size = new System.Drawing.Size(511, 20);
            this.TicketField.TabIndex = 4;
            // 
            // VerifyUrlField
            // 
            this.VerifyUrlField.Location = new System.Drawing.Point(133, 17);
            this.VerifyUrlField.Name = "VerifyUrlField";
            this.VerifyUrlField.Size = new System.Drawing.Size(511, 20);
            this.VerifyUrlField.TabIndex = 5;
            // 
            // ServerResponseField
            // 
            this.ServerResponseField.Location = new System.Drawing.Point(15, 148);
            this.ServerResponseField.Multiline = true;
            this.ServerResponseField.Name = "ServerResponseField";
            this.ServerResponseField.Size = new System.Drawing.Size(629, 271);
            this.ServerResponseField.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Final Validation URL:";
            // 
            // FinalValidationUrlField
            // 
            this.FinalValidationUrlField.Location = new System.Drawing.Point(133, 110);
            this.FinalValidationUrlField.Name = "FinalValidationUrlField";
            this.FinalValidationUrlField.Size = new System.Drawing.Size(511, 20);
            this.FinalValidationUrlField.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 431);
            this.Controls.Add(this.ServerResponseField);
            this.Controls.Add(this.VerifyUrlField);
            this.Controls.Add(this.FinalValidationUrlField);
            this.Controls.Add(this.TicketField);
            this.Controls.Add(this.ServiceField);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ServiceField;
        private System.Windows.Forms.TextBox TicketField;
        private System.Windows.Forms.TextBox VerifyUrlField;
        private System.Windows.Forms.TextBox ServerResponseField;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox FinalValidationUrlField;
    }
}

