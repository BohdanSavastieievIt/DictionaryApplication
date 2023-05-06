using Newtonsoft.Json;
using System.Collections.Generic;

namespace DictionaryApplication.Data
{
    public static class SessionExtensions
    {
        public static List<T> GetList<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<List<T>>(data);
        }

        public static void SetList<T>(this ISession session, string key, List<T> value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static KnowledgeTestModel GetKnowledgeTest(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<KnowledgeTestModel>(data);
        }

        public static void SetKnowledgeTest(this ISession session, string key, KnowledgeTestModel value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}
