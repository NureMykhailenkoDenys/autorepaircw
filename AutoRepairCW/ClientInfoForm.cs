using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AutoRepairCW
{
    public partial class ClientInfoForm : Form
    {
        private int clientID;
        private const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";

        public ClientInfoForm(int clientID)
        {
            InitializeComponent();
            this.clientID = clientID;
            LoadClientInfo();
        }

        private void LoadClientInfo()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string clientQuery = @"
                    SELECT FirstName, LastName, PhoneNumber, Email, Address, 
                           NumberOfOrders, TotalAmountOfOrders, ClientType_ID
                    FROM Client
                    WHERE Client_ID = @ClientID";

                using (SqlCommand cmd = new SqlCommand(clientQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ClientID", clientID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBox1.Text = reader["FirstName"].ToString();
                            textBox2.Text = reader["LastName"].ToString();
                            textBox3.Text = reader["PhoneNumber"].ToString();
                            textBox4.Text = reader["Email"].ToString();
                            textBox5.Text = reader["Address"].ToString();
                            label9.Text = "Кількість замовлень: " + reader["NumberOfOrders"].ToString();
                            label8.Text = "Потрачена сума: " + reader["TotalAmountOfOrders"].ToString() + " грн";

                            int clientTypeID = Convert.ToInt32(reader["ClientType_ID"]);
                            string status = "";
                            if (clientTypeID == 1)
                            {
                                status = "Новий";
                            }
                            else if (clientTypeID == 2)
                            {
                                status = "Постійний";
                            }
                            else if (clientTypeID == 3)
                            {
                                status = "VIP";
                            }
                            else
                            {
                                status = "Невідомий статус";
                            }
                            label10.Text = "Статус: " + status;

                            CheckAndUpdateClientType(reader);
                        }
                    }
                }
            }
        }

        private void CheckAndUpdateClientType(SqlDataReader reader)
        {
            int numberOfOrders = Convert.ToInt32(reader["NumberOfOrders"]);
            decimal totalAmountOfOrders = Convert.ToDecimal(reader["TotalAmountOfOrders"]);
            int currentClientTypeID = Convert.ToInt32(reader["ClientType_ID"]);

            int minOrders = 0;
            decimal minAmount = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT MinOrders, MinAmount FROM ClientType WHERE ClientType_ID = @ClientTypeID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClientTypeID", currentClientTypeID);

                    using (SqlDataReader typeReader = cmd.ExecuteReader())
                    {
                        if (typeReader.Read())
                        {
                            minOrders = Convert.ToInt32(typeReader["MinOrders"]);
                            minAmount = Convert.ToDecimal(typeReader["MinAmount"]);
                        }
                    }
                }
            }

            int newClientTypeID = DetermineNewClientType(numberOfOrders, totalAmountOfOrders);

            if (newClientTypeID != currentClientTypeID)
            {
                UpdateClientType(newClientTypeID);
                MessageBox.Show("Тип клієнта успішно оновлений!");
            }
        }

        private int DetermineNewClientType(int numberOfOrders, decimal totalAmount)
        {
            if (numberOfOrders >= 20 && totalAmount >= 30000)
                return 3;
            else if (numberOfOrders >= 5 && totalAmount >= 2500)
                return 2;
            else
                return 1;
        }

        private void UpdateClientType(int newClientTypeID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = "UPDATE Client SET ClientType_ID = @NewClientTypeID WHERE Client_ID = @ClientID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@NewClientTypeID", newClientTypeID);
                    cmd.Parameters.AddWithValue("@ClientID", clientID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string firstName = textBox1.Text;
            string lastName = textBox2.Text;
            string phoneNumber = textBox3.Text;
            string email = textBox4.Text;
            string address = textBox5.Text;

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Будь ласка, заповніть усі обов'язкові поля (ім'я, прізвище, телефон, email).", "Помилка", MessageBoxButtons.OK);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string updateQuery = @"
                    UPDATE Client
                    SET FirstName = @FirstName,
                        LastName = @LastName,
                        PhoneNumber = @PhoneNumber,
                        Email = @Email,
                        Address = @Address
                    WHERE Client_ID = @ClientID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@ClientID", clientID);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Інформація успішно оновлена!");
                }
            }
        }

        private void назадToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
