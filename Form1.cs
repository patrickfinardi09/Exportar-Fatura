using exportar_fatura.Controllers;
using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace exportar_fatura
{
    public partial class Form1 : Form
    {
        AgilityController agilityController;

        public Form1()
        {
            InitializeComponent();
            agilityController = new AgilityController();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = GetHTMLFilePath();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                return;

            var doc = GetHTMLDocument(textBox1.Text);

            var tableBody = GetTableBody(doc);

            HtmlNodeCollection tableRows = tableBody.ChildNodes;

            var lastDate = DateTime.Now;

            var finalResponse = string.Empty;

            if (checkBox1.Checked)
                finalResponse += GetFixedAccountsFile();

            foreach (var row in tableRows)
            {
                if (row.NodeType == HtmlNodeType.Element &&
                    !row.InnerText.Contains("PAGTO. POR DEB EM C/C") &&
                    !row.InnerText.Contains("SALDO ANTERIOR"))
                {
                    var i = 0;
                    HtmlNodeCollection rowCells = row.ChildNodes;
                    string date, desc, value, parcels = string.Empty;
                    foreach (var cell in rowCells)
                    {
                        if (cell.NodeType == HtmlNodeType.Element && cell.Name == "td")
                        {
                            i++;
                            switch (i)
                            {
                                case 1:
                                    date = GenerateDateColumn(cell);
                                    if (date == null)
                                        date = lastDate.ToString("dd/MM/yyyy");
                                    lastDate = DateTime.Parse(date);
                                    break;
                                case 2:
                                    desc = GenerateDescriptionColumn(cell);
                                    break;
                                case 6:
                                    value = GenerateValueColumn(cell);
                                    parcels = GenerateParcels(cell);
                                    break;
                                default:
                                    continue;
                            }
                        }
                    }
                    finalResponse += $"{date}\t{desc} {parcels}\t{value}\n";
                }
            }
            LogText(finalResponse);
        }
    }
}
