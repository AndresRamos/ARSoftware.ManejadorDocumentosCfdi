﻿using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Empresas.Commands.EliminarEmpresa
{
    public class EliminarEmpresaCommandHandler : IRequestHandler<EliminarEmpresaCommand>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public EliminarEmpresaCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(EliminarEmpresaCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _context.Empresas.Include(e => e.ConfiguracionGeneral).Include(e => e.Solicitudes).SingleOrDefaultAsync(e => e.Id == request.EmpresaId, cancellationToken);

            if (empresa is null)
            {
                throw new ObjectNotFoundException($"No se encontro la empresa con id {request.EmpresaId}.");
            }

            if (!empresa.CanEliminar)
            {
                throw new InvalidOperationException("No se puede eliminar la empresa.");
            }

            _context.Empresas.Remove(empresa);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}