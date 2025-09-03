// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace Longbow.Modbus;

/// <summary>
/// Represents a TCP socket for network communication.
/// </summary>
[UnsupportedOSPlatform("browser")]
class DefaultModbusFactory(IServiceProvider provider) : IModbusFactory
{
    private readonly ConcurrentDictionary<string, IModbusClient> _pool = new();

    public IModbusClient GetOrCreate(string name, Action<ModbusClientOptions> valueFactory)
    {
        return _pool.GetOrAdd(name, key =>
        {
            var options = new ModbusTcpClientOptions();
            valueFactory(options);
            var client = new DefaultModbusTcpClient(options)
            {
                ServiceProvider = provider,
            };
            return client;
        });
    }

    public IModbusClient? Remove(string name)
    {
        IModbusClient? client = null;
        if (_pool.TryRemove(name, out var c))
        {
            client = c;
        }
        return client;
    }
}
