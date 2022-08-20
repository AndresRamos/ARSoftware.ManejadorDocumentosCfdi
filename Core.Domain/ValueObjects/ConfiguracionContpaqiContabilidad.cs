using System.Collections.Generic;

namespace Core.Domain.ValueObjects
{
    public class ConfiguracionContpaqiContabilidad : ValueObject
    {
        private ConfiguracionContpaqiContabilidad()
        {
        }

        public string ContpaqiSqlConnectionString { get; private set; }
        public EmpresaContpaqi Empresa { get; private set; }

        public static ConfiguracionContpaqiContabilidad CreateInstance(string contpaqiSqConnectionString, EmpresaContpaqi empresa)
        {
            return new ConfiguracionContpaqiContabilidad { ContpaqiSqlConnectionString = contpaqiSqConnectionString, Empresa = empresa };
        }

        public static ConfiguracionContpaqiContabilidad CreateSinAsignar()
        {
            return CreateInstance("", EmpresaContpaqi.CreateSinAsignar());
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ContpaqiSqlConnectionString;
            yield return Empresa;
        }
    }
}
