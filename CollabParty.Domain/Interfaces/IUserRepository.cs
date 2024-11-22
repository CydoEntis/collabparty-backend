using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface IUserRepository : IBaseRepository<ApplicationUser>
{
    Task<ApplicationUser> UpdateAsync(ApplicationUser entity);
}