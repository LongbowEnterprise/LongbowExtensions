// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.Modbus;

class DefaultModbusTcpClient(ModbusTcpClientOptions options) : IModbusClient
{
    /// <summary>
    /// Gets or sets the service provider used to resolve dependencies.
    /// </summary>
    [NotNull]
    public IServiceProvider? ServiceProvider { get; set; }

    public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        throw new NotImplementedException();
    }

    public Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        throw new NotImplementedException();
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
