namespace Core.Application.Cfdis;

public class CfdiEncabezadoDto
{
    public string ComprobanteFecha { get; set; }
    public string ComprobanteTipo { get; set; }
    public string ComprobanteSerie { get; set; }
    public string ComprobanteFolio { get; set; }
    public string EmisorNombre { get; set; }
    public string EmisorRfc { get; set; }
    public string ReceptorNombre { get; set; }
    public string ReceptorRfc { get; set; }
    public string ComprobanteMoneda { get; set; }
    public string ComprobanteTotal { get; set; }
    public string Uuid { get; set; }
    public bool ExisteEnComercial { get; set; }
    public bool ExisteEnContabilidad { get; set; }
}
