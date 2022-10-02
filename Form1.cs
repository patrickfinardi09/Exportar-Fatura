using exportar_fatura.Controllers;
using System;
using System.Collections.Generic;
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
            txtPDFFile.Text = agilityController.
                GetHTMLFilePath(txtPDFFile as TextBox);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtPDFFile.Text == "")
                return;

            txtResult.Text = string.Empty;

            var doc = agilityController.GetHTMLDocument(txtPDFFile.Text);
            var tableBody = agilityController.GetTableBody(doc);
            var tableRows = tableBody.ChildNodes;
            var lastDate = DateTime.Now;
            var finalResponse = new List<Despesa>();

            if (chkNextMonth.Checked)
                finalResponse.AddRange(UtilsController.GetFixedExpensesFromFile());

            finalResponse.AddRange(agilityController.RunThroughtTableRows(tableRows, chkNextMonth.Checked));

            foreach (var despesa in finalResponse)
            {
                UtilsController.LogText(despesa.ToString(), txtResult);
            }
        }
    }
}