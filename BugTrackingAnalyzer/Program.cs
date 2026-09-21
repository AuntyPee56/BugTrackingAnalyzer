using System;
using System.Collections.Generic;


// ======================================================
// OPTION 1 - BUG SUMMARY
// ======================================================

// Class stores the results of the bug analysis
class BugSummary
{
    // Stores the total number of bugs processed
    public int TotalBugs { get; set; }

    // Stores the number of critical bugs
    public int CriticalCount { get; set; }

    // Indicates whether immediate action is required
    public bool NeedsImmediateAction { get; set; }
}


// ======================================================
// OPTION 2 - LOG TRAFFIC ANALYSIS
// ======================================================

// Class stores the results of the log traffic analysis
class LogTrafficSummary
{
    // Stores the total number of valid log entries
    public int TotalValidLogs { get; set; }

    // Stores each endpoint and the number of times it was accessed
    // Endpoint comparison is case-insensitive
    public Dictionary<string, int> EndpointHitCounts { get; set; }
        = new Dictionary<string, int>(
            StringComparer.OrdinalIgnoreCase);

    // Stores the total number of server errors
    public int TotalServerErrors { get; set; }

    // Stores the endpoint with the highest number of requests
    // empty string  used as the default value
    public string MostVisitedEndpoint { get; set; } = "";
}



// MAIN PROGRAM
class Program
{
    // ==================================================
    // OPTION 1 METHOD
    // ==================================================

    // Analyzes bug severity levels and returns a summary
    static BugSummary SummarizeBugs(List<string> severities)
    {
        // Stores the total number of bugs
        int totalBugs = severities.Count;

        // Starting the critical bug count at zero
        int criticalCount = 0;

        // Iterate through each severity level
        foreach (string severity in severities)
        {
            // Checks whether the severity is "critical",
            // ignoring differences in capitalization
            if (string.Equals(
                severity,
                "critical",
                StringComparison.OrdinalIgnoreCase))
            {
                // Increase the critical bug count
                criticalCount++;
            }
        }

        // Immediate action is required if there are
        // two or more critical bugs
        bool needsImmediateAction = criticalCount >= 2;

        // Return the completed bug summary
        return new BugSummary
        {
            TotalBugs = totalBugs,
            CriticalCount = criticalCount,
            NeedsImmediateAction = needsImmediateAction
        };
    }


    // ==================================================
    // OPTION 2 METHOD
    // ==================================================

    // Analyzes log entries and returns a traffic summary
    static LogTrafficSummary AnalyzeLogTraffic(List<string> logs)
    {
        // Stores the number of valid log entries
        int totalValidLogs = 0;

        // Stores the number of 5xx server errors
        int totalServerErrors = 0;

        // Stores each endpoint and its request count.
        // Endpoint comparison is case-insensitive.
        Dictionary<string, int> endpointHitCounts =
            new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase);

        // Processes each log entry in the list
        foreach (string log in logs)
        {
            // Splits the log using the pipe character
            string[] parts = log.Split('|');

            // If the log does not contain exactly four parts,
            // skip without causing the program to crash
            if (parts.Length != 4)
            {
                continue;
            }

            // Removes unnecessary whitespace from each field
            string timestamp = parts[0].Trim();
            string method = parts[1].Trim();
            string endpoint = parts[2].Trim();
            string statusCodeText = parts[3].Trim();

            // Checks that the required fields contain values
            if (string.IsNullOrWhiteSpace(timestamp) ||
                string.IsNullOrWhiteSpace(method) ||
                string.IsNullOrWhiteSpace(endpoint))
            {
                continue;
            }

            // Trying to convert the status code into an integer.
            // If conversion fails, skips the log.
            if (!int.TryParse(
                statusCodeText,
                out int statusCode))
            {
                continue;
            }

            // If log passed all validation checks,
            //  is considered a valid log
            totalValidLogs++;

            // Checks whether the endpoint already exists
            if (endpointHitCounts.ContainsKey(endpoint))
            {
                // Increases the endpoint request count
                endpointHitCounts[endpoint]++;
            }
            else
            {
                // Adds the endpoint with an initial count of one
                endpointHitCounts[endpoint] = 1;
            }

            // Checks whether the status code is in the 5xx range
            if (statusCode >= 500 && statusCode <= 599)
            {
                // Increases the server error count
                totalServerErrors++;
            }
        }

        // Stores the endpoint with the highest number of hits
        string mostVisitedEndpoint = "";

        // Stores the highest number of hits found
        int highestHitCount = 0;

        // Iterates through each endpoint to find the most visited one
        foreach (var entry in endpointHitCounts)
        {
            // Checks whether this endpoint has more hits
            // than the current highest count
            if (entry.Value > highestHitCount)
            {
                highestHitCount = entry.Value;
                mostVisitedEndpoint = entry.Key;
            }
        }

        // Returns the completed log traffic summary
        return new LogTrafficSummary
        {
            TotalValidLogs = totalValidLogs,
            EndpointHitCounts = endpointHitCounts,
            TotalServerErrors = totalServerErrors,
            MostVisitedEndpoint = mostVisitedEndpoint
        };
    }


    // ==================================================
    // OPTION 1
    // ==================================================

    // Displays the results for Option 1
    static void RunOption1()
    {
        Console.Clear();

        Console.WriteLine("=================================");
        Console.WriteLine("       OPTION 1 - BUG SUMMARY");
        Console.WriteLine("=================================");
        Console.WriteLine();

        // Creating the sample bug list
        var bugs = new List<string>
        {
            "low",
            "CRITICAL",
            "medium",
            "Critical",
            "High"
        };

        // Analyzes the bugs using the SummarizeBugs method
        BugSummary summary = SummarizeBugs(bugs);

        // Displays the results
        Console.WriteLine($"TotalBugs: {summary.TotalBugs}");
        Console.WriteLine($"CriticalCount: {summary.CriticalCount}");
        Console.WriteLine(
            $"NeedsImmediateAction: {summary.NeedsImmediateAction}");

        Console.WriteLine();
        Console.WriteLine("Press ENTER to return to the menu...");
        Console.ReadLine();
    }


    // ==================================================
    // OPTION 2
    // ==================================================

    // Displays the results for Option 2
    static void RunOption2()
    {
        Console.Clear();

        Console.WriteLine("=================================");
        Console.WriteLine("   OPTION 2 - LOG TRAFFIC ANALYSIS");
        Console.WriteLine("=================================");
        Console.WriteLine();

        // Creating the sample log list
        var logs = new List<string>
        {
            "2026-09-17 09:00:00 | GET | /api/orders | 200",
            "INVALID_LOG_LINE_MISSING_PIPES",
            "2026-09-17 09:01:00 | POST | /api/orders | NOT_A_NUMBER",
            "2026-09-17 09:02:00 | GET | /API/ORDERS | 500",
            "2026-09-17 09:03:00 | GET | /api/health | 200"
        };

        // Analyze the logs using the AnalyzeLogTraffic method
        LogTrafficSummary summary = AnalyzeLogTraffic(logs);

        // Displays the total number of valid logs
        Console.WriteLine(
            $"Total Valid Logs: {summary.TotalValidLogs}");

        Console.WriteLine();

        // Displays the endpoint hit counts
        Console.WriteLine("Endpoint Hit Counts:");

        foreach (var entry in summary.EndpointHitCounts)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }

        Console.WriteLine();

        // Displays the total number of server errors
        Console.WriteLine(
            $"Total Server Errors (5xx): " +
            $"{summary.TotalServerErrors}");

        // Displays the most visited endpoint
        Console.WriteLine(
            $"Most Visited Endpoint: " +
            $"{summary.MostVisitedEndpoint}");

        Console.WriteLine();
        Console.WriteLine("Press ENTER to return to the menu...");
        Console.ReadLine();
    }


    
    // MAIN MENU
    
    static void Main(string[] args)
    {
        // Variable used to store the user's menu selection
        string choice;

        // Keeps displaying the menu until the user chooses Exit
        do
        {
            Console.Clear();

            Console.WriteLine("=================================");
            Console.WriteLine("       BUG TRACKING ANALYZER");
            Console.WriteLine("=================================");
            Console.WriteLine();
            Console.WriteLine("1. Option 1 - Bug Summary");
            Console.WriteLine("2. Option 2 - Log Traffic Analysis");
            Console.WriteLine("3. Exit");
            Console.WriteLine();

            Console.Write("Select an option: ");

           
            choice = Console.ReadLine() ?? "";

            // Runs the appropriate option based on
            // user's menu selection
            switch (choice)
            {
                case "1":
                    RunOption1();
                    break;

                case "2":
                    RunOption2();
                    break;

                case "3":
                    Console.WriteLine();
                    Console.WriteLine("Exiting program...");
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine(
                        "Invalid option. Please choose 1, 2, or 3.");

                    Console.WriteLine();
                    Console.WriteLine(
                        "Press ENTER to continue...");
                    Console.ReadLine();
                    break;
            }

        } while (choice != "3");
    }
}