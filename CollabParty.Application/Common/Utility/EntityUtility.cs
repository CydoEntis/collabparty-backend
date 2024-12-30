namespace CollabParty.Application.Common.Utility;

public static class EntityUtility
{
    public static bool EntityExists<T>(T entity)
    {
        return entity != null;
    }
}