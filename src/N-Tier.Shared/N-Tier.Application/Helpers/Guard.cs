namespace N_Tier.Shared.N_Tier.Application.Helpers;

public static class Guard
{
    public static void Against<TException>(bool condition, string message) where TException : Exception, new()
    {
        if (condition)
        {
            var exception = (TException)Activator.CreateInstance(typeof(TException), message);
            throw exception;
        }
    }
}
