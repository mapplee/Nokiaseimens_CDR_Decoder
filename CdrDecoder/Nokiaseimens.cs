using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace CdrDecoder
{
    class Nokiaseimens
    {

        public class FieldInfo
        {
            public int Offset;
            public int Length;
            public Enum DataType;

            public FieldInfo(int offset, int length, Enum dataTypeType)
            {
                this.Offset = offset;
                this.Length = length;
                this.DataType = dataTypeType;
            }
        }

        public static void PrintInfo(int interval, int cdrRecord, int currentPosition)
        {
            string str = "Bogas Byte";
            if (interval == 177) str = "PTC";
            if (interval == 190) str = "POC";
            if (interval == 24) str = "Trailer";
            if (interval == 41) str = "Header";

            Console.WriteLine("cdrNO : " + cdrRecord + "  Cdrlen: " + interval + "  " + "BytePos  : " + currentPosition + "  " + str);
        }


        public static void Decode(string filePath, string decodedFileName)
        {

            List<byte> fileData = File.ReadAllBytes(filePath).ToList();
            List<List<string>> csvData = new List<List<string>>();
            int cdrRecordNumber = 0;
            int totalBytes = fileData.Count;

            for (int currentPosition = 0; currentPosition < totalBytes - 1;)
            {
                // int interval = fileData[currentPosition] + fileData[currentPosition + 1];
                // PrintInfo(interval, cdrRecordNumber, currentPosition);
                //
                // if (interval == 177 || interval == 190)
                // {
                //     if (fileData[currentPosition + 1] > fileData[currentPosition])
                //     {
                //         currentPosition++;
                //     }
                //
                //     cdrRecordNumber++;
                //     Nokia.CdrType cdrType = (interval == 177) ? Nokia.CdrType.Ptc : Nokia.CdrType.Poc;
                //     csvData.Add(Decoder.Decode(currentPosition, fileData, cdrType));
                //     currentPosition += interval;
                    bool validPoint ;
                    while (currentPosition<totalBytes)
                    {
                        validPoint = false;
                        if (fileData[currentPosition] == 190 && fileData[currentPosition+2] == 17)
                            validPoint = true;

                    if (fileData[currentPosition] == 177 && fileData[currentPosition + 2] == 18 )
                        validPoint = true;

                    if (validPoint)
                        {
                            PrintInfo(fileData[currentPosition], cdrRecordNumber, currentPosition);
                            cdrRecordNumber++;
                            Nokia.CdrType cdrType = (fileData[currentPosition] == 177) ? Nokia.CdrType.Ptc : Nokia.CdrType.Poc;
                            csvData.Add(Decoder.Decode(currentPosition, fileData, cdrType));
                            
                             currentPosition += fileData[currentPosition];
                        }
                        else
                        {
                            currentPosition++;
                        }
                        
                       
                    }
                    
                // }
                // else if (interval == 0)
                // {
                //     currentPosition += 2;
                // }
                // else if (interval == 24)
                // {
                //     currentPosition += 24;
                //     while (interval != 41 && currentPosition < totalBytes)
                //     {
                //         interval = fileData[currentPosition++];
                //     }
                //     currentPosition += interval - 1;
                // }
                // else
                // {
                //     SkipToNextValidInterval(fileData, ref currentPosition, totalBytes);
                // }
            }

            WriteCsvFile(decodedFileName, csvData);
        }

        private static void SkipToNextValidInterval(List<byte> fileData, ref int currentPosition, int totalBytes)
        {
            while (currentPosition < totalBytes)
            {
                int interval = fileData[currentPosition] + fileData[currentPosition + 1];
                if (interval == 177 || interval == 190)
                {
                    break;
                }
                else if (interval == 24 || interval == 41)
                {
                    currentPosition += interval;
                }
                else
                {
                    currentPosition += 1;
                }

            }
        }

        private static void WriteCsvFile(string decodedFileName, List<List<string>> csvData)
        {
            string outputPath = $@"decodedCdr//{decodedFileName}_by_apple.csv";

            using (var file = File.CreateText(outputPath))
            {
                file.WriteLine(string.Join(",", Nokia.FieldList));
                csvData.ForEach(data =>
                {
                    file.WriteLine(string.Join(",", data));
                });
            }
        }



    }
}


