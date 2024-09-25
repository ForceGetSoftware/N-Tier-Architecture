namespace N_Tier.Shared.N_Tier.Application.Helpers
{
    public static class ReflectionHelper
    {
        public static string GetPropertyValue<T>(T entity, string propertyName)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var propertyInfo = typeof(T).GetProperty(propertyName);

            return propertyInfo == null
                ? throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'")
                : propertyInfo.GetValue(entity).ToString();
        }
    }
}
