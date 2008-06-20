namespace WindowsApplication1
{
    partial class Sections
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Sections));
            this.listViewSch = new System.Windows.Forms.ListView();
            this.sName = new System.Windows.Forms.ColumnHeader();
            this.sVirtualSize = new System.Windows.Forms.ColumnHeader();
            this.sVirtualAddress = new System.Windows.Forms.ColumnHeader();
            this.sSizeOfRawData = new System.Windows.Forms.ColumnHeader();
            this.sPointerToRawData = new System.Windows.Forms.ColumnHeader();
            this.sPointerToRelocations = new System.Windows.Forms.ColumnHeader();
            this.sPointerToLinenumbers = new System.Windows.Forms.ColumnHeader();
            this.sNumberOfRelocations = new System.Windows.Forms.ColumnHeader();
            this.sNumberOfLinenumbers = new System.Windows.Forms.ColumnHeader();
            this.sCharacteristics = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listViewSch
            // 
            this.listViewSch.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sName,
            this.sVirtualSize,
            this.sVirtualAddress,
            this.sSizeOfRawData,
            this.sPointerToRawData,
            this.sPointerToRelocations,
            this.sPointerToLinenumbers,
            this.sNumberOfRelocations,
            this.sNumberOfLinenumbers,
            this.sCharacteristics});
            this.listViewSch.Location = new System.Drawing.Point(0, 0);
            this.listViewSch.Name = "listViewSch";
            this.listViewSch.Size = new System.Drawing.Size(532, 336);
            this.listViewSch.TabIndex = 232;
            this.listViewSch.UseCompatibleStateImageBehavior = false;
            this.listViewSch.View = System.Windows.Forms.View.Details;
            // 
            // sName
            // 
            this.sName.Text = "Name";
            this.sName.Width = 78;
            // 
            // sVirtualSize
            // 
            this.sVirtualSize.Text = "VirtualSize";
            this.sVirtualSize.Width = 78;
            // 
            // sVirtualAddress
            // 
            this.sVirtualAddress.Text = "VirtualAddress";
            this.sVirtualAddress.Width = 78;
            // 
            // sSizeOfRawData
            // 
            this.sSizeOfRawData.Text = "SizeOfRawData";
            this.sSizeOfRawData.Width = 78;
            // 
            // sPointerToRawData
            // 
            this.sPointerToRawData.Text = "PointerToRawData";
            this.sPointerToRawData.Width = 78;
            // 
            // sPointerToRelocations
            // 
            this.sPointerToRelocations.Text = "PointerToRelocations";
            this.sPointerToRelocations.Width = 78;
            // 
            // sPointerToLinenumbers
            // 
            this.sPointerToLinenumbers.Text = "PointerToLinenumbers";
            this.sPointerToLinenumbers.Width = 78;
            // 
            // sNumberOfRelocations
            // 
            this.sNumberOfRelocations.Text = "NumberOfRelocations";
            this.sNumberOfRelocations.Width = 78;
            // 
            // sNumberOfLinenumbers
            // 
            this.sNumberOfLinenumbers.Text = "NumberOfLinenumbers";
            this.sNumberOfLinenumbers.Width = 78;
            // 
            // sCharacteristics
            // 
            this.sCharacteristics.Text = "Characteristics";
            this.sCharacteristics.Width = 78;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 339);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(499, 221);
            this.label1.TabIndex = 233;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // Sections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewSch);
            this.Name = "Sections";
            this.Size = new System.Drawing.Size(532, 612);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewSch;
        private System.Windows.Forms.ColumnHeader sName;
        private System.Windows.Forms.ColumnHeader sVirtualSize;
        private System.Windows.Forms.ColumnHeader sVirtualAddress;
        private System.Windows.Forms.ColumnHeader sSizeOfRawData;
        private System.Windows.Forms.ColumnHeader sPointerToRawData;
        private System.Windows.Forms.ColumnHeader sPointerToRelocations;
        private System.Windows.Forms.ColumnHeader sPointerToLinenumbers;
        private System.Windows.Forms.ColumnHeader sNumberOfRelocations;
        private System.Windows.Forms.ColumnHeader sNumberOfLinenumbers;
        private System.Windows.Forms.ColumnHeader sCharacteristics;
        private System.Windows.Forms.Label label1;
    }
}
