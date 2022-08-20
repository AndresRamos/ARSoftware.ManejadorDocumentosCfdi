namespace Core.Application.Rfcs.Models
{
    public class RfcDto
    {
        public RfcDto(string codigo, string rfc, string razonSocial)
        {
            Codigo = codigo;
            Rfc = rfc;
            RazonSocial = razonSocial;
        }

        public string Codigo { get; set; }
        public string Rfc { get; set; }
        public string RazonSocial { get; set; }
    }
}
