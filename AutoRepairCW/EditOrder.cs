using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AutoRepairCW
{
    public partial class EditOrder : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";

        public EditOrder()
        {
            InitializeComponent();
            LoadCarData();
            LoadOrders();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            textBox1.TextChanged += textBoxInitialPrice_TextChanged;
            textBox2.TextChanged += textBoxDiscount_TextChanged;
        }

        private void LoadCarData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Car_ID, CONCAT(Brand, ' - ', LicensePlate) AS CarInfo FROM Car", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable carTable = new DataTable();
                adapter.Fill(carTable);

                comboBox5.DataSource = carTable;
                comboBox5.DisplayMember = "CarInfo";
                comboBox5.ValueMember = "Car_ID";
            }
        }

        private void LoadOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Order_ID FROM [Order]", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable ordersTable = new DataTable();
                adapter.Fill(ordersTable);

                comboBox1.DataSource = ordersTable;
                comboBox1.DisplayMember = "Order_ID";
                comboBox1.ValueMember = "Order_ID";
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                int orderId = Convert.ToInt32(comboBox1.SelectedValue);
                LoadOrderDetails(orderId); 
            }
        }

        private void LoadOrderDetails(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                @"SELECT Car_ID, DateOfOrder, Status, InitialPrice, Discount, TotalAmount, 
                  PaymentMethod, PaymentStatus
                  FROM [Order]
                  WHERE Order_ID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    comboBox5.SelectedValue = reader["Car_ID"]; 
                    dateTimePicker1.Value = Convert.ToDateTime(reader["DateOfOrder"]);
                    comboBox2.SelectedItem = reader["Status"];
                    textBox1.Text = reader["InitialPrice"].ToString();
                    textBox2.Text = reader["Discount"].ToString();
                    labelDiscount.Text = $"Знижка: {reader["Discount"]}%";
                    labelTotalAmount.Text = $"Загальна сума: {reader["TotalAmount"]}";
                    comboBox4.SelectedItem = reader["PaymentMethod"]; 
                    comboBox3.SelectedItem = reader["PaymentStatus"];  
                }
            }
        }

        private void textBoxInitialPrice_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out decimal initialPrice))
            {
                int discount = int.TryParse(textBox2.Text, out discount) ? discount : 0;
                decimal totalAmount = initialPrice - (initialPrice * discount / 100);
                labelTotalAmount.Text = $"Загальна сума: {totalAmount:F2}"; 
            }
        }

        private void textBoxDiscount_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out decimal initialPrice))
            {
                int discount = int.TryParse(textBox2.Text, out discount) ? discount : 0;
                decimal totalAmount = initialPrice - (initialPrice * discount / 100);
                labelTotalAmount.Text = $"Загальна сума: {totalAmount:F2}"; 
            }
        }


        private void UpdateDiscountAndTotal(int discount)
        {
            if (decimal.TryParse(textBox1.Text, out decimal price))
            {
                decimal total = price - (price * discount / 100);
                labelDiscount.Text = $"Знижка {discount}%";
                labelTotalAmount.Text = $"Загальна сума: {total:F2}";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                decimal totalAmount;
                if (!decimal.TryParse(labelTotalAmount.Text.Replace("Загальна сума:", "").Trim(), out totalAmount))
                {
                    MessageBox.Show("Помилка обчислення загальної суми. Перевірте введені дані.", "Помилка");
                    return;
                }
                SqlCommand cmd = new SqlCommand(
                            @"UPDATE [Order] 
                          SET DateOfOrder = @DateOfOrder, Status = @Status, 
                              InitialPrice = @InitialPrice, Discount = @Discount, 
                              TotalAmount = @TotalAmount, 
                              Car_ID = @CarID
                          WHERE Order_ID = @OrderID", conn);
                    cmd.Parameters.AddWithValue("@OrderID", comboBox1.SelectedValue);
                    cmd.Parameters.AddWithValue("@DateOfOrder", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@Status", comboBox2.SelectedItem?.ToString());
                    cmd.Parameters.AddWithValue("@InitialPrice", decimal.Parse(textBox1.Text));
                    cmd.Parameters.AddWithValue("@Discount", int.Parse(textBox2.Text));
                    cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    cmd.Parameters.AddWithValue("@CarID", comboBox5.SelectedValue);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Замовлення успішно оновлено!", "Успіх", MessageBoxButtons.OK);
                }

        }

        private void button2_Click(object sender, EventArgs e)
        {
                if (comboBox1.SelectedValue != null)
                {
                    DialogResult result = MessageBox.Show("Ви впевнені, що хочете видалити це замовлення?", "Підтвердження", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("DELETE FROM [Order] WHERE Order_ID = @OrderID", conn);
                            cmd.Parameters.AddWithValue("@OrderID", comboBox1.SelectedValue);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Замовлення успішно видалено!", "Успіх", MessageBoxButtons.OK);
                            LoadOrders();
                        }
                    }
                }
        }
    }
}
