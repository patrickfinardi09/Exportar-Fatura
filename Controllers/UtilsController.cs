using System;
using System.Collections.Generic;
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

        public static List<Despesa> GetFixedExpensesFromFile()
        {
            var tempList = new List<Despesa>();
            var file = File.ReadAllLines("DespesasFixas.txt");
            foreach (var line in file)
            {
                var split = line.Split('\t');
                tempList.Add(new Despesa()
                {
                    Data = DateTime.Parse(split[0]),
                    Descricao = split[1],
                    Valor = Double.Parse(split[2])
                });
            }
            return tempList;
        }
    }
}
