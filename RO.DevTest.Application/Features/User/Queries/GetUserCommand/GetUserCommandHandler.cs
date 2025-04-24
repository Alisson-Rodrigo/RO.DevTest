using MediatR;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Application.Features.User.Queries.GetUserCommand
{
    public class GetUserCommand : IRequest<GetUserResult> { }

    public class GetUserCommandHandler : IRequestHandler<GetUserCommand, GetUserResult>
    {
        private readonly ILogged _logged;
        public GetUserCommandHandler(
            ILogged logged)
        {
            _logged = logged;
        }
        public async Task<GetUserResult> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUser = await _logged.UserLogged();
                return new GetUserResult(currentUser);
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Falha ao obter dados do usuário");
            }
        }
    }
}