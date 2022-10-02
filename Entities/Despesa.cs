using System;

namespace exportar_fatura
{
    public class Despesa
    {
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public int Parcelas { get; set; }
        public int ParcelaAtual { get; set; }

        public override string ToString()
        {
            var data = Data.ToString("dd/MM/yyyy");
            var desc = $"{Descricao}{(Parcelas > 0 ? $" {ParcelaAtual}/{Parcelas}" : null)}";
            var val = Valor;
            return $"{data}\t{desc}\t{val}";
        }
    }
}
