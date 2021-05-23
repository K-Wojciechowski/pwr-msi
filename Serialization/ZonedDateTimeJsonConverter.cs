using System;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Text;

namespace pwr_msi.Serialization {
    public class ZonedDateTimeJsonConverter : JsonConverter<ZonedDateTime> {

        public override void WriteJson(JsonWriter writer, ZonedDateTime value, JsonSerializer serializer) {
            var dtStr = OffsetDateTimePattern.Rfc3339.Format(value.ToOffsetDateTime());
            writer.WriteValue(dtStr);
        }

        public override ZonedDateTime ReadJson(JsonReader reader, Type objectType, ZonedDateTime existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.String) {
                throw new Exception("String expected for date");
            }

            var dtString = (string)reader.Value;
            return OffsetDateTimePattern.Rfc3339.Parse(dtString).Value.InFixedZone();
        }
    }
}
