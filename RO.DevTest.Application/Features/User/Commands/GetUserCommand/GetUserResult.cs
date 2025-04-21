using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Features.User.Commands.GetUserCommand
{
    public class GetUserResult
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // Construtor padrão para serialização
        public GetUserResult()
        {
            UserName = string.Empty;
            Name = string.Empty;
            Email = string.Empty;
        }

        // Construtor com parâmetro para mapeamento fácil
        public GetUserResult(Domain.Entities.User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            UserName = user.UserName ?? string.Empty;
            Email = user.Email ?? string.Empty;
            Name = user.Name ?? string.Empty;
        }
    }
}