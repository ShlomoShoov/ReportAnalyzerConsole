using System;
using System.Diagnostics.Tracing;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Project
{
    
    class ReportAnalyzer
    {
        // Enums defeniton
        enum ReportType
        {
            Collect,
            Analyze,
            Recon,
            Intel
        }

        enum Status
        {
            Approved,
            Pending,
            Rejected
        }

        // Indexing the places, data Format and length
        const int UnityIndex = 0;
        const int ReportTypeIndex = 1;
        const int PriorityIndex = 2;
        const int ScoreIndex = 3;
        const int StatusIndex = 4;
        const int ReportLenghth = 5;

        // general consting and shared varibles

        const int ArrayLimit = 100; // max lines to process

        const int MaxPriority = 5;
        const int MinPriority = 0;

        const double MinScore = 0.0;
        const double MaxScore = 100.0;

        static bool debugeMode = true; // if true: prints all data about proccing process.
        const string ErrorMsg = "=== Error ===\n";
        const string WarningMsg = "=== Warning ===\n";

        
        static void Main(string[] args)
        {

            string path = @"C:\Users\user1\Desktop\studies\06_26\ReportAnalyzerConsole\reports.txt"; // contain the defult path to the file
            string[]? data = LoadFile(path: path);

            if (data is null) return;
            RunApp(lines: data);
        }

        // App flow function
        static void RunApp(string[] lines)
            {
            string[] units = new string[ArrayLimit];
            ReportType[] types = new ReportType[ArrayLimit];
            int[] priorities = new int[ArrayLimit];
            int[] scores = new int[ArrayLimit];
            Status[] statuses = new Status[ArrayLimit];

            int CountLines = ProcessReports(lines: lines, units: units, priorities:priorities ,types:types, scores:scores,statuses:statuses);

            }

        // Loading and Valid the data
        static string[]? LoadFile(string path)


            {
                string fileName = Path.GetFileName(path);
                if (!File.Exists(path: path))
                {

                    DisplayError($"{ErrorMsg}{fileName}>File not exists.");
                    return null;
                }

                string[] data = File.ReadAllLines(path: path);

                if (data.Length == 0)
                {
                    DisplayError($"{ErrorMsg}{fileName}>File is empty.");
                    return null;
                }
            if (debugeMode)
            {
  
                DisplayDebug($"File {fileName} loaded, {data.Length} lines found.");
                
            }

                return data;
            }

        static int ProcessReports(string[] lines, string[] units, ReportType[] types, int[] priorities, int[] scores, Status[] statuses)
            {
            if (debugeMode) Console.WriteLine("Starting proccing.");

            int CountLines = 0;

            for (int index = 0; index < lines.Length;  index++)
            {
                string[] line = lines[index].Split(",");

                bool validLine = true;

                // Line Varibeles
                string unit;
                ReportType type;
                int priority;
                int score;
                Status status;

                if (debugeMode) Console.WriteLine($"Procces... line {index+1}/{lines.Length}");

                // validetions and parses
                if (!ValidLineLength(line)) validLine = false;
                if (!TryParsePriority(line[PriorityIndex], out priority)) validLine = false;
                


                

            }
            return CountLines;
            

            }

        static bool ValidLineLength(string[] line)
        {
            if (line.Length > ReportLenghth) {
                if (debugeMode) DisplayWarning($"{WarningMsg}line contain {line.Length} columns , valid columns: {ReportLenghth} ");
                return false;
            }
            if (line.Length < ReportLenghth)
            {
                if (debugeMode) DisplayWarning($"{WarningMsg}line contain {line.Length} columns , valid columns: {ReportLenghth} ");
                return false;
            }
            return true;
        }


        static bool TryParsePriority(string priority, out int validPriority)
        {
            validPriority = 0;
            bool valid = int.TryParse(priority.Trim(), out int number);
            if (!valid)
            {
                if (debugeMode) DisplayWarning($"{WarningMsg}priority must be int, got :{priority}");
                return false;
            }
            if (number > MaxPriority || number < MinPriority)
            {
                if (debugeMode) DisplayWarning($"{WarningMsg}priority must be btewwen {MinPriority} to {MaxPriority} but got {number}");
                return false;
            }
            validPriority = number;
            return true;

        }

        static void DisplayWarning (string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static void DisplayError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static void DisplayDebug(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }





    }
}