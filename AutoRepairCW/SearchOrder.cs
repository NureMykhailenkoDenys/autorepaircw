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
    public partial class SearchOrder : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public SearchOrder()
        {
            InitializeComponent();
            UpdateOrderList();
            textBox1.TextChanged += txtSearch_TextChanged;
            comboBox1.SelectedIndexChanged += cmbSortBy_SelectedIndexChanged;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            checkedListBox2.ItemCheck += checkedListBox2_ItemCheck;
            checkedListBox3.ItemCheck += checkedListBox3_ItemCheck;
        }
        private void UpdateOrderList()
        {
            string query = textBox1.Text.Trim();

            var selectedStatuses = GetCheckedItems(checkedListBox1);
            var selectedPaymentStatuses = GetCheckedItems(checkedListBox2);
            var selectedPaymentMethods = GetCheckedItems(checkedListBox3);
            string sortBy = comboBox1.SelectedItem?.ToString();

            string sql = @"
            SELECT 
                o.Order_ID,
                o.DateOfOrder,
                o.Status,
                o.InitialPrice,
                o.Discount,
                o.TotalAmount,
                o.PaymentStatus,
                o.PaymentMethod,
                c.Brand + ' ' + c.Model + ' (' + CAST(c.YearOfRelease AS NVARCHAR) + ')' AS CarInfo
            FROM [Order] o
            LEFT JOIN Car c ON o.Car_ID = c.Car_ID
            WHERE 1 = 1";

            if (selectedStatuses.Count > 0)
            {
                sql += " AND o.Status IN (" + string.Join(",", selectedStatuses.Select(s => $"'{s}'")) + ")";
            }

            if (selectedPaymentStatuses.Count > 0)
            {
                sql += " AND o.PaymentStatus IN (" + string.Join(",", selectedPaymentStatuses.Select(s => $"'{s}'")) + ")";
            }

            if (selectedPaymentMethods.Count > 0)
            {
                sql += " AND o.PaymentMethod IN (" + string.Join(",", selectedPaymentMethods.Select(m => $"'{m}'")) + ")";
            }

            if (!string.IsNullOrEmpty(query))
            {
                sql += " AND (o.Status LIKE @query OR o.PaymentStatus LIKE @query OR o.PaymentMethod LIKE @query)";
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                sortBy = sortBy.Trim();
                if (sortBy.Equals("Дата (від меншої до більшої)", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY o.DateOfOrder ASC";
                }
                else if (sortBy.Equals("Дата (від більшої до меншої)", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY o.DateOfOrder DESC";
                }
                else if (sortBy.Equals("Фінальна ціна (від більшої до меншої)", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY o.TotalAmount DESC";
                }
                else if (sortBy.Equals("Фінальна ціна (від меншої до більшої)", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " ORDER BY o.TotalAmount ASC";
                }
            }
            else
            {
                sql += " ORDER BY o.Order_ID DESC";
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
            UpdateOrderList();
        }

        private void cmbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateOrderList();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() => UpdateOrderList()));
        }

        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() => UpdateOrderList()));
        }

        private void checkedListBox3_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() => UpdateOrderList()));
        }

        private void назадToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
