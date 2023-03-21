using AutoMapper;
using DSG.Data.Infrastructure;
using DSG.Model.Models;
using DSG.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSG.WebAPI.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Init()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductCategory, ProductCategoryViewModel>();
                cfg.CreateMap<Product, ProductViewModel>();
                cfg.CreateMap<AppRole, ApplicationRoleViewModel>();
                cfg.CreateMap<AppUser, ApplicationUserViewModel>();
                cfg.CreateMap<ApplicationGroup, ApplicationGroupViewModel>();

                //cfg.CreateMap<ProductCategoryViewModel, ProductCategory>();
                cfg.CreateMap<ProductViewModel, Product>();
                //cfg.CreateMap<ApplicationRoleViewModel, AppRole>();
                //cfg.CreateMap<ApplicationUserViewModel, AppUser>();
                //cfg.CreateMap<FunctionViewModel, Function>();
                //cfg.CreateMap<PermissionViewModel, Permission>();
            });
            //var mapper = new Mapper(config);
            //return mapper;

            mapper = config.CreateMapper();

        }

        public static IMapper mapper { get; private set; }
    }
}