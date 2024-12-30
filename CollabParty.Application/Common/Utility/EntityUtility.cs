namespace CollabParty.Application.Common.Utility;

public static class EntityUtility
{
    public static bool EntityIsNull<T>(T entity)
    {
        return entity == null;
    }
}