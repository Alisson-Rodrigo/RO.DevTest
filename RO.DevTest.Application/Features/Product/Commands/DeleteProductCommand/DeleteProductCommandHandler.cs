﻿using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly IProductRepository _productRepository;
        public DeleteProductCommandHandler(ILogged logged, IProductRepository productRepository)
        {
            _logged = logged;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle (DeleteProductCommand command, CancellationToken cancellationToken)
        {
            if (!await _logged.IsInRole("Admin"))
            {
                throw new ForbiddenException("Apenas administradores podem atualizar produtos.");
            }

            var product = _productRepository.Get(x => x.Id == command.Id);

            if (product == null)
            {
                throw new BadRequestException("Erro ao deletar produto.");
            }

            await _productRepository.Delete(product);

            return Unit.Value;
        }

    }
}
