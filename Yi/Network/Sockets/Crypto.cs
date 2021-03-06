﻿using System;
using System.IO;
using System.Text;
using Yi.Enums;
using Yi.Network.Packets.Conquer;
using Yi.Structures;
using Player = Yi.Entities.Player;

namespace Yi.Network.Sockets
{
    [Serializable]
    public class Crypto
    {
        private readonly object _lock = new object();
        private ushort _counter;
        private byte[] _key3;
        private byte[] _key4;
        private ushort _outCounter;
        private bool _server;

        public unsafe byte[] Decrypt(byte[] In, int size)
        {
            var Out = BufferPool.GetBuffer();
            lock (_lock)
            {
                fixed (byte* ptr = Out)
                {
                    for (ushort I = 0; I < size; I++)
                    {
                        *(ptr + I) = (byte) (In[I] ^ 0xAB);
                        *(ptr + I) = (byte) ((*(ptr + I) << 4) | (*(ptr + I) >> 4));
                        if (_server)
                        {
                            *(ptr + I) = (byte) (_key4[_counter >> 8] ^ *(ptr + I));
                            *(ptr + I) = (byte) (_key3[_counter & 0xFF] ^ *(ptr + I));
                        }
                        else
                        {
                            *(ptr + I) = (byte) (Key2[_counter >> 8] ^ *(ptr + I));
                            *(ptr + I) = (byte) (Key1[_counter & 0xFF] ^ *(ptr + I));
                        }

                        _counter += 1;
                    }
                }
                
            }

            return Out;
        }

        public unsafe byte[] Encrypt(byte[] In, int size)
        {
            var Out = BufferPool.GetBuffer();
            lock (_lock)
            {
                fixed (byte* ptr = Out)
                {
                    for (var I = 0; I < size; I++)
                    {
                        *(ptr + I) = (byte) (In[I] ^ 0xAB);
                        *(ptr + I) = (byte) ((*(ptr + I) << 4) | (*(ptr + I) >> 4));
                        *(ptr + I) = (byte) (Key2[_outCounter >> 8] ^ *(ptr + I));
                        *(ptr + I) = (byte) (Key1[_outCounter & 0xFF] ^ *(ptr + I));
                        _outCounter += 1;
                    }
                }
            }
            return Out;
        }

        public unsafe void SetKeys(uint inKey1, uint inKey2)
        {
            var dwKey1 = (inKey1 + inKey2) ^ 0x4321 ^ inKey1;
            var dwKey2 = dwKey1 * dwKey1;
            _key3 = new byte[256];
            _key4 = new byte[256];
            fixed (void* uKey1 = Key1, uKey3 = _key3, uKey2 = Key2, uKey4 = _key4)
            {
                const byte dwKeyLoop = 256 / 4;
                for (byte I = 0; I < dwKeyLoop; I++)
                {
                    *((uint*)uKey3 + I) = dwKey1 ^ *((uint*)uKey1 + I);
                    *((uint*)uKey4 + I) = dwKey2 ^ *((uint*)uKey2 + I);
                }
            }
            _server = true;
        }
        private static readonly byte[] Key1 = {0x9d, 0x90, 0x83, 0x8a, 0xd1, 140, 0xe7, 0xf6, 0x25, 40, 0xeb, 130, 0x99, 100, 0x8f, 0x2e, 0x2d, 0x40, 0xd3, 250, 0xe1, 0xbc, 0xb7, 230, 0xb5, 0xd8, 0x3b, 0xf2, 0xa9, 0x94, 0x5f, 30, 0xbd, 240, 0x23, 0x6a, 0xf1, 0xec, 0x87, 0xd6, 0x45, 0x88, 0x8b, 0x62, 0xb9, 0xc4, 0x2f, 14, 0x4d, 160, 0x73, 0xda, 1, 0x1c, 0x57, 0xc6, 0xd5, 0x38, 0xdb, 210, 0xc9, 0xf4, 0xff, 0xfe, 0xdd, 80, 0xc3, 0x4a, 0x11, 0x4c, 0x27, 0xb6, 0x65, 0xe8, 0x2b, 0x42, 0xd9, 0x24, 0xcf, 0xee, 0x6d, 0, 0x13, 0xba, 0x21, 0x7c, 0xf7, 0xa6, 0xf5, 0x98, 0x7b, 0xb2, 0xe9, 0x54, 0x9f, 0xde, 0xfd, 0xb0, 0x63, 0x2a, 0x31, 0xac, 0xc7, 150, 0x85, 0x48, 0xcb, 0x22, 0xf9, 0x84, 0x6f, 0xce, 0x8d, 0x60, 0xb3, 0x9a, 0x41, 220, 0x97, 0x86, 0x15, 0xf8, 0x1b, 0x92, 9, 180, 0x3f, 190, 0x1d, 0x10, 3, 10, 0x51, 12, 0x67, 0x76, 0xa5, 0xa8, 0x6b, 2, 0x19, 0xe4, 15, 0xae, 0xad, 0xc0, 0x53, 0x7a, 0x61, 60, 0x37, 0x66, 0x35, 0x58, 0xbb, 0x72, 0x29, 20, 0xdf, 0x9e, 0x3d, 0x70, 0xa3, 0xea, 0x71, 0x6c, 7, 0x56, 0xc5, 8, 11, 0xe2, 0x39, 0x44, 0xaf, 0x8e, 0xcd, 0x20, 0xf3, 90, 0x81, 0x9c, 0xd7, 70, 0x55, 0xb8, 0x5b, 0x52, 0x49, 0x74, 0x7f, 0x7e, 0x5d, 0xd0, 0x43, 0xca, 0x91, 0xcc, 0xa7, 0x36, 0xe5, 0x68, 0xab, 0xc2, 0x59, 0xa4, 0x4f, 110, 0xed, 0x80, 0x93, 0x3a, 0xa1, 0xfc, 0x77, 0x26, 0x75, 0x18, 0xfb, 50, 0x69, 0xd4, 0x1f, 0x5e, 0x7d, 0x30, 0xe3, 170, 0xb1, 0x2c, 0x47, 0x16, 5, 200, 0x4b, 0xa2, 0x79, 4, 0xef, 0x4e, 13, 0xe0, 0x33, 0x1a, 0xc1, 0x5c, 0x17, 6, 0x95, 120, 0x9b, 0x12, 0x89, 0x34, 0xbf, 0x3e};
        private static readonly byte[] Key2 = {0x62, 0x4f, 0xe8, 0x15, 0xde, 0xeb, 4, 0x91, 0x1a, 0xc7, 0xe0, 0x4d, 0x16, 0xe3, 0x7c, 0x49, 210, 0x3f, 0xd8, 0x85, 0x4e, 0xdb, 0xf4, 1, 0x8a, 0xb7, 0xd0, 0xbd, 0x86, 0xd3, 0x6c, 0xb9, 0x42, 0x2f, 200, 0xf5, 190, 0xcb, 0xe4, 0x71, 250, 0xa7, 0xc0, 0x2d, 0xf6, 0xc3, 0x5c, 0x29, 0xb2, 0x1f, 0xb8, 0x65, 0x2e, 0xbb, 0xd4, 0xe1, 0x6a, 0x97, 0xb0, 0x9d, 0x66, 0xb3, 0x4c, 0x99, 0x22, 15, 0xa8, 0xd5, 0x9e, 0xab, 0xc4, 0x51, 0xda, 0x87, 160, 13, 0xd6, 0xa3, 60, 9, 0x92, 0xff, 0x98, 0x45, 14, 0x9b, 180, 0xc1, 0x4a, 0x77, 0x90, 0x7d, 70, 0x93, 0x2c, 0x79, 2, 0xef, 0x88, 0xb5, 0x7e, 0x8b, 0xa4, 0x31, 0xba, 0x67, 0x80, 0xed, 0xb6, 0x83, 0x1c, 0xe9, 0x72, 0xdf, 120, 0x25, 0xee, 0x7b, 0x94, 0xa1, 0x2a, 0x57, 0x70, 0x5d, 0x26, 0x73, 12, 0x59, 0xe2, 0xcf, 0x68, 0x95, 0x5e, 0x6b, 0x84, 0x11, 0x9a, 0x47, 0x60, 0xcd, 150, 0x63, 0xfc, 0xc9, 0x52, 0xbf, 0x58, 5, 0xce, 0x5b, 0x74, 0x81, 10, 0x37, 80, 0x3d, 6, 0x53, 0xec, 0x39, 0xc2, 0xaf, 0x48, 0x75, 0x3e, 0x4b, 100, 0xf1, 0x7a, 0x27, 0x40, 0xad, 0x76, 0x43, 220, 0xa9, 50, 0x9f, 0x38, 0xe5, 0xae, 0x3b, 0x54, 0x61, 0xea, 0x17, 0x30, 0x1d, 230, 0x33, 0xcc, 0x19, 0xa2, 0x8f, 40, 0x55, 30, 0x2b, 0x44, 0xd1, 90, 7, 0x20, 0x8d, 0x56, 0x23, 0xbc, 0x89, 0x12, 0x7f, 0x18, 0xc5, 0x8e, 0x1b, 0x34, 0x41, 0xca, 0xf7, 0x10, 0xfd, 0xc6, 0x13, 0xac, 0xf9, 130, 0x6f, 8, 0x35, 0xfe, 11, 0x24, 0xb1, 0x3a, 0xe7, 0, 0x6d, 0x36, 3, 0x9c, 0x69, 0xf2, 0x5f, 0xf8, 0xa5, 110, 0xfb, 20, 0x21, 170, 0xd7, 240, 0xdd, 0xa6, 0xf3, 140, 0xd9};

        public static void DecryptSkill(Player player, ref MsgInteract packet, out Skill skill)
        {
            byte[] buffer = packet;
            var id = Convert.ToUInt16(((long)buffer[24] & 0xFF) | (((long)buffer[25] & 0xFF) << 8));

            id ^= 0x915d;
            id ^= (ushort)player.UniqueId;
            id = (ushort)(id << 0x3 | id >> 0xd);
            id -= 0xeb42;

            long x = (buffer[16] & 0xFF) | ((buffer[17] & 0xFF) << 8);
            long y = (buffer[18] & 0xFF) | ((buffer[19] & 0xFF) << 8);

            x = x ^ (player.UniqueId & 0xffff) ^ 0x2ed6;
            x = ((x << 1) | ((x & 0x8000) >> 15)) & 0xffff;
            x |= 0xffff0000;
            x -= 0xffff22ee;

            y = y ^ (player.UniqueId & 0xffff) ^ 0xb99b;
            y = ((y << 5) | ((y & 0xF800) >> 11)) & 0xffff;
            y |= 0xffff0000;
            y -= 0xffff8922;

            var target = BitConverter.ToInt32(buffer, 12);
            target = (int)((((target & 0xffffe000) >> 13) | ((target & 0x1fff) << 19)) ^ 0x5F2D2463 ^ player.UniqueId) - 0x746F4AE6;

            packet.TargetUniqueId = target;
            packet.X = (ushort) x;
            packet.Y = (ushort) y;
            skill = player.Skills[(SkillId)id];
            BufferPool.RecycleBuffer(buffer);
        }

        private static readonly uint[] Rc5Key = { 0xEBE854BC, 0xB04998F7, 0xFFFAA88C, 0x96E854BB, 0xA9915556, 0x48E44110, 0x9F32308F, 0x27F41D3E, 0xCF4F3523, 0xEAC3C6B4, 0xE9EA5E03, 0xE5974BBA, 0x334D7692, 0x2C6BCF2E, 0xDC53B74, 0x995C92A6, 0x7E4F6D77, 0x1EB2B79F, 0x1D348D89, 0xED641354, 0x15E04A9D, 0x488DA159, 0x647817D3, 0x8CA0BC20, 0x9264F7FE, 0x91E78C6C, 0x5C9A07FB, 0xABD4DCCE, 0x6416F98D, 0x6642AB5B };

        public static string DecryptPassword(byte[] bytes)
        {
            using (var reader = new BinaryReader(new MemoryStream(bytes, false)))
            {
                var passInts = new uint[4];
                for (uint I = 0; I < 4; I++)
                    passInts[I] = (uint)reader.ReadInt32();

                for (var I = 1; I >= 0; I--)
                {
                    var temp1 = passInts[I * 2 + 1];
                    var temp2 = passInts[I * 2];
                    for (var j = 11; j >= 0; j--)
                    {
                        temp1 = RightRotate(temp1 - Rc5Key[j * 2 + 7], temp2) ^ temp2;
                        temp2 = RightRotate(temp2 - Rc5Key[j * 2 + 6], temp1) ^ temp1;
                    }
                    passInts[I * 2 + 1] = temp1 - Rc5Key[5];
                    passInts[I * 2] = temp2 - Rc5Key[4];
                }
                var writer = new BinaryWriter(new MemoryStream(bytes, true));
                for (uint I = 0; I < 4; I++)
                    writer.Write((int)passInts[I]);
                for (var I = 0; I < 16; I++)
                {
                    if (bytes[I] == 0)
                        return Encoding.ASCII.GetString(bytes, 0, I);
                }
                return Encoding.ASCII.GetString(bytes);
            }
        }

        private static uint RightRotate(uint dwVar, uint dwOffset)
        {
            dwOffset = dwOffset & 0x1F;
            var dwTemp1 = dwVar << (int)(32 - dwOffset);
            var dwTemp2 = dwVar >> (int)dwOffset;
            dwTemp2 = dwTemp2 | dwTemp1;
            return dwTemp2;
        }
    }
}