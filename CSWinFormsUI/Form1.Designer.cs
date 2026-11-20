namespace CSWinFormsUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuSelectDb;
        private System.Windows.Forms.ToolStripMenuItem mnuOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuLang;
        private System.Windows.Forms.ToolStripMenuItem mnuLangEn;
        private System.Windows.Forms.ToolStripMenuItem mnuLangVi;
        private System.Windows.Forms.ToolStripMenuItem mnuLangZhTw;
        private System.Windows.Forms.ToolStripMenuItem mnuConfirmClose;
        private System.Windows.Forms.ToolStripMenuItem mnuLockFullScreen;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;

        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuFilterSearch;
        private System.Windows.Forms.ToolStripMenuItem mnuClearFilterSort;
        private System.Windows.Forms.ToolStripSeparator sepEdit1;
        private System.Windows.Forms.ToolStripMenuItem mnuAddRow;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteRow;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveChanges;
        private System.Windows.Forms.ToolStripMenuItem mnuReload;
        private System.Windows.Forms.ToolStripSeparator sepEdit2;
        private System.Windows.Forms.ToolStripMenuItem mnuToggleEdit;

        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuTableKhoa;
        private System.Windows.Forms.ToolStripMenuItem mnuTableNganh;
        private System.Windows.Forms.ToolStripMenuItem mnuTableLop;
        private System.Windows.Forms.ToolStripMenuItem mnuTableMon;
        private System.Windows.Forms.ToolStripMenuItem mnuTableHssv;
        private System.Windows.Forms.ToolStripMenuItem mnuTableDiem;

        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHowToUse;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;

        private System.Windows.Forms.DataGridView dgvData;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSelectDb = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLang = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLangEn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLangVi = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLangZhTw = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConfirmClose = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLockFullScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();

            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilterSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClearFilterSort = new System.Windows.Forms.ToolStripMenuItem();
            this.sepEdit1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAddRow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeleteRow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveChanges = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReload = new System.Windows.Forms.ToolStripMenuItem();
            this.sepEdit2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToggleEdit = new System.Windows.Forms.ToolStripMenuItem();

            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTableKhoa = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTableNganh = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTableLop = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTableMon = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTableHssv = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTableDiem = new System.Windows.Forms.ToolStripMenuItem();

            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHowToUse = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();

            this.dgvData = new System.Windows.Forms.DataGridView();

            this.menuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();

            // menuMain
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuFile,
                this.mnuEdit,
                this.mnuView,
                this.mnuHelp
            });
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Size = new System.Drawing.Size(960, 24);

            // File
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuSelectDb,
                this.mnuOptions,
                this.mnuExit
            });
            this.mnuFile.Text = "&File";

            this.mnuSelectDb.Text = "Select Database...";
            this.mnuSelectDb.Click += new System.EventHandler(this.mnuSelectDb_Click);

            this.mnuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuLang,
                this.mnuConfirmClose,
                this.mnuLockFullScreen
            });
            this.mnuOptions.Text = "Options";

            this.mnuLang.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuLangEn,
                this.mnuLangVi,
                this.mnuLangZhTw
            });
            this.mnuLang.Text = "Language";

            this.mnuLangEn.Checked = true;
            this.mnuLangEn.Text = "English";
            this.mnuLangEn.Click += new System.EventHandler(this.mnuLangEn_Click);

            this.mnuLangVi.Text = "Tiếng Việt";
            this.mnuLangVi.Click += new System.EventHandler(this.mnuLangVi_Click);

            this.mnuLangZhTw.Text = "繁體中文 (台灣)";
            this.mnuLangZhTw.Click += new System.EventHandler(this.mnuLangZhTw_Click);

            this.mnuConfirmClose.CheckOnClick = true;
            this.mnuConfirmClose.Text = "Confirm when closing (X)";

            this.mnuLockFullScreen.CheckOnClick = true;
            this.mnuLockFullScreen.Checked = true;
            this.mnuLockFullScreen.Text = "Lock Full-Screen";
            this.mnuLockFullScreen.CheckedChanged += new System.EventHandler(this.mnuLockFullScreen_CheckedChanged);

            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);

            // Edit
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuFilterSearch,
                this.mnuClearFilterSort,
                this.sepEdit1,
                this.mnuAddRow,
                this.mnuDeleteRow,
                this.mnuSaveChanges,
                this.mnuReload,
                this.sepEdit2,
                this.mnuToggleEdit
            });
            this.mnuEdit.Text = "&Edit";

            this.mnuFilterSearch.Text = "Filter / Search...";
            this.mnuFilterSearch.Click += new System.EventHandler(this.mnuFilterSearch_Click);

            this.mnuClearFilterSort.Text = "Clear Filter & Sort";
            this.mnuClearFilterSort.Click += new System.EventHandler(this.mnuClearFilterSort_Click);

            this.mnuAddRow.Text = "Add Row...";
            this.mnuAddRow.Click += new System.EventHandler(this.mnuAddRow_Click);

            this.mnuDeleteRow.Text = "Delete Row...";
            this.mnuDeleteRow.Click += new System.EventHandler(this.mnuDeleteRow_Click);

            this.mnuSaveChanges.Text = "Save Changes";
            this.mnuSaveChanges.Click += new System.EventHandler(this.mnuSaveChanges_Click);

            this.mnuReload.Text = "Reload / Discard";
            this.mnuReload.Click += new System.EventHandler(this.mnuReload_Click);

            this.mnuToggleEdit.CheckOnClick = true;
            this.mnuToggleEdit.Text = "Enable Cell Editing";
            this.mnuToggleEdit.CheckedChanged += new System.EventHandler(this.mnuToggleEdit_CheckedChanged);

            // View (Table switcher in top bar)
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuTableKhoa,
                this.mnuTableNganh,
                this.mnuTableLop,
                this.mnuTableMon,
                this.mnuTableHssv,
                this.mnuTableDiem
            });
            this.mnuView.Text = "&View";

            this.mnuTableKhoa.Text = "KHOA";
            this.mnuTableKhoa.Click += new System.EventHandler(this.mnuTable_Click);

            this.mnuTableNganh.Text = "NGANH";
            this.mnuTableNganh.Click += new System.EventHandler(this.mnuTable_Click);

            this.mnuTableLop.Text = "LOP";
            this.mnuTableLop.Click += new System.EventHandler(this.mnuTable_Click);

            this.mnuTableMon.Text = "MON";
            this.mnuTableMon.Click += new System.EventHandler(this.mnuTable_Click);

            this.mnuTableHssv.Text = "HSSV";
            this.mnuTableHssv.Click += new System.EventHandler(this.mnuTable_Click);

            this.mnuTableDiem.Text = "DIEM";
            this.mnuTableDiem.Click += new System.EventHandler(this.mnuTable_Click);

            // Help
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuHowToUse,
                this.mnuAbout
            });
            this.mnuHelp.Text = "&Help";

            this.mnuHowToUse.Text = "How to Use";
            this.mnuHowToUse.Click += new System.EventHandler(this.mnuHowToUse_Click);

            this.mnuAbout.Text = "About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);

            // dgvData
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(0, 24);
            this.dgvData.MultiSelect = false;
            this.dgvData.ReadOnly = true;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_CellEndEdit);
            this.dgvData.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvData_CurrentCellDirtyStateChanged);
            this.dgvData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_ColumnHeaderMouseClick);

            // Form1
            this.ClientSize = new System.Drawing.Size(960, 560);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.menuMain);
            this.MainMenuStrip = this.menuMain;
            this.MaximizeBox = false;
            this.Text = "QLSV Management Client (.NET Framework 4.7.2)";
            this.Load += new System.EventHandler(this.Form1_Load);

            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}