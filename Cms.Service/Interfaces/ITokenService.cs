using Cms.Repository.Entities;

namespace Cms.Service.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
