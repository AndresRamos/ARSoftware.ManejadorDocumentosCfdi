using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Application.Annotations;

namespace Core.Application.Empresas.Models
{
    public class EmpresaContpaqiDto : INotifyPropertyChanged
    {
        private string _baseDatos;
        private string _nombre;

        public EmpresaContpaqiDto(string nombre, string baseDatos)
        {
            Nombre = nombre;
            BaseDatos = baseDatos;
        }

        public string Nombre
        {
            get => _nombre;
            set
            {
                if (value == _nombre)
                {
                    return;
                }

                _nombre = value;
                OnPropertyChanged();
            }
        }

        public string BaseDatos
        {
            get => _baseDatos;
            set
            {
                if (value == _baseDatos)
                {
                    return;
                }

                _baseDatos = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}