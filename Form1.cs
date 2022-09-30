using HtmlAgilityPack;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace exportar_fatura
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void LogText(string message)
        {
            if (richTextBox1.Text == "")
                richTextBox1.Text += message;
            else
                richTextBox1.Text += "\n" + message; 
        }

        private string GetHTMLFilePath()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "HTML files (*.html)|*.html";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            return textBox1.Text;
        }

        private string GetMonthNumberByName(string name)
        {
            switch (name)
            {
                case "JAN":
                    return "01";
                case "FEV":
                    return "02";
                case "MAR":
                    return "03";
                case "ABR":
                    return "04";
                case "MAI":
                    return "05";
                case "JUN":
                    return "06";
                case "JUL":
                    return "07";
                case "AGO":
                    return "08";
                case "SET":
                    return "09";
                case "OUT":
                    return "10";
                case "NOV":
                    return "11";
                default:
                    return "12";
            }
        }

        private HtmlAgilityPack.HtmlDocument GetHTMLDocument(string path)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(path);
            return doc;
        }

        private HtmlNode GetTableBody(HtmlAgilityPack.HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//tbody");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = GetHTMLFilePath();
        }

        private string GenerateDateColumn(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return null;
            var month = GetMonthNumberByName(cellSpans[1].InnerText);
            return DateTime.Parse($"{cellSpans[0].InnerText}/{month}/{DateTime.Now.Year}").ToString("dd/MM/yyyy");
        }

        private string GenerateValueColumn(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return null;
            return cellSpans[0].InnerText.Contains("Parcela") ?
                cellSpans[0].InnerText.Substring(13) :
                cellSpans[0].InnerText;
        }

        private string GenerateParcels(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return null;

            if (!cellSpans[0].InnerText.Contains("Parcela"))
                return string.Empty;

            return cellSpans[0].InnerText.Substring(8, 5);
        }

        private string GenerateDescriptionColumn(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return null;
            return cellSpans[0].InnerText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                return;

            var doc = GetHTMLDocument(textBox1.Text);

            var tableBody = GetTableBody(doc);

            HtmlNodeCollection tableRows = tableBody.ChildNodes;
            var lastDate = DateTime.Now;

            foreach (var row in tableRows)
            {
                if (row.NodeType == HtmlNodeType.Element &&
                    !row.InnerText.Contains("PAGTO. POR DEB EM C/C") &&
                    !row.InnerText.Contains("SALDO ANTERIOR"))
                {
                    var i = 0;
                    HtmlNodeCollection rowCells = row.ChildNodes;
                    var date = string.Empty;
                    var desc = string.Empty;
                    var value = string.Empty;
                    var parcels = string.Empty;
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
                    LogText($"{date}\t{desc} {parcels}\t{value}");
                }
            }
        }
    }
}
