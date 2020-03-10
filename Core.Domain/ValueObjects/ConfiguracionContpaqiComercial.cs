using System.Collections.Generic;

namespace Core.Domain.ValueObjects
{
    public class ConfiguracionContpaqiComercial : ValueObject
    {
        private ConfiguracionContpaqiComercial()
        {
        }

        public string ContpaqiSqlConnectionString { get; private set; }
        public EmpresaContpaqi Empresa { get; private set; }

        public static ConfiguracionContpaqiComercial CreateInstance(string contpaqiSqConnectionString, EmpresaContpaqi empresa)
        {
            return new ConfiguracionContpaqiComercial
            {
                ContpaqiSqlConnectionString = contpaqiSqConnectionString,
                Empresa = empresa
            };
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ContpaqiSqlConnectionString;
            yield return Empresa;
        }
    }
}