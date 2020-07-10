using Newtonsoft.Json;

namespace PhotoMap.Messaging.Commands
{
    public class CommandBase
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, JsonSerializerSettings);
        }

        public static CommandBase Deserialize(string message)
        {
            return JsonConvert.DeserializeObject<CommandBase>(message, JsonSerializerSettings);
        }
    }
}
