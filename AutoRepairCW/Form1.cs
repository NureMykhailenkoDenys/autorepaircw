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
    public partial class MainForm : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "autoRepairCWDataSet.SpareParts". При необходимости она может быть перемещена или удалена.
            this.sparePartsTableAdapter.Fill(this.autoRepairCWDataSet.SpareParts);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "autoRepairCWDataSet.Repair". При необходимости она может быть перемещена или удалена.
            this.repairTableAdapter.Fill(this.autoRepairCWDataSet.Repair);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "autoRepairCWDataSet.Order". При необходимости она может быть перемещена или удалена.
            this.orderTableAdapter.Fill(this.autoRepairCWDataSet.Order);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "autoRepairCWDataSet.Master". При необходимости она может быть перемещена или удалена.
            this.masterTableAdapter.Fill(this.autoRepairCWDataSet.Master);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "autoRepairCWDataSet.ClientType". При необходимости она может быть перемещена или удалена.
            this.clientTypeTableAdapter.Fill(this.autoRepairCWDataSet.ClientType);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "autoRepairCWDataSet.Client". При необходимости она может быть перемещена или удалена.
            this.clientTableAdapter.Fill(this.autoRepairCWDataSet.Client);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "autoRepairCWDataSet.Car". При необходимости она может быть перемещена или удалена.
            this.carTableAdapter.Fill(this.autoRepairCWDataSet.Car);

            dataGridView1.AutoGenerateColumns = true;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();

            StartForm startForm = new StartForm();
            startForm.FormClosed += (s, args) =>
            {
                if (Application.OpenForms.Count == 1)
                {
                    this.Close();
                }
            };

            startForm.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            carTableAdapter.Update(autoRepairCWDataSet);
            clientTableAdapter.Update(autoRepairCWDataSet);
            clientTypeTableAdapter.Update(autoRepairCWDataSet);
            masterTableAdapter.Update(autoRepairCWDataSet);
            orderTableAdapter.Update(autoRepairCWDataSet);
            repairTableAdapter.Update(autoRepairCWDataSet);
            sparePartsTableAdapter.Update(autoRepairCWDataSet);
        }

        private void carToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = carBindingSource;
            dataGridView1.DataSource = carBindingSource;
            label1.Text = "Автомобілі";
        }

        private void clientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = clientBindingSource;
            dataGridView1.DataSource = clientBindingSource;
            label1.Text = "Кліенти";
        }

        private void orderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = orderBindingSource;
            dataGridView1.DataSource = orderBindingSource;
            label1.Text = "Замовлення";
        }

        private void repairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = repairBindingSource;
            dataGridView1.DataSource = repairBindingSource;
            label1.Text = "Ремонтні роботи";
        }

        private void masterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = masterBindingSource;
            dataGridView1.DataSource = masterBindingSource;
            label1.Text = "Майстри";
        }

        private void sparePartsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = sparePartsBindingSource;
            dataGridView1.DataSource = sparePartsBindingSource;
            label1.Text = "Запчастини";
        }

        private void clientTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = clientTypeBindingSource;
            dataGridView1.DataSource = clientTypeBindingSource;
            label1.Text = "Типи кліентів";
        }
        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var stat = new StatisticsForm();
            stat.Show();
        }

        private void orderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var ow = new OWForm();
            ow.ShowDialog();
            carTableAdapter.Fill(autoRepairCWDataSet.Car);
            orderTableAdapter.Fill(autoRepairCWDataSet.Order);
            clientTableAdapter.Fill(autoRepairCWDataSet.Client);

        }

        private void queryEditToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var qe = new QueryEdit();
            qe.Show();
        }

        private void ReloadOrdersData()
        {
            orderTableAdapter.Fill(autoRepairCWDataSet.Order);
            dataGridView1.DataSource = orderBindingSource;
            label1.Text = "Замовлення";
        }

        private void ReloadRepairsData()
        {
            repairTableAdapter.Fill(autoRepairCWDataSet.Repair);
            dataGridView1.DataSource = repairBindingSource;
            label1.Text = "Ремонтні роботи";
        }

        private void addOrder_Click(object sender, EventArgs e)
        {
            var addOrderForm = new AddOrder3();
            addOrderForm.ShowDialog();
            ReloadOrdersData();
        }

        private void editOrder_Click(object sender, EventArgs e)
        {
            var editOrder = new EditOrder();
            editOrder.ShowDialog(); 
            ReloadOrdersData();
        }

        private void addRepair_Click(object sender, EventArgs e)
        {
            var addRepair = new AddRepair();
            addRepair.ShowDialog();
        }

        private void editRepair_Click(object sender, EventArgs e)
        {
            var editRepair = new EditRepair();
            editRepair.ShowDialog();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportForm reportForm = new ReportForm();
            reportForm.ShowDialog();
        }

        private void пошукToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchCar searchCar = new SearchCar();
            searchCar.ShowDialog();
        }

        private void пошукToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SearchClient searchClient = new SearchClient();
            searchClient.ShowDialog();
        }

        private void пошукToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SearchOrder searchOrder = new SearchOrder();
            searchOrder.ShowDialog();
        }
    }
}
