using System.Reflection;

namespace CanvasLMS.Utilities
{
    public static class ObjectMapper
    {
        public static void MapProperties<TSource, TTarget>(TSource source, TTarget target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            foreach (PropertyInfo sourceProperty in sourceType.GetProperties())
            {
                PropertyInfo targetProperty = targetType.GetProperty(sourceProperty.Name);

                if (targetProperty != null && targetProperty.CanWrite)
                {
                    object value = sourceProperty.GetValue(source);
                    targetProperty.SetValue(target, value);
                }
            }
        }
    }
}
