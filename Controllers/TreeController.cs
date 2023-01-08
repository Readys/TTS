using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TTS20.Models;
using TTS20.Tools;

namespace TTS.Controllers
{
    public class TreeController : Controller
    {
        private TTS20Entities db = new TTS20Entities();
        private HRMEntities h_db = new HRMEntities();
        public const int testUser = 0; //Тестовый вход под любым пользователем//598 181 211 783 471 37
        // GET: Tree
        public ActionResult Index(int type = 1)
        {
            #region Userblock
            var allUsers = h_db.User.Where(x => x.AccountAD != null).ToList();
            var appUser = h_db.User.FirstOrDefault(x => x.UserId == testUser);

            if (testUser == 0)
            {
                appUser = allUsers.FirstOrDefault(x => x.AccountAD.ToUpper() == User.Identity.Name.ToUpper());
            }

            if (appUser == null)
            {
                return View("Access");
            }

            var access = db.UserInRole.Where(x => x.UserId == appUser.UserId).ToList();

            ViewBag.AppUserId = appUser.UserId;
            #endregion

            var tree = db.Tree.Where(w => w.Deleted != 1).ToList();

            var category = tree.Where(w => w.Deleted != 1 && w.TypeId == type).ToList(); //.OrderBy(o=> o.NameRu)

            var catTree = category.GenerateTree(c => c.TreeId, c => c.ParentId);

            ViewBag.Tree = catTree;

            ViewBag.Type = type;

            return View(category);
        }


    }
}