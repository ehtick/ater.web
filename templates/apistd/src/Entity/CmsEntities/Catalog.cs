﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.CMSEntities;
/// <summary>
/// 目录
/// </summary>
[Index(nameof(Name))]
[Index(nameof(Level))]
[Module(Modules.CMS)]
public class Catalog : IEntityBase, ITreeNode<Catalog>
{
    /// <summary>
    /// 目录名称
    /// </summary>
    [MaxLength(50)]
    public required string Name { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    public short Level { get; set; }
    /// <summary>
    /// 子目录
    /// </summary>
    public List<Catalog> Children { get; set; } = new List<Catalog>();

    /// <summary>
    /// 父目录
    /// </summary>
    [ForeignKey(nameof(ParentId))]
    public Catalog? Parent { get; set; }
    public Guid? ParentId { get; set; }
    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    public required User User { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
    public bool IsDeleted { get; set; }
}