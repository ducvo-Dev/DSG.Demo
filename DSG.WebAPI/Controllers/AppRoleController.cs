using AutoMapper;
using DSG.Common.Exceptions;
using DSG.Model.Models;
using DSG.Service;
using DSG.WebAPI.Infrastructure;
using DSG.WebAPI.Mappings;
using DSG.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DSG.WebAPI.Controllers
{

    [Authorize]
    public class AppRoleController : Controller
    {
        private IApplicationRoleService _appRoleService;
        private readonly IMapper Mapper;

        public AppRoleController(IApplicationRoleService appRoleService)
        {
            _appRoleService = appRoleService;
            Mapper = AutoMapperConfiguration.mapper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var AppGroupModel = _appRoleService.GetAll();
            var viewModel = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(AppGroupModel);
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult IndexPage(int page, int pageSize, string filter = null)
        {
            int totalRow = 6;
            var model = _appRoleService.GetAll(page, pageSize, out totalRow, filter);
            IEnumerable<ApplicationRoleViewModel> modelVm = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(model);

            PaginationSet<ApplicationRoleViewModel> pagedSet = new PaginationSet<ApplicationRoleViewModel>()
            {
                Page = page,
                TotalCount = totalRow,
                TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
                Items = modelVm
            };
            return View(pagedSet);
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            AppRole appRole = _appRoleService.GetDetail(id);
            if (appRole == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Không có dữ liệu");
            }
            else
            {
                var applicationRoleViewModel = Mapper.Map<AppRole, ApplicationRoleViewModel>(appRole);
                return View(applicationRoleViewModel);
            }
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(ApplicationRoleViewModel applicationRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var newAppRole = new AppRole();
                newAppRole.Id = Guid.NewGuid().ToString();
                newAppRole.Name = applicationRoleViewModel.Name;
                newAppRole.Description = applicationRoleViewModel.Description;
              
                try
                {
                    _appRoleService.Add(newAppRole);
                    _appRoleService.Save();
                    return RedirectToAction("Index");
                }
                catch (NameDuplicatedException dex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, dex.Message);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
               
            }
        }
        [HttpGet]
        public ActionResult Update(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            AppRole appRole = _appRoleService.GetDetail(id);
            if (appRole == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Không có dữ liệu");
            }
            else
            {
              
                var applicationRoleViewModel = Mapper.Map<AppRole, ApplicationRoleViewModel>(appRole);
                return View(applicationRoleViewModel);
            }
        }
        [HttpPost]
        public ActionResult Update(ApplicationRoleViewModel applicationRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var appRole = _appRoleService.GetDetail(applicationRoleViewModel.Id);
                try
                {
                    appRole.Id = applicationRoleViewModel.Id;

                    _appRoleService.Update(appRole);
                    _appRoleService.Save();
                    return RedirectToAction("Index");
                }
                catch (NameDuplicatedException dex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, dex.Message);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult Delete(string id)
        {
            _appRoleService.Delete(id);
            _appRoleService.Save();
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public ActionResult DeleteMulti(string checkedList)
        {
            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }
            else
            {
                var listItem = new JavaScriptSerializer().Deserialize<List<string>>(checkedList);
                foreach (var item in listItem)
                {
                    _appRoleService.Delete(item);
                }

                _appRoleService.Save();
                return RedirectToAction("Index");
            }
        }
    }
}
