using Microsoft.AspNetCore.Mvc;
using pwr_msi.Models;

namespace pwr_msi.Controllers {
    public class MsiControllerBase : ControllerBase {
        public User MsiUser => (User) HttpContext.Items["User"];
        public User MsiUserId => (User) HttpContext.Items["UserId"];
    }
}
