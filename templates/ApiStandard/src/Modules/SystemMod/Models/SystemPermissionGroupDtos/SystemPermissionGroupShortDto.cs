namespace SystemMod.Models.SystemPermissionGroupDtos;

/// <see cref="Entity.SystemMod.SystemPermissionGroup"/>
public class SystemPermissionGroupShortDto
{
    /// <summary>
    /// 权限名称标识
    /// </summary>
    [MaxLength(30)]
    public string Name { get; set; } = default!;
    /// <summary>
    /// 权限说明
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }

}
