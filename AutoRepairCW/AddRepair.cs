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
    public partial class AddRepair : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public AddRepair()
        {
            InitializeComponent();
            LoadOrders();
            LoadMasters();
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

                comboBox1.DataSource = orderTable;
                comboBox1.DisplayMember = "Order_ID";
                comboBox1.ValueMember = "Order_ID";
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

                comboBox2.DataSource = masterTable;
                comboBox2.DisplayMember = "MasterInfo";
                comboBox2.ValueMember = "Master_ID";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO Repair (StartDate,CompletionDate,DescriptionOfWorks,Order_ID,Master_ID)
                      VALUES (@StartDate,@CompletionDate,@DescriptionOfWorks,@OrderID,@MasterID)", conn);

                    cmd.Parameters.AddWithValue("@StartDate", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@CompletionDate", dateTimePicker2.Value.Date);
                    cmd.Parameters.AddWithValue("@DescriptionOfWorks", textBox1.Text);
                    cmd.Parameters.AddWithValue("@OrderID", comboBox1.SelectedValue);
                    cmd.Parameters.AddWithValue("@MasterID", comboBox2.SelectedValue);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Ремонт успішно додано друже", "Успіх", MessageBoxButtons.OK);
                    this.Close();
                }

         } 
    }
}
