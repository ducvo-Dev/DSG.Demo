using AutoMapper;
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
using System.Web.Http;

namespace DSG.WebAPI.API
{
    [RoutePrefix("api/productcategory")]
    public class ProductCategoryController : ApiControllerBase
    {
        #region Initialize

        private IProductCategoryService _productCategoryService;
        private readonly IMapper mapper;
        public ProductCategoryController(IErrorService errorService, IProductCategoryService productCategoryService)
            : base(errorService)
        {
            this._productCategoryService = productCategoryService;
            mapper = AutoMapperConfiguration.mapper;
        }
        #endregion Initialize

        [Route("getall")]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            //var mapper = AutoMapperConfiguration.Configure();
            return CreateHttpResponse(request, () =>
            {
                var model = _productCategoryService.GetAll();

                var responseData = mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }
    }
}
