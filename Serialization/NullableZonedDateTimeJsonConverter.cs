using System;
using Newtonsoft.Json;
using NodaTime;

namespace pwr_msi.Serialization {
    public class NullableZonedDateTimeJsonConverter : JsonConverter<ZonedDateTime?> {
        private readonly ZonedDateTimeJsonConverter baseConverter;

        public NullableZonedDateTimeJsonConverter() {
            baseConverter = new ZonedDateTimeJsonConverter();
        }

        public override void WriteJson(JsonWriter writer, ZonedDateTime? value, JsonSerializer serializer) {
            if (value.HasValue) {
                baseConverter.WriteJson(writer, value.Value, serializer);
            } else {
                writer.WriteNull();
            }
        }

        public override ZonedDateTime? ReadJson(JsonReader reader, Type objectType, ZonedDateTime? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) {
                return hasExistingValue ? existingValue : null;
            }

            return (ZonedDateTime?) baseConverter.ReadJson(reader, typeof(ZonedDateTime), existingValue, serializer);
        }
    }
}
