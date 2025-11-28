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
    public partial class StatisticsForm : Form
    {
        const string ConnectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public StatisticsForm()
        {
            InitializeComponent();
        }
        private DataTable ExecuteQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                string query = @"SELECT 
                FORMAT(DateOfOrder, 'yyyy-MM') AS 'Місяць',
                COUNT(Order_ID) AS 'Кількість замовлень',
                SUM(TotalAmount) AS 'Виручка'
                FROM [Order]
                GROUP BY FORMAT(DateOfOrder, 'yyyy-MM')
                ORDER BY 'Місяць'";

                dataGridView1.DataSource = ExecuteQuery(query);
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                string query = @"SELECT 
                Name AS 'Запчастина',
                SUM(Count) AS 'Кількість використаних одиниць',
                SUM(Count * Price) AS 'Загальні витрати'
                FROM SpareParts
                GROUP BY Name
                ORDER BY 'Загальні витрати' DESC;
";

                dataGridView1.DataSource = ExecuteQuery(query);
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                string query = @"SELECT TOP 5
                FirstName + ' ' + LastName AS 'Клієнт',
                NumberOfOrders AS 'Кількість замовлень',
                TotalAmountOfOrders AS 'Виручка'
                FROM Client
                ORDER BY NumberOfOrders DESC, TotalAmountOfOrders DESC";

                dataGridView1.DataSource = ExecuteQuery(query);
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                string query = @"
                SELECT 
                CONCAT(m.FirstName, ' ', m.LastName) AS 'Майстер',
                m.Specialization AS 'Спеціалізація',
                COUNT(r.Repair_ID) AS 'Кількість замовлень',
                SUM(o.TotalAmount) AS 'Заробіток'
                FROM Master m
                LEFT JOIN Repair r ON m.Master_ID = r.Master_ID
                LEFT JOIN [Order] o ON r.Order_ID = o.Order_ID
                GROUP BY m.FirstName, m.LastName, m.Specialization
                ORDER BY 'Заробіток' DESC";

                dataGridView1.DataSource = ExecuteQuery(query);
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                string query = @"
                SELECT 
                c.Client_ID,
                CONCAT(c.FirstName, ' ', c.LastName) AS 'Кліент',
                COUNT(o.Order_ID) AS 'Не оплачено',
                SUM(o.TotalAmount) AS 'Повинен заплатити'
                FROM [Order] o
                JOIN Car ca ON o.Car_ID = ca.Car_ID
                JOIN Client c ON ca.Client_ID = c.Client_ID
                WHERE o.PaymentStatus = 'Не оплачено'
                GROUP BY c.Client_ID, c.FirstName, c.LastName
                ORDER BY 'Не оплачено' DESC";

                dataGridView1.DataSource = ExecuteQuery(query);
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }
    }
}
