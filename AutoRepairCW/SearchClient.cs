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
    public partial class SearchClient : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public SearchClient()
        {
            InitializeComponent();
            UpdateClientList();
            textBox1.TextChanged += txtSearch_TextChanged;
            comboBox1.SelectedIndexChanged += cmbSortBy_SelectedIndexChanged;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
        }
        private void UpdateClientList()
        {
            string query = textBox1.Text.Trim();

            var selectedClientTypes = GetCheckedItems(checkedListBox1);
            string sortBy = comboBox1.SelectedItem?.ToString();

            string sql = @"
            SELECT 
                cl.Client_ID, 
                cl.FirstName, 
                cl.LastName, 
                cl.PhoneNumber, 
                cl.Email, 
                cl.NumberOfOrders, 
                cl.TotalAmountOfOrders, 
                ct.Name AS ClientType
            FROM Client cl
            LEFT JOIN ClientType ct ON cl.ClientType_ID = ct.ClientType_ID
            WHERE 1 = 1";

            if (selectedClientTypes.Count > 0)
            {
                sql += " AND ct.Name IN (" + string.Join(",", selectedClientTypes.Select(t => $"'{t}'")) + ")";
            }

            if (!string.IsNullOrEmpty(query))
            {
                sql += " AND (cl.FirstName LIKE @query OR cl.LastName LIKE @query OR cl.PhoneNumber LIKE @query)";
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                sortBy = sortBy.Trim();
                if (sortBy.Equals("Ім'я (від A до Z)", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY cl.FirstName ASC";
                }
                else if (sortBy.Equals("Прізвище (від A до Z)", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY cl.LastName ASC";
                }
                else if (sortBy.Equals("Кількість замовлень", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY cl.NumberOfOrders DESC";
                }
                else if (sortBy.Equals("Сума витрат", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY cl.TotalAmountOfOrders DESC";
                }
            }
            else
            {
                sql += " ORDER BY cl.Client_ID DESC";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (!string.IsNullOrEmpty(query))
                    cmd.Parameters.AddWithValue("@query", "%" + query + "%");

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private List<string> GetCheckedItems(CheckedListBox checkedListBox)
        {
            var items = checkedListBox.CheckedItems.Cast<string>().ToList();

            if (checkedListBox.SelectedIndex >= 0)
            {
                var currentItem = checkedListBox.Items[checkedListBox.SelectedIndex]?.ToString();
                var isChecked = checkedListBox.GetItemCheckState(checkedListBox.SelectedIndex) == CheckState.Checked;

                if (isChecked && !items.Contains(currentItem))
                {
                    items.Add(currentItem);
                }
                else if (!isChecked && items.Contains(currentItem))
                {
                    items.Remove(currentItem);
                }
            }

            return items;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            UpdateClientList();
        }

        private void cmbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateClientList();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() => UpdateClientList()));
        }

        private void назадToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
