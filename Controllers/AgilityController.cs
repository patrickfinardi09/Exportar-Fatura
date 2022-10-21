using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace exportar_fatura.Controllers
{
    class AgilityController
    {
        DateTime LastDate;

        public string GetHTMLFilePath(TextBox textBox)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "HTML files (*.html)|*.html",
                FilterIndex = 0,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            return textBox.Text;
        }

        public HtmlDocument GetHTMLDocument(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(path);
            return doc;
        }

        public HtmlNode GetTableBody(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//tbody");
        }

        public List<Despesa> RunThroughtTableRows(HtmlNodeCollection tableRows, bool nextMonth)
        {
            var tempList = new List<Despesa>();
            foreach (var row in tableRows)
            {
                if (row.NodeType == HtmlNodeType.Element &&
                    !row.InnerText.Contains("PAGTO. POR DEB EM C/C") &&
                    !row.InnerText.Contains("SALDO ANTERIOR"))
                {
                    var tempDespesa = new Despesa();
                    var rowCells = row.Descendants("td").ToList();

                    tempDespesa.ParcelaAtual = GenerateActualParcel(rowCells[7]);
                    tempDespesa.Parcelas = GenerateParcels(rowCells[7]);
                    tempDespesa.Valor = GenerateValueColumn(rowCells[7]);
                    tempDespesa.Data = GenerateDateColumn(rowCells[0]);
                    tempDespesa.Descricao = GenerateDescriptionColumn(rowCells[1]);

                    LastDate = tempDespesa.Data;

                    if (nextMonth)
                    {
                        if (!row.InnerText.Contains("Parcela"))
                            continue;
                        
                        if (tempDespesa.ParcelaAtual == tempDespesa.Parcelas)
                            continue;

                        tempDespesa.ParcelaAtual = tempDespesa.ParcelaAtual + 1;
                    }
                    tempList.Add(tempDespesa);
                }
            }
            return tempList;
        }

        public DateTime GenerateDateColumn(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return LastDate;
            var month = UtilsController.GetMonthNumberByName(cellSpans[1].InnerText);
            return DateTime.Parse($"{cellSpans[0].InnerText}/{month}/{DateTime.Now.Year}");
        }

        public double GenerateValueColumn(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return 0.0;
            return cellSpans[0].InnerText.Contains("Parcela") ?
                double.Parse(cellSpans[0].InnerText.Substring(13)) :
                double.Parse(cellSpans[0].InnerText);
        }

        public int GenerateParcels(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return 0;

            if (!cellSpans[0].InnerText.Contains("Parcela"))
                return 0;

            return int.Parse(cellSpans[0].InnerText.Substring(11, 2));
        }

        public int GenerateActualParcel(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return 0;

            if (!cellSpans[0].InnerText.Contains("Parcela"))
                return 0;

            return int.Parse(cellSpans[0].InnerText.Substring(8, 2));
        }

        public string GenerateDescriptionColumn(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return null;
            return cellSpans[0].InnerText;
        }
    }
}
