using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Abp.DoNetCore;
using Abp.DoNetCore.Common;
using Abp.DoNetCore.Application;
using Abp.DoNetCore.Application.Dtos;

namespace MimeoOAWeb.Controllers
{
    [Route("api/digitalAssets")]
    [Authorize(Policy = MimeoOAPolicyType.PolicyName)]
    public class LibraryController : BaseController
    {
        private readonly IUserAppService _userAppService;
        private readonly IDigitalAssetService _digitalAssetService;

        public LibraryController(IUserAppService userAppService, IDigitalAssetService digitalAssetService)
        {
            _userAppService = userAppService;
            _digitalAssetService = digitalAssetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDigitalAssets(int index, int size)
        {
            return Ok(await _digitalAssetService.GetDigitalAssets(CurrentUser, index, size));
        }

        [HttpPut("digitalAsset")]
        public async Task<IActionResult> GetDigitalAssets([FromBody] DigitalAssetItemDataObject digitalAssetItem)
        {
            return Ok(await _digitalAssetService.UpdateDigitalAssets(CurrentUser.Id.Value, digitalAssetItem));
        }

    }
}