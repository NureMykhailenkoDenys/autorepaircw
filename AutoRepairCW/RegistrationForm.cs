using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AutoRepairCW
{
    public partial class RegistrationForm : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";

        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();
            string phoneNumber = textBox3.Text.Trim();
            string email = textBox5.Text.Trim();
            string password = textBox4.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля", "Помилка", MessageBoxButtons.OK);
                return;
            }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string checkQuery = @"
                        SELECT COUNT(*) FROM Client WHERE PhoneNumber = @PhoneNumber";

                    using (SqlCommand cmdCheck = new SqlCommand(checkQuery, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        int count = (int)cmdCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Клієнт з таким номером телефону вже існує!", "Помилка", MessageBoxButtons.OK);
                            return;
                        }
                    }

                    string insertQuery = @"
                        INSERT INTO Client (FirstName, LastName, PhoneNumber, Email, NumberOfOrders, TotalAmountOfOrders, ClientType_ID)
                        VALUES (@FirstName, @LastName, @PhoneNumber, @Email, 0, 0, 1); -- Default ClientType_ID = 1 (New)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Email", email);

                        cmd.ExecuteNonQuery();
                    }

                    string getClientIDQuery = @"
                        SELECT Client_ID FROM Client WHERE PhoneNumber = @PhoneNumber";

                    using (SqlCommand cmd = new SqlCommand(getClientIDQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        int clientID = (int)cmd.ExecuteScalar();

                        MessageBox.Show("Реєстрація успішна!", "Успіх", MessageBoxButtons.OK);

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
                }
        }
    }
}
