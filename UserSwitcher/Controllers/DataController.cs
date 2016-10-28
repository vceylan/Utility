using System.Web.Mvc;
using UserSwitherComponent;

namespace UserSwitcher.Controllers
{
    public class DataController : Controller
    {
        public JsonResult GetUsers()
        {
            var result = UserSwitcherComponent.Instance.GetUsers();
            return Json(result);
        }

        public JsonResult GetDefaultUserInfoFilePath()
        {
            var result = UserSwitcherComponent.Instance.GetDefaultUserInfoFilePath();
            return Json(result);
        }
    }
}
