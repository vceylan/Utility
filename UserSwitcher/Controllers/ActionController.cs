using System.Web.Mvc;
using Model;
using UserSwitherComponent;

namespace UserSwitcher.Controllers
{
    public class ActionController : Controller
    {
        public JsonResult SaveUser(UserSwitchModel user)
        {
            var result = UserSwitcherComponent.Instance.SaveUser(user);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveUser(int userId)
        {
            var result = UserSwitcherComponent.Instance.RemoveUser(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult SetDefaultUser(int userId, string filePath)
        {
            var result = UserSwitcherComponent.Instance.SetDefaultUser(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetDefaultUserInfoFilePath(string filePath)
        {
            var result = UserSwitcherComponent.Instance.SetDefaultUserInfoFilePath(filePath);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
