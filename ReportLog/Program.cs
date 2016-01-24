using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportLog
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> levelsCount = new Dictionary<string,int>();
            string []levels = {"|INFO|", "|WARN|", "|ERROR|"};
            string filePath = "..\\..\\..\\FileSystemVisitorTests\\bin\\Debug\\FileSystemVisitor.log";
            Console.WriteLine("Write path to log file or press ENTER to get default path: ");
            string userPath = Console.ReadLine();
            filePath = string.IsNullOrEmpty(userPath) ? filePath : userPath;
            using (StreamReader sr = new StreamReader(filePath))
            {
                string logLine;
                while ((logLine = sr.ReadLine()) != null)
                {
                    foreach(string level in levels)
                    {
                        if (logLine.Contains(level))
                        {
                            if (levelsCount.ContainsKey(level))
                                ++levelsCount[level];
                            else
                                levelsCount[level] = 1;
                        }
                        if (level == "|ERROR|" && logLine.Contains(level))
                            Console.WriteLine(logLine);
                    }
                }
            }

            foreach(var p in levelsCount)
            {
                Console.WriteLine("{0} : {1}", p.Key, p.Value);
            }
        }
    }
}
