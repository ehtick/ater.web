> **注意** 本仓库内容已合并到 [Ater.dry](https://github.com/AterDev/ater.dry.cli).


# 说明

本仓库包含了Ater.Web 相关内容的源代码。主要包括：

- templates:dotnet 模板内容
- 基础包:templates\ApiStandard\src\Infrastructure
- analyzer:分析器项目

The web framework with best practices base on ASP.NET Core.

基于`ASP.NET Core`和`Entity Framework Core`的快速开发模板，提供一个规范化的项目目录及工程结构。

集成了`ater.droplet.cli`代码生成工具，帮助你生成基础代码，减少重复性的代码编写工作。

## 版本

|版本|.NET版本|支持
|-|-|-|
|6.x|.NET6|不再维护
|7.x|.NET7|仅修复Bug
|8.x|.NET8|当前版本

## 文档

请查阅[使用文档](https://docs.dusi.dev/)！

> [!TIP]
> 如有问题，可在GitHub上提交问题，或者加入QQ群:149272857.

## 安装

## 使用源代码安装

- 拉取源代码
- 执行`install.ps1`脚本安装。

## 使用Nuget安装

模板已经发布到[`nuget`](https://www.nuget.org/packages/ater.web.templates)上，请根据你的项目版本下载对应的模板。

```pwsh
dotnet new --install ater.web.templates::8.0.0
```

## 创建项目

```pwsh
dotnet new atapi  
```

or

```pwsh
dotnet new atapi -n <projectname>
```

## 数据库

模板默认使用`PostgreSQL`，如果您使用其他数据库，你需要进行的操作：

- 修改`appsettings.json`等配置文件中的**数据库连接字符串**
- 在`Application`项目中添加相应的数据库驱动包
- 在`Http.API`项目`Program.cs`中，修改数据库上下文的注入。

## 数据迁移

### 7.0之前

项目`src\Database\EntityFramework.Migrator`目录下，执行脚本`MigrationContext.ps1`。

```pwsh
cd src\Database\EntityFramework.Migrator
.\MigrationContext.ps1
```

### 7.0及之后

移除了`EntityFramework.Migrator`，迁移代码将直接生成在`Http.API`项目中。

可直接运行`scripts\EFMigrations.ps1`脚本生成迁移内容，程序在启动时会执行迁移。

```pwsh
cd scripts
.\EFMigrations.ps1
```

该脚本可跟一个参数，参数为迁移生成时的名称，如`.\EFMigrations.ps1  Init` .

## 运行项目

### 配置

在运行项目前，请先检查`appsettings.json`中的配置，以确保数据库可以正常连接.

### 运行后台项目

```pwsh
cd src\Http.API
dotnet watch run 
```

使用`admin/Hello.Net`初始管理账号登录。
