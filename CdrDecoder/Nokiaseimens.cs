using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
namespace CdrDecoder
{
    class Nokiaseimens
    {

        public class FieldInfo
        {
            public int Offset;
            public int Length;
            public Enum Operation;

            public FieldInfo(int offset, int length, Enum operationType)
            {
                this.Offset = offset;
                this.Length = length;
                this.Operation = operationType;
            }
        }


        public static void ptcDecoder(int currentPosition, List<byte> fileData)
        {
            string header = string.Join(",", Nokia.FieldList);
            List<string> data = new List<string>();

            int sum = 0;
            Nokia.FieldList.ForEach(f =>
            {
                int length = Nokia.PocFieldsDetails[f].Length;
                int offset = Nokia.PocFieldsDetails[f].Offset + currentPosition;
                List<byte> recordBytes = new List<byte>(fileData.Skip(offset).Take(length));
                switch (Nokia.PocFieldsDetails[f].Operation)
                {
                    case Nokia.DataType.HexWord:
                        {
                            HexWord hexWord = new HexWord(new HexByte[]
                            {
                            new HexByte(recordBytes[0]),
                            new HexByte(recordBytes[1]),

                            });
                            sum += length;
                            Console.WriteLine("Get " + f.ToString() + " : " + hexWord.getInt16().ToString() +
                                              " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
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
                            Console.WriteLine("Get " + f.ToString() + " : " + hexWord.getHexStrReversed().ToString() +
                                              " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;

                    case Nokia.DataType.TimeFieldDate:
                        {

                            List<byte> year = new List<byte>(recordBytes.Skip(5).Take(2));

                            TimeFieldDate td = new TimeFieldDate(new List<byte>(recordBytes.Take(5)), year);
                            sum += length;
                            Console.WriteLine("Get " + f.ToString() + " : " + td.dt.Date.ToString() + " Starting pos: " +
                                              offset + " len: " + length + " Byte sum : " + sum);
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
                            Console.WriteLine("Get " + f.ToString() + " : " + word1.getHexStrReversed() +
                                              word2.getHexStrReversed() + new HexByte(recordBytes[4]).ToString() +
                                              " Starting pos: " + offset + " len: " + length + " Byte sum : " + sum);
                        }
                        break;

                    case Nokia.DataType.BcdByte:
                        {
                            List<char> rs = new List<char>();
                            recordBytes.ForEach(b =>
                            {
                                foreach (var c in new HexByte(b).ToString().Reverse()) rs.Add(c);
                            });

                            sum += length;
                            Console.WriteLine("Get " + f.ToString() + " " + string.Join("", rs) + " Starting pos: " +
                                              offset + " len: " + length + " Byte sum : " +
                                              sum);
                        }
                        break;


                    default: break;
                }

            });
            // Console.WriteLine(header);

        }


        public static void Decode(string filePath)
        {
            List<byte> fileData = new List<byte>();
            fileData = File.ReadAllBytes(filePath).ToList();
            int interval = 0;
            using (var file = File.CreateText("data.csv"))
            {
                file.WriteLine(string.Join(",", Nokia.FieldList));
                for (int currentPosition = 0; currentPosition <= fileData.Count;)
                {
                    interval = fileData[currentPosition + 1] + fileData[currentPosition];

                    if (interval == 177)
                    {
                        List<string> row = PtcDecoder.Decode(currentPosition, fileData);

                        file.WriteLine(string.Join(",", row));

                    }
                    else if (interval == 190)
                    {
                        List<string> row = PocDecoder.Decode(currentPosition, fileData);
                        file.WriteLine(string.Join(",", row));
                    }

                    currentPosition += interval;
                    if (interval == 0) break;

                }
                file.Close();
            }
        }
    }
}


