namespace FileExplorer
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_upLevel = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsl_path = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tv_files = new System.Windows.Forms.TreeView();
            this.il_DiscFoldersFilesIcons_Small = new System.Windows.Forms.ImageList(this.components);
            this.lv_files = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.il_DiscFoldersFilesIcons_Large = new System.Windows.Forms.ImageList(this.components);
            this.il_ForTree = new System.Windows.Forms.ImageList(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_upLevel,
            this.toolStripLabel1,
            this.tsl_path,
            this.toolStripComboBox1,
            this.toolStripLabel2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1265, 28);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_upLevel
            // 
            this.tsb_upLevel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_upLevel.Image = global::OutlookSave.Properties.Resources.column_up;
            this.tsb_upLevel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_upLevel.Name = "tsb_upLevel";
            this.tsb_upLevel.Size = new System.Drawing.Size(24, 25);
            this.tsb_upLevel.Text = "Up";
            this.tsb_upLevel.Click += new System.EventHandler(this.tsb_upLevel_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(94, 25);
            this.toolStripLabel1.Text = "Current path:";
            // 
            // tsl_path
            // 
            this.tsl_path.BackColor = System.Drawing.SystemColors.Control;
            this.tsl_path.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.tsl_path.Name = "tsl_path";
            this.tsl_path.Size = new System.Drawing.Size(190, 25);
            this.tsl_path.Text = "There will be a current path";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(160, 28);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(48, 25);
            this.toolStripLabel2.Text = "View: ";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(16, 34);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tv_files);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lv_files);
            this.splitContainer1.Size = new System.Drawing.Size(1233, 560);
            this.splitContainer1.SplitterDistance = 405;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.TabStop = false;
            // 
            // tv_files
            // 
            this.tv_files.AllowDrop = true;
            this.tv_files.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tv_files.ImageKey = "drive.ico";
            this.tv_files.ImageList = this.il_DiscFoldersFilesIcons_Small;
            this.tv_files.LabelEdit = true;
            this.tv_files.Location = new System.Drawing.Point(4, 4);
            this.tv_files.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tv_files.Name = "tv_files";
            this.tv_files.SelectedImageIndex = 3;
            this.tv_files.Size = new System.Drawing.Size(401, 555);
            this.tv_files.TabIndex = 0;
            // 
            // il_DiscFoldersFilesIcons_Small
            // 
            this.il_DiscFoldersFilesIcons_Small.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il_DiscFoldersFilesIcons_Small.ImageStream")));
            this.il_DiscFoldersFilesIcons_Small.TransparentColor = System.Drawing.Color.Transparent;
            this.il_DiscFoldersFilesIcons_Small.Images.SetKeyName(0, "drive.ico");
            this.il_DiscFoldersFilesIcons_Small.Images.SetKeyName(1, "folder.ico");
            this.il_DiscFoldersFilesIcons_Small.Images.SetKeyName(2, "column_down.png");
            this.il_DiscFoldersFilesIcons_Small.Images.SetKeyName(3, "column_up.png");
            // 
            // lv_files
            // 
            this.lv_files.AllowDrop = true;
            this.lv_files.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lv_files.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lv_files.LargeImageList = this.il_DiscFoldersFilesIcons_Large;
            this.lv_files.Location = new System.Drawing.Point(4, 4);
            this.lv_files.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lv_files.MultiSelect = false;
            this.lv_files.Name = "lv_files";
            this.lv_files.Size = new System.Drawing.Size(817, 555);
            this.lv_files.SmallImageList = this.il_DiscFoldersFilesIcons_Small;
            this.lv_files.TabIndex = 0;
            this.lv_files.UseCompatibleStateImageBehavior = false;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Date created";
            this.columnHeader3.Width = 120;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Date changed";
            this.columnHeader4.Width = 120;
            // 
            // il_DiscFoldersFilesIcons_Large
            // 
            this.il_DiscFoldersFilesIcons_Large.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il_DiscFoldersFilesIcons_Large.ImageStream")));
            this.il_DiscFoldersFilesIcons_Large.TransparentColor = System.Drawing.Color.Transparent;
            this.il_DiscFoldersFilesIcons_Large.Images.SetKeyName(0, "drive.ico");
            this.il_DiscFoldersFilesIcons_Large.Images.SetKeyName(1, "folder.ico");
            this.il_DiscFoldersFilesIcons_Large.Images.SetKeyName(2, "column_down.png");
            this.il_DiscFoldersFilesIcons_Large.Images.SetKeyName(3, "column_up.png");
            // 
            // il_ForTree
            // 
            this.il_ForTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il_ForTree.ImageStream")));
            this.il_ForTree.TransparentColor = System.Drawing.Color.Transparent;
            this.il_ForTree.Images.SetKeyName(0, "folder_tree.ico");
            this.il_ForTree.Images.SetKeyName(1, "folderopen_tree.png");
            this.il_ForTree.Images.SetKeyName(2, "drive.ico");
            this.il_ForTree.Images.SetKeyName(3, "");
            this.il_ForTree.Images.SetKeyName(4, "");
            this.il_ForTree.Images.SetKeyName(5, "");
            this.il_ForTree.Images.SetKeyName(6, "");
            this.il_ForTree.Images.SetKeyName(7, "");
            this.il_ForTree.Images.SetKeyName(8, "");
            this.il_ForTree.Images.SetKeyName(9, "");
            this.il_ForTree.Images.SetKeyName(10, "");
            this.il_ForTree.Images.SetKeyName(11, "");
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.Location = new System.Drawing.Point(484, 602);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 26);
            this.button1.TabIndex = 1;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button2.Location = new System.Drawing.Point(612, 602);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 26);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1265, 649);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(527, 358);
            this.Name = "Form1";
            this.Text = "Explorer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView lv_files;
        private System.Windows.Forms.ImageList il_DiscFoldersFilesIcons_Small;
        private System.Windows.Forms.ImageList il_DiscFoldersFilesIcons_Large;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ToolStripButton tsb_upLevel;
        private System.Windows.Forms.ToolStripLabel tsl_path;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.TreeView tv_files;
        private System.Windows.Forms.ImageList il_ForTree;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

