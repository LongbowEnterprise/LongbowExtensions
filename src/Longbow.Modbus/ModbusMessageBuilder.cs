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
    /// 构建 Modbus TCP 请求消息方法
    /// </summary>
    /// <param name="slaveAddress"></param>
    /// <param name="functionCode"></param>
    /// <param name="startAddress"></param>
    /// <param name="numberOfPoints"></param>
    /// <returns></returns>
    public byte[] Build(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints)
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
}
