using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdrDecoder
{
    internal class Decoder
    {
        public static List<string> Decode(int currentPosition, List<byte> fileData, Nokia.CdrType cdrType)
        {
            List<string> records = new List<string>();
            int totalSkip = 0, totalLengthNeedToIncrease=0;
            bool skiped = false;
            List<string> allFieldList = Nokia.FieldList;

            allFieldList.ForEach(f =>
            {
                int offset = 0, length = 0, tempOfset = 0;
                Nokia.DataType dataType = Nokia.DataType.Unknown;
                switch (cdrType)
                {
                    case Nokia.CdrType.Ptc:
                        {
                            length = Nokia.PtcFieldsDetails[f].Length;
                            offset = Nokia.PtcFieldsDetails[f].Offset + currentPosition;
                            tempOfset = Nokia.PtcFieldsDetails[f].Offset + currentPosition;
                            dataType = (Nokia.DataType)Nokia.PtcFieldsDetails[f].DataType;
                            if (f == "exchange_id")
                            {
                                totalSkip += TotalSkip(fileData, tempOfset);
                            }

                        }
                        break;
                    case Nokia.CdrType.Poc:
                        {
                            length = Nokia.PocFieldsDetails[f].Length;
                            offset = Nokia.PocFieldsDetails[f].Offset + currentPosition;
                            tempOfset = offset;

                            dataType = (Nokia.DataType)Nokia.PocFieldsDetails[f].DataType;
                            if (f == "exchange_id")
                            {
                                totalSkip += TotalSkip(fileData, tempOfset);
                            }
                        }
                        break;

                    default:
                        {
                            Console.WriteLine("----Unknown Block-----");
                        };
                        break;
                } 
                List<byte> recordBytes = fileData.GetRange(offset + (totalSkip), length);

                string recordData = GetRecordData(dataType, recordBytes);
                records.Add(recordData);
                if (f == "calling_number")
                {
                    int tmp = offset + totalSkip + length ;
                    while (fileData[tmp]==255)
                    {
                        totalSkip++;
                        tmp++;
                    }
                }

            });

            return records;
        }

        private static string GetRecordData(Nokia.DataType dataType, List<byte> recordBytes)
        {
            string record = "";
            switch (dataType)
            {
                case Nokia.DataType.HexByte:
                    {
                        List<char> rs = new List<char>();

                        recordBytes.ForEach(b =>
                        {
                            foreach (var c in new HexByte(b).ToString()) rs.Add(c);
                        });

                        record = string.Join("", rs);
                    }
                    break;
                case Nokia.DataType.HexByteRev:
                    {
                        List<char> rs = new List<char>();
                        recordBytes.ForEach(b =>
                        {
                            foreach (var c in new HexByte(b).ToString().Reverse()) rs.Add(c);
                        });
                        record = string.Join("", rs.ToArray().Reverse());
                    }
                    break;

                case Nokia.DataType.HexWordHexbyte:
                    {
                        HexWord hexWord = new HexWord(new HexByte[] { new HexByte(recordBytes[0]), new HexByte(recordBytes[1]) });
                        HexByte hexByte = new HexByte(recordBytes[2]);
                        record = string.Join("", hexByte.ToString());
                    }
                    break;

                case Nokia.DataType.HexWord:
                    {
                        HexWord hexWord = new HexWord(new HexByte[]
                        {
                        new HexByte(recordBytes[0]),
                        new HexByte(recordBytes[1]),
                        });
                        record = string.Join("", hexWord.getInt16().ToString());
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
                        record = string.Join("", hexDWord.getHexDwordStrReversed());
                    }
                    break;
                case Nokia.DataType.BcdWord:
                    {
                        HexWord hexWord = new HexWord(new HexByte[]
                        {
                        new HexByte(recordBytes[0]),
                        new HexByte(recordBytes[1]),
                        });
                        record = string.Join("", hexWord.getHexStrReversed());
                    }
                    break;

                case Nokia.DataType.TimeFieldDate:
                    {
                        List<byte> year = new List<byte>(recordBytes.Skip(5).Take(2));

                        TimeFieldData td = new TimeFieldData(new List<byte>(recordBytes.Take(5)), year);
                        record = string.Join("", td.dt.ToString());
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
                        record = string.Join("",
                            word1.getHexStrReversed() + word2.getHexStrReversed() + new HexByte(recordBytes[4]));
                    }
                    break;

                case Nokia.DataType.BcdByte:
                    {
                        List<char> rs = new List<char>();
                        recordBytes.ForEach(b =>
                        {
                            foreach (var c in new BcdBytes(b).ToString().Reverse()) rs.Add(c);
                        });
                        record = string.Join("", rs);
                    }
                    break;
                case Nokia.DataType.BcdByteRev:
                {
                    Stack<char> myStack = new Stack<char>();
                        List<char> rs = new List<char>();
                    recordBytes.ForEach(b =>
                    {
                       var s = b.ToString().Reverse();
                        foreach (var c in new BcdBytes(b).ToString().Reverse())
                        {
                            rs.Add(c);
                            myStack.Push(c);
                        }
                            
                    });
                    record = string.Join("", myStack);
                }
                    break;


                default: break;
            }

            return record;
        }

        private static int TotalSkip(List<byte> fileData, int tempOfset)
        {
            int totalSkip = 0;
            while (fileData[tempOfset] != 136)
            {
                totalSkip += 1;
                tempOfset += 1;
                if (fileData[tempOfset] == 136)
                {
                    break;
                }
            }
            while (fileData[tempOfset + 1] == 136)
            {
                totalSkip += 1;
                tempOfset += 1;
            }

            return totalSkip;
        }
    }
}

