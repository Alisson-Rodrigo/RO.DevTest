using MediatR;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Application.Features.Product.Commands.CreatedProductCommand
{
    public class CreateProductCommandHandler : IRequestHandler<CreatedProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;

        public CreateProductCommandHandler(IProductRepository productRepository, ILogged logged)
        {
            _productRepository = productRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(CreatedProductCommand command, CancellationToken cancellationToken)
        {
            if (!await _logged.IsInRole("Admin"))
            {
                throw new ForbiddenException("Apenas administradores podem cadastrar produtos.");
            }

            await ValidateProduct(command, cancellationToken);

            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "products", "images");
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            var imagensUrls = new List<string>();
            const string UrlBase = "http://localhost:8080/products/images/";

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

            var product = command.AssignToProduct();
            product.ImageUrl = imagensUrls;

            await _productRepository.CreateAsync(product, cancellationToken);

            return Unit.Value;
        }

        private async Task ValidateProduct(CreatedProductCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatedProductCommandValidator();
            var response = await validator.ValidateAsync(request, cancellationToken);

            if (!response.IsValid)
            {
                throw new BadRequestException(response);
            }
        }
    }
}
