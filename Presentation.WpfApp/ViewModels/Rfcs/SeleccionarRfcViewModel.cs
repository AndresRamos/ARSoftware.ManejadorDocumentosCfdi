using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Rfcs.Models;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Rfcs
{
    public sealed class SeleccionarRfcViewModel : Screen
    {
        private string _filtro;
        private RfcDto _rfcSeleccionado;

        public SeleccionarRfcViewModel()
        {
            DisplayName = "Seleccionar RFC";
            RfcsView = CollectionViewSource.GetDefaultView(Rfcs);
            RfcsView.Filter = RfcsView_Filter;
        }

        public string Filtro
        {
            get => _filtro;
            set
            {
                if (value == _filtro)
                {
                    return;
                }

                _filtro = value;
                NotifyOfPropertyChange(() => Filtro);
                RfcsView.Refresh();
            }
        }

        public BindableCollection<RfcDto> Rfcs { get; } = new BindableCollection<RfcDto>();

        public ICollectionView RfcsView { get; }

        public RfcDto RfcSeleccionado
        {
            get => _rfcSeleccionado;
            set
            {
                if (Equals(value, _rfcSeleccionado))
                {
                    return;
                }

                _rfcSeleccionado = value;
                NotifyOfPropertyChange(() => RfcSeleccionado);
            }
        }

        public bool SeleccionoRfc { get; private set; }

        public void Inicializar(IEnumerable<RfcDto> rfcs)
        {
            SeleccionoRfc = false;
            Rfcs.Clear();
            Rfcs.AddRange(rfcs);
        }

        public void Seleccionar()
        {
            SeleccionoRfc = true;
            TryClose();
        }

        public void Cancelar()
        {
            RfcSeleccionado = null;
            SeleccionoRfc = false;
            TryClose();
        }

        private bool RfcsView_Filter(object obj)
        {
            if (!(obj is RfcDto rfc))
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return string.IsNullOrEmpty(Filtro) ||
                   rfc.Codigo.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   rfc.Rfc.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   rfc.RazonSocial.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}