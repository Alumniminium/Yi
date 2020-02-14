using System;
using System.Collections.Generic;
using YiX.Network.Packets.Conquer;

namespace YiX.Helpers
{
    public struct NpcDialog : IDisposable
    {
        #region Properties

        private readonly List<byte[]> _packets;

        #endregion

        public NpcDialog(ushort face)
        {
            _packets = new List<byte[]> {LegacyPackets.NpcFace(face)};
        }

        public void Dispose()
        {
        }

        public NpcDialog Finish()
        {
            _packets.Add(LegacyPackets.NpcFinish());
            return this;
        }

        public NpcDialog Input(string link, byte control)
        {
            _packets.Add(LegacyPackets.NpcInputBox(link, control));
            return this;
        }

        public NpcDialog Link(string link, byte control)
        {
            _packets.Add(LegacyPackets.NpcLink(link, control));
            return this;
        }

        public NpcDialog Text(string text)
        {
            for (var i = 0; i < text.Length; i += 255)
                _packets.Add(LegacyPackets.NpcSay(text.Substring(i, Math.Min(255, text.Length - i))));
            return this;
        }

        public static implicit operator byte[][](NpcDialog writer) => writer._packets.ToArray();
    }
}