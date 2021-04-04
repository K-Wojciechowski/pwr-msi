using System;

namespace pwr_msi.AuthPolicies {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AdminAuthorizeAttribute : Attribute {
    }
}
