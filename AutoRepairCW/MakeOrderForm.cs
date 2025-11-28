using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AutoRepairCW
{
    public partial class MakeOrderForm : Form
    {
        private int clientID;
        private const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";

        public MakeOrderForm(int clientID)
        {
            InitializeComponent();
            this.clientID = clientID;
            LoadClientCars();
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
        }
        public class ComboBoxItem
        {
            public int CarID;
            public string DisplayText;

            public override string ToString()
            {
                return DisplayText;
            }
        }

        private void LoadClientCars()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                SELECT Car_ID, Brand, Model, YearOfRelease, LicensePlate
                FROM Car
                WHERE Client_ID = @ClientID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClientID", clientID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string brand = reader["Brand"].ToString();
                            string model = reader["Model"].ToString();
                            int year = Convert.ToInt32(reader["YearOfRelease"]);
                            string licensePlate = reader["LicensePlate"].ToString();
                            ComboBoxItem item = new ComboBoxItem
                            {
                                CarID = Convert.ToInt32(reader["Car_ID"]),
                                DisplayText = $"{brand} {model} ({year}) - {licensePlate}"
                            };
                            comboBox1.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string service = comboBox2.SelectedItem.ToString();
            if (service == "Діагностика")
            {
                label1.Text = "Ціна: 100 грн.";
            }
            else if (service == "Заміна шин")
            {
                label1.Text = "Ціна: 250 грн.";
            }
            else if (service == "Замінити олію")
            {
                label1.Text = "Ціна: 300 грн.";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string service = comboBox2.SelectedItem.ToString();
            decimal price = service == "Діагностика" ? 100 : service == "Заміна шин" ? 250 : 300;
            string paymentStatus = "Не оплачено";
            string paymentMethod = comboBox4.SelectedItem.ToString();
            DateTime orderDate = dateTimePicker1.Value;
            int carID = (comboBox1.SelectedItem as ComboBoxItem).CarID;

            string orderStatus = "Очікує";
            decimal discount = GetClientDiscount();
            decimal finalPrice = price - (price * discount / 100);

            int orderId = CreateOrder(orderDate, orderStatus, price, discount, finalPrice, paymentStatus, paymentMethod, carID);

            UpdateClientOrderInfo(finalPrice);

            MessageBox.Show("Замовлення успішно створено!");
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string service = comboBox2.SelectedItem.ToString();
            decimal price = service == "Діагностика" ? 100 : service == "Заміна шин" ? 250 : 300;
            string paymentStatus = "Оплачено";
            string paymentMethod = comboBox4.SelectedItem.ToString();
            DateTime orderDate = dateTimePicker1.Value;
            int carID = (comboBox1.SelectedItem as ComboBoxItem).CarID;

            string orderStatus = "Очікує";
            decimal discount = GetClientDiscount();
            decimal finalPrice = price - (price * discount / 100);

            int orderId = CreateOrder(orderDate, orderStatus, price, discount, finalPrice, paymentStatus, paymentMethod, carID);

            UpdateClientOrderInfo(finalPrice);

            MessageBox.Show("Замовлення успішно створено!");
            this.Close();
        }

        private int CreateOrder(DateTime orderDate, string orderStatus, decimal price, decimal discount, decimal finalPrice, string paymentStatus, string paymentMethod, int carID)
        {
            int orderId = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string insertOrderQuery = @"
                    INSERT INTO [Order] (DateOfOrder, Status, InitialPrice, Discount, TotalAmount, PaymentStatus, PaymentMethod, Car_ID)
                    VALUES (@DateOfOrder, @Status, @InitialPrice, @Discount, @TotalAmount, @PaymentStatus, @PaymentMethod, @CarID);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(insertOrderQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@DateOfOrder", orderDate);
                    cmd.Parameters.AddWithValue("@Status", orderStatus);
                    cmd.Parameters.AddWithValue("@InitialPrice", price);
                    cmd.Parameters.AddWithValue("@Discount", discount);
                    cmd.Parameters.AddWithValue("@TotalAmount", finalPrice);
                    cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@CarID", carID);

                    object result = cmd.ExecuteScalar();
                }
            }

            return orderId;
        }

        private void UpdateClientOrderInfo(decimal servicePrice)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string updateClientQuery = @"
            UPDATE Client
            SET NumberOfOrders = NumberOfOrders + (1/2),
                TotalAmountOfOrders = TotalAmountOfOrders + (@ServicePrice/2)
            WHERE Client_ID = @ClientID";

                using (SqlCommand cmd = new SqlCommand(updateClientQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ServicePrice", servicePrice);
                    cmd.Parameters.AddWithValue("@ClientID", clientID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private decimal GetClientDiscount()
        {
            decimal discount = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string clientQuery = "SELECT ClientType_ID FROM Client WHERE Client_ID = @ClientID";
                using (SqlCommand cmd = new SqlCommand(clientQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ClientID", clientID);

                    object result = cmd.ExecuteScalar();
                    int clientTypeID = result != DBNull.Value ? Convert.ToInt32(result) : -1;

                    if (clientTypeID == 2)
                    {
                        discount = 5;
                    }
                    else if (clientTypeID == 3)
                    {
                        discount = 10;
                    }
                }
            }

            return discount;
        }


    }
}
