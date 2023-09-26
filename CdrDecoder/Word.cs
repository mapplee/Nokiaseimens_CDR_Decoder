using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CdrDecoder
{
    public class Word<T>
    {
        public T[] Value { get; set; } = new T[2];
        public Word(T[] values)
        {
            if (values.Length != 2) throw new Exception("Array Length must be 2");
            this.Value[0] = values[0];
            this.Value[1] = values[1];
        }
        public override string ToString()
        {
            return string.Join("", this.Value.Select(v => v.ToString()));
        }

        public int getInt16()
        {
            return Convert.ToInt16(getHexStrReversed(), 16);
        }

        public int getInt32()
        {
            return Convert.ToInt32(getHexStrReversed(), 32);
        }
        public long getInt64()
        {
            return Convert.ToInt64(getHexStrReversed(), 8);
        }
        public Decimal getDecimal()
        {
            int decimalValue = Convert.ToInt32(Convert.ToInt16(getHexDwordStrReversed()).ToString(), 32);
            return decimalValue;
        }
        public string getReverseHexString()
        {
            string decimalValue = Convert.ToInt16(getHexDwordStrReversed()).ToString();
            return decimalValue;
        }

        public string getHexStrReversed()
        {
            var reversed = this.Value.Reverse();
            string hexStr = string.Join("", reversed.Select(hexByte => hexByte.ToString()));
            return hexStr;
        }

        public string reverseSubstrings(string input)
        {
            int index = 2;
            string firstSubstring = input.Substring(0, index);
            string secondSubstring = input.Substring(index);
            return secondSubstring + firstSubstring;
        }
        public string getHexDwordStrReversed()
        {
            var reversedFirstHexWord = reverseSubstrings(this.Value[0].ToString());
            var reversedSecondHexWord = reverseSubstrings(this.Value[1].ToString());
            return reversedFirstHexWord + reversedSecondHexWord;
        }

    }

    public class HexWord : Word<HexByte>
    {
        public Word<HexByte> Value { get; set; }
        public HexWord(HexByte[] value) : base(value) { }
    }
    public class HexDWord : Word<HexWord>
    {
        public Word<HexWord> Value { get; set; }
        public HexDWord(HexWord[] value) : base(value) { }

    }

    public class BcdDWord : Word<HexWord>
    {
        public Word<HexWord> Value { get; set; }
        public BcdDWord(HexWord[] value) : base(value) { }
    }
    // 5 BCD bytes + 1 BCD word:

    public class TimeFieldData
    {
        private BcdBytes[] TimeSpan { get; set; } = new BcdBytes[5];
        public DateTime dt { get; set; }
        private HexWord Year { get; set; }

        public TimeFieldData(List<byte> timeSpan,List<byte> year)
        {
            try
            {
                string yearString = new HexByte(year[1]).ToString() + new HexByte(year[0]).ToString();

                int month = Convert.ToInt16(new HexByte(timeSpan[4]).ToString());
                int day = Convert.ToInt16(new HexByte(timeSpan[3]).ToString());
                int hour = Convert.ToInt16(new HexByte(timeSpan[2]).ToString());
                int minute = Convert.ToInt16(new HexByte(timeSpan[1]).ToString());
                int second = Convert.ToInt16(new HexByte(timeSpan[0]).ToString());
                this.dt = new DateTime(Convert.ToInt32(yearString), month, day, hour, minute, second);
            }
            catch (Exception e)
            {
                // Console.WriteLine(e);
                // throw;
            }
           
            
        }
    }

    

}