﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Services;
using Moq.Protected;
using Moq.Sequences;
using Tests.Helpers;

namespace Tests
{
    public class TelnetServiceTests
    {
        private TelnetService<byte[]> _service;

        [SetUp]
        public void SetUp()
        {
            _service = new TelnetService<byte[]>(new ByteArray3270Translator());
        }

        [Test]
        public void Handshake()
        {
            var willTerminalType = new byte[] { 0xff, 0xfb, 0x18 };
            var doTerminalType = new byte[] { 0xff, 0xfd, 0x18 };
            var sendTerminalType = new byte[] { 0xff, 0xfa, 0x18, 0x01, 0xff, 0xf0 };
            var heresTerminalType = new byte[] { 0xff, 0xfa, 0x18, 0x00, 0x49, 0x42, 0x4d, 0x2d, 0x33, 0x32, 0x37, 0x39, 0x2d, 0x32, 0x2d, 0x45, 0xff, 0xf0 };
            var willBinaryTransmission = new byte[] { 0xff, 0xfb, 0x00 };
            var doBinaryTransmission = new byte[] { 0xff, 0xfd, 0x00 };
            var willEndOfRecord = new byte[] { 0xff, 0xfb, 0x19 };
            var doEndOfRecord = new byte[] { 0xff, 0xfd, 0x19 };

            using (Sequence.Create())
            {
                Assert.AreEqual(willTerminalType, _service.ProcessOutbound(doTerminalType));
                Assert.AreEqual(heresTerminalType, _service.ProcessOutbound(sendTerminalType));
                Assert.AreEqual(doBinaryTransmission, _service.ProcessOutbound(willBinaryTransmission));
                Assert.AreEqual(willBinaryTransmission, _service.ProcessOutbound(doBinaryTransmission));
                Assert.AreEqual(doEndOfRecord, _service.ProcessOutbound(willEndOfRecord));
                Assert.AreEqual(willEndOfRecord, _service.ProcessOutbound(doEndOfRecord));
            }
        }

        [Test]
        public void ScreenDisplay()
        {
            var bytes = new byte[]{ 0xf5, 0x42, 0x11, 0x40, 0x40, 0x1d, 0x60, 0xc8, 0x85, 0x99, 0x83, 0xa4, 0x93, 0x85, 0xa2, 0x40, 0xe5, 0x85, 0x99, 0xa2, 0x89, 0x96, 0x95, 0x40, 0x40, 0x7a, 0x11, 0x40, 0xd4, 0x1d, 0xe8, 0xf4, 0x4b, 0xf6, 0x4b, 0xf0, 0x4b, 0xf1, 0xf0, 0xf9, 0xf4, 0xf1, 0x60, 0xe2, 0xc4, 0xd3, 0x60, 0x87, 0xf6, 0xf5, 0x83, 0xf9, 0xf7, 0x86, 0x84, 0xf6, 0x11, 0xc1, 0x50, 0x1d, 0x60, 0xc8, 0x96, 0xa2, 0xa3, 0x40, 0x95, 0x81, 0x94, 0x85, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x7a, 0x11, 0xc1, 0xe4, 0x1d, 0xe8, 0xc7, 0xc1, 0xe4, 0xc7, 0xc1, 0xd4, 0xc5, 0xd3, 0xc1, 0x11, 0xc2, 0x60, 0x1d, 0x60, 0xc8, 0x96, 0xa2, 0xa3, 0x40, 0xd6, 0xe2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x7a, 0x11, 0xc2, 0xf4, 0x1d, 0xe8, 0xe6, 0x89, 0x95, 0x84, 0x96, 0xa6, 0xa2, 0x60, 0xf6, 0x4b, 0xf2, 0x4b, 0xf9, 0xf2, 0xf0, 0xf0, 0x40, 0x40, 0xd7, 0x99, 0x96, 0x86, 0x85, 0xa2, 0xa2, 0x89, 0x96, 0x95, 0x81, 0x93, 0x40, 0xf6, 0xf4, 0x60, 0x82, 0x89, 0xa3, 0x11, 0xc3, 0xf0, 0x1d, 0x60, 0xc8, 0x96, 0xa2, 0xa3, 0x40, 0xc1, 0x99, 0x83, 0x88, 0x89, 0xa3, 0x85, 0x83, 0xa3, 0xa4, 0x99, 0x85, 0x40, 0x7a, 0x11, 0xc4, 0xc4, 0x1d, 0xe8, 0xc9, 0x95, 0xa3, 0x85, 0x93, 0x4d, 0xd9, 0x5d, 0x40, 0xa7, 0xf6, 0xf4, 0x11, 0xc5, 0x40, 0x1d, 0x60, 0xd7, 0x99, 0x96, 0x83, 0x85, 0xa2, 0xa2, 0x96, 0x99, 0xa2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x7a, 0x11, 0xc5, 0xd4, 0x1d, 0xe8, 0xd3, 0xd7, 0x7e, 0xf1, 0xf2, 0x6b, 0x40, 0xc3, 0x96, 0x99, 0x85, 0xa2, 0x7e, 0xf6, 0x6b, 0x40, 0xc3, 0xd7, 0xe4, 0xa2, 0x7e, 0xf1, 0x11, 0xc6, 0x50, 0x1d, 0x60, 0xd3, 0xd7, 0xc1, 0xd9, 0x40, 0xd5, 0x81, 0x94, 0x85, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x7a, 0x11, 0xc6, 0xe4, 0x1d, 0xe8, 0xc8, 0xc5, 0xd9, 0xc3, 0xe4, 0xd3, 0xc5, 0xe2, 0x11, 0xc7, 0x60, 0x1d, 0x60, 0xc4, 0x85, 0xa5, 0x89, 0x83, 0x85, 0x40, 0x95, 0xa4, 0x94, 0x82, 0x85, 0x99, 0x40, 0x40, 0x40, 0x40, 0x40, 0x7a, 0x11, 0xc7, 0xf4, 0x1d, 0xe8, 0xf0, 0x7a, 0xf0, 0xf0, 0xc3, 0xf0, 0x11, 0xc8, 0xf0, 0x1d, 0x60, 0xe2, 0xa4, 0x82, 0x83, 0x88, 0x81, 0x95, 0x95, 0x85, 0x93, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x7a, 0x11, 0xc9, 0xc4, 0x1d, 0xe8, 0xf0, 0xf0, 0xf1, 0xc1, 0x11, 0xc8, 0xf0, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0xd2, 0xd2, 0x40, 0x40, 0xd2, 0xd2, 0xd2, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0x11, 0x4a, 0x40, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0x11, 0x4b, 0x50, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0x11, 0x4c, 0x60, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0x11, 0x4d, 0xf0, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf4, 0x93, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x6d, 0x6b, 0x6b, 0x6b, 0x60, 0x60, 0x60, 0x6b, 0x6b, 0x6d, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0x4f, 0x40, 0x1d, 0x60, 0x40, 0xe9, 0xe9, 0xe9, 0xa9, 0xa9, 0x40, 0x61, 0x6b, 0x7d, 0x4b, 0x60, 0x7d, 0x79, 0x7d, 0x40, 0x40, 0x40, 0x40, 0x60, 0x4b, 0x40, 0x40, 0x5e, 0x60, 0x5e, 0x5e, 0x6b, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0x40, 0x40, 0x40, 0x40, 0x11, 0x50, 0x50, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf4, 0x6b, 0xf4, 0x60, 0x40, 0x40, 0x5d, 0x40, 0x5d, 0x60, 0x6b, 0x6d, 0x4b, 0x40, 0x6b, 0x4d, 0x40, 0x4d, 0x40, 0x40, 0x7d, 0x7d, 0x60, 0x7d, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0xd2, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd1, 0x60, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x7d, 0x60, 0x60, 0x60, 0x7d, 0x7d, 0x4d, 0x6d, 0x61, 0x60, 0x60, 0x7d, 0x40, 0x40, 0x79, 0x60, 0x7d, 0x5d, 0x6d, 0x5d, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0x11, 0xd2, 0xf0, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0x11, 0xd4, 0x40, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0x88, 0x85, 0x40, 0xd4, 0xe5, 0xe2, 0x40, 0xf3, 0x4b, 0xf8, 0x91, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0x11, 0xd5, 0x50, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xa4, 0x99, 0x4d, 0x95, 0x5d, 0x92, 0x85, 0xa8, 0x40, 0xe2, 0xa8, 0xa2, 0xa3, 0x85, 0x94, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0x11, 0xd6, 0x60, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0x40, 0xd2, 0xd2, 0xd2, 0x40, 0x40, 0x40, 0x40, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0xf5, 0x11, 0xd7, 0xf0, 0x1d, 0x60, 0x11, 0xd9, 0x40, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xd2, 0xf3, 0x40, 0x83, 0x99, 0x85, 0x81, 0xa3, 0x85, 0x84, 0x40, 0x82, 0xa8, 0x40, 0x40, 0x40, 0xe5, 0x96, 0x93, 0x92, 0x85, 0x99, 0x40, 0xc2, 0x81, 0x95, 0x84, 0x92, 0x85, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xa5, 0x96, 0x93, 0x92, 0x85, 0x99, 0x7c, 0x82, 0x81, 0x95, 0x84, 0x92, 0x85, 0x4b, 0x96, 0x99, 0x87, 0x11, 0x5a, 0x50, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xd2, 0xf4, 0x60, 0x40, 0xa4, 0x97, 0x84, 0x81, 0xa3, 0x85, 0x40, 0x82, 0xa8, 0x40, 0x40, 0x40, 0xd1, 0xa4, 0x85, 0x99, 0x87, 0x85, 0x95, 0x40, 0xe6, 0x89, 0x95, 0x92, 0x85, 0x93, 0x94, 0x81, 0x95, 0x95, 0x40, 0x40, 0x91, 0xa4, 0x85, 0x99, 0x87, 0x85, 0x95, 0x4b, 0xa6, 0x89, 0x95, 0x92, 0x85, 0x93, 0x94, 0x81, 0x95, 0x95, 0x7c, 0x97, 0x85, 0x82, 0x82, 0x93, 0x85, 0x60, 0x82, 0x85, 0x81, 0x83, 0x88, 0x4b, 0x83, 0x88, 0x11, 0x5b, 0x60, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0xe3, 0xd2, 0xf5, 0x40, 0x40, 0xa4, 0x97, 0x84, 0x81, 0xa3, 0x85, 0x40, 0x82, 0xa8, 0x40, 0x40, 0x40, 0xd9, 0x96, 0x82, 0x40, 0xd7, 0x99, 0x89, 0x95, 0xa2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x97, 0x99, 0x89, 0x95, 0xf0, 0xf0, 0xf9, 0xf6, 0x7c, 0x87, 0x94, 0x81, 0x89, 0x93, 0x4b, 0x83, 0x96, 0x94, 0x11, 0x5c, 0xf0, 0x1d, 0x60, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xa2, 0x85, 0x85, 0x40, 0xe2, 0xe8, 0xe2, 0xf2, 0x4b, 0xd1, 0xc3, 0xd3, 0xd3, 0xc9, 0xc2, 0x4d, 0xc3, 0xd9, 0xc5, 0xc4, 0xc9, 0xe3, 0xe2, 0x5d, 0x40, 0x86, 0x96, 0x99, 0x40, 0x83, 0x96, 0x94, 0x97, 0x93, 0x85, 0xa3, 0x85, 0x40, 0x83, 0x99, 0x85, 0x84, 0x89, 0xa3, 0xa2, 0xff, 0xef };
        }
    }
}
