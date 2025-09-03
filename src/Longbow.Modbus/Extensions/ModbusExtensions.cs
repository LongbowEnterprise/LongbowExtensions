﻿// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Versioning;

namespace Longbow.Modbus;

/// <summary>
/// IModbusFactory 扩展方法
/// </summary>
[UnsupportedOSPlatform("browser")]
public static class ModbusExtensions
{
    /// <summary>
    /// 增加 <see cref="IModbusFactory"/> 服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBootstrapBlazorModbusFactory(this IServiceCollection services)
    {
        // 添加 IModbusFactory 服务
        services.AddSingleton<IModbusFactory, DefaultModbusFactory>();

        return services;
    }
}
