using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPlatform.Web.Helpers;
using ReportPlatform.Web.Models;
using System.Security.Claims;

namespace ReportPlatform.Web.Controllers
{
    /// <summary>
    /// 使用者控制器
    /// </summary>
    /// <remarks>
    /// 建構子
    /// </remarks>
    /// <param name="logger"></param>
    /// <param name="jwtHelpers"></param>
    /// <param name="user"></param>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(ILogger<UserController> logger, JwtHelpers jwtHelpers, ClaimsPrincipal user) : ControllerBase
    {
        private readonly ILogger<UserController> logger = logger;
        private readonly JwtHelpers jwtHelpers = jwtHelpers;
        private readonly ClaimsPrincipal user = user;

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (ValidateUser(model))
                    {
                        var token = jwtHelpers.GenerateToken(model.Username);

                        // 紀錄登入人名稱與登入人IP
                        logger.LogInformation("{Username} is logged in from {IP}", new { model.Username }, new { Request.HttpContext.Connection.RemoteIpAddress });

                        return Ok(new { token });
                    }
                    else
                    {
                        // 紀錄因使用者名稱或密碼錯誤而登入失敗與登入人IP，並回傳401
                        logger.LogWarning("Login failed because of invalid username or password from {IP}", new { Request.HttpContext.Connection.RemoteIpAddress });
                        return Unauthorized();
                    }
                }
                else
                {
                    // 紀錄因使用者名稱或密碼結構錯誤而登入失敗與登入人IP，並回傳400
                    logger.LogWarning("Login failed because of invalid username or password from {IP}", new { Request.HttpContext.Connection.RemoteIpAddress });
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                // 紀錄因登入失敗而發生例外狀況，並回傳400
                logger.LogError(ex, "Error while logging in");
                return BadRequest();
            }
        }

        /// <summary>
        /// 取得 JWT Token 中的所有 Claims
        /// </summary>
        /// <returns></returns>
        [HttpGet("claims")]
        public IActionResult Claims()
        {
            return Ok(user.Claims.Select(p => new { p.Type, p.Value }));
        }

        /// <summary>
        /// 取得 JWT Token 中的使用者名稱
        /// </summary>
        /// <returns></returns>
        [HttpGet("username")]
        public IActionResult GetUsername()
        {
            return Ok(user.Identity?.Name);
        }

        /// <summary>
        /// 取得使用者是否擁有特定角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet("isInRole")]
        public IActionResult IsInRole(string role)
        {
            return Ok(User.IsInRole(role));
        }

        private static bool ValidateUser(LoginModel model)
        {
            string username = model.Username;


            if (!string.IsNullOrEmpty(username))
            {
                if (username != "admin")
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
