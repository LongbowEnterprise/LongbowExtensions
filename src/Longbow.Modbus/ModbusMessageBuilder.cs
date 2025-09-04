// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.Modbus;

/// <summary>
/// Modbus TCP 消息构建器
/// </summary>
public class ModbusTcpMessageBuilder
{
    // 事务标识符计数器
    private ushort _transactionId = 0;

    /// <summary>
    /// 构建 Modbus TCP 读取消息方法
    /// </summary>
    /// <param name="slaveAddress"></param>
    /// <param name="functionCode"></param>
    /// <param name="startAddress"></param>
    /// <param name="numberOfPoints"></param>
    /// <returns></returns>
    public ReadOnlyMemory<byte> BuildReadRequest(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints)
    {
        byte[] request =
        [
            // MBAP头（7字节）
            (byte)(_transactionId >> 8),   // 00 事务标识符高字节（可随机）
            (byte)(_transactionId & 0xFF), // 01 事务标识符低字节
            0x00,                          // 02 协议标识符高字节（Modbus固定0）
            0x00,                          // 03 协议标识符低字节
            0x00,                          // 04 长度高字节（后续字节数）
            0x06,                          // 05 长度低字节（6字节PDU）
            // PDU部分
            slaveAddress,                  // 06 从站地址
            functionCode,                  // 07 功能码
            (byte)(startAddress >> 8),     // 08 起始地址高字节
            (byte)(startAddress & 0xFF),   // 09 起始地址低字节
            (byte)(numberOfPoints >> 8),   // 10 寄存器数量高字节
            (byte)(numberOfPoints & 0xFF), // 11 寄存器数量低字节
        ];

        // 递增事务标识符以供下次使用
        if (_transactionId >= ushort.MaxValue)
        {
            _transactionId = 0;
        }
        else
        {
            _transactionId++;
        }

        return request;
    }


    /// <summary>
    /// 构建 Modbus TCP 写入消息方法
    /// </summary>
    /// <param name="slaveAddress"></param>
    /// <param name="functionCode"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public ReadOnlyMemory<byte> BuildWriteRequest(byte slaveAddress, byte functionCode, ReadOnlyMemory<byte> data)
    {
        var request = new byte[8 + data.Length];

        // MBAP头（7字节）
        request[0] = (byte)(_transactionId >> 8);   // 00 事务标识符高字节（可随机）
        request[1] = (byte)(_transactionId & 0xFF); // 01 事务标识符低字节
        request[2] = 0x00;                          // 02 协议标识符高字节（Modbus固定0）
        request[3] = 0x00;                          // 03 协议标识符低字节
        request[4] = 0x00;                          // 04 长度高字节（后续字节数）
        request[5] = (byte)(2 + data.Length);           // 05 长度低字节（6字节PDU）

        // PDU部分
        request[6] = slaveAddress;                  // 06 从站地址
        request[7] = functionCode;                  // 07 功能码

        // 写入数据部分
        data.CopyTo(request.AsMemory(8));

        // 递增事务标识符以供下次使用
        if (_transactionId == ushort.MaxValue)
        {
            _transactionId = 0;
        }
        else
        {
            _transactionId++;
        }

        return request;
    }
}
