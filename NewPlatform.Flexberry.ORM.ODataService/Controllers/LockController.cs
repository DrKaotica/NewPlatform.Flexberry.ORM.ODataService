﻿namespace NewPlatform.Flexberry.ORM.ODataService.Controllers
{
    using ICSSoft.STORMNET.Business;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NewPlatform.Flexberry.Services;
    using System;
    using System.Diagnostics.Contracts;
    using System.Web.Http;

    //using NewPlatform.Flexberry.Services;

    /// <summary>
    /// WebAPI controller for Flexberry Lock Service (<see cref="ILockService"/>).
    /// </summary>
    /// <seealso cref="ApiController" />
    [Authorize]
    public class LockController : BaseApiController
    {
        private readonly ILockService _lockService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockController"/> class.
        /// </summary>
        /// <param name="lockService">The lock service.</param>
        public LockController(ILockService lockService)
        {
            Contract.Requires<ArgumentNullException>(lockService != null);

            _lockService = lockService;
        }

        /// <summary>
        /// Locks the specified data object by identifier.
        /// </summary>
        /// <param name="dataObjectId">The data object identifier.</param>
        /// <returns>Information about lock.</returns>
        [HttpGet]
        [ActionName("Lock")]
        public LockInfo Lock(string dataObjectId)
        {
            return _lockService.LockObject(dataObjectId, User.Identity.Name);
        }

        /// <summary>
        /// Unlocks the specified data object by identifier.
        /// </summary>
        /// <param name="dataObjectId">The data object identifier.</param>
        /// <returns>Returns <c>true</c> if object is successfully unlocked or <c>false</c> if it's not exist.</returns>
        [HttpGet]
        [ActionName("Unlock")]
        public IActionResult Unlock(string dataObjectId)
        {
            return _lockService.UnlockObject(dataObjectId)
                ? (IActionResult)Ok()
                : NotFound();
        }
    }
}
