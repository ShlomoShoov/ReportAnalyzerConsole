using System;


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
        const int MinPriority = 1;

        const double MinScore = 0.0;
        const double MaxScore = 100.0;

        const string defaultPath = @"reports.txt"; 
        static bool debugeMode = true; // if true: prints all data about proccing process.
        const string ErrorMsg = "=== Error ===\n";
        const string WarningMsg = "=== Warning ===\n";

        
        static void Main(string[] args)
        {

            string reportPath;
            string? outPath;

            bool validArgs = ConfigSystem(args, out reportPath, out outPath);
            if (!validArgs) return;

            string[]? data = LoadFile(path: reportPath);
            if (data is null) return;

            RunApp(lines: data, outPath);
        }


        // App flow function
        static void RunApp(string[] lines, string? outPath)
            {
            string[] units = new string[ArrayLimit];
            ReportType[] types = new ReportType[ArrayLimit];
            int[] priorities = new int[ArrayLimit];
            double[] scores = new double[ArrayLimit];
            Status[] statuses = new Status[ArrayLimit];

            int countLines = ProcessReports(lines: lines, units: units, priorities:priorities ,types:types, scores:scores,statuses:statuses);
            if (countLines == 0) {DisplayError($"{ErrorMsg}No valid lines to procces. "); return;};

            string report = CreateAndDiplayReport(units, types, priorities, scores, statuses, countLines);
            if (outPath != null) SaveToFile(report, outPath);


            }

        // Loading and Valid the data
        static string[]? LoadFile(string path)


            {
                string fileName = Path.GetFileName(path);

                if (!ValidPath(path)) return null; // check if the path (if given) exists.

                if (!File.Exists(path: path))
                {

                    DisplayError($"Error: File {fileName} not found");
                    return null;
                }

                string[] data = File.ReadAllLines(path: path);

                if (data.Length == 0)
                {
                    DisplayError($"Error: {fileName} File is empty.");
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
            int invalidLines = 0;

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
                if (currentIndex == ArrayLimit)
                {
                    DisplayError($"{ErrorMsg}File has more validable lines then the Memory Limit\n" +
                        $"Array limit : {ArrayLimit} \n" +
                        $"| notice that we enter and calaulate all valid lines until line {index+1}");
                    break;
                }

                // valid line length
                if (!ValidLineLength(line))
                {
                    if (debugeMode) DisplayWarning($"{WarningMsg}unproccesable line: {index+1} not in a valid format, not enter into procces.");
                    invalidLines++;
                    continue;
                }
                

                // validetions and parses

                if (!TryParsePriority(line[PriorityIndex], out priority)) validLine = false;
                if (!TryParseScore(line[ScoreIndex], out score)) validLine = false;
                if (!TryParseStatus(line[StatusIndex], out status)) validLine = false;
                if (!TryParseType(line[ReportTypeIndex], out type)) validLine = false;
                if (!TryParseUnit(line[UnityIndex], out unit)) validLine = false ;

                if (!validLine)
                {
                    if (debugeMode) DisplayWarning($"{WarningMsg}line {index + 1} invalid elemets. not enter to procces");
                    invalidLines++;
                    continue;
                }


                // put data in the arrays
                units[currentIndex] = unit;
                priorities[currentIndex] = priority;
                statuses[currentIndex] = status;
                scores[currentIndex] = score;
                types[currentIndex] = type;

                currentIndex++;


            }
            int validRecords = currentIndex;
            int invalidRecords = invalidLines;
            if (debugeMode)DisplayDebug($"Processing complete\n" +
                $"Valid records: {currentIndex}\r\n" +
                $"Invalid records:{invalidRecords} \r\n" +
                $"Stored {currentIndex} valid records for analysis");
            return validRecords;
            

            }

        // validation functions
        static bool ValidLineLength(string[] line)
        {
            if (line.Length != ReportLenghth) {
                if (debugeMode) DisplayWarning($"{WarningMsg}line contain {line.Length} columns , valid columns: {ReportLenghth} ");
                return false;
            }
            return true;
        }

        static bool TryParseUnit(string givenUnit, out string unit)
        {
            unit = givenUnit.Trim();
            if (string.IsNullOrWhiteSpace(unit))
            {
                if (debugeMode) DisplayWarning($"{WarningMsg}Unit must be a string but got NOTHING.");
                return false;
            }
            return true;
        }

        static bool TryParseType(string typeStr, out ReportType type)
        {
            typeStr = typeStr.Trim();
            if (!Enum.TryParse(typeStr, ignoreCase: true, out type) || ! Enum.IsDefined(type)) 
            {
                if (debugeMode) DisplayWarning($"Invalid record: Unknown report type - {typeStr}");
                return false;
            }
            
            return true;
        }

        static bool TryParseStatus(string statusStr, out Status status)
        {
            statusStr = statusStr.Trim();
            if (!Enum.TryParse(statusStr, ignoreCase: true, out status) || !Enum.IsDefined(status))
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

            static bool TryParseScore(string strScore, out double validScore)
            {
                validScore = 0;
                bool valid = Double.TryParse(strScore.Trim(), out double number);
                if (!valid)
                {
                    if (debugeMode) DisplayWarning($"score must be double, got :{strScore}");
                    return false;
                }
                if (number > MaxScore || number < MinScore)
                {
                    if (debugeMode) DisplayWarning($"score must be btewwen {MinScore} to {MaxScore} but got {number}");
                    return false;
                }
                validScore = number;
                return true;

            }

        // create report functions
        static string CreateAndDiplayReport(string[] units, ReportType[] types, int[] priorities, double[] scores, Status[] statuses, int actualLength)
        {
            string report = "";

            report += DisplayBasicStatistics(scores, actualLength);
            report += DisplayStatusCounts(statuses, actualLength);
            report += DisplayTypeCounts(types, actualLength);
            report += DisplayHighestPriorityApproved(units, types, priorities, scores, statuses, actualLength);
            report += DisplayAverageByPriority(priorities, scores, actualLength);

            return report;
        }


        // calc statistics functions on score
        static double CalculateAverage(double[] scores, int count)
        {
            if (count == 0) return 0.0;

            double sum = 0.0;
            for (int index = 0; index < count; index++) sum += scores[index];
            return sum / count;

        }

        static double FindMaxScore(double[] scores, int count)
        {
            double max = scores[0];
            for (int index = 0; index < count; index++) if (scores[index]>max) max = scores[index];
            return max;
        }

        static double FindMinScore(double[] scores, int count)
        {
            double min = scores[0]; 
            for (int index = 0; index < count; index++) if (scores[index] < min) min = scores[index];
            return min;
        }

        static int CountByStatus(Status[] statuses, Status targetStatus, int count)
        {
            int cnt = 0;
            for (int index = 0; index<count; index++)
                if (statuses[index] == targetStatus) cnt += 1;
            return cnt;
        }

        static int CountByType(ReportType[] types, ReportType targetType, int count)
        {
            int cnt = 0;
            for (int index = 0; index<count; index++)
                if (types[index] == targetType) cnt += 1;
            return cnt;
        }

        // Repoets functions

        static string DisplayBasicStatistics(double[] scores, int actualLength)
        {
            Double averageScore = CalculateAverage(scores, actualLength);
            Double maxScore = FindMaxScore(scores, actualLength);
            Double minScore = FindMinScore(scores, actualLength);
            string basicStatistics = $"=== Report Statistics ===\r\n" +
                                        $"Total Reports: {actualLength}\r\n" +
                                        $"Average Score: {averageScore.ToString("F2")}\r\n" +
                                        $"Highest Score: {maxScore}\r\n" +
                                        $"Lowest Score: {minScore}\r\n";
            Console.WriteLine(basicStatistics);
            return basicStatistics;
         }

        static string DisplayStatusCounts(Status[] statuses, int actualLength)
        {
            int pendigCnt = CountByStatus(statuses, Status.Pending, actualLength);
            int approvedCnt = CountByStatus(statuses, Status.Approved, actualLength);
            int rejectedCnt = CountByStatus(statuses, Status.Rejected, actualLength);

            string statusCount = $"=== Reports by Status ===\r\n" +
                $"Pending: {pendigCnt}\r\n" +
                $"Approved: {approvedCnt}\r\n" +
                $"Rejected: {rejectedCnt}\r\n";
            Console.WriteLine(statusCount);
            return statusCount;
        }

        static string DisplayTypeCounts(ReportType[] types, int actualLength)
        {
            int collectCnt = CountByType(types, ReportType.Collect, actualLength);
            int analzeCnt = CountByType(types, ReportType.Analyze, actualLength);
            int reconCnt = CountByType(types, ReportType.Recon, actualLength);
            int intelCnt = CountByType(types, ReportType.Intel, actualLength);

            string typeCounst = $"=== Reports by Type ===\r\n" +
                                $"Collect: {collectCnt}\r\n" +
                                $"Analyze: {analzeCnt}\r\n" +
                                $"Recon: {reconCnt}\r\n" +
                                $"Intel: {intelCnt}\r\n";
            Console.WriteLine(typeCounst);
            return typeCounst;

        }

        // HighestPriorityApproved
        static string DisplayHighestPriorityApproved(string[] units, ReportType[] types, int[] priorities, double[] scores, Status[] statuses,int actualLength)
        {
            int indexProirty = GetIndexHighestPriorityApproved(statuses, priorities, actualLength);
            string data;
            if (indexProirty == -1) data = "There is no Approved Statuses in YOUR data.\r\n";
            else data = GetFormatedLine(units, types, priorities, scores, statuses, indexProirty);

            string highestPriorityApproved = $"=== Highest Priority Approved Report ===\r\n" +
                                             $"{data}";
            Console.WriteLine(highestPriorityApproved);
            return highestPriorityApproved;
        }

        static int GetIndexHighestPriorityApproved(Status[] statuses, int[] priorities, int actualLength)
        {
            int highestPrioirty = 0;
            int? indexProirty = null;
            for (int index = 0; index<actualLength; index++)
            {
                if (statuses[index] == Status.Approved &&
                    priorities[index] > highestPrioirty) 
                {
                    highestPrioirty = priorities[index];
                    indexProirty = index;
                }
            }

            if (indexProirty == null)
            {
                return -1;
            }

            return (int)indexProirty;
        }

        static string GetFormatedLine(string[] units, ReportType[] types, int[] priorities, double[] scores, Status[] statuses, int index)
        {
            string line = $"Unit: {units[index]}\r\n" +
                            $"Type: {types[index]}\r\n" +
                            $"Priority: {priorities[index]}\r\n" +
                            $"Score: {scores[index]}\r\n";
            return line;
            
        }

        // === Average Score by Priority ===

        static string DisplayAverageByPriority(int[] priorities, double[] scores, int actualLentgh)
        {
            string averageByPriority = $"=== Average Score by Priority ===\r\n";
            for (int priority = 1; priority<=MaxPriority; priority++)
            {
                
                averageByPriority += $"Priority {priority}:";

                double? priorityAverage = GetAverageByPriority(priorities, scores, priority, actualLentgh);
                if (priorityAverage is null) averageByPriority += "No reports\r\n";
                else
                {
                    double doublePriorityAverage = (double)priorityAverage;
                    averageByPriority += doublePriorityAverage.ToString("F2") + "\r\n";
                }

            }
            Console.WriteLine(averageByPriority);
            return averageByPriority;
        }

        static double? GetAverageByPriority(int[] priorities, double[] scores, int priority, int actualLentgh)
        {
            double sum = 0.0;
            int cnt = 0;

            for (int index = 0; index < actualLentgh; index++)
            {
                
                if (priorities[index] == priority) 
                {
                    sum += scores[index];
                    cnt++;
                }
            }
            if (cnt == 0) return null;
            return sum / cnt;
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


        // (Extra) process flags and config system 

        static bool ConfigSystem(string[] userConfig, out string path, out string? outputPath)
        {
            bool cofigertionCompleted = true; 
            path = defaultPath;
            outputPath = null;
            for (int index = 0; index<userConfig.Length; index++ )
            {
                string flag = userConfig[index].Trim();
                
                switch (flag)
                {
                    case "-f":
                        if (index + 1 < userConfig.Length)
                        {
                            path = userConfig[index + 1].Trim();

                            index++;
                        }
                        else
                        {
                            DisplayError($"{ErrorMsg}flag '-f' was given but no file loction provieded after");
                            cofigertionCompleted = false;
                        }
                        break;

                    case "--debug":
                        debugeMode = false;
                        break;

                    case "-o":
                        if (index + 1 < userConfig.Length)
                        {

                            string givenPath = userConfig[index + 1].Trim();
                            if (!ValidPath (givenPath))
                            {
                                
                                cofigertionCompleted = false;
                                
                            }
                            else outputPath = givenPath;
                            index++;
                        }
                        else
                        {
                            DisplayError($"{ErrorMsg}flag '-o' was given but no file loction provieded after");
                            cofigertionCompleted = false;
                        }
                        break;

                    default:
                        DisplayError($"{ErrorMsg}Unknown flag was given: {flag}");
                        cofigertionCompleted = false;
                        break;
                }

            }
            if (!cofigertionCompleted) DisplayError($"To see the valid option for flags look up in the readme.md file");
            return cofigertionCompleted;
        }

        static bool ValidPath(string path)
        {
            
            
            string? dictPath = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            
            
            if (!dictPath.IsWhiteSpace() && !Path.Exists(dictPath))
            {
                DisplayError($"{ErrorMsg} Path to file not exist: {dictPath}");
                return false;
            }
           
            return true;
        }

        static void SaveToFile(string data, string filePath)
        {
            File.WriteAllText(path: filePath, contents: data);
            
        }

    }
}