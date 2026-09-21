using System;
using System.Collections.Generic;

// Class used to store the log traffic analysis results
class LogTrafficSummary
{
    // Stores the total number of valid log entries
    public int TotalValidLogs { get; set; }

    // Stores each endpoint and its total number of requests
    public Dictionary<string, int> EndpointHitCounts { get; set; }

    // Stores the total number of server errors
    public int TotalServerErrors { get; set; }

    // Stores the endpoint with the highest number of requests
    public string MostVisitedEndpoint { get; set; }
}

class Program
{
    // Analyzes a list of log entries and returns a traffic summary
    static LogTrafficSummary AnalyzeLogTraffic(List<string> logs)
    {
        // Store the number of valid log entries
        int totalValidLogs = 0;

        // Store the number of server errors with status codes from 500 to 599
        int totalServerErrors = 0;

        // Store each endpoint and its number of requests.
        // Ignore differences in capitalization when comparing endpoints.
        Dictionary<string, int> endpointHitCounts =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Process each log entry
        foreach (string log in logs)
        {
            // Split the log entry using the pipe character
            string[] parts = log.Split('|');

            // Skip malformed logs that do not contain exactly four parts
            if (parts.Length != 4)
            {
                continue;
            }

            // Remove extra spaces from the log components
            string method = parts[1].Trim();
            string endpoint = parts[2].Trim();
            string statusCodeText = parts[3].Trim();

            // Try to convert the status code to an integer.
            // Skip the log if the conversion fails.
            if (!int.TryParse(statusCodeText, out int statusCode))
            {
                continue;
            }

            // Increase the valid log count
            totalValidLogs++;

            // Check if the endpoint already exists
            if (endpointHitCounts.ContainsKey(endpoint))
            {
                // Increase the request count for the endpoint
                endpointHitCounts[endpoint]++;
            }
            else
            {
                // Add the endpoint with a request count of 1
                endpointHitCounts[endpoint] = 1;
            }

            // Check whether the status code is a 5xx server error
            if (statusCode >= 500 && statusCode <= 599)
            {
                // Increase the server error count
                totalServerErrors++;
            }
        }

        // Store the most visited endpoint
        string mostVisitedEndpoint = "";

        // Store the highest number of endpoint hits
        int highestHitCount = 0;

        // Search for the endpoint with the highest number of requests
        foreach (var entry in endpointHitCounts)
        {
            if (entry.Value > highestHitCount)
            {
                highestHitCount = entry.Value;
                mostVisitedEndpoint = entry.Key;
            }
        }

        // Return the completed analysis
        return new LogTrafficSummary
        {
            TotalValidLogs = totalValidLogs,
            EndpointHitCounts = endpointHitCounts,
            TotalServerErrors = totalServerErrors,
            MostVisitedEndpoint = mostVisitedEndpoint
        };
    }

    static void Main(string[] args)
    {
        // Create a list containing the sample log entries
        var logs = new List<string>
        {
            "2026-09-17 09:00:00 | GET | /api/orders | 200",
            "INVALID_LOG_LINE_MISSING_PIPES",
            "2026-09-17 09:01:00 | POST | /api/orders | NOT_A_NUMBER",
            "2026-09-17 09:02:00 | GET | /API/ORDERS | 500",
            "2026-09-17 09:03:00 | GET | /api/health | 200"
        };

        // Analyze the logs and store the returned summary
        LogTrafficSummary summary = AnalyzeLogTraffic(logs);

        // Display the total number of valid logs
        Console.WriteLine($"Total Valid Logs: {summary.TotalValidLogs}");

        // Display the endpoint hit counts
        Console.WriteLine("Endpoint Hit Counts:");

        foreach (var entry in summary.EndpointHitCounts)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }

        // Display the total number of server errors
        Console.WriteLine(
            $"Total Server Errors (5xx): {summary.TotalServerErrors}"
        );

        // Display the most visited endpoint
        Console.WriteLine(
            $"Most Visited Endpoint: {summary.MostVisitedEndpoint}"
        );
    }
}