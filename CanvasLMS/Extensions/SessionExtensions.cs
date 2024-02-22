using System.Text.Json;

namespace CanvasLMS.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            var jsonValue = JsonSerializer.Serialize(value);
            session.SetString(key, jsonValue);
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var jsonValue = session.GetString(key);
            return jsonValue == null ? default : JsonSerializer.Deserialize<T>(jsonValue);
        }
    }
}
