using System.IO;
using System.Windows.Forms;

namespace exportar_fatura.Controllers
{
    public static class UtilsController
    {
        public static void LogText(string message, RichTextBox richText)
        {
            richText.Text += message + "\n";
        }

        public static string GetMonthNumberByName(string name)
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

        private static string GetFixedAccountsFile()
        {
            return File.ReadAllText("DespesasFixas.txt") + "\n";
        }
    }
}
