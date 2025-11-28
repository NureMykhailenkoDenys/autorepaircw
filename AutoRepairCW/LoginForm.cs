using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace AutoRepairCW
{
    public partial class LoginForm : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();
            string phoneNumber = textBox3.Text.Trim();
            string password = textBox4.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля", "Помилка", MessageBoxButtons.OK);
                return;
            }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                    SELECT Client_ID
                    FROM Client
                    WHERE FirstName = @FirstName AND LastName = @LastName AND PhoneNumber = @PhoneNumber";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            int clientID = (int)result;
                            MessageBox.Show("Успішний вхід", "Успіх", MessageBoxButtons.OK);

                            ClientForm clientForm = new ClientForm(clientID);
                            clientForm.FormClosed += (s, args) =>
                            {
                                if (Application.OpenForms.Count == 1)
                                {
                                    this.Close();
                                }
                            };
                            clientForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Кліента не знайдено","Помилка",MessageBoxButtons.OK);
                        }
                    }

                }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.Show();
            this.Close();
        }
    }
}
