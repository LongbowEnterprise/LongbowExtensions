// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.DependencyInjection;

namespace UnitTestModbus;

public class TcpClientTest
{
    [Fact]
    public async Task ReadCoilsAsync_Ok()
    {
        var sc = new ServiceCollection();
        sc.AddTcpSocketFactory();
        sc.AddModbusFactory();

        var provider = sc.BuildServiceProvider();
        var factory = provider.GetRequiredService<IModbusFactory>();
        var client = factory.GetOrCreate("test", op =>
        {
            op.ReadTimeout = 3000;
            op.WriteTimeout = 3000;
        });

        // 连接 Master
        await client.ConnectAsync("127.0.0.1", 502);

        // 读取 0x01 从站线圈数据
        var response = await client.ReadCoilsAsync(0x01, 0, 5);
    }
}
