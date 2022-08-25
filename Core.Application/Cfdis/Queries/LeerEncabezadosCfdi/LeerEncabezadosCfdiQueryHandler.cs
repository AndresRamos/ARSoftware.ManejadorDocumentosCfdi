using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Application.Comprobantes.Interfaces;
using MediatR;

namespace Core.Application.Cfdis.Queries.LeerEncabezadosCfdi;

public class LeerEncabezadosCfdiQueryHandler : IRequestHandler<LeerEncabezadosCfdiQuery, IEnumerable<CfdiEncabezadoDto>>
{
    private readonly IComprobanteAddComercialRepository _comprobanteAddComercialRepository;
    private readonly IComprobanteAddContabilidadRepository _comprobanteAddContabilidadRepository;

    public LeerEncabezadosCfdiQueryHandler(IComprobanteAddContabilidadRepository comprobanteAddContabilidadRepository,
                                           IComprobanteAddComercialRepository comprobanteAddComercialRepository)
    {
        _comprobanteAddContabilidadRepository = comprobanteAddContabilidadRepository;
        _comprobanteAddComercialRepository = comprobanteAddComercialRepository;
    }

    public async Task<IEnumerable<CfdiEncabezadoDto>> Handle(LeerEncabezadosCfdiQuery request, CancellationToken cancellationToken)
    {
        var cfdiEncabezadoList = new List<CfdiEncabezadoDto>();

        foreach (string archivo in request.ArchivosCfdi)
        {
            XNamespace cfdiNs = "http://www.sat.gob.mx/cfd/3";
            XNamespace timbreFiscalDigitalNs = "http://www.sat.gob.mx/TimbreFiscalDigital";

            XDocument doc = XDocument.Load(archivo);

            XElement comprobanteElement = doc.Element(cfdiNs + "Comprobante");
            XElement emisorElement = doc.Descendants(cfdiNs + "Emisor").FirstOrDefault();
            XElement receptorElement = doc.Descendants(cfdiNs + "Receptor").FirstOrDefault();
            XElement timbreFiscalDigitalElement = doc.Descendants(timbreFiscalDigitalNs + "TimbreFiscalDigital").FirstOrDefault();

            var cfdiEncabezado = new CfdiEncabezadoDto
            {
                ComprobanteSerie = comprobanteElement?.Attribute("Serie")?.Value,
                ComprobanteFolio = comprobanteElement?.Attribute("Folio")?.Value,
                ComprobanteFecha = comprobanteElement?.Attribute("Fecha")?.Value,
                ComprobanteTipo = comprobanteElement?.Attribute("TipoDeComprobante")?.Value,
                ComprobanteMoneda = comprobanteElement?.Attribute("Moneda")?.Value,
                ComprobanteTotal = comprobanteElement?.Attribute("Total")?.Value,
                EmisorNombre = emisorElement?.Attribute("Nombre")?.Value,
                EmisorRfc = emisorElement?.Attribute("Rfc")?.Value,
                ReceptorNombre = receptorElement?.Attribute("Nombre")?.Value,
                ReceptorRfc = receptorElement?.Attribute("Rfc")?.Value,
                Uuid = timbreFiscalDigitalElement?.Attribute("UUID")?.Value
            };

            cfdiEncabezadoList.Add(cfdiEncabezado);
        }

        foreach (CfdiEncabezadoDto cfdi in cfdiEncabezadoList.Where(cfdi => !string.IsNullOrWhiteSpace(cfdi.Uuid)))
        {
            cfdi.ExisteEnComercial = _comprobanteAddComercialRepository != null &&
                                     await _comprobanteAddComercialRepository.ExisteUuidAsync(new Guid(cfdi.Uuid), cancellationToken);
            cfdi.ExisteEnContabilidad = _comprobanteAddContabilidadRepository != null &&
                                        await _comprobanteAddContabilidadRepository.ExisteUuidAsync(new Guid(cfdi.Uuid), cancellationToken);
        }

        return cfdiEncabezadoList;
    }
}
