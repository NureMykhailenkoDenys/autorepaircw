using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Data.SqlClient;

namespace AutoRepairCW
{
    public partial class ReportForm : Form
    {
        const string connectionString = @"Data Source = DESKTOP-T3GBRSE\SQLEXPRESS; Initial Catalog = AutoRepairCW; Integrated Security = True; Encrypt = True; TrustServerCertificate = True";
        public ReportForm()
        {
            InitializeComponent();
        }

        private void GeneratedPdfReport(DataTable data, string fileName, string reportTitle)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = reportTitle;

            PdfPage page = pdf.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12, XFontStyleEx.Regular);
            XFont titleFont = new XFont("Verdana", 14, XFontStyleEx.Bold);

            gfx.DrawString(reportTitle, titleFont, XBrushes.Black,
                new XRect(0, XUnit.FromPoint(20).Point, page.Width.Point, 0),
                XStringFormats.TopCenter);
            int yPoint = 80;  

            if (data.Rows.Count > 0)
            {
                gfx.DrawString("Запчасть | Кількість замовлених | Сума витрат", font, XBrushes.Black,
                    new XRect(XUnit.FromPoint(40).Point, XUnit.FromPoint(yPoint).Point, page.Width.Point, 0));
                yPoint += 20;

                decimal totalSpent = 0; 

                foreach (DataRow row in data.Rows)
                {
                    string sparePartName = row["Запчасть"].ToString();
                    string orderedQuantity = row["Кількість замовлених"].ToString();
                    string totalCost = row["Сума витрат"].ToString();

                    gfx.DrawString($"{sparePartName} | {orderedQuantity} | {totalCost}", font, XBrushes.Black,
                        new XRect(XUnit.FromPoint(40).Point, XUnit.FromPoint(yPoint).Point, page.Width.Point, 0));
                    yPoint += 20;

                    totalSpent += Convert.ToDecimal(totalCost); 

                    if (yPoint > page.Height.Point - 40)
                    {
                        page = pdf.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPoint = 50; 
                    }
                }

                yPoint += 10;
                gfx.DrawString("=======================================", font, XBrushes.Black,
                    new XRect(XUnit.FromPoint(40).Point, XUnit.FromPoint(yPoint).Point, page.Width.Point, 0));
                yPoint += 20;
                gfx.DrawString($"Загалом потрачено на запчасті: {totalSpent:C}", font, XBrushes.Black,
                    new XRect(XUnit.FromPoint(40).Point, XUnit.FromPoint(yPoint).Point, page.Width.Point, 0));
            }
            else
            {
                gfx.DrawString("Немає даних для звіту", font, XBrushes.Black,
                    new XRect(XUnit.FromPoint(40).Point, XUnit.FromPoint(yPoint).Point, page.Width.Point, 0));
            }

            pdf.Save(fileName);
        }


        private void GeneratedPdfReport2(DataTable data, string fileName, string reportTitle)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = reportTitle;

            PdfPage page = pdf.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12, XFontStyleEx.Regular);
            XFont titleFont = new XFont("Verdana", 14, XFontStyleEx.Bold);

            gfx.DrawString(reportTitle, titleFont, XBrushes.Black,
                new XRect(0, XUnit.FromPoint(20).Point, page.Width.Point, 0),
                XStringFormats.TopCenter);
            int yPoint = 80; 

            if (data.Rows.Count > 0)
            {
                gfx.DrawString("Дата | Статус | Ціна | Знижка | Загальна сума | Статус оплати | Метод оплати", font, XBrushes.Black,
                    new XRect(XUnit.FromPoint(40).Point, XUnit.FromPoint(yPoint).Point, page.Width.Point, 0));
                yPoint += 20;

                foreach (DataRow row in data.Rows)
                {
                    string dateOfOrder = Convert.ToDateTime(row["Дата початку"]).ToString("dd.MM.yyyy");
                    string status = row["Статус"].ToString();
                    string initialPrice = row["InitialPrice"].ToString();
                    string discount = row["Discount"].ToString();
                    string totalAmount = row["TotalAmount"].ToString();
                    string paymentStatus = row["PaymentStatus"].ToString();
                    string paymentMethod = row["PaymentMethod"].ToString();

                    gfx.DrawString($"{dateOfOrder} | {status} | {initialPrice} | {discount} % | {totalAmount} | {paymentStatus} | {paymentMethod}", font, XBrushes.Black,
                        new XRect(XUnit.FromPoint(40).Point, XUnit.FromPoint(yPoint).Point, page.Width.Point, 0));
                    yPoint += 20;

                    if (yPoint > page.Height.Point - 40)
                    {
                        page = pdf.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPoint = 50; 
                    }
                }
            }
            else
            {
                gfx.DrawString("Немає даних для звіту", font, XBrushes.Black,
                    new XRect(XUnit.FromPoint(40).Point, XUnit.FromPoint(yPoint).Point, page.Width.Point, 0));
            }

            pdf.Save(fileName);
        }



        private DataTable GetOrderReport(DateTime startDate, DateTime endDate)
        {
            string query = @"
            SELECT 
            CONVERT(DATE, o.DateOfOrder) AS 'Дата початку',  -- Извлекаем только дату, без времени
            o.Status AS 'Статус',
            o.InitialPrice,
            o.Discount,
            o.TotalAmount,
            o.PaymentStatus,
            o.PaymentMethod
            FROM [Order] o
            WHERE o.DateOfOrder >= @StartDate AND o.DateOfOrder <= @EndDate
            ORDER BY o.DateOfOrder";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }


        private DataTable GetSparePartsReport()
        {
            string query = @"
            SELECT 
            sp.Name AS 'Запчасть',
            SUM(sp.Count) AS 'Кількість замовлених',
            SUM(sp.Price * sp.Count) AS 'Сума витрат'
            FROM SpareParts sp
            GROUP BY sp.Name
            ORDER BY sp.Name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = "";
            if (radioButton1.Checked)
            {
                DateTime startDate = dateTimePicker1.Value.Date;
                DateTime endDate = dateTimePicker2.Value.Date;

                DataTable data = GetOrderReport(startDate, endDate);
                fileName = @"C:\Reports\OrdersReport.pdf";
                GeneratedPdfReport2(data, fileName, $"Звіт про ремонти з {startDate.ToShortDateString()} по {endDate.ToShortDateString()} ");
            }
            else if (radioButton2.Checked)
            {
                DataTable data = GetSparePartsReport();
                fileName = @"C:\Reports\SparePartsReport.pdf";
                GeneratedPdfReport(data, fileName, $"Звіт про прибуток, замовлення та витрати на запчастини");
            }

            System.Diagnostics.Process.Start(fileName);
        }


    }
}
