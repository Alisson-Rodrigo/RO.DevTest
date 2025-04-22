using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Services.LoggedUser;
using RO.DevTest.Domain.Exception;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;

        public UpdateProductCommandHandler(IProductRepository productRepository, ILogged logged)
        {
            _productRepository = productRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var produto = await ValidateUpdateProduct(command.Id, command, cancellationToken);

            if (!await _logged.IsInRole("Admin"))
            {
                throw new ForbiddenException("Apenas administradores podem atualizar produtos.");
            }

            produto.Name = command.Name;
            produto.Description = command.Description;
            produto.Price = command.Price;
            produto.Stock = command.Stock;
            produto.Category = command.Category;
            produto.IsActive = command.IsActive;
            produto.ModifiedOn = DateTime.UtcNow;

            var pasta = Path.Combine("wwwroot", "products", "images");

            var imagensUrls = new List<string>();
            const string UrlBase = "https://localhost:7014/products/images/";

            if (command.Imagens != null && command.Imagens.Any())
            {
                foreach (var imagem in command.Imagens)
                {
                    var nomeArquivo = $"{Guid.NewGuid()}_{imagem.FileName}";
                    var caminho = Path.Combine(pasta, nomeArquivo);

                    using (var stream = new FileStream(caminho, FileMode.Create))
                    {
                        await imagem.CopyToAsync(stream);
                    }

                    var url = $"{UrlBase}{nomeArquivo}";
                    imagensUrls.Add(url);
                }
            }

            produto.ImageUrl = imagensUrls;

            _productRepository.Update(produto);

            return Unit.Value;
        }

        public async Task<Domain.Entities.Product> ValidateUpdateProduct(Guid id, UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = _productRepository.Get(x => x.Id == id);

            if (product == null)
            {
                throw new BadRequestException("Produto não encontrado.");
            }

            var validator = new UpdateProductCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

            return product;
        }
    }
}
