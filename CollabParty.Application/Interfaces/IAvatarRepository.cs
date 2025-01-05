using System.Linq.Expressions;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;

namespace CollabParty.Application.Interfaces;

public interface IAvatarRepository : IBaseRepository<Avatar>
{
    Task<int> CountAsync(Expression<Func<Avatar, bool>> filter = null);
}