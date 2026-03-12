using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace baithuchanh2
{
    public partial class frm_qlsv : Form
    {
        private readonly BindingList<Student> _students = new BindingList<Student>();
        private readonly BindingSource _source = new BindingSource();

        public frm_qlsv()
        {
            InitializeComponent();
        }

        private void frm_qlsv_Load(object sender, EventArgs e)
        {
            _source.DataSource = _students;
            dgvSinhVien.AutoGenerateColumns = false;
            dgvSinhVien.DataSource = _source;

            cboGioiTinh.Items.AddRange(new[] { "Nam", "Nữ", "Khác" });
            if (cboGioiTinh.Items.Count > 0)
                cboGioiTinh.SelectedIndex = 0;

            dtNgaySinh.Value = DateTime.Today;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out string message))
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_students.Any(s => s.MaSV.Equals(txtMaSV.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sinh viên đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _students.Add(new Student
            {
                MaSV = txtMaSV.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtNgaySinh.Value.Date,
                GioiTinh = cboGioiTinh.Text,
                Lop = txtLop.Text.Trim()
            });

            ClearForm();
        }

        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow == null)
            {
                MessageBox.Show("Chọn một sinh viên để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateInputs(out string message))
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvSinhVien.CurrentRow.DataBoundItem is Student student)
            {
                student.MaSV = txtMaSV.Text.Trim();
                student.HoTen = txtHoTen.Text.Trim();
                student.NgaySinh = dtNgaySinh.Value.Date;
                student.GioiTinh = cboGioiTinh.Text;
                student.Lop = txtLop.Text.Trim();
                dgvSinhVien.Refresh();
                ClearForm();
            }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            LoadSelectedToForm();
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow == null)
            {
                MessageBox.Show("Chọn một sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvSinhVien.CurrentRow.DataBoundItem is Student student)
            {
                var confirm = MessageBox.Show($"Xóa sinh viên {student.HoTen}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    _students.Remove(student);
                    ClearForm();
                }
            }
        }

        private void DgvSinhVien_SelectionChanged(object sender, EventArgs e)
        {
            LoadSelectedToForm();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                dgvSinhVien.ClearSelection();
                return;
            }

            var match = _students.FirstOrDefault(s =>
                s.MaSV.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                s.HoTen.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                s.Lop.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            if (match != null)
            {
                int index = _students.IndexOf(match);
                if (index >= 0)
                {
                    dgvSinhVien.ClearSelection();
                    dgvSinhVien.Rows[index].Selected = true;
                    dgvSinhVien.FirstDisplayedScrollingRowIndex = index;
                    LoadSelectedToForm();
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadSelectedToForm()
        {
            if (dgvSinhVien.CurrentRow?.DataBoundItem is Student student)
            {
                txtMaSV.Text = student.MaSV;
                txtHoTen.Text = student.HoTen;
                dtNgaySinh.Value = student.NgaySinh == DateTime.MinValue ? DateTime.Today : student.NgaySinh;
                cboGioiTinh.SelectedItem = student.GioiTinh;
                if (cboGioiTinh.SelectedItem == null && cboGioiTinh.Items.Count > 0)
                    cboGioiTinh.SelectedIndex = 0;
                txtLop.Text = student.Lop;
            }
        }

        private bool ValidateInputs(out string message)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                message = "Mã sinh viên không được để trống.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                message = "Họ tên không được để trống.";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private void ClearForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtLop.Clear();
            dtNgaySinh.Value = DateTime.Today;
            if (cboGioiTinh.Items.Count > 0) cboGioiTinh.SelectedIndex = 0;
            txtMaSV.Focus();
        }

        private class Student
        {
            public string MaSV { get; set; }
            public string HoTen { get; set; }
            public DateTime NgaySinh { get; set; }
            public string GioiTinh { get; set; }
            public string Lop { get; set; }
        }
    }
}
