using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace pwr_msi.Serialization {
    public static class MsiSerializerSettings {
        public static readonly JsonSerializerSettings jsonSerializerSettings = new() {
            FloatParseHandling = FloatParseHandling.Decimal,
            ContractResolver = new DefaultContractResolver {NamingStrategy = new CamelCaseNamingStrategy()},
            Converters = new List<JsonConverter> {
                new StringEnumConverter(new UpperSnakeCaseNamingStrategy(), allowIntegerValues: false)
                },
        };
    }
}
