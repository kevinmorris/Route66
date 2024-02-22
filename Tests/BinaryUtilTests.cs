using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
// ReSharper disable UseUtf8StringLiteral
#pragma warning disable IDE0230

namespace Tests
{
    public class BinaryUtilTests
    {
        [Test]
        public void BufferAddressXY()
        {
            Assert.AreEqual((0, 0), BinaryUtil.AddressCoordinates(BinaryUtil.BufferAddress(new byte[] { 0x40, 0x40 })));
            Assert.AreEqual((0, 20), BinaryUtil.AddressCoordinates(BinaryUtil.BufferAddress(new byte[] { 0x40, 0xd4 })));
            Assert.AreEqual((1, 0), BinaryUtil.AddressCoordinates(BinaryUtil.BufferAddress(new byte[] { 0xc1, 0x50 })));
            Assert.AreEqual((23, 0), BinaryUtil.AddressCoordinates(BinaryUtil.BufferAddress(new byte[] { 0x5c, 0xf0 })));
            Assert.AreEqual((5, 31), BinaryUtil.AddressCoordinates(BinaryUtil.BufferAddress(new byte[] { 0xc6, 0x6f })));
            Assert.AreEqual((19, 56), BinaryUtil.AddressCoordinates(BinaryUtil.BufferAddress(new byte[] { 0xd8, 0xe8 })));
            Assert.AreEqual((23, 79), BinaryUtil.AddressCoordinates(BinaryUtil.BufferAddress(new byte[] { 0x5d, 0x7f })));
            Assert.AreEqual((1, 14), BinaryUtil.AddressCoordinates(BinaryUtil.BufferAddress(new byte[] { 0xc1, 0x5e }))); }

        [Test]
        public void AddressBuffer12Bit()
        {
            Assert.AreEqual(new byte[] { 0x5c, 0x70 }, BinaryUtil.AddressBuffer12Bit(BinaryUtil.CoordinateAddress((23, 0))));
            Assert.AreEqual(new byte[] { 0x41, 0x5e }, BinaryUtil.AddressBuffer12Bit(BinaryUtil.CoordinateAddress((1, 14))));
            Assert.AreEqual(new byte[] { 0x40, 0x40 }, BinaryUtil.AddressBuffer12Bit(BinaryUtil.CoordinateAddress((0, 0))));
            Assert.AreEqual(new byte[] { 0x40, 0x54 }, BinaryUtil.AddressBuffer12Bit(BinaryUtil.CoordinateAddress((0, 20))));
            Assert.AreEqual(new byte[] { 0x41, 0x50 }, BinaryUtil.AddressBuffer12Bit(BinaryUtil.CoordinateAddress((1, 0))));
            Assert.AreEqual(new byte[] { 0x46, 0x6f }, BinaryUtil.AddressBuffer12Bit(BinaryUtil.CoordinateAddress((5, 31))));
            Assert.AreEqual(new byte[] { 0x58, 0x68 }, BinaryUtil.AddressBuffer12Bit(BinaryUtil.CoordinateAddress((19, 56))));
            Assert.AreEqual(new byte[] { 0x5d, 0x7f }, BinaryUtil.AddressBuffer12Bit(BinaryUtil.CoordinateAddress((23, 79))));
        }
    }
}
