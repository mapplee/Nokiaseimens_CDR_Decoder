using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdrDecoder
{
    internal class PocDecoder
    {
        public static List<string> Decode(int currentPosition, List<byte> fileData)
        {
            int sum = 0;
            List<string> records = new List<string>();
            Nokia.FieldList.ForEach(f =>
            {
                int length = Nokia.PocFieldsDetails[f].Length;
                int offset = Nokia.PocFieldsDetails[f].Offset + currentPosition;
                List<byte> recordBytes = new List<byte>(fileData.Skip(offset).Take(length));
                switch (Nokia.PocFieldsDetails[f].DataType)
                {
                    case Nokia.DataType.HexByte:
                        {
                            List<char> rs = new List<char>();

                            recordBytes.ForEach(b =>
                            {
                                foreach (var c in new HexByte(b).ToString()) rs.Add(c);
                            });

                            sum += length;
                            records.Add(string.Join("", rs));
                            // Console.WriteLine("Get " + f.ToString() + " " + string.Join("", rs) + " Starting pos: " + offset + " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;
                    case Nokia.DataType.HexByteRev:
                    {
                        List<char> rs = new List<char>();
                        recordBytes.ForEach(b =>
                        {
                            foreach (var c in new HexByte(b).ToString().Reverse()) rs.Add(c);
                        });

                        sum += length;
                        records.Add(string.Join("", rs.ToArray().Reverse()));
                        // Console.WriteLine("Get " + f.ToString() + " " + string.Join("", rs) + " Starting pos: " + offset + " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                    }
                        break;

                    case Nokia.DataType.HexWordHexbyte:
                        {
                            HexWord hexWord = new HexWord(new HexByte[] { new HexByte(recordBytes[0]), new HexByte(recordBytes[1]) });
                            HexByte hexByte = new HexByte(recordBytes[2]);
                            sum += length;
                            records.Add(string.Join("", hexByte.ToString()));

                            // Console.WriteLine("Get " + f.ToString() + " " + string.Join("", hexByte.ToString(), hexWord.getHexStrReversed()) + " Starting pos: " + offset + " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;

                    case Nokia.DataType.HexWord:
                        {
                            HexWord hexWord = new HexWord(new HexByte[]
                            {
                            new HexByte(recordBytes[0]),
                            new HexByte(recordBytes[1]),

                            });
                            sum += length;
                            records.Add(string.Join("", hexWord.getInt16().ToString()));

                            // Console.WriteLine("Get " + f.ToString() + " : " + hexWord.getInt16().ToString() + " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;
                    case Nokia.DataType.HexDword:
                        {
                            HexWord hexWord1 = new HexWord(new HexByte[]
                            {
                            new HexByte(recordBytes[0]),
                            new HexByte(recordBytes[1]),

                            });
                            HexWord hexWord2 = new HexWord(new HexByte[]
                            {
                            new HexByte(recordBytes[2]),
                            new HexByte(recordBytes[3]),

                            });
                            HexDWord hexDWord = new HexDWord(new HexWord[]
                            {
                           hexWord2,
                           hexWord1

                            });
                            sum += length;
                            records.Add(string.Join("", hexDWord.getHexDwordStrReversed()));

                            // Console.WriteLine("Get " + f.ToString() + " : " + hexDWord.getHexDwordStrReversed() + " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;
                    case Nokia.DataType.BcdWord:
                        {
                            HexWord hexWord = new HexWord(new HexByte[]
                            {
                            new HexByte(recordBytes[0]),
                            new HexByte(recordBytes[1]),

                            });
                            sum += length;
                            records.Add(string.Join("", hexWord.getHexStrReversed()));
                            // Console.WriteLine("Get " + f.ToString() + " : " + hexWord.getHexStrReversed() + " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;

                    case Nokia.DataType.TimeFieldDate:
                        {

                            List<byte> year = new List<byte>(recordBytes.Skip(5).Take(2));

                            TimeFieldDate td = new TimeFieldDate(new List<byte>(recordBytes.Take(5)), year);
                            sum += length;
                            records.Add(string.Join("", td.dt.ToString()));
                            // Console.WriteLine("Get " + f.ToString() + " : " + td.dt.ToString() + " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;

                    case Nokia.DataType.WordWordByte:
                        {
                            Word<HexByte> word1 = new Word<HexByte>(
                                new HexByte[]
                                {
                                new HexByte(recordBytes[0]),
                                new HexByte(recordBytes[1]),
                                }
                            );
                            Word<HexByte> word2 = new Word<HexByte>(
                                new HexByte[]
                                {
                                new HexByte(recordBytes[2]),
                                new HexByte(recordBytes[3]),
                                }
                            );

                            sum += length;
                            records.Add(string.Join("", word1.getHexStrReversed() + word2.getHexStrReversed() + new HexByte(recordBytes[4])));

                            // Console.WriteLine("Get " + f.ToString() + " : " + word1.getHexStrReversed() + word2.getHexStrReversed() + new HexByte(recordBytes[4]).ToString() + " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;

                    case Nokia.DataType.BcdByte:
                        {
                            List<char> rs = new List<char>();
                            recordBytes.ForEach(b =>
                            {
                                foreach (var c in new BcdBytes(b).ToString().Reverse()) rs.Add(c);
                            });

                            sum += length;
                            records.Add(string.Join("", rs));
                            // Console.WriteLine("Get " + f.ToString() + " " + string.Join("", rs) + " Starting pos: " + offset + " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;


                    default: break;
                }

            });
            return records;
        }
    }


}
