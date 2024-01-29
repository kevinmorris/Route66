﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Services;
using Moq.Sequences;
using Tests.Helpers;

namespace Tests
{
    public class TelnetServiceTests
    {
        private TelnetService<(byte[], IDictionary<int, IDictionary<byte, byte>>)> _service;
        private readonly byte[] _data = new byte[]
        {
            0x11, 0xc3, 0xe4, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xc8, 0xc5, 0xd9, 0xc3,
            0xf0, 0xf1, 0x40, 0x40, 0x11, 0xc3, 0x6f, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x11, 0xc3, 0xf0, 0x1d, 0xf8,
            0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xf0, 0x40, 0x11, 0xc3, 0xf6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xc9,
            0xe2, 0xd7, 0xc6, 0x40, 0xd7, 0xc1, 0xd9, 0xd4, 0xe2, 0x40, 0x40, 0x11, 0xc4, 0xc3, 0x1d, 0xf0, 0x28,
            0x42, 0xf4, 0x40, 0xe2, 0x97, 0x85, 0x83, 0x89, 0x86, 0xa8, 0x40, 0xa3, 0x85, 0x99, 0x94, 0x89, 0x95,
            0x81, 0x93, 0x40, 0x81, 0x95, 0x84, 0x40, 0xa4, 0xa2, 0x85, 0x99, 0x40, 0x97, 0x81, 0x99, 0x94, 0xa2,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xc4, 0x6b, 0x1d, 0xf0, 0xe3, 0xc9, 0xd4, 0xc5, 0x40,
            0x40, 0x40, 0x40, 0x11, 0xc4, 0xf4, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xf0, 0xf0, 0x7a, 0xf2,
            0xf4, 0x40, 0x40, 0x40, 0x11, 0xc4, 0x7f, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x11, 0xc5, 0x40, 0x1d, 0xf8,
            0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xf1, 0x40, 0x11, 0xc5, 0xc6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xc2,
            0xd9, 0xd6, 0xe6, 0xe2, 0xc5, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xc5, 0xd3, 0x1d, 0xf0, 0x28,
            0x42, 0xf4, 0x40, 0xc4, 0x89, 0xa2, 0x97, 0x93, 0x81, 0xa8, 0x40, 0xa2, 0x96, 0xa4, 0x99, 0x83, 0x85,
            0x40, 0x84, 0x81, 0xa3, 0x81, 0x40, 0xa4, 0xa2, 0x89, 0x95, 0x87, 0x40, 0xd9, 0x85, 0xa5, 0x89, 0x85,
            0xa6, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xc5, 0x7b, 0x1d, 0xf0, 0xe3, 0xc5, 0xd9, 0xd4, 0xc9,
            0xd5, 0xc1, 0xd3, 0x11, 0xc6, 0xc4, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xf3, 0xf2, 0xf7, 0xf7,
            0x40, 0x40, 0x40, 0x40, 0x11, 0xc6, 0x4f, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x11, 0xc6, 0x50, 0x1d, 0xf8,
            0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xf2, 0x40, 0x11, 0xc6, 0xd6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xc5,
            0xc4, 0xc9, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xc6, 0xe3, 0x1d, 0xf0, 0x28,
            0x42, 0xf4, 0x40, 0xc3, 0x88, 0x81, 0x95, 0x87, 0x85, 0x40, 0xa2, 0x96, 0xa4, 0x99, 0x83, 0x85, 0x40,
            0x84, 0x81, 0xa3, 0x81, 0x40, 0xa4, 0xa2, 0x89, 0x95, 0x87, 0x40, 0xd9, 0x85, 0xa5, 0x85, 0x84, 0x89,
            0xa3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xc7, 0x4b, 0x1d, 0xf0, 0xd7, 0xc6, 0x40, 0xd2, 0xc5,
            0xe8, 0xe2, 0x40, 0x11, 0xc7, 0xd4, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xf0, 0xf0, 0xf1, 0xf2,
            0x40, 0x40, 0x40, 0x40, 0x11, 0xc7, 0x5f, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x11, 0xc7, 0x60, 0x1d, 0xf8,
            0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xd9, 0x40, 0x11, 0xc7, 0xe6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xd9,
            0xd7, 0xc6, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xc7, 0xf3, 0x1d, 0xf0, 0x28,
            0x42, 0xf4, 0x40, 0xc2, 0x99, 0x96, 0xa6, 0xa2, 0x85, 0x6b, 0x40, 0xc5, 0xc4, 0xc9, 0xe3, 0x6b, 0x40,
            0xd9, 0x85, 0xa2, 0x85, 0xa3, 0x6b, 0x40, 0xd7, 0xc4, 0xe2, 0x40, 0xa6, 0x89, 0xa3, 0x88, 0x40, 0xd9,
            0xd7, 0xc6, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xc8, 0x5b, 0x1d, 0xf0, 0xd7, 0xc1, 0xd5, 0xc5, 0xd3,
            0x40, 0x40, 0x40, 0x11, 0xc8, 0xe4, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xc9, 0xe2, 0xd7, 0x7c,
            0xd7, 0xd9, 0xc9, 0xd4, 0x40, 0x11, 0xc8, 0xf0, 0x1d, 0xf8, 0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xf3,
            0x40, 0x11, 0xc8, 0xf6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xe4, 0xe3, 0xc9, 0xd3, 0xc9, 0xe3, 0xc9, 0xc5,
            0xe2, 0x40, 0x40, 0x40, 0x11, 0xc9, 0xc3, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x40, 0xd7, 0x85, 0x99, 0x86,
            0x96, 0x99, 0x94, 0x40, 0xa4, 0xa3, 0x89, 0x93, 0x89, 0xa3, 0xa8, 0x40, 0x86, 0xa4, 0x95, 0x83, 0xa3,
            0x89, 0x96, 0x95, 0xa2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x11, 0xc9, 0x6b, 0x1d, 0xf0, 0xe2, 0xc3, 0xd9, 0xc5, 0xc5, 0xd5, 0x40, 0x40, 0x11, 0xc9, 0xf4, 0x1d,
            0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xf1, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xc9, 0x7f,
            0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x11, 0x4a, 0x40, 0x1d, 0xf8, 0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xf4,
            0x40, 0x11, 0x4a, 0xc6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xc6, 0xd6, 0xd9, 0xc5, 0xc7, 0xd9, 0xd6, 0xe4,
            0xd5, 0xc4, 0x40, 0x40, 0x11, 0x4a, 0xd3, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x40, 0xc9, 0x95, 0xa5, 0x96,
            0x92, 0x85, 0x40, 0x93, 0x81, 0x95, 0x87, 0xa4, 0x81, 0x87, 0x85, 0x40, 0x97, 0x99, 0x96, 0x83, 0x85,
            0xa2, 0xa2, 0x96, 0x99, 0xa2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x11, 0x4a, 0x7b, 0x1d, 0xf0, 0xd3, 0xc1, 0xd5, 0xc7, 0xe4, 0xc1, 0xc7, 0xc5, 0x11, 0x4b, 0xc4, 0x1d,
            0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xc5, 0xd5, 0xc7, 0xd3, 0xc9, 0xe2, 0xc8, 0x40, 0x11, 0x4b, 0x4f,
            0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x11, 0x4b, 0x50, 0x1d, 0xf8, 0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xf5,
            0x40, 0x11, 0x4b, 0xd6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xc2, 0xc1, 0xe3, 0xc3, 0xc8, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x11, 0x4b, 0xe3, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x40, 0xe2, 0xa4, 0x82, 0x94,
            0x89, 0xa3, 0x40, 0x91, 0x96, 0x82, 0x40, 0x86, 0x96, 0x99, 0x40, 0x93, 0x81, 0x95, 0x87, 0xa4, 0x81,
            0x87, 0x85, 0x40, 0x97, 0x99, 0x96, 0x83, 0x85, 0xa2, 0xa2, 0x89, 0x95, 0x87, 0x40, 0x40, 0x40, 0x40,
            0x11, 0x4c, 0x4b, 0x1d, 0xf0, 0xc1, 0xd7, 0xd7, 0xd3, 0x40, 0xc9, 0xc4, 0x40, 0x11, 0x4c, 0xd4, 0x1d,
            0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xc9, 0xe2, 0xd7, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0x4c, 0x5f,
            0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x11, 0x4c, 0x60, 0x1d, 0xf8, 0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xf6,
            0x40, 0x11, 0x4c, 0xe6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xc3, 0xd6, 0xd4, 0xd4, 0xc1, 0xd5, 0xc4, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x11, 0x4c, 0xf3, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x40, 0xc5, 0x95, 0xa3, 0x85,
            0x99, 0x40, 0xe3, 0xe2, 0xd6, 0x40, 0x83, 0x96, 0x94, 0x94, 0x81, 0x95, 0x84, 0x40, 0x96, 0x99, 0x40,
            0xc3, 0xd3, 0xc9, 0xe2, 0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x11, 0x4d, 0x5b, 0x1d, 0xf0, 0xd9, 0xc5, 0xd3, 0xc5, 0xc1, 0xe2, 0xc5, 0x40, 0x11, 0x4d, 0xe4, 0x1d,
            0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xe5, 0xf2, 0x4b, 0xf2, 0x4b, 0xf0, 0xf0, 0xf0, 0x40, 0x11, 0x4d,
            0xf0, 0x1d, 0xf8, 0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xf7, 0x40, 0x11, 0x4d, 0xf6, 0x1d, 0xf0, 0x28,
            0x42, 0xf5, 0xc4, 0xc9, 0xc1, 0xd3, 0xd6, 0xc7, 0x40, 0xe3, 0xc5, 0xe2, 0xe3, 0x40, 0x11, 0x4e, 0xc3,
            0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x40, 0xd7, 0x85, 0x99, 0x86, 0x96, 0x99, 0x94, 0x40, 0x84, 0x89, 0x81,
            0x93, 0x96, 0x87, 0x40, 0xa3, 0x85, 0xa2, 0xa3, 0x89, 0x95, 0x87, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0x4e, 0x6b, 0x1d, 0xf0, 0xd7, 0xd9,
            0xc9, 0xd4, 0x40, 0x40, 0x40, 0x40, 0x11, 0x4e, 0xf4, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xe8,
            0xc5, 0xe2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0x4e, 0x7f, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x11, 0x4f,
            0x40, 0x1d, 0xf8, 0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xc3, 0x40, 0x11, 0x4f, 0xc6, 0x1d, 0xf0, 0x28,
            0x42, 0xf5, 0xc3, 0xc8, 0xc1, 0xd5, 0xc7, 0xc5, 0xe2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0x4f, 0xd3,
            0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x40, 0xe2, 0xa4, 0x94, 0x94, 0x81, 0x99, 0xa8, 0x40, 0x96, 0x86, 0x40,
            0x83, 0x88, 0x81, 0x95, 0x87, 0x85, 0xa2, 0x40, 0x86, 0x96, 0x99, 0x40, 0xa3, 0x88, 0x89, 0xa2, 0x40,
            0x99, 0x85, 0x93, 0x85, 0x81, 0xa2, 0x85, 0x40, 0x40, 0x40, 0x11, 0x4f, 0x7b, 0x1d, 0xf0, 0xe2, 0xe8,
            0xe2, 0xe3, 0xc5, 0xd4, 0xc9, 0xc4, 0x11, 0x50, 0xc4, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xe3,
            0xd2, 0xf5, 0xd9, 0x40, 0x40, 0x40, 0x40, 0x11, 0x50, 0x4f, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x11, 0x50,
            0x50, 0x1d, 0xf8, 0x28, 0x42, 0xf7, 0x40, 0x40, 0x40, 0xd4, 0x40, 0x11, 0x50, 0xd6, 0x1d, 0xf0, 0x28,
            0x42, 0xf5, 0xe3, 0xe2, 0xd6, 0xc1, 0xd7, 0xd7, 0xd3, 0xe2, 0x40, 0x40, 0x40, 0x40, 0x11, 0x50, 0xe3,
            0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x40, 0xd7, 0x99, 0x96, 0x84, 0xa4, 0x83, 0xa3, 0x89, 0xa5, 0x89, 0xa3,
            0xa8, 0x40, 0xa3, 0x96, 0x96, 0x93, 0xa2, 0x40, 0x81, 0x95, 0x84, 0x40, 0x88, 0x81, 0x95, 0x84, 0xa8,
            0x40, 0x81, 0x97, 0x97, 0xa2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd1, 0x4b, 0x1d, 0xf0, 0xd7, 0xd9,
            0xd6, 0xc3, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd1, 0xd4, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0x7a, 0x40, 0xc9,
            0xe2, 0xd7, 0xd3, 0xd6, 0xc7, 0xd6, 0xd5, 0x40, 0x11, 0xd1, 0x60, 0x1d, 0xf8, 0x28, 0x42, 0xf7, 0x40,
            0x40, 0x40, 0xe3, 0x40, 0x11, 0xd1, 0xe6, 0x1d, 0xf0, 0x28, 0x42, 0xf6, 0xe3, 0xe4, 0xe3, 0xd6, 0xd9,
            0xc9, 0xc1, 0xd3, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd1, 0xf3, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x40, 0xc4,
            0x89, 0xa2, 0x97, 0x93, 0x81, 0xa8, 0x40, 0x89, 0x95, 0x86, 0x96, 0x99, 0x94, 0x81, 0xa3, 0x89, 0x96,
            0x95, 0x40, 0x81, 0x82, 0x96, 0xa4, 0xa3, 0x40, 0xc9, 0xe2, 0xd7, 0xc6, 0x11, 0xd2, 0xd3, 0x1d, 0xf0,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd2, 0xf0, 0x1d, 0xf8, 0x28,
            0x42, 0xf7, 0x40, 0x40, 0x40, 0xe3, 0xf1, 0x11, 0xd2, 0xf6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xe3, 0xd2,
            0xf5, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd3, 0xc3, 0x1d, 0xf0, 0x28, 0x42,
            0xf4, 0x40, 0xe2, 0xa4, 0x94, 0x94, 0x81, 0x99, 0xa8, 0x40, 0x96, 0x86, 0x40, 0x83, 0x88, 0x81, 0x95,
            0x87, 0x85, 0xa2, 0x40, 0x94, 0x81, 0x84, 0x85, 0x40, 0x89, 0x95, 0x40, 0xe3, 0xd2, 0xf5, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd4, 0x40, 0x1d, 0xf8, 0x28, 0x42,
            0xf7, 0x40, 0x40, 0x40, 0xe7, 0x40, 0x11, 0xd4, 0xc6, 0x1d, 0xf0, 0x28, 0x42, 0xf5, 0xc5, 0xe7, 0xc9,
            0xe3, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd4, 0xd3, 0x1d, 0xf0, 0x28, 0x42, 0xf4,
            0x40, 0xe3, 0x85, 0x99, 0x94, 0x89, 0x95, 0x81, 0xa3, 0x85, 0x40, 0xc9, 0xe2, 0xd7, 0xc6, 0x40, 0xa4,
            0xa2, 0x89, 0x95, 0x87, 0x40, 0x93, 0x96, 0x87, 0x40, 0x81, 0x95, 0x84, 0x40, 0x93, 0x89, 0xa2, 0xa3,
            0x40, 0x84, 0x85, 0x86, 0x81, 0xa4, 0x93, 0xa3, 0xa2, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd5, 0x50, 0x1d, 0xf8, 0x28, 0x42, 0xf7,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd6, 0x60, 0x1d, 0xf0, 0x28,
            0x42, 0xf4, 0xc5, 0x95, 0xa3, 0x85, 0x99, 0x11, 0xd6, 0xe6, 0x1d, 0xf8, 0x28, 0x42, 0xf7, 0xc5, 0xd5,
            0xc4, 0x11, 0xd6, 0x6a, 0x1d, 0xf0, 0x28, 0x42, 0xf4, 0x83, 0x96, 0x94, 0x94, 0x81, 0x95, 0x84, 0x40,
            0xa3, 0x96, 0x40, 0xa3, 0x85, 0x99, 0x94, 0x89, 0x95, 0x81, 0xa3, 0x85, 0x40, 0xc9, 0xe2, 0xd7, 0xc6,
            0x4b, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xd7, 0xf0, 0x1d, 0xf8, 0x28, 0x42,
            0xf7, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x11, 0xc1, 0x5e, 0x13, 0xff,
            0xef
        };

        [SetUp]
        public void SetUp()
        {
            _service = new TelnetService<(byte[], IDictionary<int, IDictionary<byte, byte>>)>(new ByteArray3270Translator());
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
        public void StartField()
        {
            var handler = _service.Handlers[0];
            var eventInvoked = false;
            handler.RowUpdated += (sender, args) =>
            {
                eventInvoked = true;
            };

            var expected = new byte[80];
            Array.Fill(expected, (byte)0x0);
            expected[0] = 0b11110000;
            expected[1] = 0x7a;
            expected[2] = 0x40;
            expected[3] = 0xc8;
            expected[4] = 0xc5;
            expected[5] = 0xd9;
            expected[6] = 0xc3;
            expected[7] = 0xf0;
            expected[8] = 0xf1;
            expected[9] = 0x40;
            expected[10] = 0x40;

            Assert.AreEqual((18, 11), _service.OrderStartField(_data, 3, 0));
            handler.Update();

            Assert.True(eventInvoked);
            Assert.AreEqual(
                expected,
                handler.Buffer);
        }

        [Test]
        public void ModifyField()
        {
            var data = new byte[]
            {
                Orders.MODIFY_FIELD, 0xf3,
                Attributes.FOREGROUND_COLOR, Colors.BLUE,
                Attributes.FIELD, 0b01000100,
                Attributes.OUTLINE, 0b00000100
            };

            RowUpdateEventArgs<(byte[], IDictionary<int, IDictionary<byte, byte>>)> eventArgs = null;

            var expectedBuffer = new byte[80];
            expectedBuffer[68] = 0b01000100;

            var expectedAttr = new Dictionary<int, IDictionary<byte, byte>>()
            {
                [68] = new Dictionary<byte, byte>()
                {
                    [Attributes.FOREGROUND_COLOR] = Colors.BLUE,
                    [Attributes.OUTLINE] = 0b00000100,
                    [Attributes.FIELD] = 0b01000100
                }
            };

            var handler = _service.Handlers[2];
            var eventInvoked = false;
            handler.RowUpdated += (sender, args) =>
            {
                eventInvoked = true;
                eventArgs = args;
            };

            Assert.AreEqual((8, 229), _service.OrderModifyField(data, 0, 228));
            handler.Update();

            Assert.True(eventInvoked);
            Assert.AreEqual(
                expectedBuffer,
                eventArgs.Data.Item1);

            Assert.AreEqual(
                expectedAttr,
                eventArgs.Data.Item2);
        }

        [Test]
        public void SetBufferAddress()
        {
            Assert.AreEqual((3, 228), _service.OrderSetBufferAddress(_data, 0));
        }
    }
}
