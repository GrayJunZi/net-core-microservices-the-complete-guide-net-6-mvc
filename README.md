# Going

---

## .NET Core Microservices - The Complete Guide

### 介绍

- 为什么选择微服务？(Why Microservices)
    1. 微服务可独立部署。
    2. 微服务可独立伸缩扩展的。
    3. 微服务通过故障隔离(fault isolution)减少停机时间。
    4. 更小的代码库使团队更容易理解代码，从而更容易维护。

- 概述（Overview）
    1. .NET API
    2. Identity Server
    3. Service Bus
    4. Microservice Communication
    5. Gateway
    6. 最佳实践

- 微服务架构（Microservices architecture）
    1. 微服务架构是一种方法，在这种方法中，单个应用程序由许多松散耦合且可独立部署的较小服务组成。
    2. 它们有自己的技术栈，包括数据库和数据管理模型。
    3. 它们通过REST API、事件流和消息代理的组合互相通信。
    4. 按业务能力组织，使用分隔服务的行(通常称为有界上下文)。

### 第一部分

- 创建项目
- 创建 Product API


1.创建项目解决方案
```
dotnet new sln -n MangoRestaurant
```

2.创建Web项目
```
# 创建 FrontEnd 文件夹
md FrontEnd

# 进入 FrontEnd 文件夹
cd FrontEnd

# 创建 mvc 项目
dotnet new mvc -n Mango.Web

# 进入上级目录
cd ..

# 将项目添加到解决方案中
dotnet sln add .\FrontEnd\Mango.Web
```

3.创建 Product API 项目
```
# 创建 Services 文件夹
md Services

# 进入 Services 文件夹
cd Services

# 创建 webapi 项目
dotnet new webapi -n Mango.Services.ProductAPI

# 进入上级目录
cd ..

# 将项目添加到解决方案中
dotnet sln add .\Services\Mango.Services.ProductAPI
```

4. 为 Product API 项目添加引用
```
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Swashbuckle.AspNetCore.SwaggerUI
dotnet add package Swashbuckle.AspNetCore.Annotations
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

5. 为 Product API 迁移数据库
```
# 生成迁移文件
dotnet ef migrations add InitialDataBase -p Mango.Services.ProductAPI -s Mango.Services.ProductAPI

# 更新数据库
dotnet ef database update -p Mango.Services.ProductAPI -s Mango.Services.ProductAPI
```

### 第二部分

- ProductAPI 添加种子数据
- Web 项目中调用 ProductAPI 接口

1. 为 Product API 迁移数据库
```
# 生成迁移文件
dotnet ef migrations add AddSeedData -p Mango.Services.ProductAPI -s Mango.Services.ProductAPI

# 更新数据库
dotnet ef database update -p Mango.Services.ProductAPI -s Mango.Services.ProductAPI
```

### 第三部分

- Product 增删改查操作