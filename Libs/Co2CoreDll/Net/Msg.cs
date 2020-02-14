// * ************************************************************
// * * START:                                            msg.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Msg class for the library.
// * msg.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (October 19th, 2011)
// * Copyright (C) 2011 CptSky
// * 
// * ************************************************************

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL.Net
{
    public unsafe class Msg
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MsgHeader
        {
            public Int16 Length;
            public Int16 Type;
        }

        public const Int32 MSG_SEEK_SET = 0;
        public const Int32 MSG_SEEK_CUR = 1;
        public const Int32 MSG_SEEK_END = 2;

        private Encoding Encoding;
        private Byte* pBuffer;
        private Int32 Length;
        private Int32 Position;

        public String GetEncoding() { return Encoding.WebName; }
        public Int32 GetLength() { return Length; }

        /// <summary>
        /// Create a new message with the specified length.
        /// </summary>
        public Msg(Int32 Length)
        {
            this.Encoding = Encoding.GetEncoding("Windows-1252");
            this.pBuffer = (Byte*)Kernel.calloc(Length);
            this.Length = Length;
            this.Position = 0;
        }

        /// <summary>
        /// Create a new message with the specified length and with the specified encoding for the text.
        /// </summary>
        public Msg(Int32 Length, Encoding Encoding)
        {
            this.Encoding = Encoding;
            this.pBuffer = (Byte*)Kernel.calloc(Length);
            this.Length = Length;
            this.Position = 0;
        }

        ~Msg()
        {
            if (pBuffer != null)
                Kernel.free(pBuffer);
            pBuffer = null;
        }

        /// <summary>
        /// Change the current position in the buffer.
        /// </summary>
        public void Seek(Int32 Position, Int32 Mode)
        {
            if (Mode == MSG_SEEK_SET)
                this.Position = Position;
            else if (Mode == MSG_SEEK_CUR)
                this.Position += Position;
            else if (Mode == MSG_SEEK_END)
                this.Position = Length - Position;
        }

        /// <summary>
        /// Return the current position in the buffer.
        /// </summary>
        public Int32 Tell() { return Position; }

        /// <summary>
        /// Write a 8-bits signed integer at the current position.
        /// </summary>
        public void Write(SByte Param) { *(pBuffer + Position) = (Byte)Param; Position += sizeof(SByte); }

        /// <summary>
        /// Write a 8-bits unsigned integer at the current position.
        /// </summary>
        public void Write(Byte Param) { *(pBuffer + Position) = Param; Position += sizeof(Byte); }

        /// <summary>
        /// Write a 16-bits signed integer at the current position.
        /// </summary>
        public void Write(Int16 Param) { *((Int16*)(pBuffer + Position)) = Param; Position += sizeof(Int16); }

        /// <summary>
        /// Write a 16-bits unsigned integer at the current position.
        /// </summary>
        public void Write(UInt16 Param) { *((UInt16*)(pBuffer + Position)) = Param; Position += sizeof(UInt16); }

        /// <summary>
        /// Write a 32-bits signed integer at the current position.
        /// </summary>
        public void Write(Int32 Param) { *((Int32*)(pBuffer + Position)) = Param; Position += sizeof(Int32); }

        /// <summary>
        /// Write a 32-bits unsigned integer at the current position.
        /// </summary>
        public void Write(UInt32 Param) { *((UInt32*)(pBuffer + Position)) = Param; Position += sizeof(UInt32); }

        /// <summary>
        /// Write a 64-bits signed integer at the current position.
        /// </summary>
        public void Write(Int64 Param) { *((Int64*)(pBuffer + Position)) = Param; Position += sizeof(Int64); }

        /// <summary>
        /// Write a 64-bits unsigned integer at the current position.
        /// </summary>
        public void Write(UInt64 Param) { *((UInt64*)(pBuffer + Position)) = Param; Position += sizeof(UInt64); }

        /// <summary>
        /// Write a 8-bits unsigned integer array at the current position.
        /// </summary>
        public void Write(Byte[] Param)
        { 
            Kernel.memcpy((pBuffer + Position), Param, Param.Length);
            Position += Param.Length * sizeof(Byte);
        }

        /// <summary>
        /// Write a string decoded with the specified encoding in a 8-bits unsigned integer array.
        /// </summary>
        public void Write(String Param)
        {
            var Buffer = Encoding.GetBytes(Param);
            Kernel.memcpy(Buffer, (pBuffer + Position), Buffer.Length);
            Position += Buffer.Length * sizeof(Byte);
        }

        /// <summary>
        /// Write an array of strings decoded with the specified encoding in a 8-bits unsigned integer array.
        /// </summary>
        public void Write(String[] Params)
        {
            foreach (var Param in Params)
                Write(Param);
        }

        /// <summary>
        /// Write a string decoded with the specified encoding in a 8-bits unsigned integer array with an 8-bits unsigned
        /// integer as the length.
        /// </summary>
        public void Write(String Param, Boolean WithLength)
        {
            var Buffer = Encoding.GetBytes(Param);
            if (WithLength)
                Write((Byte)Buffer.Length);
            Write(Param);
        }

        /// <summary>
        /// Write an array of strings decoded with the specified encoding in a 8-bits unsigned integer array with an 8-bits
        /// unsigned integer as the length.
        /// </summary>
        public void Write(String[] Params, Boolean WithLength)
        {
            foreach (var Param in Params)
            {
                var Buffer = Encoding.GetBytes(Param);
                if (WithLength)
                    Write((Byte)Buffer.Length);
                Write(Param);
            }
        }

        /// <summary>
        /// Cast the class to a 8-bits unsigned integer array containing a copy of the buffer.
        /// </summary>
        public static implicit operator Byte[](Msg Packet)
        {
            var Buffer = new Byte[Packet.Length];
            Kernel.memcpy(Buffer, Packet.pBuffer, Packet.Length);
            return Buffer;
        }

        /// <summary>
        /// Cast the class to a pointer to the buffer.
        /// </summary>
        public static implicit operator Byte*(Msg Packet)
        {
            return Packet.pBuffer;
        }
    }
}

// * ************************************************************
// * * END:                                              msg.cs *
// * ************************************************************