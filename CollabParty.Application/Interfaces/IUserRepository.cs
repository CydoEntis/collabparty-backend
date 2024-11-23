using CollabParty.Domain.Entities;

namespace CollabParty.Application.Interfaces;

public interface IUserRepository : IBaseRepository<ApplicationUser>
{
    Task<ApplicationUser> UpdateAsync(ApplicationUser entity);
}