using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace baithuchanh2
{
    public partial class frm_lop : Form
    {
        private readonly BindingList<Lop> _classes = new BindingList<Lop>();
        private readonly BindingSource _source = new BindingSource();

        public frm_lop()
        {
            InitializeComponent();
        }

        private void frm_lop_Load(object sender, EventArgs e)
        {
            _source.DataSource = _classes;
            dgvLop.AutoGenerateColumns = false;
            dgvLop.DataSource = _source;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out string message))
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_classes.Any(c => c.MaLop.Equals(txtMaLop.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã lớp đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _classes.Add(new Lop
            {
                MaLop = txtMaLop.Text.Trim(),
                TenLop = txtTenLop.Text.Trim(),
                Khoa = txtKhoa.Text.Trim(),
                SiSo = (int)numSiSo.Value
            });

            ClearForm();
        }

        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            if (dgvLop.CurrentRow == null)
            {
                MessageBox.Show("Chọn một lớp để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateInputs(out string message))
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvLop.CurrentRow.DataBoundItem is Lop lop)
            {
                lop.MaLop = txtMaLop.Text.Trim();
                lop.TenLop = txtTenLop.Text.Trim();
                lop.Khoa = txtKhoa.Text.Trim();
                lop.SiSo = (int)numSiSo.Value;
                dgvLop.Refresh();
                ClearForm();
            }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            LoadSelectedToForm();
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvLop.CurrentRow == null)
            {
                MessageBox.Show("Chọn một lớp để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvLop.CurrentRow.DataBoundItem is Lop lop)
            {
                var confirm = MessageBox.Show($"Xóa lớp {lop.TenLop}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    _classes.Remove(lop);
                    ClearForm();
                }
            }
        }

        private void DgvLop_SelectionChanged(object sender, EventArgs e)
        {
            LoadSelectedToForm();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                dgvLop.ClearSelection();
                return;
            }

            var match = _classes.FirstOrDefault(c =>
                c.MaLop.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.TenLop.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.Khoa.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            if (match != null)
            {
                int index = _classes.IndexOf(match);
                if (index >= 0)
                {
                    dgvLop.ClearSelection();
                    dgvLop.Rows[index].Selected = true;
                    dgvLop.FirstDisplayedScrollingRowIndex = index;
                    LoadSelectedToForm();
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy lớp phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadSelectedToForm()
        {
            if (dgvLop.CurrentRow?.DataBoundItem is Lop lop)
            {
                txtMaLop.Text = lop.MaLop;
                txtTenLop.Text = lop.TenLop;
                txtKhoa.Text = lop.Khoa;
                numSiSo.Value = lop.SiSo < numSiSo.Minimum ? numSiSo.Minimum : lop.SiSo;
            }
        }

        private bool ValidateInputs(out string message)
        {
            if (string.IsNullOrWhiteSpace(txtMaLop.Text))
            {
                message = "Mã lớp không được để trống.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTenLop.Text))
            {
                message = "Tên lớp không được để trống.";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private void ClearForm()
        {
            txtMaLop.Clear();
            txtTenLop.Clear();
            txtKhoa.Clear();
            numSiSo.Value = 0;
            txtMaLop.Focus();
        }

        private class Lop
        {
            public string MaLop { get; set; }
            public string TenLop { get; set; }
            public string Khoa { get; set; }
            public int SiSo { get; set; }
        }
    }
}
