using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AutoRepairCW
{
    public partial class AddOrder3 : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";

        public AddOrder3()
        {
            InitializeComponent();
            LoadCarID();
        }

        private void LoadCarID()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Car_ID, CONCAT(Brand, ' - ', LicensePlate) AS CarInfo FROM Car", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable carTable = new DataTable();
                adapter.Fill(carTable);

                comboBox1.DataSource = carTable;
                comboBox1.DisplayMember = "CarInfo"; 
                comboBox1.ValueMember = "Car_ID";  
            }
        }


        private void LoadDiscount(int carId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    @"SELECT ct.Discount 
                      FROM Client c
                      INNER JOIN ClientType ct ON c.ClientType_ID = ct.ClientType_ID
                      INNER JOIN Car ca ON c.Client_ID = ca.Client_ID
                      WHERE ca.Car_ID = @CarID", conn);
                cmd.Parameters.AddWithValue("@CarID", carId);
                object discountObj = cmd.ExecuteScalar();

                if (discountObj != null && int.TryParse(discountObj.ToString(), out int discount))
                {
                    CalculateTotal(discount); 
                }
                else
                {
                    CalculateTotal(0); 
                }
            }
        }

        private void CalculateTotal(int discount)
        {
            if (decimal.TryParse(textBox1.Text, out decimal price))
            {
                decimal total = price - (price * discount / 100);
                textBox3.Text = total.ToString("F2"); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO [Order] 
                              (DateOfOrder, [Status], InitialPrice, Discount, TotalAmount, PaymentStatus, PaymentMethod, Car_ID)
                              VALUES (@DateOfOrder, @Status, @InitialPrice, @Discount, @TotalAmount, @PaymentStatus, @PaymentMethod, @CarID)", conn);
                        cmd.Parameters.AddWithValue("@DateOfOrder", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@Status", comboBox2.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@InitialPrice", decimal.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@Discount", int.Parse(textBox2.Text));
                        cmd.Parameters.AddWithValue("@TotalAmount", decimal.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@PaymentStatus", comboBox3.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@PaymentMethod", comboBox4.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@CarID", int.Parse(comboBox1.SelectedValue.ToString()));

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Замовлення успішно додано!", "Успіх", MessageBoxButtons.OK);
                        this.Close();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Помилка при додаванні замовлення: " ,"Помилка", MessageBoxButtons.OK);
            }
        }

        private bool ValidateForm()
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null ||
                comboBox3.SelectedItem == null || comboBox4.SelectedItem == null ||
                string.IsNullOrWhiteSpace(textBox1.Text))  
            {
                MessageBox.Show("Будь ласка, заповніть усі поля!", "Помилка", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
