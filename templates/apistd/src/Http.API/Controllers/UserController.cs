using Share.Models.AuthDtos;
using Share.Models.UserDtos;

namespace Http.API.Controllers;

/// <summary>
/// 用户账户
/// </summary>
/// <see cref="Application.Manager.UserManager"/>
public class UserController : ClientControllerBase<IUserManager>
{
    private readonly CacheService _cache;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public UserController(
        IUserContext user,
        ILogger<UserController> logger,
        IUserManager manager,
        CacheService cache,
        IEmailService emailService,
        IConfiguration config) : base(manager, user, logger)
    {
        _cache = cache;
        _emailService = emailService;
        _config = config;
    }

    /// <summary>
    /// 登录时，发送邮箱验证码
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost("verifyCode")]
    [AllowAnonymous]
    public async Task<ActionResult> SendVerifyCodeAsync(string email)
    {
        if (!manager.Query.Db.Any(q => q.Email != null && q.Email.Equals(email)))
        {
            return NotFound("不存在的邮箱账号");
        }
        var captcha = manager.GetCaptcha();
        var key = "VerifyCode:" + email;
        if (_cache.GetValue<string>(key) != null)
        {
            return Conflict("验证码已发送!");
        }
        // 缓存，默认5分钟过期
        await _cache.SetValueAsync(key, captcha, 60 * 5);
        // 自定义html内容
        var htmlContent = $"登录验证码:{captcha}";
        // 使用 smtp，可替换成其他
        await _emailService.SendLoginVerifyAsync(email, captcha);
        return Ok();
    }

    /// <summary>
    /// 登录获取Token
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResult>> LoginAsync(LoginDto dto)
    {
        // 查询用户
        var user = await manager.Query.Db.Where(u => u.UserName.Equals(dto.UserName))
            .FirstOrDefaultAsync();
        if (user == null)
        {
            return NotFound("不存在该用户");
        }

        // 可将 dto.VerifyCode 设置为必填，以强制验证
        if (dto.VerifyCode != null)
        {
            var key = "VerifyCode:" + user.Email;
            var cacheCode = _cache.GetValue<string>(key);
            if (cacheCode == null)
            {
                return NotFound("验证码已过期");
            }
            if (!cacheCode.Equals(dto.VerifyCode))
            {
                return NotFound("验证码错误");
            }
        }
        if (HashCrypto.Validate(dto.Password, user.PasswordSalt, user.PasswordHash))
        {
            // 获取Jwt配置
            var jwtOption = _config.GetSection("Authentication:Jwt").Get<JwtOption>()
                ?? throw new ArgumentNullException("未找到Jwt选项!");
            var sign = jwtOption.Sign;
            var issuer = jwtOption.ValidIssuer;
            var audience = jwtOption.ValidAudiences;

            // 构建返回内容
            if (!string.IsNullOrWhiteSpace(sign) &&
                !string.IsNullOrWhiteSpace(issuer) &&
                !string.IsNullOrWhiteSpace(audience))
            {
                // 设置角色或用户等级以区分权限
                var roles = new List<string> { AppConst.User };
                // 过期时间:minutes
                var expired = 60 * 24;
                JwtService jwt = new(sign, audience, issuer)
                {
                    TokenExpires = expired,
                };
                // 添加管理员用户标识
                if (!roles.Contains(AppConst.AdminUser))
                {
                    roles.Add(AppConst.AdminUser);
                }
                var token = jwt.GetToken(user.Id.ToString(), roles.ToArray());
                // 缓存登录状态
                await _cache.SetValueAsync("UserLogin" + user.Id.ToString(), true, expired * 60);

                return new AuthResult
                {
                    Id = user.Id,
                    Roles = roles.ToArray(),
                    Token = token,
                    Username = user.UserName
                };
            }
            else
            {
                throw new Exception("缺少Jwt配置内容");
            }
        }
        else
        {
            return Problem("用户名或密码错误", title: "");
        }
    }

    /// <summary>
    /// 退出 
    /// </summary>
    /// <returns></returns>
    [HttpPut("logout/{id}")]
    public async Task<ActionResult<bool>> LogoutAsync([FromRoute] Guid id)
    {
        if (await manager.ExistAsync(id))
        {
            // 清除缓存状态
            await _cache.RemoveAsync(id.ToString());
            return Ok();
        }
        return NotFound();
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<User>> AddAsync(UserAddDto dto)
    {
        var entity = await manager.CreateNewEntityAsync(dto);
        return await manager.AddAsync(entity);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<User?>> UpdateAsync([FromRoute] Guid id, UserUpdateDto dto)
    {
        var current = await manager.GetCurrentAsync(id);
        if (current == null) { return NotFound(ErrorMsg.NotFoundResource); };
        return await manager.UpdateAsync(current, dto);
    }

    /// <summary>
    /// 详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<User?>> GetDetailAsync([FromRoute] Guid id)
    {
        var res = await manager.FindAsync(id);
        return (res == null) ? NotFound() : res;
    }

    /// <summary>
    /// ⚠删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // [ApiExplorerSettings(IgnoreApi = true)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<User?>> DeleteAsync([FromRoute] Guid id)
    {
        // 注意删除权限
        var entity = await manager.GetCurrentAsync(id);
        if (entity == null) { return NotFound(); };
        // return Forbid();
        return await manager.DeleteAsync(entity);
    }
}