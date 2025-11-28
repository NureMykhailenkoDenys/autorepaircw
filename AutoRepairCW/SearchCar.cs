using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace AutoRepairCW
{
    public partial class SearchCar : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";

        public SearchCar()
        {
            InitializeComponent();
            UpdateCarList();
            textBox1.TextChanged += txtSearch_TextChanged;
            comboBox1.SelectedIndexChanged += cmbSortBy_SelectedIndexChanged;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            checkedListBox2.ItemCheck += checkedListBox2_ItemCheck;
        }


        private void UpdateCarList()
        {
            string query = textBox1.Text.Trim();

            var selectedBrands = GetCheckedItems(checkedListBox1);
            var selectedYears = GetCheckedItems(checkedListBox2);
            string sortBy = comboBox1.SelectedItem?.ToString();

            string sql = @"
            SELECT 
                c.Car_ID, 
                c.Brand, 
                c.Model, 
                c.YearOfRelease, 
                c.LicensePlate, 
                cl.FirstName + ' ' + cl.LastName AS FullName
            FROM Car c
            LEFT JOIN Client cl ON c.Client_ID = cl.Client_ID
            WHERE 1 = 1";

            if (selectedBrands.Contains("Інші"))
            {
                sql += " AND c.Brand NOT IN (" + string.Join(",", checkedListBox1.Items.Cast<string>()
                    .Where(b => b != "Інші").Select(b => $"'{b}'")) + ")";
            }
            else if (selectedBrands.Count > 0)
            {
                sql += " AND c.Brand IN (" + string.Join(",", selectedBrands.Select(b => $"'{b}'")) + ")";
            }

            if (selectedYears.Contains("Інші"))
            {
                sql += " AND (c.YearOfRelease < 2015 OR c.YearOfRelease > 2025)";
            }
            else if (selectedYears.Count > 0)
            {
                sql += " AND c.YearOfRelease IN (" + string.Join(",", selectedYears.Select(y => y)) + ")";
            }

            if (!string.IsNullOrEmpty(query))
            {
                sql += " AND (c.Brand LIKE @query OR c.Model LIKE @query OR c.LicensePlate LIKE @query)";
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                sortBy = sortBy.Trim();
                if (sortBy.Equals("Рік (від нового до старого)", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY c.YearOfRelease DESC";
                }
                else if (sortBy.Equals("Марка (від A до Z)", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY c.Brand ASC";
                }
                else if (sortBy.Equals("Модель (від A до Z)", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY c.Model ASC";
                }
            }
            else
            {
                sql += " ORDER BY c.Car_ID DESC";
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
            UpdateCarList();
        }

        private void cmbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCarList();
        }


        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() => UpdateCarList()));
        }

        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() => UpdateCarList()));
        }

        private void назадToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
