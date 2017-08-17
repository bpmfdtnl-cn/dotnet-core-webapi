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
        /// ��ȡ��ǰ�û�����
        /// </summary>
        /// <returns></returns>
        protected Token _getToken()
        {
            Token token = Token.get(Request);
            return token;
        }

        /// <summary>
        /// ��ȡ��ǰ�û�Id
        /// </summary>
        protected string _getUId()
        {
            return _getToken().userId;
        }

        /// <summary>
        /// ��ȡ��ǰ�û�
        /// </summary>
        /// <returns></returns>
        protected User _getUser()
        {
            //todo:ͨ��_getUid��ȡ�û�
            return null;
        }

        /// <summary>
        /// ����Ψһ���
        /// </summary>
        /// <returns></returns>
        protected string _newId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}