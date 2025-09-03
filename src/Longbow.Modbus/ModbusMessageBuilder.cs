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
    /// <param name="unitId"></param>
    /// <param name="functionCode"></param>
    /// <param name="pduData"></param>
    /// <returns></returns>
    public byte[] Build(byte unitId, byte functionCode, ReadOnlySpan<byte> pduData)
    {
        // 1. 计算长度：单元标识符(1) + 功能码(1) + PDU数据长度(n)
        ushort length = (ushort)(2 + pduData.Length);

        // 2. 创建请求数据缓冲区：MBAP头(7) + 功能码(1) + PDU数据(n)
        var frame = new byte[7 + 1 + pduData.Length];

        // 3. 填充MBAP头
        // 事务标识符 (2字节)
        frame[0] = (byte)(_transactionId >> 8);   // 高位字节
        frame[1] = (byte)(_transactionId & 0xFF); // 低位字节

        // 协议标识符 (2字节), Modbus TCP固定为0
        frame[2] = 0x00;
        frame[3] = 0x00;

        // 长度字段 (2字节)
        frame[4] = (byte)(length >> 8);   // 长度高位字节
        frame[5] = (byte)(length & 0xFF); // 长度低位字节

        // 单元标识符 (1字节)
        frame[6] = unitId;

        // 4. 填充功能码和数据域 (PDU)
        frame[7] = functionCode;

        pduData.CopyTo(frame.AsSpan(8));

        // 5. 递增事务标识符以供下次使用
        if (_transactionId >= ushort.MaxValue)
        {
            _transactionId = 0;
        }
        else
        {
            _transactionId++;
        }

        return frame;
    }
}
