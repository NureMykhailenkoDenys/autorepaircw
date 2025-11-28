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

namespace AutoRepairCW
{
    public partial class AddCarForm : Form
    {
        private int clientID;
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public AddCarForm(int clientID)
        {
            InitializeComponent();
            this.clientID = clientID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string brand = textBox1.Text;
            string model = textBox2.Text;
            int year = int.Parse(textBox3.Text);
            if (!int.TryParse(textBox3.Text, out year) || textBox3.Text.Length != 4)
            {
                MessageBox.Show("Введіть коректний рік (4 цифри)");
                return;
            }
            string licensePlate = textBox4.Text;
            if (!IsLicensePlateUnique(licensePlate))
            {
                MessageBox.Show("Номер автомобіля не пройшов перевірку");
                return;
            }

            if (string.IsNullOrEmpty(brand) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(licensePlate))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля", "Помилка", MessageBoxButtons.OK);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                    INSERT INTO Car (Brand, Model, YearOfRelease,LicensePlate, Client_ID)
                    VALUES (@Brand, @Model, @Year, @LicensePlate, @ClientID)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Brand", brand);
                        cmd.Parameters.AddWithValue("@Model", model);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@LicensePlate", licensePlate);
                        cmd.Parameters.AddWithValue("@ClientID", clientID);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Автомобіль успішно додано!");
                        this.Close();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Помилка");
            }
        }
        private bool IsLicensePlateUnique(string licensePlate)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Car WHERE LicensePlate = @LicensePlate";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LicensePlate", licensePlate);

                        int count = (int)cmd.ExecuteScalar();
                        return count == 0;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Номер автомобіля не пройшов перевірку");
                return false;
            }
        }


    }
}
