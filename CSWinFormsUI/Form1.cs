using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace CSWinFormsUI
{
    public partial class Form1 : Form
    {
        private readonly string[] _tables = { "KHOA", "NGANH", "LOP", "MON", "HSSV", "DIEM" };
        private DataTable _currentTable;
        private string _currentTableName = "KHOA";
        private bool _suppressSelectionPrompt;
        private string _currentDbName = "CSDL_QLSV";
        private string _lang = "en";
        private string _sortExpression = "";

        private string _filterCol = "*";
        private int _filterOpIndex = 0;
        private string _filterVal = "";
        private Process _backendProcess;
        public Form1()
        {
            InitializeComponent();
            this.MaximizeBox = !mnuLockFullScreen.Checked;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            SetLanguage(_lang);
            UpdateTableMenuChecks();
            await EnsureBackendRunningAsync();
            await LoadSelectedTableAsync();
        }

        // --- File & Options ---

        private void mnuSelectDb_Click(object sender, EventArgs e)
        {
            using (var dlg = new SelectDatabaseDialog(
                T("Select Database", "Chọn cơ sở dữ liệu", "選擇資料庫"),
                T("Database name:", "Tên cơ sở dữ liệu:", "資料庫名稱："),
                _currentDbName ?? "CSDL_QLSV",
                T("OK", "Đồng ý", "確定"),
                T("Cancel", "Hủy", "取消")))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.DatabaseName))
                {
                    _currentDbName = dlg.DatabaseName;
                    UpdateWindowTitle();
                }
            }
        }

        private void mnuLangEn_Click(object sender, EventArgs e) => SetLanguage("en");
        private void mnuLangVi_Click(object sender, EventArgs e) => SetLanguage("vi");
        private void mnuLangZhTw_Click(object sender, EventArgs e) => SetLanguage("zh-TW");
        private void mnuExit_Click(object sender, EventArgs e) => Close();

        private void mnuLockFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            this.MaximizeBox = !mnuLockFullScreen.Checked;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (mnuConfirmClose != null && mnuConfirmClose.Checked)
            {
                var res = MessageBox.Show(
                    T("Are you sure you want to close the application?",
                      "Bạn có chắc muốn đóng ứng dụng không?",
                      "確定要關閉應用程式嗎？"),
                    T("Confirm Close", "Xác nhận đóng", "確認關閉"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (res != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            StopBackendProcessIfOwned();
            base.OnFormClosing(e);
        }

        private void SetLanguage(string lang)
        {
            _lang = lang;
            mnuLangEn.Checked = lang == "en";
            mnuLangVi.Checked = lang == "vi";
            mnuLangZhTw.Checked = lang == "zh-TW";

            mnuFile.Text = T("&File", "&Tệp", "檔案(&F)");
            mnuSelectDb.Text = T("Select Database...", "Chọn cơ sở dữ liệu...", "選擇資料庫...");
            mnuOptions.Text = T("Options", "Tùy chọn", "選項");
            mnuLang.Text = T("Language", "Ngôn ngữ", "語言");
            mnuConfirmClose.Text = T("Confirm when closing (X)", "Xác nhận khi đóng (X)", "關閉視窗時確認 (X)");
            mnuLockFullScreen.Text = T("Lock Full-Screen", "Khóa toàn màn hình", "鎖定全螢幕");
            mnuExit.Text = T("Exit", "Thoát", "結束");

            mnuEdit.Text = T("&Edit", "&Chỉnh sửa", "編輯(&E)");
            mnuFilterSearch.Text = T("Filter / Search...", "Lọc / Tìm kiếm...", "篩選 / 搜尋...");
            mnuClearFilterSort.Text = T("Clear Filter & Sort", "Xóa lọc & sắp xếp", "清除篩選與排序");
            mnuAddRow.Text = T("Add Row...", "Thêm dòng...", "新增列...");
            mnuDeleteRow.Text = T("Delete Row...", "Xóa dòng...", "刪除列...");
            mnuSaveChanges.Text = T("Save Changes", "Lưu thay đổi", "儲存變更");
            mnuReload.Text = T("Reload / Discard", "Tải lại / Hủy", "重新載入 / 放棄");
            mnuToggleEdit.Text = T("Enable Cell Editing", "Cho phép sửa ô", "啟用儲存格編輯");

            mnuView.Text = T("&View", "&Xem", "檢視(&V)");
            mnuHelp.Text = T("&Help", "&Trợ giúp", "說明(&H)");
            mnuHowToUse.Text = T("How to Use", "Hướng dẫn sử dụng", "使用說明");
            mnuAbout.Text = T("About", "Giới thiệu", "關於");

            UpdateWindowTitle();
        }

        private string T(string en, string vi, string zhTw) =>
            _lang == "vi" ? vi : (_lang == "zh-TW" ? zhTw : en);

        private bool EnsureDatabaseAndTableSelected()
        {
            if (string.IsNullOrWhiteSpace(_currentDbName) || _currentTable == null)
            {
                MessageBox.Show(
                    T("No database or table selected. Use File -> Select Database... or see Help -> How to Use.",
                      "Chưa chọn cơ sở dữ liệu hoặc bảng. Vào File -> Chọn cơ sở dữ liệu... hoặc xem Help -> Hướng dẫn sử dụng.",
                      "尚未選擇資料庫或資料表。請至 File -> 選擇資料庫...，或查看 Help -> 使用說明。"),
                    T("Selection Required", "Yêu cầu chọn", "需要選擇"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        // --- View (Table Switcher) ---

        private async void mnuTable_Click(object sender, EventArgs e)
        {
            if (_suppressSelectionPrompt || !(sender is ToolStripMenuItem item)) return;
            string nextTable = item.Text;
            if (string.IsNullOrEmpty(nextTable) || nextTable == _currentTableName) return;

            if (HasPendingChanges())
            {
                var result = MessageBox.Show(
                    T("You have unsaved changes in the current table. Discard them and switch?",
                      "Bạn có thay đổi chưa lưu trong bảng hiện tại. Hủy bỏ và chuyển bảng?",
                      "目前資料表有未儲存的變更，要捨棄並切換嗎？"),
                    T("Unsaved Changes", "Thay đổi chưa lưu", "未儲存的變更"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;
            }

            _currentTableName = nextTable;
            UpdateTableMenuChecks();
            await LoadSelectedTableAsync();
        }

        private void UpdateTableMenuChecks()
        {
            mnuTableKhoa.Checked = _currentTableName == "KHOA";
            mnuTableNganh.Checked = _currentTableName == "NGANH";
            mnuTableLop.Checked = _currentTableName == "LOP";
            mnuTableMon.Checked = _currentTableName == "MON";
            mnuTableHssv.Checked = _currentTableName == "HSSV";
            mnuTableDiem.Checked = _currentTableName == "DIEM";
        }

        // --- Edit (Filter/Search, Add, Delete, Save, Reload, Edit Mode) ---

        private void mnuFilterSearch_Click(object sender, EventArgs e)
        {
            if (_currentTable == null) return;

            var cols = new List<string>();
            foreach (DataColumn c in _currentTable.Columns) cols.Add(c.ColumnName);

            var opLabels = new[]
            {
                T("Contains", "Chứa", "包含"),
                T("Equals", "Bằng", "等於"),
                T("Starts With", "Bắt đầu bằng", "開頭為")
            };

            using (var dlg = new FilterDialog(
                T("Filter / Search", "Lọc / Tìm kiếm", "篩選 / 搜尋"),
                cols,
                _filterCol,
                _filterOpIndex,
                _filterVal,
                opLabels,
                T("Apply", "Áp dụng", "套用"),
                T("Clear", "Xóa", "清除"),
                T("Cancel", "Hủy", "取消")))
            {
                var res = dlg.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    _filterCol = dlg.SelectedColumn;
                    _filterOpIndex = dlg.OperatorIndex;
                    _filterVal = dlg.FilterValue;
                    ApplyFilterAndSort();
                }
                else if (res == DialogResult.Retry)
                {
                    ResetSortAndFilter();
                }
            }
        }

        private void mnuClearFilterSort_Click(object sender, EventArgs e)
        {
            ResetSortAndFilter();
        }

        private void mnuAddRow_Click(object sender, EventArgs e)
        {
            if (_currentTable == null) return;
            if (!EnsureDatabaseAndTableSelected()) return;

            using (var dlg = new AddRowDialog(
                $"{T("Add Row", "Thêm dòng", "新增列")} - {_currentTable.TableName}",
                _currentTable,
                T("Add", "Thêm", "新增"),
                T("Cancel", "Hủy", "取消")))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                DataRow row = _currentTable.NewRow();
                foreach (var kvp in dlg.Values)
                {
                    row[kvp.Key] = kvp.Value ?? DBNull.Value;
                }

                _currentTable.Rows.Add(row);
                UpdateWindowTitle();

                if (dgvData.Rows.Count > 0)
                {
                    int newIdx = dgvData.Rows.Count - 1;
                    dgvData.ClearSelection();
                    dgvData.Rows[newIdx].Selected = true;
                    dgvData.CurrentCell = dgvData.Rows[newIdx].Cells[0];
                }
            }
        }

        private void mnuDeleteRow_Click(object sender, EventArgs e)
        {
            if (_currentTable == null || dgvData.CurrentRow == null) return;
            if (!EnsureDatabaseAndTableSelected()) return;

            var confirm = MessageBox.Show(
                T("Delete the selected row?", "Xóa dòng đang chọn?", "要刪除選取的資料列嗎？"),
                T("Confirm Delete", "Xác nhận xóa", "確認刪除"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            if (dgvData.CurrentRow.DataBoundItem is DataRowView drv)
            {
                drv.Row.Delete();
                UpdateWindowTitle();
            }
        }

        private async void mnuSaveChanges_Click(object sender, EventArgs e)
        {
            if (_currentTable == null) return;
            if (!EnsureDatabaseAndTableSelected()) return;

            dgvData.EndEdit();

            DataTable changes = _currentTable.GetChanges();
            if (changes == null)
            {
                MessageBox.Show(
                    T("No pending changes to save.", "Không có thay đổi nào để lưu.", "沒有待儲存的變更。"),
                    T("Info", "Thông tin", "提示"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            SetControlsEnabled(false);
            this.Text = T("Saving changes...", "Đang lưu thay đổi...", "正在儲存變更...");

            try
            {
                string tableName = _currentTable.TableName;

                foreach (DataRow row in changes.Select(null, null, DataViewRowState.Deleted))
                    await DeleteRowInApiAsync(tableName, row);

                foreach (DataRow row in changes.Select(null, null, DataViewRowState.Added))
                    await CreateRowInApiAsync(tableName, row);

                foreach (DataRow row in changes.Select(null, null, DataViewRowState.ModifiedCurrent))
                    await UpdateRowInApiAsync(tableName, row);

                _currentTable.AcceptChanges();
                UpdateWindowTitle();
                MessageBox.Show(
                    T("All changes saved to SQL Server via API.", "Đã lưu tất cả thay đổi vào SQL Server qua API.", "已透過 API 將所有變更儲存至 SQL Server。"),
                    T("Saved", "Đã lưu", "已儲存"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{T("Save failed", "Lưu thất bại", "儲存失敗")}: {ex.Message}\n{T("Local changes remain staged.", "Các thay đổi cục bộ vẫn được giữ lại.", "本機變更仍保留在暫存中。")}",
                    T("Error", "Lỗi", "錯誤"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UpdateWindowTitle();
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private async void mnuReload_Click(object sender, EventArgs e)
        {
            await LoadSelectedTableAsync();
        }

        private void mnuToggleEdit_CheckedChanged(object sender, EventArgs e)
        {
            dgvData.ReadOnly = !mnuToggleEdit.Checked;
            dgvData.SelectionMode = mnuToggleEdit.Checked
                ? DataGridViewSelectionMode.RowHeaderSelect
                : DataGridViewSelectionMode.FullRowSelect;
            UpdateWindowTitle();
        }

        private void dgvData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvData.IsCurrentCellDirty)
                dgvData.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateWindowTitle();
        }

        // --- Filtering & Sorting ---

        private void ApplyFilterAndSort()
        {
            if (_currentTable == null) return;
            DataView dv = _currentTable.DefaultView;

            string rawTrimmed = (_filterVal ?? "").Trim();
            if (string.IsNullOrEmpty(rawTrimmed))
            {
                dv.RowFilter = "";
            }
            else
            {
                string escapedLike = EscapeLike(rawTrimmed);
                string escapedExact = rawTrimmed.Replace("'", "''");

                if (_filterCol == "*")
                {
                    var parts = new List<string>();
                    foreach (DataColumn c in _currentTable.Columns)
                    {
                        if (c.DataType == typeof(string))
                            parts.Add(BuildStringPredicate($"[{c.ColumnName}]", _filterOpIndex, escapedLike, escapedExact));
                    }
                    dv.RowFilter = string.Join(" OR ", parts);
                }
                else
                {
                    DataColumn targetCol = _currentTable.Columns[_filterCol];
                    if (targetCol != null)
                    {
                        if (targetCol.DataType == typeof(string))
                        {
                            dv.RowFilter = BuildStringPredicate($"[{_filterCol}]", _filterOpIndex, escapedLike, escapedExact);
                        }
                        else if (_filterOpIndex == 1)
                        {
                            dv.RowFilter = $"CONVERT([{_filterCol}], 'System.String') = '{escapedExact}'";
                        }
                        else if (_filterOpIndex == 2)
                        {
                            dv.RowFilter = $"CONVERT([{_filterCol}], 'System.String') LIKE '{escapedLike}%'";
                        }
                        else
                        {
                            dv.RowFilter = $"CONVERT([{_filterCol}], 'System.String') LIKE '%{escapedLike}%'";
                        }
                    }
                }
            }

            dv.Sort = _sortExpression;
            UpdateWindowTitle();
        }

        private string BuildStringPredicate(string colExpr, int opIndex, string escapedLike, string escapedExact)
        {
            if (opIndex == 1) return $"{colExpr} = '{escapedExact}'";
            if (opIndex == 2) return $"{colExpr} LIKE '{escapedLike}%'";
            return $"{colExpr} LIKE '%{escapedLike}%'";
        }

        private void ResetSortAndFilter()
        {
            _filterCol = "*";
            _filterOpIndex = 0;
            _filterVal = "";
            _sortExpression = "";
            if (_currentTable != null)
            {
                _currentTable.DefaultView.RowFilter = "";
                _currentTable.DefaultView.Sort = "";
            }
            UpdateWindowTitle();
        }

        private string EscapeLike(string value)
        {
            return value
                .Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("]", "[]]")
                .Replace("%", "[%]")
                .Replace("*", "[*]");
        }

        private void dgvData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_currentTable == null || e.ColumnIndex < 0) return;
            string colName = dgvData.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(colName)) return;

            if (_sortExpression == $"[{colName}] ASC")
                _sortExpression = $"[{colName}] DESC";
            else if (_sortExpression == $"[{colName}] DESC")
                _sortExpression = "";
            else
                _sortExpression = $"[{colName}] ASC";

            ApplyFilterAndSort();
        }

        // --- Help ---

        private void mnuHowToUse_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                T("1. Select a database via File -> Select Database... and pick a table under View.\n" +
                  "2. Use Edit -> Filter / Search... or click column headers to filter and sort.\n" +
                  "3. Use Edit -> Add Row..., Delete Row..., or enable cell editing, then Save Changes.",
                  "1. Chọn cơ sở dữ liệu qua File -> Chọn cơ sở dữ liệu... và chọn bảng dưới View.\n" +
                  "2. Dùng Edit -> Lọc / Tìm kiếm... hoặc bấm tiêu đề cột để lọc và sắp xếp.\n" +
                  "3. Dùng Edit -> Thêm dòng..., Xóa dòng..., hoặc bật sửa ô, rồi bấm Lưu thay đổi.",
                  "1. 從 File -> 選擇資料庫... 選擇資料庫，並從 View 切換資料表。\n" +
                  "2. 使用 Edit -> 篩選 / 搜尋... 或點擊欄位標題進行篩選與排序。\n" +
                  "3. 使用 Edit -> 新增列...、刪除列... 或啟用儲存格編輯，再按下儲存變更。"),
                T("How to Use", "Hướng dẫn sử dụng", "使用說明"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                T("CSDL_QLSV WinForms Client (.NET Framework 4.7.2)\nConnected to Spring Boot REST API (http://localhost:8080).",
                  "CSDL_QLSV WinForms Client (.NET Framework 4.7.2)\nKết nối tới Spring Boot REST API (http://localhost:8080).",
                  "CSDL_QLSV WinForms 用戶端（.NET Framework 4.7.2）\n連線至 Spring Boot REST API（http://localhost:8080）。"),
                T("About", "Giới thiệu", "關於"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // --- Data Loading & API Sync ---

        private async Task LoadSelectedTableAsync()
        {
            string tableName = _currentTableName ?? "KHOA";
            SetControlsEnabled(false);
            this.Text = $"{T("Loading", "Đang tải", "載入中")} {tableName}...";

            try
            {
                DataTable dt = await BuildDataTableFromApiAsync(tableName);
                _currentTable = dt;
                _sortExpression = "";
                _filterVal = "";
                dgvData.DataSource = _currentTable;
                ApplyFilterAndSort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{T("Failed to load", "Không thể tải", "無法載入")} {tableName}: {ex.Message}",
                    T("Error", "Lỗi", "錯誤"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UpdateWindowTitle();
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private async Task<DataTable> BuildDataTableFromApiAsync(string tableName)
        {
            var dt = new DataTable(tableName);

            switch (tableName)
            {
                case "KHOA":
                    dt.Columns.Add("MAKHOA", typeof(string));
                    dt.Columns.Add("TENKHOA", typeof(string));
                    dt.PrimaryKey = new[] { dt.Columns["MAKHOA"] };
                    foreach (var item in await ApiService.GetListAsync<KhoaDto>("api/khoa"))
                        dt.Rows.Add(item.MaKhoa, item.TenKhoa);
                    break;

                case "NGANH":
                    dt.Columns.Add("MANGANH", typeof(string));
                    dt.Columns.Add("TENGANH", typeof(string));
                    dt.PrimaryKey = new[] { dt.Columns["MANGANH"] };
                    foreach (var item in await ApiService.GetListAsync<NganhDto>("api/nganh"))
                        dt.Rows.Add(item.MaNganh, item.TenNganh);
                    break;

                case "LOP":
                    dt.Columns.Add("MALOP", typeof(string));
                    dt.Columns.Add("TENLOP", typeof(string));
                    dt.Columns.Add("MAKHOA", typeof(string));
                    dt.PrimaryKey = new[] { dt.Columns["MALOP"] };
                    foreach (var item in await ApiService.GetListAsync<LopDto>("api/lop"))
                        dt.Rows.Add(item.MaLop, item.TenLop, item.MaKhoa);
                    break;

                case "MON":
                    dt.Columns.Add("MAMON", typeof(string));
                    dt.Columns.Add("TENMON", typeof(string));
                    dt.Columns.Add("SOTINCHI", typeof(double));
                    dt.PrimaryKey = new[] { dt.Columns["MAMON"] };
                    foreach (var item in await ApiService.GetListAsync<MonDto>("api/mon"))
                        dt.Rows.Add(item.MaMon, item.TenMon, (object)item.SoTinChi ?? DBNull.Value);
                    break;

                case "HSSV":
                    dt.Columns.Add("MSV", typeof(string));
                    dt.Columns.Add("HODEM", typeof(string));
                    dt.Columns.Add("TEN", typeof(string));
                    dt.Columns.Add("GIOITINH", typeof(bool));
                    dt.Columns.Add("NGAYSINH", typeof(DateTime));
                    dt.Columns.Add("MANGANH", typeof(string));
                    dt.Columns.Add("MALOP", typeof(string));
                    dt.PrimaryKey = new[] { dt.Columns["MSV"] };
                    foreach (var item in await ApiService.GetListAsync<HssvDto>("api/hssv"))
                        dt.Rows.Add(
                            item.Msv,
                            item.HoDem,
                            item.Ten,
                            item.GioiTinh ?? false,
                            (object)item.NgaySinh ?? DBNull.Value,
                            item.MaNganh,
                            item.MaLop);
                    break;

                case "DIEM":
                    dt.Columns.Add("MSV", typeof(string));
                    dt.Columns.Add("MAMON", typeof(string));
                    dt.Columns.Add("DIEML1", typeof(double));
                    dt.Columns.Add("DIEML2", typeof(double));
                    dt.Columns.Add("GHICHU", typeof(string));
                    dt.PrimaryKey = new[] { dt.Columns["MSV"], dt.Columns["MAMON"] };
                    foreach (var item in await ApiService.GetListAsync<DiemDto>("api/diem"))
                        dt.Rows.Add(
                            item.Msv,
                            item.MaMon,
                            (object)item.DiemL1 ?? DBNull.Value,
                            (object)item.DiemL2 ?? DBNull.Value,
                            item.GhiChu);
                    break;
            }

            dt.AcceptChanges();
            return dt;
        }

        private async Task CreateRowInApiAsync(string tableName, DataRow row)
        {
            switch (tableName)
            {
                case "KHOA":
                    await ApiService.PostAsync("api/khoa", new KhoaDto { MaKhoa = GetString(row, "MAKHOA"), TenKhoa = GetString(row, "TENKHOA") });
                    break;
                case "NGANH":
                    await ApiService.PostAsync("api/nganh", new NganhDto { MaNganh = GetString(row, "MANGANH"), TenNganh = GetString(row, "TENGANH") });
                    break;
                case "LOP":
                    await ApiService.PostAsync("api/lop", new LopDto { MaLop = GetString(row, "MALOP"), TenLop = GetString(row, "TENLOP"), MaKhoa = GetString(row, "MAKHOA") });
                    break;
                case "MON":
                    await ApiService.PostAsync("api/mon", new MonDto { MaMon = GetString(row, "MAMON"), TenMon = GetString(row, "TENMON"), SoTinChi = GetNullableDouble(row, "SOTINCHI") });
                    break;
                case "HSSV":
                    await ApiService.PostAsync("api/hssv", new HssvDto { Msv = GetString(row, "MSV"), HoDem = GetString(row, "HODEM"), Ten = GetString(row, "TEN"), GioiTinh = GetBool(row, "GIOITINH"), NgaySinh = GetNullableDateTime(row, "NGAYSINH"), MaNganh = GetString(row, "MANGANH"), MaLop = GetString(row, "MALOP") });
                    break;
                case "DIEM":
                    await ApiService.PostAsync("api/diem", new DiemDto { Msv = GetString(row, "MSV"), MaMon = GetString(row, "MAMON"), DiemL1 = GetNullableDouble(row, "DIEML1"), DiemL2 = GetNullableDouble(row, "DIEML2"), GhiChu = GetString(row, "GHICHU") });
                    break;
            }
        }

        private async Task UpdateRowInApiAsync(string tableName, DataRow row)
        {
            switch (tableName)
            {
                case "KHOA":
                    await ApiService.PutAsync($"api/khoa/{Uri.EscapeDataString(GetString(row, "MAKHOA", DataRowVersion.Original))}", new KhoaDto { MaKhoa = GetString(row, "MAKHOA"), TenKhoa = GetString(row, "TENKHOA") });
                    break;
                case "NGANH":
                    await ApiService.PutAsync($"api/nganh/{Uri.EscapeDataString(GetString(row, "MANGANH", DataRowVersion.Original))}", new NganhDto { MaNganh = GetString(row, "MANGANH"), TenNganh = GetString(row, "TENGANH") });
                    break;
                case "LOP":
                    await ApiService.PutAsync($"api/lop/{Uri.EscapeDataString(GetString(row, "MALOP", DataRowVersion.Original))}", new LopDto { MaLop = GetString(row, "MALOP"), TenLop = GetString(row, "TENLOP"), MaKhoa = GetString(row, "MAKHOA") });
                    break;
                case "MON":
                    await ApiService.PutAsync($"api/mon/{Uri.EscapeDataString(GetString(row, "MAMON", DataRowVersion.Original))}", new MonDto { MaMon = GetString(row, "MAMON"), TenMon = GetString(row, "TENMON"), SoTinChi = GetNullableDouble(row, "SOTINCHI") });
                    break;
                case "HSSV":
                    await ApiService.PutAsync($"api/hssv/{Uri.EscapeDataString(GetString(row, "MSV", DataRowVersion.Original))}", new HssvDto { Msv = GetString(row, "MSV"), HoDem = GetString(row, "HODEM"), Ten = GetString(row, "TEN"), GioiTinh = GetBool(row, "GIOITINH"), NgaySinh = GetNullableDateTime(row, "NGAYSINH"), MaNganh = GetString(row, "MANGANH"), MaLop = GetString(row, "MALOP") });
                    break;
                case "DIEM":
                    string dMsv = Uri.EscapeDataString(GetString(row, "MSV", DataRowVersion.Original));
                    string dMaMon = Uri.EscapeDataString(GetString(row, "MAMON", DataRowVersion.Original));
                    await ApiService.PutAsync($"api/diem/item?msv={dMsv}&mamon={dMaMon}", new DiemDto { Msv = GetString(row, "MSV"), MaMon = GetString(row, "MAMON"), DiemL1 = GetNullableDouble(row, "DIEML1"), DiemL2 = GetNullableDouble(row, "DIEML2"), GhiChu = GetString(row, "GHICHU") });
                    break;
            }
        }

        private async Task DeleteRowInApiAsync(string tableName, DataRow row)
        {
            switch (tableName)
            {
                case "KHOA":
                    await ApiService.DeleteAsync($"api/khoa/{Uri.EscapeDataString(GetString(row, "MAKHOA", DataRowVersion.Original))}");
                    break;
                case "NGANH":
                    await ApiService.DeleteAsync($"api/nganh/{Uri.EscapeDataString(GetString(row, "MANGANH", DataRowVersion.Original))}");
                    break;
                case "LOP":
                    await ApiService.DeleteAsync($"api/lop/{Uri.EscapeDataString(GetString(row, "MALOP", DataRowVersion.Original))}");
                    break;
                case "MON":
                    await ApiService.DeleteAsync($"api/mon/{Uri.EscapeDataString(GetString(row, "MAMON", DataRowVersion.Original))}");
                    break;
                case "HSSV":
                    await ApiService.DeleteAsync($"api/hssv/{Uri.EscapeDataString(GetString(row, "MSV", DataRowVersion.Original))}");
                    break;
                case "DIEM":
                    string msv = Uri.EscapeDataString(GetString(row, "MSV", DataRowVersion.Original));
                    string maMon = Uri.EscapeDataString(GetString(row, "MAMON", DataRowVersion.Original));
                    await ApiService.DeleteAsync($"api/diem/item?msv={msv}&mamon={maMon}");
                    break;
            }
        }

        // --- Status & Helpers ---

        private bool HasPendingChanges() => _currentTable != null && _currentTable.GetChanges() != null;

        private void UpdateWindowTitle()
        {
            string dbLabel = string.IsNullOrWhiteSpace(_currentDbName)
                ? T("No DB", "Chưa chọn CSDL", "未選資料庫")
                : _currentDbName;

            string mode = mnuToggleEdit.Checked
                ? T("Edit", "Sửa", "編輯")
                : T("Read-Only", "Chỉ đọc", "唯讀");

            if (_currentTable == null)
            {
                this.Text = $"QLSV [{dbLabel}] [{mode}]";
                return;
            }

            int added = _currentTable.Select(null, null, DataViewRowState.Added).Length;
            int modified = _currentTable.Select(null, null, DataViewRowState.ModifiedCurrent).Length;
            int deleted = _currentTable.Select(null, null, DataViewRowState.Deleted).Length;
            int totalPending = added + modified + deleted;

            string counts = totalPending == 0
                ? $"{_currentTable.DefaultView.Count}/{_currentTable.Rows.Count}"
                : $"+{added} ~{modified} -{deleted}";

            this.Text = $"QLSV - {_currentTable.TableName} [{dbLabel}] [{mode}] ({counts})";
        }

        private void SetControlsEnabled(bool enabled)
        {
            menuMain.Enabled = enabled;
            dgvData.Enabled = enabled;
        }

        private static string GetString(DataRow row, string col, DataRowVersion version = DataRowVersion.Current)
        {
            object val = row[col, version];
            return val == null || val == DBNull.Value ? null : val.ToString();
        }

        private static bool GetBool(DataRow row, string col)
        {
            object val = row[col];
            return val != null && val != DBNull.Value && Convert.ToBoolean(val, CultureInfo.InvariantCulture);
        }

        private static double? GetNullableDouble(DataRow row, string col)
        {
            object val = row[col];
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()))
                return null;
            return Convert.ToDouble(val, CultureInfo.InvariantCulture);
        }

        private static DateTime? GetNullableDateTime(DataRow row, string col)
        {
            object val = row[col];
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()))
                return null;
            if (val is DateTime dt)
                return dt;
            if (DateTime.TryParse(val.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
                return parsed;
            return null;
        }
        private async Task EnsureBackendRunningAsync()
        {
            if (IsPortOpen("127.0.0.1", 8080))
                return;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string jarPath = Path.Combine(baseDir, "JavaCRUD-1.0.jar");
            if (!File.Exists(jarPath))
            {
                MessageBox.Show(
                    "Backend JAR not found:\n" + jarPath,
                    "Backend Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string javaExe = ResolveJavaExecutable(baseDir);
            if (string.IsNullOrEmpty(javaExe))
            {
                MessageBox.Show(
                    "Could not find java.exe in jdk-17.0.12, JAVA_HOME, or PATH next to:\n" + baseDir,
                    "Java Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = javaExe,
                    Arguments = "-jar \"" + jarPath + "\"",
                    WorkingDirectory = baseDir,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                _backendProcess = Process.Start(psi);

                for (int i = 0; i < 60; i++)
                {
                    if (IsPortOpen("127.0.0.1", 8080))
                        return;
                    await Task.Delay(250);
                }

                MessageBox.Show(
                    "Started Java, but port 8080 did not open in time.",
                    "Backend Timeout",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to start backend JAR:\n" + ex.Message,
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static string ResolveJavaExecutable(string baseDir)
        {
            string[] directCandidates =
            {
        Path.Combine(baseDir, "jdk-17.0.12", "bin", "java.exe"),
        Path.Combine(baseDir, "jdk17", "bin", "java.exe"),
        Path.Combine(baseDir, "jdk17", "jdk-17.0.12", "bin", "java.exe")
    };

            foreach (string candidate in directCandidates)
            {
                if (File.Exists(candidate))
                    return candidate;
            }

            string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrWhiteSpace(javaHome))
            {
                string envJava = Path.Combine(javaHome.Trim(), "bin", "java.exe");
                if (File.Exists(envJava))
                    return envJava;
            }

            if (CanExecute("java"))
                return "java";

            return null;
        }

        private static bool CanExecute(string fileName)
        {
            try
            {
                var check = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = "-version",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };
                using (var p = Process.Start(check))
                {
                    p?.WaitForExit(2000);
                    return p != null;
                }
            }
            catch
            {
                return false;
            }
        }

        private void StopBackendProcessIfOwned()
        {
            try
            {
                if (_backendProcess != null && !_backendProcess.HasExited)
                {
                    _backendProcess.Kill();
                    _backendProcess.WaitForExit(2000);
                }
            }
            catch
            {
                // Ignore cleanup errors on exit
            }
            finally
            {
                if (_backendProcess != null)
                {
                    _backendProcess.Dispose();
                    _backendProcess = null;
                }
            }
        }

        private static bool IsPortOpen(string host, int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    bool connected = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(250));
                    if (!connected) return false;
                    client.EndConnect(result);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}