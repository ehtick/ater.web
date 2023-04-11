namespace Application.Implement;

public static class StoreServicesExtensions
{
    public static void AddDataStore(this IServiceCollection services)
    {
        services.AddScoped(typeof(DataStoreContext));
        services.AddScoped(typeof(BlogQueryStore));
        services.AddScoped(typeof(CatalogQueryStore));
        services.AddScoped(typeof(RolePermissionQueryStore));
        services.AddScoped(typeof(SystemConfigQueryStore));
        services.AddScoped(typeof(SystemLogsQueryStore));
        services.AddScoped(typeof(SystemMenuQueryStore));
        services.AddScoped(typeof(SystemOrganizationQueryStore));
        services.AddScoped(typeof(SystemPermissionQueryStore));
        services.AddScoped(typeof(SystemRoleQueryStore));
        services.AddScoped(typeof(SystemUserQueryStore));
        services.AddScoped(typeof(TagsQueryStore));
        services.AddScoped(typeof(UserQueryStore));
        services.AddScoped(typeof(BlogCommandStore));
        services.AddScoped(typeof(CatalogCommandStore));
        services.AddScoped(typeof(RolePermissionCommandStore));
        services.AddScoped(typeof(SystemConfigCommandStore));
        services.AddScoped(typeof(SystemLogsCommandStore));
        services.AddScoped(typeof(SystemMenuCommandStore));
        services.AddScoped(typeof(SystemOrganizationCommandStore));
        services.AddScoped(typeof(SystemPermissionCommandStore));
        services.AddScoped(typeof(SystemRoleCommandStore));
        services.AddScoped(typeof(SystemUserCommandStore));
        services.AddScoped(typeof(TagsCommandStore));
        services.AddScoped(typeof(UserCommandStore));

    }

    public static void AddManager(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IUserContext, UserContext>();
        services.AddScoped<IBlogManager, BlogManager>();
        services.AddScoped<ICatalogManager, CatalogManager>();
        services.AddScoped<ISystemConfigManager, SystemConfigManager>();
        services.AddScoped<ISystemLogsManager, SystemLogsManager>();
        services.AddScoped<ISystemMenuManager, SystemMenuManager>();
        services.AddScoped<ISystemOrganizationManager, SystemOrganizationManager>();
        services.AddScoped<ISystemPermissionManager, SystemPermissionManager>();
        services.AddScoped<ISystemRoleManager, SystemRoleManager>();
        services.AddScoped<ISystemUserManager, SystemUserManager>();
        services.AddScoped<ITagsManager, TagsManager>();
        services.AddScoped<IUserManager, UserManager>();

    }
}
