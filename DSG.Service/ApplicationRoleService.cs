using DSG.Common.Exceptions;
using DSG.Data.Infrastructure;
using DSG.Data.Repositories;
using DSG.Model.Models;
using System.Collections.Generic;
using System.Linq;



namespace DSG.Service
{
    public interface IApplicationRoleService
    {
        AppRole GetDetail(string id);

        IEnumerable<AppRole> GetAll(int page, int pageSize, out int totalRow, string filter);

        IEnumerable<AppRole> GetAll();

        AppRole Add(AppRole appRole);

        void Update(AppRole AppRole);

        void Delete(string id);

        //Add roles to a sepcify group
        bool AddRolesToGroup(IEnumerable<ApplicationRoleGroup> roleGroups, int groupId);

        //Get list role by group id
        IEnumerable<AppRole> GetListRoleByGroupId(int groupId);

        void Save();
    }

    public class ApplicationRoleService : IApplicationRoleService
    {
        private IApplicationRoleRepository _appRoleRepository;
        private IApplicationRoleGroupRepository _appRoleGroupRepository;
        private IUnitOfWork _unitOfWork;

        public ApplicationRoleService(IUnitOfWork unitOfWork,
            IApplicationRoleRepository appRoleRepository, IApplicationRoleGroupRepository appRoleGroupRepository)
        {
            this._appRoleRepository = appRoleRepository;
            this._appRoleGroupRepository = appRoleGroupRepository;
            this._unitOfWork = unitOfWork;
        }

        public AppRole Add(AppRole appRole)
        {
            if (_appRoleRepository.CheckContains(x => x.Description == appRole.Description))
                throw new NameDuplicatedException("Tên không được trùng");
            return _appRoleRepository.Add(appRole);
        }

        public bool AddRolesToGroup(IEnumerable<ApplicationRoleGroup> roleGroups, int groupId)
        {
            _appRoleGroupRepository.DeleteMulti(x => x.GroupId == groupId);
            foreach (var roleGroup in roleGroups)
            {
                _appRoleGroupRepository.Add(roleGroup);
            }
            return true;
        }

        public void Delete(string id)
        {
            _appRoleRepository.DeleteMulti(x => x.Id == id);
        }

        public IEnumerable<AppRole> GetAll()
        {
            return _appRoleRepository.GetAll();
        }

        public IEnumerable<AppRole> GetAll(int page, int pageSize, out int totalRow, string filter = null)
        {
            var query = _appRoleRepository.GetAll();
            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.Description.Contains(filter));

            totalRow = query.Count();
            return query.OrderBy(x => x.Description).Skip(page * pageSize).Take(pageSize);
        }

        public AppRole GetDetail(string id)
        {
            return _appRoleRepository.GetSingleByCondition(x => x.Id == id);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(AppRole Approle)
        {
            if (_appRoleRepository.CheckContains(x => x.Description == Approle.Description && x.Id != Approle.Id))
                throw new NameDuplicatedException("Tên không được trùng");
            _appRoleRepository.Update(Approle);
        }

        public IEnumerable<AppRole> GetListRoleByGroupId(int groupId)
        {
            return _appRoleRepository.GetListRoleByGroupId(groupId);
        }
    }
}

