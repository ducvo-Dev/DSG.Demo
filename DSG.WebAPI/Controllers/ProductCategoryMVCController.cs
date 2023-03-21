using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSG.WebAPI.Controllers
{
    [Authorize]
    public class ProductCategoryMVCController : Controller
    {
        // GET: ProductCategoryMVC
        public ActionResult Index()
        {
            return View();
        }
    }
}