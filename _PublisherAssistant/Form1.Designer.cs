namespace _PublisherAssistant
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Directory_Structure_txt = new TextBox();
            label1 = new Label();
            label2 = new Label();
            Web_Config_txt = new TextBox();
            Directory_Structure_btn = new Button();
            Web_Config_btn = new Button();
            Directory_Structure_ListBox = new ListBox();
            Total_Number_Of_Files_Lbl = new Label();
            Total_File_Size_Lbl = new Label();
            SuspendLayout();
            // 
            // Directory_Structure_txt
            // 
            Directory_Structure_txt.Enabled = false;
            Directory_Structure_txt.Location = new Point(12, 32);
            Directory_Structure_txt.Name = "Directory_Structure_txt";
            Directory_Structure_txt.Size = new Size(861, 27);
            Directory_Structure_txt.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(133, 20);
            label1.TabIndex = 1;
            label1.Text = "Directory Structure";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 62);
            label2.Name = "label2";
            label2.Size = new Size(86, 20);
            label2.TabIndex = 2;
            label2.Text = "Web.Config";
            // 
            // Web_Config_txt
            // 
            Web_Config_txt.Enabled = false;
            Web_Config_txt.Location = new Point(12, 85);
            Web_Config_txt.Name = "Web_Config_txt";
            Web_Config_txt.Size = new Size(861, 27);
            Web_Config_txt.TabIndex = 3;
            // 
            // Directory_Structure_btn
            // 
            Directory_Structure_btn.Location = new Point(879, 12);
            Directory_Structure_btn.Name = "Directory_Structure_btn";
            Directory_Structure_btn.Size = new Size(54, 47);
            Directory_Structure_btn.TabIndex = 4;
            Directory_Structure_btn.Text = "...";
            Directory_Structure_btn.UseVisualStyleBackColor = true;
            Directory_Structure_btn.Click += Directory_Structure_btn_Click;
            // 
            // Web_Config_btn
            // 
            Web_Config_btn.Location = new Point(879, 65);
            Web_Config_btn.Name = "Web_Config_btn";
            Web_Config_btn.Size = new Size(54, 47);
            Web_Config_btn.TabIndex = 5;
            Web_Config_btn.Text = "...";
            Web_Config_btn.UseVisualStyleBackColor = true;
            Web_Config_btn.Click += Web_Config_btn_Click;
            // 
            // Directory_Structure_ListBox
            // 
            Directory_Structure_ListBox.FormattingEnabled = true;
            Directory_Structure_ListBox.Location = new Point(12, 118);
            Directory_Structure_ListBox.Name = "Directory_Structure_ListBox";
            Directory_Structure_ListBox.SelectionMode = SelectionMode.MultiSimple;
            Directory_Structure_ListBox.Size = new Size(921, 324);
            Directory_Structure_ListBox.TabIndex = 6;
            // 
            // Total_Number_Of_Files_Lbl
            // 
            Total_Number_Of_Files_Lbl.AutoSize = true;
            Total_Number_Of_Files_Lbl.Location = new Point(12, 445);
            Total_Number_Of_Files_Lbl.Name = "Total_Number_Of_Files_Lbl";
            Total_Number_Of_Files_Lbl.Size = new Size(167, 20);
            Total_Number_Of_Files_Lbl.TabIndex = 7;
            Total_Number_Of_Files_Lbl.Text = "Total Number of Files: X";
            // 
            // Total_File_Size_Lbl
            // 
            Total_File_Size_Lbl.AutoSize = true;
            Total_File_Size_Lbl.Location = new Point(12, 465);
            Total_File_Size_Lbl.Name = "Total_File_Size_Lbl";
            Total_File_Size_Lbl.Size = new Size(116, 20);
            Total_File_Size_Lbl.TabIndex = 8;
            Total_File_Size_Lbl.Text = "Total File Size: X";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(945, 526);
            Controls.Add(Total_File_Size_Lbl);
            Controls.Add(Total_Number_Of_Files_Lbl);
            Controls.Add(Directory_Structure_ListBox);
            Controls.Add(Web_Config_btn);
            Controls.Add(Directory_Structure_btn);
            Controls.Add(Web_Config_txt);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(Directory_Structure_txt);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox Directory_Structure_txt;
        private Label label1;
        private Label label2;
        private TextBox Web_Config_txt;
        private Button Directory_Structure_btn;
        private Button Web_Config_btn;
        private ListBox Directory_Structure_ListBox;
        private Label Total_Number_Of_Files_Lbl;
        private Label Total_File_Size_Lbl;
    }
}
