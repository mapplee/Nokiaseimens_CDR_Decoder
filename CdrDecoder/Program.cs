using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace CdrDecoder
{
    class Program
    {
        static void Main(string[] args)
        {

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();


            string directoryPath = "D:\\Nokiaseimens_CDR_Decoder\\CdrDecoder\\bin\\Debug\\newcdr";
            string[] filePaths = Directory.GetFiles(directoryPath, "*7058.DAT");
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath); ;
                Nokiaseimens.Decode(filePath, fileName);
            }

            TimeSpan elapsed = stopwatch.Elapsed;

            Console.WriteLine($"Elapsed time: {elapsed.TotalSeconds} ms"); 
            Console.Read();
        }
    }
}
