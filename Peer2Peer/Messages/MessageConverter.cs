using System.Text.Json;
using System.Text.Json.Serialization;

namespace Peer2Peer.Messages
{
    public class MessageConverter : JsonConverter<Message>
    {
        public override Message Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;
                string type = root.GetProperty("Type").GetString();
                string json = root.GetRawText();

                return type switch
                {
                    nameof(AssignWorkMessage) => JsonSerializer.Deserialize<AssignWorkMessage>(json, options),
                    nameof(ConnectMessage) => JsonSerializer.Deserialize<ConnectMessage>(json, options),
                    nameof(NewNodeMessage) => JsonSerializer.Deserialize<NewNodeMessage>(json, options),
                    nameof(NodeRegistryMessage) => JsonSerializer.Deserialize<NodeRegistryMessage>(json, options),
                    nameof(PasswordFoundMessage) => JsonSerializer.Deserialize<PasswordFoundMessage>(json, options),
                    nameof(RequestWorkMessage) => JsonSerializer.Deserialize<RequestWorkMessage>(json, options),
                    nameof(SetCoordinatorMessage) => JsonSerializer.Deserialize<SetCoordinatorMessage>(json, options),
                    nameof(WorkCompletedMessage) => JsonSerializer.Deserialize<WorkCompletedMessage>(json, options),
                    _ => throw new NotSupportedException($"Message type '{type}' is not supported")
                };
            }
        }

        public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}