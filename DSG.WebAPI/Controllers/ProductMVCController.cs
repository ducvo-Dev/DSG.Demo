using AutoMapper;
using DSG.Common;
using DSG.Model.Models;
using DSG.Service;
using DSG.WebAPI.Infrastructure;
using DSG.WebAPI.Mappings;
using DSG.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DSG.WebAPI.Controllers
{
    [Authorize]
    public class ProductMVCController : Controller
    {
        private IProductService _productService;
        private IProductCategoryService _productCategoryService;
        private readonly IMapper Mapper;
        public ProductMVCController(IProductService productService, IProductCategoryService productCategoryService)
        {
            this._productService = productService;
            this._productCategoryService = productCategoryService;
            Mapper = AutoMapperConfiguration.mapper;
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create( ProductViewModel productVm)
        {
                if (!ModelState.IsValid)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"Không thành công" );
                }
                else
                {
                    productVm.CreatedDate = DateTime.Now;
                    productVm.UpdatedDate = DateTime.Now;
                    productVm.CreatedBy = User.Identity.Name;
                    var responseData = Mapper.Map<ProductViewModel, Product>(productVm);
                    _productService.Add(responseData);
                    _productService.Save();

                return RedirectToAction("Index");
                }          
        }
        public ActionResult Detail(int productId)
        {
          
            var productModel = _productService.GetById(productId);
            var viewModel = Mapper.Map<Product, ProductViewModel>(productModel);

            var relatedProduct = _productService.GetReatedProducts(productId, 6);
            ViewBag.RelatedProducts = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(relatedProduct);
            return View(viewModel);
        }

       
        public ActionResult Search(string keyword, int page = 1, string sort = "")
        {
            int pageSize = int.Parse(ConfigHelper.GetByKey("PageSize"));
            int totalRow = 0;
            var productModel = _productService.Search(keyword, page, pageSize, sort, out totalRow);
            var productViewModel = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            ViewBag.Keyword = keyword;
            var paginationSet = new PaginationSet<ProductViewModel>()
            {
                Items = productViewModel,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            return View(paginationSet);
        }

        public JsonResult GetListProductByName(string keyword)
        {
            var model = _productService.GetListProductByName(keyword);
            return Json(new
            {
                data = model
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            
            var productModel = _productService.GetAll();
            var viewModel = Mapper.Map < IEnumerable<Product>, IEnumerable< ProductViewModel >> (productModel);
            return View(viewModel);
        }
        public ActionResult Status(int id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            var result = _productService.GetById(id);
            if (result == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Không có dữ liệu");
            }
            result.Status = (result.Status == true) ? false : true;
            result.UpdatedDate = DateTime.Now;
            result.UpdatedBy = User.Identity.Name;
            _productService.Update(result);
            _productService.Save();

            var responseData = Mapper.Map<Product, ProductViewModel>(result);
            return RedirectToAction("Index","ProductMVC", responseData);
        }
        public ActionResult Delete(int id)
        {
            _productService.Delete(id);
            _productService.Save();
            return RedirectToAction("Index");
        }

    }
}