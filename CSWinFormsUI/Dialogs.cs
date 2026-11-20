using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CSWinFormsUI
{
    public class SelectDatabaseDialog : Form
    {
        private readonly TextBox _txtDb = new TextBox { Left = 14, Top = 36, Width = 250 };
        public string DatabaseName => _txtDb.Text.Trim();

        public SelectDatabaseDialog(string title, string prompt, string defaultValue, string okText, string cancelText)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(278, 112);
            MinimizeBox = MaximizeBox = false;
            ShowInTaskbar = false;
            _txtDb.Text = defaultValue ?? "CSDL_QLSV";

            var ok = new Button { Text = okText, Left = 108, Top = 70, Width = 75, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = cancelText, Left = 189, Top = 70, Width = 75, DialogResult = DialogResult.Cancel };
            Controls.AddRange(new Control[] { new Label { Text = prompt, Left = 14, Top = 12, AutoSize = true }, _txtDb, ok, cancel });
            AcceptButton = ok;
            CancelButton = cancel;
        }
    }

    public class FilterDialog : Form
    {
        private readonly ComboBox _cmbCol = new ComboBox { Left = 14, Top = 34, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox _cmbOp = new ComboBox { Left = 162, Top = 34, Width = 136, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox _txtVal = new TextBox { Left = 14, Top = 68, Width = 284 };

        public string SelectedColumn => _cmbCol.SelectedItem?.ToString() ?? "*";
        public int OperatorIndex => _cmbOp.SelectedIndex;
        public string FilterValue => _txtVal.Text.Trim();

        public FilterDialog(
            string title,
            IEnumerable<string> columns,
            string currentCol,
            int currentOpIndex,
            string currentVal,
            string[] opLabels,
            string okText,
            string clearText,
            string cancelText)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(312, 142);
            MinimizeBox = MaximizeBox = false;
            ShowInTaskbar = false;

            _cmbCol.Items.Add("*");
            foreach (var c in columns) _cmbCol.Items.Add(c);
            _cmbCol.SelectedItem = currentCol ?? "*";
            if (_cmbCol.SelectedIndex < 0) _cmbCol.SelectedIndex = 0;

            _cmbOp.Items.AddRange(opLabels);
            _cmbOp.SelectedIndex = currentOpIndex >= 0 && currentOpIndex < _cmbOp.Items.Count ? currentOpIndex : 0;

            _txtVal.Text = currentVal ?? "";

            var btnOk = new Button { Text = okText, Left = 62, Top = 102, Width = 75, DialogResult = DialogResult.OK };
            var btnClear = new Button { Text = clearText, Left = 143, Top = 102, Width = 75, DialogResult = DialogResult.Retry };
            var btnCancel = new Button { Text = cancelText, Left = 224, Top = 102, Width = 75, DialogResult = DialogResult.Cancel };

            Controls.AddRange(new Control[]
            {
                new Label { Text = title, Left = 14, Top = 10, AutoSize = true },
                _cmbCol, _cmbOp, _txtVal, btnOk, btnClear, btnCancel
            });

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }

    public class AddRowDialog : Form
    {
        private readonly Dictionary<string, Control> _inputs = new Dictionary<string, Control>();
        private readonly DataTable _schema;

        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();

        public AddRowDialog(string title, DataTable schema, string okText, string cancelText)
        {
            _schema = schema;
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = MaximizeBox = false;
            ShowInTaskbar = false;
            AutoScroll = true;

            int y = 14;
            foreach (DataColumn col in schema.Columns)
            {
                var lbl = new Label { Text = col.ColumnName, Left = 14, Top = y + 3, Width = 110 };
                Control editor;

                if (col.DataType == typeof(bool))
                {
                    editor = new CheckBox { Left = 132, Top = y, Width = 200 };
                }
                else if (col.DataType == typeof(DateTime))
                {
                    editor = new DateTimePicker { Left = 132, Top = y, Width = 200, Format = DateTimePickerFormat.Short };
                }
                else
                {
                    editor = new TextBox { Left = 132, Top = y, Width = 200 };
                }

                _inputs[col.ColumnName] = editor;
                Controls.Add(lbl);
                Controls.Add(editor);
                y += 32;
            }

            ClientSize = new Size(348, Math.Min(y + 52, 460));

            var btnOk = new Button { Text = okText, Left = 176, Top = y + 8, Width = 75 };
            var btnCancel = new Button { Text = cancelText, Left = 257, Top = y + 8, Width = 75, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;

            Controls.AddRange(new Control[] { btnOk, btnCancel });
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            foreach (DataColumn col in _schema.Columns)
            {
                Control ctrl = _inputs[col.ColumnName];
                if (col.DataType == typeof(bool))
                {
                    Values[col.ColumnName] = ((CheckBox)ctrl).Checked;
                }
                else if (col.DataType == typeof(DateTime))
                {
                    Values[col.ColumnName] = ((DateTimePicker)ctrl).Value.Date;
                }
                else
                {
                    string txt = ((TextBox)ctrl).Text.Trim();
                    if (string.IsNullOrEmpty(txt))
                    {
                        Values[col.ColumnName] = DBNull.Value;
                    }
                    else if (col.DataType == typeof(double) || col.DataType == typeof(decimal))
                    {
                        if (!double.TryParse(txt, out double num))
                        {
                            MessageBox.Show($"Invalid number for {col.ColumnName}.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        Values[col.ColumnName] = num;
                    }
                    else
                    {
                        Values[col.ColumnName] = txt;
                    }
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}