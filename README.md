﻿# EMASmsLib
翼信云通信短信模块封装。

> 官网：[http://c.yixin.im/](http://c.yixin.im/) <br />
> 管理中心：[https://naas.ecplive.cn/login.htm](https://naas.ecplive.cn/login.htm)

#### 引用

编译后引入 `EMASmsLib.dll`，通过 `Nuget` 管理器引用 `RestSharp 105.0.0.0`，即可使用。

#### 调用方法

1. 通过 `App.config` 或 `web.config` 配置参数。

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <add key="accountSid" value="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"/>
    <add key="ecpId" value="09xx12345678"/>
    <add key="appId" value="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"/>
    <add key="token" value="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"/>
  </appSettings>
</configuration>

```

> 注意：上边的参数通过管理中心获取。

C# 代码

```
var emaSms = EMASms.CreateWithAppSettings();
var result = emaSms.Send("186xxxxxxxx", "短信接口测试成功，现在时间是：" + DateTime.Now);
```

2. 通过静态方法直接传递参数。

```
var emaSms = EMASms.CreateWithParam(
    accountSid: "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    ecpId: "09xx12345678",
    appId: "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    token: "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
var result = emaSms2.Send("186xxxxxxxx", "短信接口测试成功，现在时间是：" + DateTime.Now);
```

---

如有任何疑问，发送电子邮件：`admin@mrhuo.com` 或者 `QQ：491217650` 联系。
