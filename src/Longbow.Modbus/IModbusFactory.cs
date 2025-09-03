﻿// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.Modbus;

/// <summary>
/// ITcpSocketFactory Interface
/// </summary>
public interface IModbusFactory
{
    /// <summary>
    /// 获得/创建 <see cref="IModbusClient"/> 客户端实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="valueFactory"></param>
    /// <returns></returns>
    IModbusClient GetOrCreate(string name, Action<ModbusClientOptions> valueFactory);

    /// <summary>
    /// 移除指定名称的 <see cref="IModbusClient"/> 客户端实例
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IModbusClient? Remove(string name);
}
