namespace Core.Domain.Entities
{
    public class PaqueteId
    {
        private PaqueteId()
        {
        }

        public int Id { get; private set; }
        public string IdPaquete { get; private set; }
        public bool IsDescargado { get; private set; }

        public static PaqueteId Crear(string idPaquete)
        {
            return new PaqueteId
            {
                IdPaquete = idPaquete
            };
        }

        public void SetDescargado()
        {
            IsDescargado = true;
        }
    }
}