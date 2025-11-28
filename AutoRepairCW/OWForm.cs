using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoRepairCW
{
    public partial class OWForm : Form
    {
        public OWForm()
        {
            InitializeComponent();
        }

        private void carBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.carBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.autoRepairCWDataSet);

        }

        private void OWForm_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "autoRepairCWDataSet.Order". При необходимости она может быть перемещена или удалена.
            this.orderTableAdapter.Fill(this.autoRepairCWDataSet.Order);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "autoRepairCWDataSet.Car". При необходимости она может быть перемещена или удалена.
            this.carTableAdapter.Fill(this.autoRepairCWDataSet.Car);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Ви дійсно хочете підтвердити зміни?", "Зміна даних", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                carBindingSource.EndEdit();
                carTableAdapter.Update(autoRepairCWDataSet);
                orderTableAdapter.Update(autoRepairCWDataSet);
            }
        }
    }
}
