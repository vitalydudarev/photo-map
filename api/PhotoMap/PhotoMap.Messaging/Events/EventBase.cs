using Newtonsoft.Json;

namespace PhotoMap.Messaging.Events
{
    public class EventBase
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, JsonSerializerSettings);
        }

        public static EventBase Deserialize(string message)
        {
            return JsonConvert.DeserializeObject<EventBase>(message, JsonSerializerSettings);
        }
    }
}
