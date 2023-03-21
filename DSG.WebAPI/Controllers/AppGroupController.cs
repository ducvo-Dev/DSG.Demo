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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DSG.WebAPI.Controllers
{

    [Authorize]
    public class AppGroupController : Controller
    {
        private IApplicationGroupService _appGroupService;
        private IApplicationRoleService _appRoleService;
        private ApplicationUserManager _userManager;
        private readonly IMapper Mapper;

        public AppGroupController(IErrorService errorService,
            IApplicationRoleService appRoleService,
            ApplicationUserManager userManager,
            IApplicationGroupService appGroupService)
        {
            _appGroupService = appGroupService;
            _appRoleService = appRoleService;
            _userManager = userManager;
            Mapper = AutoMapperConfiguration.mapper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var AppGroupModel = _appGroupService.GetAll();
            var viewModel = Mapper.Map<IEnumerable<ApplicationGroup>, IEnumerable<ApplicationGroupViewModel>>(AppGroupModel);
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult GetListPaging(int page, int pageSize, string filter = null)
        {
  
            int totalRow = 0;
            var model = _appGroupService.GetAll(page, pageSize, out totalRow, filter);
            IEnumerable<ApplicationGroupViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationGroup>, IEnumerable<ApplicationGroupViewModel>>(model);

            PaginationSet<ApplicationGroupViewModel> pagedSet = new PaginationSet<ApplicationGroupViewModel>()
            {
                Page = page,
                TotalCount = totalRow,
                TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
                Items = modelVm
            };
            // request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return View(pagedSet);
        }

        [HttpGet]
        public ActionResult Details( int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            ApplicationGroup appGroup = _appGroupService.GetDetail(id);
            var appGroupViewModel = Mapper.Map<ApplicationGroup, ApplicationGroupViewModel>(appGroup);
            if (appGroup == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Không có Group");
            }
            var listRole = _appRoleService.GetListRoleByGroupId(appGroupViewModel.ID);
            appGroupViewModel.Roles = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(listRole);
            return View(appGroupViewModel); 
        }
        public ActionResult Create()
        {
            var listGroup = _appRoleService.GetAll();
            ApplicationGroupViewModel applicationGroupViewModel = new ApplicationGroupViewModel();
            applicationGroupViewModel.Roles = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(listGroup);
            return View(applicationGroupViewModel);
        }
        [HttpPost]
        public ActionResult Create(ApplicationGroupViewModel appGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                var newAppGroup = new ApplicationGroup();
                newAppGroup.Name = appGroupViewModel.Name;
                try
                {
                    var appGroup = _appGroupService.Add(newAppGroup);
                    _appGroupService.Save();
                    //save group
                    var listRoleGroup = new List<ApplicationRoleGroup>();
                    foreach (var role in appGroupViewModel.Roles)
                    {
                        listRoleGroup.Add(new ApplicationRoleGroup()
                        {
                            GroupId = appGroup.ID,
                            RoleId = role.Id
                        });
                    }
                    _appRoleService.AddRolesToGroup(listRoleGroup, appGroup.ID);
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
                //return View(ModelState);
            }
        }
        public ActionResult Update(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            ApplicationGroup appGroup = _appGroupService.GetDetail(id);
            var appGroupViewModel = Mapper.Map<ApplicationGroup, ApplicationGroupViewModel>(appGroup);
            if (appGroup == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Không có Group");
            }
            var listRole = _appRoleService.GetListRoleByGroupId(appGroupViewModel.ID);
            appGroupViewModel.Roles = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(listRole);
            return View(appGroupViewModel);
        }
        [HttpPost]
        public async Task<ActionResult> Update(ApplicationGroupViewModel appGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                var appGroupVM = _appGroupService.GetDetail(appGroupViewModel.ID);
                try
                {
                    var appgroup = new ApplicationGroup();

                    appgroup.ID = appGroupVM.ID;
                    appgroup.Name = appGroupVM.Name;
                    _appGroupService.Update(appgroup);
                    //_appGroupService.Save();

                    //save group
                    var listRoleGroup = new List<ApplicationRoleGroup>();
                    foreach (var role in appGroupViewModel.Roles)
                    {
                        listRoleGroup.Add(new ApplicationRoleGroup()
                        {
                            GroupId = appgroup.ID,
                            RoleId = role.Id
                        });
                    }
                    _appRoleService.AddRolesToGroup(listRoleGroup, appgroup.ID);
                    _appRoleService.Save();

                    //add role to user
                    var listRole = _appRoleService.GetListRoleByGroupId(appgroup.ID);
                    var listUserInGroup = _appGroupService.GetListUserByGroupId(appgroup.ID);
                    foreach (var user in listUserInGroup)
                    {
                        var listRoleName = listRole.Select(x => x.Name).ToArray();
                        foreach (var roleName in listRoleName)
                        {
                            await _userManager.RemoveFromRoleAsync(user.Id, roleName);
                            await _userManager.AddToRoleAsync(user.Id, roleName);
                        }
                    }
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

        public ActionResult Delete(int id)
        {
            _appGroupService.Delete(id);
            _appGroupService.Save();
            return RedirectToAction("Index");
        }

       
        [HttpDelete]
        public ActionResult DeleteMulti( string checkedList)
        {

            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }
            else
            {
                var listItem = new JavaScriptSerializer().Deserialize<List<int>>(checkedList);
                foreach (var item in listItem)
                {
                    _appGroupService.Delete(item);
                }

                _appGroupService.Save();

                return View(listItem.Count);
            }
            
        }
    }
}