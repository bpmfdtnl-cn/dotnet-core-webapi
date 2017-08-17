/**
 * Created by w on 2017/7/12.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using W.WebApi.Helper;
using W.WebApi.Models;

namespace W.WebApi.Controllers
{
    public abstract class BaseController : Controller
    {
        public ApiContext _context;

        public BaseController(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取当前用户令牌
        /// </summary>
        /// <returns></returns>
        protected Token _getToken()
        {
            Token token = Token.get(Request);
            return token;
        }

        /// <summary>
        /// 获取当前用户Id
        /// </summary>
        protected string _getUId()
        {
            return _getToken().userId;
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        protected User _getUser()
        {
            //todo:通过_getUid获取用户
            return null;
        }

        /// <summary>
        /// 生成唯一编号
        /// </summary>
        /// <returns></returns>
        protected string _newId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}