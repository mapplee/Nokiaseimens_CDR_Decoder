using System;
using System.Collections.Generic;
using System.Linq;


namespace CdrDecoder
{
    class Program
    {
        static void Main(string[] args)
        {
            //HexByte hb = new HexByte(160);
            //Console.WriteLine("hexByte rep of 38=" + hb.ToString());

            //HexWord hw = new HexWord(new HexByte[] { new HexByte(160), new HexByte(00) });
            //HexWord hw2 = new HexWord(new HexByte[] { new HexByte(02), new HexByte(00) });


            //Console.WriteLine("string rep of hexbytes 0002=" + hw.ToString());
            ////Console.WriteLine("int value of hexbytes 00A0=" + hw.getInt32());
            //BcdDWord bcdDword = new BcdDWord(new HexWord[] { hw2, hw });

            //Console.WriteLine("string rep of hexDword =" + bcDDword.ToString());
            //Console.WriteLine("int value of hexDword A0 00 02 00 = " + bcdDword.getDecimal());


            Nokiaseimens.Decode("CF5648.DAT");
            Console.Read();
        }
    }
}
