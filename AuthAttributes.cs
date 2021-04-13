using System;

namespace pwr_msi {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal class AdminAuthorizeAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal class RestaurantAuthorizeAttribute : Attribute {
        public RestaurantPermission Permission { get; }
        public string RouteArgument { get; }

        public RestaurantAuthorizeAttribute(RestaurantPermission permission, string routeArgument = "restaurantId") {
            Permission = permission;
            RouteArgument = routeArgument;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal class ManageRestaurantAuthorizeAttribute : RestaurantAuthorizeAttribute {
        public ManageRestaurantAuthorizeAttribute(string routeArgument = "restaurantId") : base(RestaurantPermission.MANAGE, routeArgument) {
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal class AcceptOrdersRestaurantAuthorizeAttribute : RestaurantAuthorizeAttribute {
        public AcceptOrdersRestaurantAuthorizeAttribute(string routeArgument = "restaurantId") : base(RestaurantPermission.ACCEPT, routeArgument) {
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal class DeliverFromRestaurantAuthorizeAttribute : RestaurantAuthorizeAttribute {
        public DeliverFromRestaurantAuthorizeAttribute(string routeArgument = "restaurantId") : base(RestaurantPermission.DELIVER, routeArgument) {
        }
    }

    internal enum RestaurantPermission {
        MANAGE,
        ACCEPT,
        DELIVER,
    };
}
