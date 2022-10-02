using HtmlAgilityPack;
using System;
using System.Linq;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace exportar_fatura.Controllers
{
    class AgilityController
    {
        private string GetHTMLFilePath(TextBox textBox)
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

        private HtmlDocument GetHTMLDocument(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(path);
            return doc;
        }

        private HtmlNode GetTableBody(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//tbody");
        }

        private string GenerateDateColumn(HtmlNode cell)
        {
            var cellSpans = cell.Descendants("span").ToList();
            if (cellSpans.Count <= 0 || cellSpans == null)
                return null;
            var month = UtilsController.GetMonthNumberByName(cellSpans[1].InnerText);
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
    }
}
