using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace baithuchanh2
{
    public partial class frm_qlsv : Form
    {
        private readonly string _connString = ConfigurationManager.ConnectionStrings["QLSV"].ConnectionString;
        private DataTable _table = new DataTable();

        public frm_qlsv()
        {
            InitializeComponent();
        }

        private void frm_qlsv_Load(object sender, EventArgs e)
        {
            LoadData();
            // vô hiệu hóa các chức năng sửa/ghi
            btnThem.Enabled = btnCapNhat.Enabled = btnSua.Enabled = btnXoa.Enabled = false;
        }

        private void LoadData(string keyword = "")
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT MaSV, HoTen, NgaySinh, GioiTinh, MaLop FROM SinhVien
                                     WHERE (@kw = '' OR MaSV LIKE @kwLike OR HoTen LIKE @kwLike OR MaLop LIKE @kwLike)";
                cmd.Parameters.AddWithValue("@kw", keyword);
                cmd.Parameters.AddWithValue("@kwLike", "%" + keyword + "%");
                var da = new SqlDataAdapter(cmd);
                _table = new DataTable();
                da.Fill(_table);
                dgvSinhVien.AutoGenerateColumns = false;
                dgvSinhVien.DataSource = _table;
            }
        }

        private void LoadLopToCombo()
        {
            using (var conn = new SqlConnection(_connString))
            using (var da = new SqlDataAdapter("SELECT MaLop FROM Lop", conn))
            {
                var dt = new DataTable();
                da.Fill(dt);
                cboGioiTinh.Items.Clear(); // giữ nguyên cho giới tính, combo lớp chưa có; không dùng.
            }
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng thêm đã tắt (chỉ xem và tìm kiếm).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng cập nhật đã tắt (chỉ xem và tìm kiếm).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng sửa đã tắt (chỉ xem và tìm kiếm).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xóa đã tắt (chỉ xem và tìm kiếm).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DgvSinhVien_SelectionChanged(object sender, EventArgs e)
        {
            LoadSelectedToForm();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim());
        }

        private void LoadSelectedToForm()
        {
            if (dgvSinhVien.CurrentRow?.DataBoundItem is DataRowView drv)
            {
                txtMaSV.Text = drv["MaSV"]?.ToString();
                txtHoTen.Text = drv["HoTen"]?.ToString();
                txtLop.Text = drv["MaLop"]?.ToString();
                cboGioiTinh.Text = drv["GioiTinh"]?.ToString();
                if (DateTime.TryParse(drv["NgaySinh"]?.ToString(), out var dob))
                    dtNgaySinh.Value = dob;
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
            if (string.IsNullOrWhiteSpace(txtLop.Text))
            {
                message = "Mã lớp không được để trống.";
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
            cboGioiTinh.SelectedIndex = cboGioiTinh.Items.Count > 0 ? 0 : -1;
            dtNgaySinh.Value = DateTime.Today;
            txtMaSV.Focus();
        }

        private class Student
        {
            public string MaSV { get; set; }
            public string HoTen { get; set; }
            public DateTime NgaySinh { get; set; }
            public string GioiTinh { get; set; }
            public string MaLop { get; set; }
        }

        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
