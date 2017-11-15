using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.DoNetCore.Application.Dtos;
using Abp.DoNetCore.Application.Dtos.Users;
using Abp.DoNetCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.DoNetCore.Application
{
    public class DigitalAssetService : DomainService, IDigitalAssetService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserDepartment> _userDepartmentReposiotry;
        private readonly IRepository<DigitalAssetItem> _digitalAssetRepository;
        public DigitalAssetService(IRepository<User> userRepository, IRepository<UserDepartment> userDepartmentReposiotry, IRepository<DigitalAssetItem> digitalAssetRepository)
        {
            _userRepository = userRepository;
            _digitalAssetRepository = digitalAssetRepository;
            _userDepartmentReposiotry = userDepartmentReposiotry;

        }

        [UnitOfWork(IsDisabled =true)]
        public async Task<RESTResult> GetDigitalAssets(UserDataTransferObject currentUser, int pageIndex, int pageSize)
        {
            RESTResult result = new RESTResult();
            IEnumerable<DigitalAssetItem> digitalAssetItems=null;
            if (currentUser.Roles.Where(item=>item.Level==Common.RoleLevelStatus.SupperAdmin).Count()>0)
            {
                digitalAssetItems = (await _digitalAssetRepository.GetAllListAsync()).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            var userDepartmentList = await _userDepartmentReposiotry.GetAllListAsync(o => o.UserId == currentUser.Id);
            if (userDepartmentList.Count > 0)
            {
                var users = await _userDepartmentReposiotry.GetAllListAsync(o => o.DepartmentId == userDepartmentList.First().DepartmentId);
                List<Guid> userIds = new List<Guid>();
                foreach (var user in users)
                {
                    userIds.Add(user.UserId);
                }

                digitalAssetItems =  _digitalAssetRepository.GetAllList(d => d.IsDeleted == false && d.Status == DigitalAssetItemStatus.Public && userIds.Contains(d.CreateByUserId)).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            
            result.Code = Common.RESTStatus.Success;
            result.Data = digitalAssetItems;

            return result;
        }

        public async Task<RESTResult> UpdateDigitalAssets(Guid currentUserId, DigitalAssetItemDataObject digitalAssetItem)
        {
            RESTResult result = new RESTResult();
            var originItem = await _digitalAssetRepository.GetAsync(digitalAssetItem.ItemId);
            if (originItem == null)
                throw new ArgumentException("the digitalAsset not exist!");

            originItem.Status = (DigitalAssetItemStatus)digitalAssetItem.ItemStatus;
            originItem.ModifyByUserId = currentUserId;
            originItem.ModifyTime = DateTime.Now;
            originItem.Description = digitalAssetItem.Description;
            await _digitalAssetRepository.UpdateAsync(originItem);

            result.Code = Common.RESTStatus.Success;
            return result;
        }

    }
}
