using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace baithuchanh2
{
    public partial class frm_lop : Form
    {
        private readonly string _connString = ConfigurationManager.ConnectionStrings["QLSV"].ConnectionString;
        private DataTable _table = new DataTable();

        public frm_lop()
        {
            InitializeComponent();
        }

        private void frm_lop_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qLSVDataSet.Lop' table. You can move, or remove it, as needed.
            this.lopTableAdapter.Fill(this.qLSVDataSet.Lop);
            LoadData();
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out string message))
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"INSERT INTO Lop(MaLop, TenLop, Khoa, SiSo)
                                              VALUES(@MaLop,@TenLop,@Khoa,@SiSo)", conn))
            {
                cmd.Parameters.AddWithValue("@MaLop", txtMaLop.Text.Trim());
                cmd.Parameters.AddWithValue("@TenLop", txtTenLop.Text.Trim());
                cmd.Parameters.AddWithValue("@Khoa", txtKhoa.Text.Trim());
                cmd.Parameters.AddWithValue("@SiSo", (int)numSiSo.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadData();
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

            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"UPDATE Lop SET TenLop=@TenLop, Khoa=@Khoa, SiSo=@SiSo
                                              WHERE MaLop=@MaLop", conn))
            {
                cmd.Parameters.AddWithValue("@MaLop", txtMaLop.Text.Trim());
                cmd.Parameters.AddWithValue("@TenLop", txtTenLop.Text.Trim());
                cmd.Parameters.AddWithValue("@Khoa", txtKhoa.Text.Trim());
                cmd.Parameters.AddWithValue("@SiSo", (int)numSiSo.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadData();
            ClearForm();
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

            string maLop = dgvLop.CurrentRow.Cells["colMaLop"].Value?.ToString();
            if (string.IsNullOrEmpty(maLop)) return;

            var confirm = MessageBox.Show($"Xóa lớp {maLop}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("DELETE FROM Lop WHERE MaLop=@MaLop", conn))
            {
                cmd.Parameters.AddWithValue("@MaLop", maLop);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadData();
            ClearForm();
        }

        private void DgvLop_SelectionChanged(object sender, EventArgs e)
        {
            LoadSelectedToForm();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim());
        }

        private void LoadSelectedToForm()
        {
            if (dgvLop.CurrentRow == null) return;

            txtMaLop.Text = dgvLop.CurrentRow.Cells["colMaLop"].Value?.ToString();
            txtTenLop.Text = dgvLop.CurrentRow.Cells["colTenLop"].Value?.ToString();
            txtKhoa.Text = dgvLop.CurrentRow.Cells["colKhoa"].Value?.ToString();
            if (int.TryParse(dgvLop.CurrentRow.Cells["colSiSo"].Value?.ToString(), out int siSo))
            {
                if (siSo < numSiSo.Minimum) siSo = (int)numSiSo.Minimum;
                numSiSo.Value = Math.Min(siSo, (int)numSiSo.Maximum);
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

        private void LoadData(string keyword = "")
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"SELECT MaLop, TenLop, Khoa, SiSo 
                                              FROM Lop
                                              WHERE (@kw = '' OR MaLop LIKE @kwLike OR TenLop LIKE @kwLike OR Khoa LIKE @kwLike)", conn))
            {
                cmd.Parameters.AddWithValue("@kw", keyword);
                cmd.Parameters.AddWithValue("@kwLike", "%" + keyword + "%");
                var da = new SqlDataAdapter(cmd);
                _table = new DataTable();
                da.Fill(_table);
                dgvLop.AutoGenerateColumns = false;
                dgvLop.DataSource = _table;
            }
        }
    }
}
