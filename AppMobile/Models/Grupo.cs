using System;

namespace AppMobile.Models
{
    public class Grupo
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public byte[] Icone { get; set; }
        public int QuantidadeMaxParticipantes { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataRevelacao { get; set; }
        public string Descricao { get; set; }
    }
}
