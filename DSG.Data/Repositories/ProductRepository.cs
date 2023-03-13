using DSG.Data.Infrastructure;
using DSG.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG.Data.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetListProductByCate(int IDcategory,int page, int pageSize, out int totalRow);
    }

    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<Product> GetListProductByCate(int IDcategory, int page, int pageSize, out int totalRow)
        {
            var query = from p in DbContext.Products
                        join ct in DbContext.ProductCategories
                        on p.CategoryID equals ct.ParentID
                        where ct.ParentID == IDcategory
                        select p;
            totalRow = query.Count();

            return query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
