using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Features.User.Commands.GetUserCommand
{
    public class GetUserResult
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public GetUserResult(Domain.Entities.User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            Id = user.Id ?? string.Empty;
            UserName = user.UserName ?? string.Empty;
            Email = user.Email ?? string.Empty;
            Name = user.Name ?? string.Empty;
        }
    }
}