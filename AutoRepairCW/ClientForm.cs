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
    public partial class ClientForm : Form
    {
        private int clientID;
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public ClientForm(int clientID)
        {
            InitializeComponent();
            this.clientID = clientID;
            LoadClient();
        }

        private void LoadClient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))

                {
                    conn.Open();

                    string clientQuery = @"
                    SELECT FirstName,LastName,PhoneNumber
                    FROM Client
                    WHERE Client_ID = @ClientID";

                    using (SqlCommand cmd = new SqlCommand(clientQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                label2.Text = reader["FirstName"].ToString();
                                label3.Text = reader["LastName"].ToString();
                                label4.Text = reader["PhoneNumber"].ToString();
                            }
                        }
                    }
                    string carQuery = @"
                SELECT Brand, Model, YearOfRelease, LicensePlate
                FROM Car
                WHERE Client_ID = @ClientID";

                    using (SqlCommand cmd = new SqlCommand(carQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientID);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable carTable = new DataTable();
                            adapter.Fill(carTable);
                            dataGridView1.DataSource = carTable;
                        }
                    }
                }
            }
            catch (Exception) 
            {
                MessageBox.Show("Помилка","Помилка", MessageBoxButtons.OK);
            }
        }

        private void закритиToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void addcarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddCarForm addCarForm = new AddCarForm(clientID);
            addCarForm.ShowDialog();
            LoadClient();
        }
        private void deletecarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteCarForm deleteCarForm = new DeleteCarForm(clientID);
            deleteCarForm.ShowDialog();
            LoadClient();
        }
        private void makeorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeOrderForm makeOrderForm = new MakeOrderForm(clientID);
            makeOrderForm.ShowDialog();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientInfoForm clientInfoForm = new ClientInfoForm(clientID);
            clientInfoForm.ShowDialog();
        }

        private void вихідToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartForm startForm = new StartForm();
            startForm.Show();
            this.Close();
        }


    }
}
