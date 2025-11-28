using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoRepairCW
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();

            MainForm adminForm = new MainForm();
            adminForm.FormClosed += (s, args) =>
            {
                if (Application.OpenForms.Count == 1)
                {
                    this.Close();
                }
            };

            adminForm.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();

            LoginForm loginForm = new LoginForm();
            loginForm.FormClosed += (s, args) =>
            {
                if (Application.OpenForms.Count == 1)
                {
                    this.Close();
                }
            };

            loginForm.Show();
        }
    }
}
