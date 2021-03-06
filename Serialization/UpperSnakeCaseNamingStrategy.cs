using Newtonsoft.Json.Serialization;

namespace pwr_msi.Serialization {
    public class UpperSnakeCaseNamingStrategy : SnakeCaseNamingStrategy {
        protected override string ResolvePropertyName(string name) {
            return base.ResolvePropertyName(name).ToUpperInvariant();
        }
    }
}
