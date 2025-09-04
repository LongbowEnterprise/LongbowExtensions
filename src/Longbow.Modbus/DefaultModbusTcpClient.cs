// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Longbow.TcpSocket;
using System.Net;

namespace Longbow.Modbus;

class DefaultModbusTcpClient(ITcpSocketClient client) : IModbusTcpClient
{
    private CancellationTokenSource? _receiveCancellationTokenSource;

    private readonly ModbusTcpMessageBuilder _builder = new();

    [NotNull]
    public IServiceProvider? ServiceProvider { get; set; }

    public Exception? Exception { get; private set; }

    public ValueTask<bool> ConnectAsync(IPEndPoint endPoint, CancellationToken token = default) => client.ConnectAsync(endPoint, token);

    public ValueTask<bool[]?> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints) => ReadAsync(slaveAddress, 0x01, startAddress, numberOfPoints, ReadBool);

    public ValueTask<bool[]?> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfInputs) => ReadAsync(slaveAddress, 0x02, startAddress, numberOfInputs, ReadBool);

    public ValueTask<ushort[]?> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints) => ReadAsync(slaveAddress, 0x03, startAddress, numberOfPoints, ReadUShort);

    public ValueTask<ushort[]?> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints) => ReadAsync(slaveAddress, 0x04, startAddress, numberOfPoints, ReadUShort);

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

    private async ValueTask<TResult?> ReadAsync<TResult>(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints, Func<ReadOnlyMemory<byte>, ushort, TResult> parser)
    {
        if (!client.IsConnected)
        {
            throw new InvalidOperationException("站点未连接请先使用 ConnectAsync 连接设备");
        }

        var data = _builder.Build(slaveAddress, functionCode, startAddress, numberOfPoints);
        var result = await client.SendAsync(data);
        if (!result)
        {
            return default;
        }

        _receiveCancellationTokenSource ??= new();
        var received = await client.ReceiveAsync(_receiveCancellationTokenSource.Token);
        var response = received.Span;

        if (!ValidateResponse(response, data.Span[..2], functionCode))
        {
            return default;
        }

        return parser(received, numberOfPoints);
    }

    private static bool[] ReadBool(ReadOnlyMemory<byte> response, ushort numberOfPoints)
    {
        var values = new bool[numberOfPoints];
        for (var i = 0; i < numberOfPoints; i++)
        {
            var byteIndex = 9 + i / 8;
            var bitIndex = i % 8;
            values[i] = (response.Span[byteIndex] & (1 << bitIndex)) != 0;
        }

        return values;
    }

    private static ushort[] ReadUShort(ReadOnlyMemory<byte> response, ushort numberOfPoints)
    {
        var values = new ushort[numberOfPoints];
        for (var i = 0; i < numberOfPoints; i++)
        {
            int offset = 9 + (i * 2);
            values[i] = (ushort)((response.Span[offset] << 8) | response.Span[offset + 1]);
        }

        return values;
    }

    private bool ValidateResponse(ReadOnlySpan<byte> response, ReadOnlySpan<byte> transactionId, byte functionCode)
    {
        // 解析电文
        // 检查响应长度
        if (response.Length < 9)
        {
            Exception = new Exception("Response length is insufficient 响应长度不足");
            return false;
        }

        // 检查事务标识符是否匹配
        if (response[0] != transactionId[0] || response[1] != transactionId[1])
        {
            Exception = new Exception("Transaction identifier mismatch 事务标识符不匹配");
            return false;
        }

        // 检查功能码 (正常响应应与请求相同，异常响应 = 请求功能码 + 0x80)
        if (response[7] == 0x80 + functionCode)
        {
            Exception = new Exception($"Modbus abnormal response, error code: {response[8]}. 异常响应，错误码: {response[8]}");
            return false;
        }
        else if (response[7] != functionCode)
        {
            Exception = new Exception($"Function code does not match 功能码不匹配期望值 0x{functionCode:X2} 实际值 0x{response[7]:X2}");
            return false;
        }

        // 获取数据字节数
        var byteCount = response[8];
        if (byteCount + 9 != response.Length)
        {
            Exception = new Exception($"Response length does not match byte count 响应长度与字节计数不匹配 期望值 {byteCount + 9} 实际值 {response.Length}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask CloseAsync()
    {
        await CloseCoreAsync();
    }

    private async ValueTask CloseCoreAsync()
    {
        // 取消接收数据的任务
        if (_receiveCancellationTokenSource != null)
        {
            _receiveCancellationTokenSource.Cancel();
            _receiveCancellationTokenSource.Dispose();
            _receiveCancellationTokenSource = null;
        }

        if (client.IsConnected)
        {
            await client.CloseAsync();
        }
    }

    /// <summary>
    /// Releases the resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method is called to free both managed and unmanaged resources. If the <paramref
    /// name="disposing"/> parameter is <see langword="true"/>, the method releases managed resources in addition to
    /// unmanaged resources. Override this method in a derived class to provide custom cleanup logic.</remarks>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only
    /// unmanaged resources.</param>
    private async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            await CloseAsync();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
