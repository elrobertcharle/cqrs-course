using CQRS.Core.Events;
using Post.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Post.Query.Consumer
{
    public class EventJsonConverter : JsonConverter<BaseEvent>
    {
        public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //roberto: there is a better way to do this

            if (!JsonDocument.TryParseValue(ref reader, out var doc))
                throw new JsonException($"Failed to parse {nameof(JsonDocument)}.");

            if (!doc.RootElement.TryGetProperty("Type", out var type))
                throw new InvalidOperationException("The property type is missing.");

            var typeDiscriminator = type.GetString();
            var json = doc.RootElement.GetRawText();

            return typeDiscriminator switch
            {
                nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json, options),
                nameof(PostMessageUpdatedEvent) => JsonSerializer.Deserialize<PostMessageUpdatedEvent>(json, options),
                nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json, options),
                nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
                nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
                nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(json, options),
                nameof(PostRemovedEvent) => JsonSerializer.Deserialize<PostRemovedEvent>(json, options),
                _ => throw new InvalidOperationException($"Unknown discriminator type. TypeDiscriminator={typeDiscriminator}.")
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
