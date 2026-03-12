using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace baithuchanh2
{
    public partial class frm_login : Form
    {
        private const string ValidUsername = "admin";
        private const string ValidPassword = "123456";

        public frm_login()
        {
            InitializeComponent();
        }

        private void TxtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (username == ValidUsername && password == ValidPassword)
            {
                var main = new frm_qlsv();
                // Khi form chính đóng, thoát luôn ứng dụng thay vì để form đăng nhập ẩn.
                main.FormClosed += (s, args) => this.Close();
                main.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}
