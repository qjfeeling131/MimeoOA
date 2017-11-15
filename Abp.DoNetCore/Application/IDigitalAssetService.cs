using Abp.DoNetCore.Application.Dtos;
using Abp.DoNetCore.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DoNetCore.Application
{
    public interface IDigitalAssetService
    {
        Task<RESTResult> GetDigitalAssets(UserDataTransferObject currentUser, int index, int size);
        Task<RESTResult> UpdateDigitalAssets(Guid currentUserId, DigitalAssetItemDataObject digitalAssetItem);
    }
}
