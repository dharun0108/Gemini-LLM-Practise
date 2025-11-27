using Google.GenAI;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

class Program
{
    static readonly HttpClient http = new HttpClient();
    static readonly int maxRetries = 3; 
    static readonly TimeSpan retryDelay = TimeSpan.FromSeconds(2); 

    static async Task Main()
    {
        var apiKey = "AIzaSyDHtUYZH0i86Cetc3iHCsT_2vFy8vun0Cc";
        Environment.SetEnvironmentVariable("GOOGLE_API_KEY", apiKey);

        var client = new Client();

        while (true)
        {
            Console.WriteLine("Enter a prompt for the AI (or type 'exit' to quit):");
            string userInput = Console.ReadLine();

            if (userInput.ToLower() == "exit")
            {
                break;
            }

            bool success = false;
            int attempt = 0;

            while (attempt < maxRetries && !success)
            {
                try
                {
                    var response = await client.Models.GenerateContentAsync(
                        model: "gemini-2.5-flash", contents: userInput
                    );

                    if (response.Candidates != null && response.Candidates.Count > 0)
                    {
                        Console.WriteLine("AI Response: " + response.Candidates[0].Content.Parts[0].Text);
                        success = true; 
                    }
                    else
                    {
                        Console.WriteLine("No response received from the AI model.");
                        break; 
                    }
                }
                catch (Google.GenAI.ServerError)
                {
                    attempt++;
                    Console.WriteLine($"Server error. Attempt {attempt} of {maxRetries}. Retrying in {retryDelay.TotalSeconds} seconds...");
                    await Task.Delay(retryDelay); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    break; 
                }
            }

            if (!success)
            {
                Console.WriteLine("Failed to retrieve response after multiple attempts.");
            }
        }

        Console.WriteLine("Exiting program...");
    }
}
