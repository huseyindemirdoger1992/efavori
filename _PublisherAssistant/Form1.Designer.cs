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
            Directory_Structure_btn = new Button();
            Directory_Structure_ListBox = new ListBox();
            Total_Number_Of_Files_Lbl = new Label();
            Total_File_Size_Lbl = new Label();
            TestButton = new Button();
            ActionProgressBar = new ProgressBar();
            ActionHistoryList = new ListBox();
            button1 = new Button();
            button2 = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            SuspendLayout();
            // 
            // Directory_Structure_txt
            // 
            Directory_Structure_txt.Enabled = false;
            Directory_Structure_txt.Location = new Point(12, 85);
            Directory_Structure_txt.Name = "Directory_Structure_txt";
            Directory_Structure_txt.Size = new Size(861, 27);
            Directory_Structure_txt.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 62);
            label1.Name = "label1";
            label1.Size = new Size(133, 20);
            label1.TabIndex = 1;
            label1.Text = "Directory Structure";
            // 
            // Directory_Structure_btn
            // 
            Directory_Structure_btn.Location = new Point(879, 85);
            Directory_Structure_btn.Name = "Directory_Structure_btn";
            Directory_Structure_btn.Size = new Size(54, 27);
            Directory_Structure_btn.TabIndex = 4;
            Directory_Structure_btn.Text = "...";
            Directory_Structure_btn.UseVisualStyleBackColor = true;
            // 
            // Directory_Structure_ListBox
            // 
            Directory_Structure_ListBox.FormattingEnabled = true;
            Directory_Structure_ListBox.Location = new Point(12, 118);
            Directory_Structure_ListBox.Name = "Directory_Structure_ListBox";
            Directory_Structure_ListBox.SelectionMode = SelectionMode.MultiSimple;
            Directory_Structure_ListBox.Size = new Size(921, 144);
            Directory_Structure_ListBox.TabIndex = 6;
            // 
            // Total_Number_Of_Files_Lbl
            // 
            Total_Number_Of_Files_Lbl.AutoSize = true;
            Total_Number_Of_Files_Lbl.Location = new Point(12, 465);
            Total_Number_Of_Files_Lbl.Name = "Total_Number_Of_Files_Lbl";
            Total_Number_Of_Files_Lbl.Size = new Size(167, 20);
            Total_Number_Of_Files_Lbl.TabIndex = 7;
            Total_Number_Of_Files_Lbl.Text = "Total Number of Files: X";
            // 
            // Total_File_Size_Lbl
            // 
            Total_File_Size_Lbl.AutoSize = true;
            Total_File_Size_Lbl.Location = new Point(12, 485);
            Total_File_Size_Lbl.Name = "Total_File_Size_Lbl";
            Total_File_Size_Lbl.Size = new Size(116, 20);
            Total_File_Size_Lbl.TabIndex = 8;
            Total_File_Size_Lbl.Text = "Total File Size: X";
            // 
            // TestButton
            // 
            TestButton.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            TestButton.ForeColor = Color.Olive;
            TestButton.Location = new Point(12, 12);
            TestButton.Name = "TestButton";
            TestButton.Size = new Size(921, 47);
            TestButton.TabIndex = 9;
            TestButton.Text = "Connection Test";
            TestButton.UseVisualStyleBackColor = true;
            TestButton.Click += TestButton_Click;
            // 
            // ActionProgressBar
            // 
            ActionProgressBar.Location = new Point(12, 268);
            ActionProgressBar.Name = "ActionProgressBar";
            ActionProgressBar.Size = new Size(921, 29);
            ActionProgressBar.TabIndex = 11;
            // 
            // ActionHistoryList
            // 
            ActionHistoryList.FormattingEnabled = true;
            ActionHistoryList.Location = new Point(12, 303);
            ActionHistoryList.Name = "ActionHistoryList";
            ActionHistoryList.Size = new Size(921, 124);
            ActionHistoryList.TabIndex = 12;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            button1.ForeColor = Color.Maroon;
            button1.Location = new Point(12, 433);
            button1.Name = "button1";
            button1.Size = new Size(248, 29);
            button1.TabIndex = 19;
            button1.Text = "FTP (Clear Server)";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            button2.ForeColor = Color.Teal;
            button2.Location = new Point(685, 433);
            button2.Name = "button2";
            button2.Size = new Size(248, 29);
            button2.TabIndex = 20;
            button2.Text = "Release the New Structure";
            button2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 505);
            label2.Name = "label2";
            label2.Size = new Size(109, 20);
            label2.TabIndex = 21;
            label2.Text = "Action Start At:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 525);
            label3.Name = "label3";
            label3.Size = new Size(115, 20);
            label3.TabIndex = 22;
            label3.Text = "Action Finish At:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 545);
            label4.Name = "label4";
            label4.Size = new Size(82, 20);
            label4.TabIndex = 23;
            label4.Text = "Total Time:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(945, 589);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(ActionHistoryList);
            Controls.Add(ActionProgressBar);
            Controls.Add(TestButton);
            Controls.Add(Total_File_Size_Lbl);
            Controls.Add(Total_Number_Of_Files_Lbl);
            Controls.Add(Directory_Structure_ListBox);
            Controls.Add(Directory_Structure_btn);
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
        private Button Directory_Structure_btn;
        private ListBox Directory_Structure_ListBox;
        private Label Total_Number_Of_Files_Lbl;
        private Label Total_File_Size_Lbl;
        private Button TestButton;
        private ProgressBar ActionProgressBar;
        private ListBox ActionHistoryList;
        private Button button1;
        private Button button2;
        private Label label2;
        private Label label3;
        private Label label4;
    }
}
