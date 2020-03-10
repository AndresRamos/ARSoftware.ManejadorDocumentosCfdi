using System.Collections.Generic;

namespace Core.Domain.ValueObjects
{
    public class EmpresaContpaqi : ValueObject
    {
        private EmpresaContpaqi()
        {
        }

        public string Nombre { get; private set; }
        public string BaseDatos { get; private set; }

        public static EmpresaContpaqi CreateInstance(string nombre, string baseDatos)
        {
            return new EmpresaContpaqi
            {
                Nombre = nombre,
                BaseDatos = baseDatos
            };
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Nombre;
            yield return BaseDatos;
        }
    }
}