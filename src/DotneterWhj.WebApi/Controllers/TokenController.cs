using DotneterWhj.Core.AuthenticationCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotneterWhj.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TokenController : ControllerBase
    {
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]

        public async Task<IActionResult> GetJwtStr(string name, string pass)
        {
            // 将用户id和角色名，作为单独的自定义变量封装进 token 字符串中。
            TokenModelJwt tokenModel = new TokenModelJwt { Uid = 1, Role = "Admin" };
            var jwtStr = JWTHelper.IssueJwt(tokenModel);//登录，获取到一定规则的 Token 令牌
            var suc = true;
            return Ok(new
            {
                success = suc,
                token = jwtStr
            });
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> GetTest()
        {
            return Ok("test");
        }

        [Authorize(Policy = "System")]
        [HttpGet]
        [Route("test2")]
        public async Task<IActionResult> GetTest2()
        {
            return Ok("test2");
        }

    }
}
