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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AutoRepairCW
{
    public partial class EditRepair : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public EditRepair()
        {
            InitializeComponent();
            LoadRepairs();
            LoadOrders();
            LoadMasters();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }
        private void LoadRepairs()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Repair_ID FROM Repair", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable repairTable = new DataTable();
                adapter.Fill(repairTable);

                comboBox1.DataSource = repairTable;
                comboBox1.DisplayMember = "Repair_ID";
                comboBox1.ValueMember = "Repair_ID";
            }
        }
        private void LoadOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Order_ID FROM [Order]", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable orderTable = new DataTable();
                adapter.Fill(orderTable);

                comboBox2.DataSource = orderTable;
                comboBox2.DisplayMember = "Order_ID";
                comboBox2.ValueMember = "Order_ID";
            }
        }

        private void LoadMasters()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Master_ID, CONCAT(FirstName , ' ', LastName) AS MasterInfo FROM Master", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable masterTable = new DataTable();
                adapter.Fill(masterTable);

                comboBox3.DataSource = masterTable;
                comboBox3.DisplayMember = "MasterInfo";
                comboBox3.ValueMember = "Master_ID";
            }
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                int repairId = Convert.ToInt32(comboBox1.SelectedValue);
                LoadRepairDetails(repairId);
            }
        }
        private void LoadRepairDetails(int repairId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                @"SELECT StartDate, CompletionDate, DescriptionOfWorks, Order_ID, Master_ID
                  FROM Repair
                  WHERE Repair_ID = @RepairID", conn);
                cmd.Parameters.AddWithValue("@RepairID", repairId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    dateTimePicker1.Value = Convert.ToDateTime(reader["StartDate"]);
                    dateTimePicker2.Value = Convert.ToDateTime(reader["CompletionDate"]);
                    textBox1.Text = reader["DescriptionOfWorks"].ToString();
                    comboBox2.SelectedValue = reader["Order_ID"];
                    comboBox3.SelectedValue = reader["Master_ID"];
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE Repair
                    SET StartDate = @StartDate, CompletionDate = @CompletionDate, DescriptionOfWorks = @DescriptionOfWorks, Order_ID = @OrderID, Master_ID = @MasterID
                    WHERE Repair_ID = @RepairID", conn);
                cmd.Parameters.AddWithValue("@StartDate", dateTimePicker1.Value.Date);
                cmd.Parameters.AddWithValue("@CompletionDate", dateTimePicker2.Value.Date);
                cmd.Parameters.AddWithValue("@DescriptionOfWorks", textBox1.Text);
                cmd.Parameters.AddWithValue("@OrderID", comboBox2.SelectedValue);
                cmd.Parameters.AddWithValue("@MasterID", comboBox3.SelectedValue);
                cmd.Parameters.AddWithValue("@RepairID", comboBox1.SelectedValue);
                
                cmd.ExecuteNonQuery();
                MessageBox.Show("Ремонт успішно оновлено","Успіх",MessageBoxButtons.OK);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                DialogResult result = MessageBox.Show("Ви впевнені, що хочете видалити цей ремонт?", "Підтвердження", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Repair WHERE Repair_ID = @RepairID", conn);
                        cmd.Parameters.AddWithValue("@RepairID", comboBox1.SelectedValue);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Замовлення успішно видалено!", "Успіх", MessageBoxButtons.OK);
                        LoadRepairs();
                    }
                }
            }
        }
    }
}
