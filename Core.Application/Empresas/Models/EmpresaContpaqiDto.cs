using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Application.Annotations;

namespace Core.Application.Empresas.Models
{
    public class EmpresaContpaqiDto : INotifyPropertyChanged
    {
        private string _baseDatos;
        private string _guidAdd;
        private string _nombre;

        public EmpresaContpaqiDto(string nombre, string baseDatos, string guidAdd)
        {
            Nombre = nombre;
            BaseDatos = baseDatos;
            GuidAdd = guidAdd;
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

        public string GuidAdd
        {
            get => _guidAdd;
            set
            {
                if (value == _guidAdd)
                {
                    return;
                }

                _guidAdd = value;
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