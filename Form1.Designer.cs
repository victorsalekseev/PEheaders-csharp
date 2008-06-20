namespace WindowsApplication1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControlHdr = new System.Windows.Forms.TabControl();
            this.tabPageDos = new System.Windows.Forms.TabPage();
            this.dosHeader1 = new WindowsApplication1.DosHeader();
            this.tabPageNt = new System.Windows.Forms.TabPage();
            this.ntHeaders1 = new WindowsApplication1.NtHeaders();
            this.tabPageSc = new System.Windows.Forms.TabPage();
            this.sections1 = new WindowsApplication1.Sections();
            this.tabControlHdr.SuspendLayout();
            this.tabPageDos.SuspendLayout();
            this.tabPageNt.SuspendLayout();
            this.tabPageSc.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(623, 228);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Писать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(623, 89);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Читать";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControlHdr
            // 
            this.tabControlHdr.Controls.Add(this.tabPageDos);
            this.tabControlHdr.Controls.Add(this.tabPageNt);
            this.tabControlHdr.Controls.Add(this.tabPageSc);
            this.tabControlHdr.Location = new System.Drawing.Point(12, 12);
            this.tabControlHdr.Name = "tabControlHdr";
            this.tabControlHdr.SelectedIndex = 0;
            this.tabControlHdr.Size = new System.Drawing.Size(605, 654);
            this.tabControlHdr.TabIndex = 3;
            // 
            // tabPageDos
            // 
            this.tabPageDos.Controls.Add(this.dosHeader1);
            this.tabPageDos.Location = new System.Drawing.Point(4, 22);
            this.tabPageDos.Name = "tabPageDos";
            this.tabPageDos.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDos.Size = new System.Drawing.Size(597, 628);
            this.tabPageDos.TabIndex = 0;
            this.tabPageDos.Text = "tabPage1";
            this.tabPageDos.UseVisualStyleBackColor = true;
            // 
            // dosHeader1
            // 
            this.dosHeader1.Location = new System.Drawing.Point(0, 0);
            this.dosHeader1.Name = "dosHeader1";
            this.dosHeader1.Size = new System.Drawing.Size(532, 587);
            this.dosHeader1.TabIndex = 4;
            // 
            // tabPageNt
            // 
            this.tabPageNt.Controls.Add(this.ntHeaders1);
            this.tabPageNt.Location = new System.Drawing.Point(4, 22);
            this.tabPageNt.Name = "tabPageNt";
            this.tabPageNt.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNt.Size = new System.Drawing.Size(597, 628);
            this.tabPageNt.TabIndex = 1;
            this.tabPageNt.Text = "tabPage2";
            this.tabPageNt.UseVisualStyleBackColor = true;
            // 
            // ntHeaders1
            // 
            this.ntHeaders1.Location = new System.Drawing.Point(0, 3);
            this.ntHeaders1.Name = "ntHeaders1";
            this.ntHeaders1.Size = new System.Drawing.Size(532, 620);
            this.ntHeaders1.TabIndex = 0;
            // 
            // tabPageSc
            // 
            this.tabPageSc.Controls.Add(this.sections1);
            this.tabPageSc.Location = new System.Drawing.Point(4, 22);
            this.tabPageSc.Name = "tabPageSc";
            this.tabPageSc.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSc.Size = new System.Drawing.Size(597, 628);
            this.tabPageSc.TabIndex = 2;
            this.tabPageSc.Text = "tabPage1";
            this.tabPageSc.UseVisualStyleBackColor = true;
            // 
            // sections1
            // 
            this.sections1.Location = new System.Drawing.Point(6, 6);
            this.sections1.Name = "sections1";
            this.sections1.Size = new System.Drawing.Size(532, 612);
            this.sections1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 671);
            this.Controls.Add(this.tabControlHdr);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControlHdr.ResumeLayout(false);
            this.tabPageDos.ResumeLayout(false);
            this.tabPageNt.ResumeLayout(false);
            this.tabPageSc.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabControlHdr;
        private System.Windows.Forms.TabPage tabPageDos;
        private DosHeader dosHeader1;
        private System.Windows.Forms.TabPage tabPageNt;
        private NtHeaders ntHeaders1;
        private System.Windows.Forms.TabPage tabPageSc;
        private Sections sections1;
    }
}

