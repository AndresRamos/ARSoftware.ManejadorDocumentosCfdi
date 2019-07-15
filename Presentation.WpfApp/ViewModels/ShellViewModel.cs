using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MediatR;
using Presentation.WpfApp.ViewModels.Solicitudes;

namespace Presentation.WpfApp.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;

        public ShellViewModel(IMediator mediator, IWindowManager windowManager, ListaSolicitudesViewModel listaSolicitudesViewModel)
        {
            _mediator = mediator;
            _windowManager = windowManager;
            Items.Add(listaSolicitudesViewModel);
        }
    }
}