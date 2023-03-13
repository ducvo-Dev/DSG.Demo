

namespace DSG.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private DsgDbContext dbContext;

        public DsgDbContext Init()
        {
            return dbContext ?? (dbContext = new DsgDbContext()); // if null create new dsgcontext
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
