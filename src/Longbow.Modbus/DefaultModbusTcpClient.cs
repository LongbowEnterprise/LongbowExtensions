// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Longbow.TcpSocket;
using System.Net;

namespace Longbow.Modbus;

class DefaultModbusTcpClient(ITcpSocketClient client) : IModbusClient
{
    private readonly ModbusTcpMessageBuilder _builder = new();

    [NotNull]
    public IServiceProvider? ServiceProvider { get; set; }

    public ValueTask<bool> ConnectAsync(IPEndPoint endPoint, CancellationToken token = default) => client.ConnectAsync(endPoint, token);

    public async ValueTask<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        if (!client.IsConnected)
        {
            throw new InvalidOperationException("站点未连接请先使用 ConnectAsync 连接设备");
        }

        var data = _builder.Build(slaveAddress, 0x01, startAddress, numberOfPoints);
        await client.SendAsync(data);

        var received = await client.ReceiveAsync();
        var response = received.Span;

        // 解析电文
        // 检查响应长度
        if (response.Length < 9)
        {
            throw new Exception("响应长度不足");
        }

        // 检查事务标识符是否匹配
        if (response[0] != data[0] || response[1] != data[1])
        {
            throw new Exception("事务标识符不匹配");
        }

        // 检查功能码 (正常响应应与请求相同，异常响应 = 请求功能码 + 0x80)
        if (response[7] == 0x83)
        {
            throw new Exception($"Modbus 异常响应，错误码: {response[8]}");
        }
        else if (response[7] != 0x01)
        {
            throw new Exception("功能码不匹配");
        }

        // 获取数据字节数
        var byteCount = response[8];
        if (byteCount + 9 != response.Length)
        {
            throw new Exception("响应长度与字节计数不匹配");
        }

        // 解析线圈状态
        var coils = new bool[numberOfPoints];
        for (var i = 0; i < numberOfPoints; i++)
        {
            var byteIndex = 9 + i / 8;
            var bitIndex = i % 8;
            coils[i] = (response[byteIndex] & (1 << bitIndex)) != 0;
        }

        return coils;
    }

    public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        throw new NotImplementedException();
    }

    public Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        throw new NotImplementedException();
    }

    public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        throw new NotImplementedException();
    }

    public Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        throw new NotImplementedException();
    }

    public bool[] ReadInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        throw new NotImplementedException();
    }

    public Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        throw new NotImplementedException();
    }

    public ushort[] ReadWriteMultipleRegisters(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
    {
        throw new NotImplementedException();
    }

    public Task<ushort[]> ReadWriteMultipleRegistersAsync(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
    {
        throw new NotImplementedException();
    }

    public void WriteFileRecord(byte slaveAdress, ushort fileNumber, ushort startingAddress, byte[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data)
    {
        throw new NotImplementedException();
    }

    public Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteMultipleRegisters(byte slaveAddress, ushort startAddress, ushort[] data)
    {
        throw new NotImplementedException();
    }

    public Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteSingleCoil(byte slaveAddress, ushort coilAddress, bool value)
    {
        throw new NotImplementedException();
    }

    public Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value)
    {
        throw new NotImplementedException();
    }

    public void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value)
    {
        throw new NotImplementedException();
    }

    public Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value)
    {
        throw new NotImplementedException();
    }
}
