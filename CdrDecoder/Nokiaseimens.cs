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
            string str = "Irrelevant";
            if (interval == 177) str = "PTC";
            if (interval == 190) str = "POC";
            Console.WriteLine("cdrNO : " + cdrRecord + "  Cdrlen: " + interval + "  " + "BytePos  : " + currentPosition + "  " + str);
        }


        public static void Decode(string filePath)
        {
            List<byte> fileData = File.ReadAllBytes(filePath).ToList();

            List<List<string>> csvData = new List<List<string>>();

            int cdrRecordNumber = 0;

            int totalBytes = fileData.Count;
            for (int currentPosition = 0; currentPosition < totalBytes - 1;)
            {
                int interval = fileData[currentPosition] + fileData[currentPosition + 1];
                PrintInfo(interval, cdrRecordNumber, currentPosition);
                if (interval == 177)
                {
                    cdrRecordNumber++;
                    csvData.Add(Decoder.Decode(currentPosition, fileData, Nokia.CdrType.Ptc));
                }
                else if (interval == 190)
                {
                    cdrRecordNumber++;
                    csvData.Add(Decoder.Decode(currentPosition, fileData, Nokia.CdrType.Poc));
                }

                if (interval == 0)
                {
                    currentPosition += 2;
                }
                else if (interval == 24)
                {
                    currentPosition += 24;
                    while (interval != 41 && currentPosition < totalBytes)
                    {
                        interval = fileData[currentPosition++];
                    }
                    currentPosition += interval - 1;
                }
                else
                {
                    currentPosition += interval;
                }
            }
            using (var file = File.CreateText("data3.csv"))
            {
                file.WriteLine(string.Join(",", Nokia.FieldList));
                csvData.ForEach(data =>
                    {
                        file.WriteLine(string.Join(",", data));
                    });

                file.Close();
            }
        }
    }
}


