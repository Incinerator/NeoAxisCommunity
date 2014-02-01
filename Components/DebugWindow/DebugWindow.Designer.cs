namespace Engine.Utils
{
    partial class DebugWindow
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
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonSaveMap = new System.Windows.Forms.Button();
            this.checkBoxTransparent = new System.Windows.Forms.CheckBox();
            this.buttonUP = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.entityList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.checkBoxTopMost = new System.Windows.Forms.CheckBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.propEditor = new System.Windows.Forms.PropertyGrid();
            this.label4 = new System.Windows.Forms.Label();
            this.typeEditor = new System.Windows.Forms.PropertyGrid();
            this.buttonSaveType = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(670, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Entity";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonSaveMap);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxTransparent);
            this.splitContainer1.Panel1.Controls.Add(this.buttonUP);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxFilter);
            this.splitContainer1.Panel1.Controls.Add(this.entityList);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxTopMost);
            this.splitContainer1.Panel1.Controls.Add(this.buttonRefresh);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(952, 674);
            this.splitContainer1.SplitterDistance = 457;
            this.splitContainer1.TabIndex = 10;
            // 
            // buttonSaveMap
            // 
            this.buttonSaveMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveMap.Location = new System.Drawing.Point(381, 0);
            this.buttonSaveMap.Name = "buttonSaveMap";
            this.buttonSaveMap.Size = new System.Drawing.Size(73, 24);
            this.buttonSaveMap.TabIndex = 13;
            this.buttonSaveMap.Text = "Save Map";
            this.buttonSaveMap.UseVisualStyleBackColor = true;
            this.buttonSaveMap.Visible = false;
            // 
            // checkBoxTransparent
            // 
            this.checkBoxTransparent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxTransparent.AutoSize = true;
            this.checkBoxTransparent.Location = new System.Drawing.Point(15, 629);
            this.checkBoxTransparent.Name = "checkBoxTransparent";
            this.checkBoxTransparent.Size = new System.Drawing.Size(109, 17);
            this.checkBoxTransparent.TabIndex = 12;
            this.checkBoxTransparent.Text = "Semi-Transparent";
            this.checkBoxTransparent.UseVisualStyleBackColor = true;
            this.checkBoxTransparent.CheckedChanged += new System.EventHandler(this.checkBoxTransparent_CheckedChanged);
            // 
            // buttonUP
            // 
            this.buttonUP.Location = new System.Drawing.Point(3, 3);
            this.buttonUP.Name = "buttonUP";
            this.buttonUP.Size = new System.Drawing.Size(32, 21);
            this.buttonUP.TabIndex = 11;
            this.buttonUP.Text = "UP";
            this.buttonUP.UseVisualStyleBackColor = true;
            this.buttonUP.Click += new System.EventHandler(this.buttonUP_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(132, 649);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Filter";
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.Location = new System.Drawing.Point(167, 646);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(219, 20);
            this.textBoxFilter.TabIndex = 7;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            this.textBoxFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFilter_KeyDown);
            // 
            // entityList
            // 
            this.entityList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.entityList.AutoArrange = false;
            this.entityList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.entityList.FullRowSelect = true;
            this.entityList.GridLines = true;
            this.entityList.HideSelection = false;
            this.entityList.Location = new System.Drawing.Point(0, 29);
            this.entityList.Name = "entityList";
            this.entityList.Size = new System.Drawing.Size(451, 594);
            this.entityList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.entityList.TabIndex = 4;
            this.entityList.UseCompatibleStateImageBehavior = false;
            this.entityList.View = System.Windows.Forms.View.Details;
            this.entityList.SelectedIndexChanged += new System.EventHandler(this.entityList_SelectedIndexChanged);
            this.entityList.DoubleClick += new System.EventHandler(this.entityList_DoubleClick);
            this.entityList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityList_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Entity Name";
            this.columnHeader1.Width = 277;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Entity Type";
            this.columnHeader2.Width = 110;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Children";
            this.columnHeader3.Width = 57;
            // 
            // checkBoxTopMost
            // 
            this.checkBoxTopMost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxTopMost.AutoSize = true;
            this.checkBoxTopMost.Location = new System.Drawing.Point(15, 652);
            this.checkBoxTopMost.Name = "checkBoxTopMost";
            this.checkBoxTopMost.Size = new System.Drawing.Size(71, 17);
            this.checkBoxTopMost.TabIndex = 6;
            this.checkBoxTopMost.Text = "Top Most";
            this.checkBoxTopMost.UseVisualStyleBackColor = true;
            this.checkBoxTopMost.CheckedChanged += new System.EventHandler(this.checkBoxTopMost_CheckedChanged);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRefresh.Location = new System.Drawing.Point(392, 643);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(59, 25);
            this.buttonRefresh.TabIndex = 5;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.propEditor);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.buttonSaveType);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.typeEditor);
            this.splitContainer2.Size = new System.Drawing.Size(491, 674);
            this.splitContainer2.SplitterDistance = 244;
            this.splitContainer2.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 24);
            this.label3.TabIndex = 11;
            this.label3.Text = "Object properties";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // propEditor
            // 
            this.propEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propEditor.Location = new System.Drawing.Point(3, 29);
            this.propEditor.Name = "propEditor";
            this.propEditor.Size = new System.Drawing.Size(238, 642);
            this.propEditor.TabIndex = 10;
            this.propEditor.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propEditor_SelectedGridItemChanged);
            this.propEditor.Click += new System.EventHandler(this.propEditor_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(-2, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(153, 24);
            this.label4.TabIndex = 13;
            this.label4.Text = "Type properties";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // typeEditor
            // 
            this.typeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.typeEditor.Location = new System.Drawing.Point(3, 29);
            this.typeEditor.Name = "typeEditor";
            this.typeEditor.Size = new System.Drawing.Size(237, 642);
            this.typeEditor.TabIndex = 12;
            // 
            // buttonSaveType
            // 
            this.buttonSaveType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveType.Location = new System.Drawing.Point(167, 29);
            this.buttonSaveType.Name = "buttonSaveType";
            this.buttonSaveType.Size = new System.Drawing.Size(73, 24);
            this.buttonSaveType.TabIndex = 11;
            this.buttonSaveType.Text = "Save Type";
            this.buttonSaveType.UseVisualStyleBackColor = true;
            this.buttonSaveType.Click += new System.EventHandler(this.buttonSaveType_Click);
            // 
            // DebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 698);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DebugWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Debug Window";
            this.Load += new System.EventHandler(this.DebugForm_Load);
            this.Activated += new System.EventHandler(this.DebugWindow_Activated);
            this.MouseLeave += new System.EventHandler(this.DebugWindow_MouseLeave);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DebugWindow_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DebugWindow_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox checkBoxTopMost;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ListView entityList;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PropertyGrid propEditor;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PropertyGrid typeEditor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonUP;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button buttonSaveType;
        private System.Windows.Forms.CheckBox checkBoxTransparent;
        private System.Windows.Forms.Button buttonSaveMap;
    }
}