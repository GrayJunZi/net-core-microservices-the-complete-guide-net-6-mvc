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
- 创建产品服务项目 (Product API)


1.创建项目解决方案
```bash
dotnet new sln -n MangoRestaurant
```

2.创建Web项目
```bash
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
```bash
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

4.为 Product API 项目添加引用
```bash
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Swashbuckle.AspNetCore.SwaggerUI
dotnet add package Swashbuckle.AspNetCore.Annotations
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

5.为 Product API 迁移数据库
```bash
# 生成迁移文件
dotnet ef migrations add InitialDataBase -p Mango.Services.ProductAPI -s Mango.Services.ProductAPI

# 更新数据库
dotnet ef database update -p Mango.Services.ProductAPI -s Mango.Services.ProductAPI
```

### 第二部分

- ProductAPI 添加种子数据
- Web 项目中调用 ProductAPI 接口

1.为 Product API 迁移数据库
```bash
# 生成迁移文件
dotnet ef migrations add AddSeedData -p Mango.Services.ProductAPI -s Mango.Services.ProductAPI

# 更新数据库
dotnet ef database update -p Mango.Services.ProductAPI -s Mango.Services.ProductAPI
```

### 第三部分

- Product 增删改查操作

### 第四部分

- OAuth2
    1. Oauth2是一个开放协议，允许在web、移动和桌面应用程序中以简单和标准的方法进行安全授权。
    2. OAuth2处理授权。
    3. Oauth2使用access_token，客户端应用程序可以使用该token访问api。
    4. 不同应用类型的端点。

- OpenID Connect
    1. OpenID Connect 是 OAuth2协议之上的一个简单的身份层。
    2. 使用 OpenID Connect 应用程序可以在需要时在访问令牌旁边接收一个身份令牌。
    3. 它定义了不同类型的客户端应用程序如何安全地从安全令牌服务获取这些令牌。

- Identity Server
    1. Client - 客户端是一种从 IdentityServer 请求令牌的软件，用于对用户进行身份验证或访问资源。
    2. API Resource - API资源代表了客户端想要调用的功能，通常被建模为 Web APIs。
    3. Identity Resource - 关于用户的身份信息(aka claims), e.g. 电子邮件地址的名称。
    4. Identity Token - 标识令牌表示身份验证过程的结果。
    5. Access Token - 访问令牌允许访问API资源。


- 身份认证服务

1.创建 Identity 项目
```bash
# 进入 Services 文件夹
cd Services

# 创建 mvc 项目
dotnet new mvc -n Mango.Services.Identity

# 进入上级目录
cd ..

# 将项目添加到解决方案中
dotnet sln add .\Services\Mango.Services.Identity
```

2.为 Identity 项目添加引用
```bash
dotnet add package Duende.IdentityServer.AspNetIdentity
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

3.为 Identity 项目迁移数据库
```bash
# 生成迁移文件
dotnet ef migrations add InitialDataBase -p Mango.Services.Identity -s Mango.Services.Identity

# 更新数据库
dotnet ef database update -p Mango.Services.Identity -s Mango.Services.Identity
```

4.为 Web 项目添加引用
```bash
dotnet add package Microsoft.AspNetCore.Authentication
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
dotnet add package System.IdentityModel.Tokens.Jwt
```

### 第五部分

- 首页与详情页

### 第六部分

- 创建购物车项目 (Shopping Cart API)

1.创建 ShoppingCartAPI 项目
```bash
# 进入 Services 文件夹
cd Services

# 创建 webapi 项目
dotnet new webapi -n Mango.Services.ShoppingCartAPI

# 进入上级目录
cd ..

# 将项目添加到解决方案中
dotnet sln add .\Services\Mango.Services.ShoppingCartAPI
```

2.为 ShoppingCartAPI 项目添加引用
```bash
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore.SwaggerUI
dotnet add package Swashbuckle.AspNetCore.Annotations
```

3.为 ShoppingCartAPI 项目迁移数据库
```bash
# 生成迁移文件
dotnet ef migrations add InitialDataBase -p Mango.Services.ShoppingCartAPI -s Mango.Services.ShoppingCartAPI

# 更新数据库
dotnet ef database update -p Mango.Services.ShoppingCartAPI -s Mango.Services.ShoppingCartAPI
```

### 第七部分

- Web项目添加购物车功能

### 第八部分

- Web项目完善购物车功能

### 第九部分

- 创建优惠券项目 (Coupon API)

1.创建 CouponAPI 项目
```bash
# 进入 Services 文件夹
cd Services

# 创建 webapi 项目
dotnet new webapi -n Mango.Services.CouponAPI

# 进入上级目录
cd ..

# 将项目添加到解决方案中
dotnet sln add .\Services\Mango.Services.CouponAPI
```

2.为 CouponAPI 项目添加引用
```bash
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore.SwaggerUI
dotnet add package Swashbuckle.AspNetCore.Annotations
```

3.为 CouponAPI 项目迁移数据库
```bash
# 生成迁移文件
dotnet ef migrations add InitialDataBase -p Mango.Services.CouponAPI -s Mango.Services.CouponAPI

# 更新数据库
dotnet ef database update -p Mango.Services.CouponAPI -s Mango.Services.CouponAPI
```