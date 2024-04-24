namespace AppMobile.Models
{
    public class Sorteio
    {
        public int ID { get; set; }
        public int ID_Grupo { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_ParticipanteSorteado { get; set; }
        public DateTime DataSorteio { get; set; }
    }
}
