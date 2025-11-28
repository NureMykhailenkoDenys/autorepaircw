using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AutoRepairCW
{
    public partial class DeleteCarForm : Form
    {
        private int clientID;
        private const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";

        public DeleteCarForm(int clientID)
        {
            InitializeComponent();
            this.clientID = clientID;
            LoadCars();
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
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

        private void LoadCars()
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (ComboBoxItem)comboBox1.SelectedItem;
            if (selectedItem != null)
            {
                label2.Text = $"Марка: {selectedItem.DisplayText.Split(' ')[0]}";
                label3.Text = $"Модель: {selectedItem.DisplayText.Split(' ')[1]}";
                label4.Text = $"Рік: {selectedItem.DisplayText.Split(' ')[2].Trim('(').Trim(')')}";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedItem = (ComboBoxItem)comboBox1.SelectedItem;
            if (selectedItem != null)
            {
                DialogResult result = MessageBox.Show("Ви впевнені, що хочете видалити цей автомобіль?", "Підтвердження", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string deleteQuery = "DELETE FROM Car WHERE Car_ID = @CarID";

                        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@CarID", selectedItem.CarID);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Автомобіль успішно видалено!");
                            this.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть автомобіль для видалення.");
            }
        }
    }


}
