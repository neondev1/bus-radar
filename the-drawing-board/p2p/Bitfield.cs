// This code literally serves no functional purpose whatsoever
// I just wrote it because I was bored

using System;

namespace p2p
{
    internal class Bitfield
    {
        private readonly int _length;

        public byte[] Bytes { get; }
        public int Length { get => Bytes.Length; }
        public int Count { get; }
        public bool CanUseHammingCodes { get; private set; }

        public Bitfield(byte[] bits, bool hamming = false)
        {
            this._length = bits.Length;
            int power = 3, count = 1;
            for (; count < bits.Length; power++)
                count *= 2;
            this.Bytes = new byte[count];
            for (int i = 0; i < bits.Length; i++)
                this.Bytes[i + count - bits.Length] = bits[i];
            this.CanUseHammingCodes = count * 8 - this._length * 8 - power - 1 >= 0;
            bool flag = false;
            if (!this.CanUseHammingCodes)
            {
                for (int i = 0; i < power + 1; i++)
                    if (this[i] != 0)
                        flag = true;
                if (hamming && flag)
                    this.Bytes = new byte[count * 2];
                for (int i = 0; hamming && flag && i < bits.Length; i++)
                    this.Bytes[i + count * 2 - bits.Length] = bits[i];
                if (hamming || !flag)
                    this.CanUseHammingCodes = true;
            }
            this.Count = this.Bytes.Length * 8;
        }

        public Bitfield(Bitfield field, bool hamming = false)
        {
            this.Bytes = new byte[field.Bytes.Length];
            bool flag = false;
            if (hamming && !field.CanUseHammingCodes)
            {
                int power = 3, count = 1;
                for (; count < field.Bytes.Length; power++)
                    count *= 2;
                for (int i = 0; i < power + 1; i++)
                    if (field[i] != 0)
                        flag = true;
                if (flag)
                    this.Bytes = new byte[field.Bytes.Length * 2];
                for (int i = 0; flag && i < field.Bytes.Length; i++)
                {
                    this.Bytes[i] = 0;
                    this.Bytes[i + field.Bytes.Length] = field.Bytes[i];
                }
                this.CanUseHammingCodes = true;
            }
            else
                this.CanUseHammingCodes = field.CanUseHammingCodes;
            for (int i = 0; !flag && i < field.Bytes.Length; i++)
                this.Bytes[i] = field.Bytes[i];
            this.Count = this.Bytes.Length * 8;
            this._length = field._length;
        }

        public byte this[int index]
        {
            get => (byte)(1 & (this.Bytes[index / 8] >> (7 - index % 8)));
            set
            {
                if (value > 1)
                    throw new ArgumentException("Bits may only be set to 0 or 1.");
                if (value > 0)
                    this.Bytes[index / 8] |= (byte)(0b10000000 >> (index % 8));
                else
                    this.Bytes[index / 8] &= (byte)~(0b10000000 >> (index % 8));
                int power = 3, count = 1;
                for (; count < this.Bytes.Length; power++)
                    count *= 2;
                for (int i = 0; i < power + 1; i++)
                    if (this[i] != 0)
                        this.CanUseHammingCodes = false;
            }
        }

        public void Write(ulong value, int count, ref int index)
        {
            index += count;
            for (int i = 0; i < count; i++)
                if (index - i - 1 < this.Count)
                    this[index - i - 1] = (byte)(1 & (value >> i));
        }

        public ulong Read(int count, ref int index)
        {
            ulong result = 0;
            for (int i = 0; i < count; i++)
                result |= (ulong)this[index + i] << (count - i - 1);
            index += count;
            return result;
        }

        public Bitfield Shl(int position)
        {
            Bitfield field = new(this);
            for (int i = 0; i < position / 8; i++)
            {
                field.Bytes[i] = (byte)(0b11111110 & (this.Bytes[i] << 1));
                field.Bytes[i] |= (byte)((0b10000000 & field.Bytes[i + 1]) >> 7);
            }
            for (int i = position - position % 8; i < position; i++)
                field[i] = field[i + 1];
            field[position] = 0;
            int power = 3, count = 1;
            for (; count < field.Bytes.Length; power++)
                count *= 2;
            for (int i = 0; i < power + 1; i++)
                if (field[i] != 0)
                    field.CanUseHammingCodes = false;
            return field;
        }

        public Bitfield Shr(int position)
        {
            Bitfield field = new(this);
            for (int i = position; i > position - position % 8 && i > 0; i--)
                field[i] = field[i - 1];
            field[position - position % 8] = field[position - position % 8 - 1];
            for (int i = position / 8 - 1; i >= 0; i--)
            {
                field.Bytes[i] = (byte)(0b01111111 & (this.Bytes[i] >> 1));
                field.Bytes[i] |= (byte)(i > 0 ? (1 & field.Bytes[i - 1]) << 7 : 0);
            }
            return field;
        }

        public Bitfield ToHammingCode()
        {
            Bitfield field = new(this, true);
            for (int i = field.Count / 2; i >= 1; i /= 2)
                field = field.Shl(i);
            int flags = 0;
            for (int i = 1; i < field.Count; i++)
                if (field[i] != 0)
                    flags ^= i;
            for (int i = 1, j = 0; i < field.Count; i *= 2, j++)
                field[i] = (byte)((flags & i) >> j);
            for (int i = 1; i < field.Count; i++)
                field[0] ^= field[i];
            field.CanUseHammingCodes = false;
            return field;
        }

        public byte[] FromHammingCode(out bool error)
        {
            Bitfield field = new(this);
            int pos = 0, parity = field[0];
            for (int i = 1; i < field.Count; i++)
            {
                parity ^= field[i];
                if (field[i] != 0)
                    pos ^= i;
            }
            error = pos > 0;
            if (error && parity == 0)
                throw new InvalidOperationException(
                    "At least two errors are present within the Hamming code.");
            else
                field[pos] = (byte)(1 ^ field[pos]);
            field[0] = 0;
            for (int i = 1; i < field.Count; i *= 2)
                field = field.Shr(i);
            byte[] result = field.Bytes[(field.Bytes.Length - field._length)..];
            return result;
        }
    }
}
