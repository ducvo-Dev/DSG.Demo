using DSG.Common.Exceptions;
using DSG.Model.Models;
using DSG.Service;
using DSG.WebAPI.Infrastructure;
using DSG.WebAPI.Mappings;
using DSG.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;
using AutoMapper;

namespace DSG.WebAPI.Controllers
{
    [Authorize]
    public class ApplicationUserController : Controller
    {
        private ApplicationUserManager _userManager;
        private IApplicationGroupService _appGroupService;
        private IApplicationRoleService _appRoleService;
        private readonly IMapper _mapper;
        public ApplicationUserController(
            IApplicationGroupService appGroupService,
            IApplicationRoleService appRoleService,
            ApplicationUserManager userManager
            )
        {
            _appRoleService = appRoleService;
            _appGroupService = appGroupService;
            _userManager = userManager;
            _mapper = Mappings.AutoMapperConfiguration.mapper;
        }

        [HttpGet]
        [Authorize(Roles ="ViewUser")]
        public ActionResult Index()
        {
            var model = _userManager.Users;
            var modelVm = _mapper.Map<IEnumerable<AppUser>, IEnumerable<ApplicationUserViewModel>>(model);
            return View(modelVm);
        }

        [HttpGet]
        [Authorize(Roles = "ViewUser")]
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            var user = _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Không có dữ liệu");
            }
            else
            {
                var applicationUserViewModel = _mapper.Map<AppUser, ApplicationUserViewModel>(user.Result);
                var listGroup = _appGroupService.GetListGroupByUserId(applicationUserViewModel.Id);
                applicationUserViewModel.Groups = _mapper.Map<IEnumerable<ApplicationGroup>, IEnumerable<ApplicationGroupViewModel>>(listGroup);
                return View(applicationUserViewModel);
            }

        }
        
        [Authorize(Roles = "AddUser")]
        public ActionResult Create()
        {
            var listGroup = _appGroupService.GetAll();
            ApplicationUserViewModel applicationGroupViewModel = new ApplicationUserViewModel();
            applicationGroupViewModel.Groups = _mapper.Map<IEnumerable<ApplicationGroup>, IEnumerable<ApplicationGroupViewModel>>(listGroup);

            return View(applicationGroupViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AddUser")]
        public async Task<ActionResult> Create(ApplicationUserViewModel applicationUserViewModel)
        {

            if (ModelState.IsValid)
            {
                var newAppUser = new AppUser();
                newAppUser.Id = applicationUserViewModel.Id;
                newAppUser.FullName = applicationUserViewModel.FullName;
                newAppUser.BirthDay = applicationUserViewModel.BirthDay;
                newAppUser.Gender = applicationUserViewModel.Gender;
                newAppUser.Email = applicationUserViewModel.Email;
                newAppUser.UserName = applicationUserViewModel.UserName;
                newAppUser.PhoneNumber = applicationUserViewModel.PhoneNumber;
                newAppUser.EmailConfirmed = true;
                try
                {
                    newAppUser.Id = Guid.NewGuid().ToString();
                    var result = await _userManager.CreateAsync(newAppUser, applicationUserViewModel.Password);
                    if (result.Succeeded)
                    {
                        var listAppUserGroup = new List<ApplicationUserGroup>();
                        foreach (var group in applicationUserViewModel.Groups)
                        {
                            listAppUserGroup.Add(new ApplicationUserGroup()
                            {
                                GroupId = group.ID,
                                UserId = newAppUser.Id
                            });
                            //add role to user
                            var listRole = _appRoleService.GetListRoleByGroupId(group.ID);
                            foreach (var role in listRole)
                            {
                                await _userManager.RemoveFromRoleAsync(newAppUser.Id, role.Name);
                                await _userManager.AddToRoleAsync(newAppUser.Id, role.Name);
                            }
                        }
                        _appGroupService.AddUserToGroups(listAppUserGroup, newAppUser.Id);
                        _appGroupService.Save();

                        return View(applicationUserViewModel);
                    }
                    else
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, string.Join(",", result.Errors));//request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(",", result.Errors));
                }
                catch (NameDuplicatedException dex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, dex.Message);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Authorize(Roles = "UpdateUser")]
        public ActionResult Update(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            var user = _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Không có dữ liệu");
            }
            else
            {
                var applicationUserViewModel = _mapper.Map<AppUser, ApplicationUserViewModel>(user.Result);
                var listGroup = _appGroupService.GetListGroupByUserId(applicationUserViewModel.Id);
                applicationUserViewModel.Groups = _mapper.Map<IEnumerable<ApplicationGroup>, IEnumerable<ApplicationGroupViewModel>>(listGroup);
                return View(applicationUserViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "UpdateUser")]
        public async Task<ActionResult> Update(ApplicationUserViewModel applicationUserViewModel,string chkValue)
        {

            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByIdAsync(applicationUserViewModel.Id);
                try
                {
                    //var appUser = new AppUser();
                    //appUser.Id = applicationUserViewModel.Id;
                    //appUser.FullName = applicationUserViewModel.FullName;
                    //appUser.BirthDay = applicationUserViewModel.BirthDay;
                    //appUser.Email = applicationUserViewModel.Email;
                    //appUser.UserName = applicationUserViewModel.UserName;
                    //appUser.PhoneNumber = applicationUserViewModel.PhoneNumber;
                    appUser.UpdateUser(applicationUserViewModel);
                    var result = await _userManager.UpdateAsync(appUser);
                    if (result.Succeeded)
                    {
                        var listAppUserGroup = new List<ApplicationUserGroup>();
                        foreach (var group in applicationUserViewModel.Groups)
                        {
                            listAppUserGroup.Add(new ApplicationUserGroup()
                            {
                                GroupId = group.ID,
                                UserId = applicationUserViewModel.Id
                            });
                            //add role to user
                            var listRole = _appRoleService.GetListRoleByGroupId(group.ID);
                            foreach (var role in listRole)
                            {
                                await _userManager.RemoveFromRoleAsync(appUser.Id, role.Name);
                                await _userManager.AddToRoleAsync(appUser.Id, role.Name);
                            }
                        }
                        _appGroupService.AddUserToGroups(listAppUserGroup, applicationUserViewModel.Id);
                        _appGroupService.Save();
                        return RedirectToAction("Index");
                    }
                    else
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, string.Join(",", result.Errors));
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

        [Authorize(Roles ="DeleteUser")]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Không có dữ liệu");
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK, string.Join(",", result.Errors));
            }
        }

        //[HttpGet]
        //public JsonResult showlistGroup()
        //{
        //    try
        //    {
        //        var list = _appGroupService.GetAll();
        //        return Json(new { code = 200, L = list, msg = "Lây thông tin thành công" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(new { code = 500, msg = "Lấy thông tin thất bại. lỗi" + ex.Message }, JsonRequestBehavior.AllowGet);
        //    }

        //}
    }
}
