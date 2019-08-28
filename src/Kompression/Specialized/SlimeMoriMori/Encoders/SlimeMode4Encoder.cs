﻿using System.IO;
using Kompression.IO;
using Kompression.PatternMatch;
using Kompression.Specialized.SlimeMoriMori.ValueWriters;

namespace Kompression.Specialized.SlimeMoriMori.Encoders
{
    class SlimeMode4Encoder : ISlimeEncoder
    {
        private IValueWriter _valueWriter;

        public SlimeMode4Encoder(IValueWriter valueWriter)
        {
            _valueWriter = valueWriter;
        }

        public void Encode(Stream input, BitWriter bw, Match[] matches)
        {
            while (input.Position < input.Length)
            {
                _valueWriter.WriteValue(bw, (byte)input.ReadByte());
            }
        }
    }
}