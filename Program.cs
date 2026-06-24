using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Security.Cryptography;
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
            double[] scores = new double[ArrayLimit];
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

        static int ProcessReports(string[] lines, string[] units, ReportType[] types, int[] priorities, double[] scores, Status[] statuses)
            {
            if (debugeMode) DisplayDebug("Starting proccing.");

            int currentIndex = 0;

            for (int index = 0; index < lines.Length;  index++)
            {
                string[] line = lines[index].Split(",");

                bool validLine = true;

                // Line Varibeles
                string unit;
                ReportType type;
                int priority;
                double score;
                Status status;

                if (debugeMode) Console.WriteLine($"Procces... line {index+1}/{lines.Length}");

                // valid array not over the limit befor assiging
                if (currentIndex+1 == ArrayLimit)
                {
                    DisplayError($"{ErrorMsg}File has more validable lines then the Memory Limit\n" +
                        $"Array limit : {ArrayLimit} \n" +
                        $"| notice that we enter and calaulate all valid lines until line {index+1}");
                    break;
                }

                // validetions and parses
                if (!ValidLineLength(line)) validLine = false;
                if (!TryParsePriority(line[PriorityIndex], out priority)) validLine = false;
                if (!TryParseScore(line[ScoreIndex], out score)) validLine = false;
                if (!TryParseStatus(line[StatusIndex], out status)) validLine = false;
                if (!TryParseType(line[ReportTypeIndex], out type)) validLine = false;
                unit = line[UnityIndex];

                if (!validLine)
                {
                    if (debugeMode) DisplayWarning($"{WarningMsg}line {index + 1} invalid. not enter to procces");
                    continue;
                }


                // put data in the arrays
                units[currentIndex] = unit;
                priorities[currentIndex] = priority;
                statuses[currentIndex] = status;
                scores[currentIndex] = score;
                types[currentIndex] = type;

                currentIndex += 1;


            }
            int validRecords = currentIndex;
            int invalidRecords = lines.Length - validRecords;
            if (debugeMode)DisplayDebug($"Processing complete\n" +
                $"Valid records: {validRecords}\r\n" +
                $"Invalid records:{invalidRecords} \r\n" +
                $"Stored {validRecords} valid records for analysis");
            return validRecords;
            

            }

        // validation functions
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

        static bool TryParseType(string typeStr, out ReportType type)
        {
            typeStr = typeStr.Trim();
            if (!Enum.TryParse(typeStr, ignoreCase: true, out type))
            {
                if (debugeMode) DisplayWarning($"Invalid record: Unknown report type - {typeStr}");
                return false;
            }
            return true;
        }

        static bool TryParseStatus(string statusStr, out Status status)
        {
            statusStr = statusStr.Trim();
            if (!Enum.TryParse(statusStr, ignoreCase: true, out status))
            {
                if (debugeMode) DisplayWarning($"Invalid record: Unknown status  - {statusStr}");
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
                    if (debugeMode) DisplayWarning($"priority must be int, got :{priority}");
                    return false;
                }
                if (number > MaxPriority || number < MinPriority)
                {
                    if (debugeMode) DisplayWarning($"priority must be btewwen {MinPriority} to {MaxPriority} but got {number}");
                    return false;
                }
                validPriority = number;
                return true;

            }

            static bool TryParseScore(string priority, out double validPriority)
            {
                validPriority = 0;
                bool valid = Double.TryParse(priority.Trim(), out double number);
                if (!valid)
                {
                    if (debugeMode) DisplayWarning($"score must be double, got :{priority}");
                    return false;
                }
                if (number > MaxScore || number < MinScore)
                {
                    if (debugeMode) DisplayWarning($"score must be btewwen {MinPriority} to {MaxPriority} but got {number}");
                    return false;
                }
                validPriority = number;
                return true;

            }

        // create report functions
        static string CreateReport(string[] units, ReportType[] types, int[] priorities, double[] scores, Status[] statuses)
        {
            string report = "";
            return report;
        }


        // calc statistics functions on score
        static double CalculateAverage(double[] scores, int count)
        {
            double sum = 0.0;
            for (int index = 0; index < count; index++) sum += scores[index];
            return sum / count;

        }

        static double FindMaxScore(double[] scores, int count)
        {
            double max = 0.0;
            for (int index = 0; index < count; index++) if (scores[index]>max) max = scores[index];
            return max;
        }

        static double FindMinScore(double[] scores, int count)
        {
            double min = 100.0;
            for (int index = 0; index < count; index++) if (scores[index] < min) min = scores[index];
            return min;
        }





        // display functions
        static void DisplayWarning(string msg)
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