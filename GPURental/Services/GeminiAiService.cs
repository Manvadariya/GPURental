using GenerativeAI;
using GPURental.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPURental.Services
{
    public class PriceSuggestionResponse { public int suggested_price_cents { get; set; } }
    public class GpuSuggestionResponse { public List<string> suggested_gpus { get; set; } }

    public class GeminiAiService : IAiService
    {
        private readonly GenerativeModel _model;

        public GeminiAiService(IConfiguration configuration)
        {
            var apiKey = configuration["GeminiApiKey"];
            var googleAI = new GoogleAi(apiKey);
            _model = googleAI.CreateGenerativeModel("gemini-2.5-flash");
        }

        public async Task<int> SuggestPriceAsync(string gpuModel, int vram, List<GpuListing> existingListings)
        {
            // --- NEW, STRICTER SYSTEM PROMPT ---
            var systemPrompt = @"You are an automated pricing analyst for a GPU rental marketplace. Your response MUST be a single JSON object and nothing else.
            
            Your rules are:
            1. Analyze the GPU Model and VRAM of the user's GPU.
            2. Compare it to the provided list of 'CURRENT MARKET CONTEXT' GPUs.
            3. Suggest a competitive hourly price in cents that is attractive to renters, typically slightly below the average for similar models.
            4. If the market context is empty or irrelevant, make a reasonable estimate based on the GPU model.
            5. Your entire output MUST be a JSON object with a single key: 'suggested_price_cents'.

            Example JSON response:
            {
              ""suggested_price_cents"": 45
            }";
            // ------------------------------------
            
            var contextPrices = existingListings.Any()
                ? string.Join(", ", existingListings.Select(l => $"{l.GpuModel} with {l.VramInGB}GB VRAM is ${l.PricePerHourInCents / 100.0m:F2}"))
                : "No other listings are currently available.";

            var userPrompt = $"GPU TO PRICE: {gpuModel} with {vram}GB VRAM.\nCURRENT MARKET CONTEXT: {contextPrices}.";

            var fullPrompt = systemPrompt + "\n" + userPrompt;
            Console.WriteLine(fullPrompt);
            try
            {
                var result = await _model.GenerateContentAsync(fullPrompt);
                var responseText = result.Text.Trim().Replace("```json", "").Replace("```", "");
                var parsedJson = JsonSerializer.Deserialize<PriceSuggestionResponse>(responseText);
                var i = parsedJson?.suggested_price_cents ?? 0;
                Console.WriteLine($"Parsed Suggested Price: {i} cents");
                return i;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Gemini API Error (Price): {ex.Message}");
                return 0;
            }
        }

        public async Task<List<string>> GetGpuSuggestionsAsync(string userTask, List<GpuListing> availableListings)
        {
            // --- NEW, STRICTER SYSTEM PROMPT ---
            var systemPrompt = @"You are an expert AI/ML hardware analyst for a GPU rental marketplace. Your response MUST be a single JSON object and nothing else.

            Your rules are:
            1. Analyze the USER TASK to estimate the minimum required VRAM and compute power.
            2. Compare these requirements against the list of AVAILABLE GPUs.
            3. Return a JSON array containing the IDs of only the GPUs that are a good fit.
            4. **CRITICAL RULE: If no GPUs in the provided list are a suitable match for the task, you MUST return an empty array `[]`. Do not suggest unsuitable GPUs.**
            5. Your entire output MUST be a JSON object with a single key: 'suggested_gpus'.

            Example response for a successful match:
            {
              ""suggested_gpus"": [""some-guid-id-1"", ""some-guid-id-3""]
            }
            Example response for NO match:
            {
              ""suggested_gpus"": []
            }";
            // ------------------------------------

            var contextListings = string.Join("\n", availableListings.Select(l => $"ID: {l.ListingId}, Specs: {l.GpuModel}, VRAM: {l.VramInGB}GB, RAM: {l.RamInGB}GB").ToList());
            var userPrompt = $"USER TASK: '{userTask}'.\nAVAILABLE GPUs:\n{contextListings}";

            var fullPrompt = systemPrompt + "\n" + userPrompt;

            Console.WriteLine("Full Prompt to Gemini:");
            Console.WriteLine(fullPrompt);

            try
            {
                var result = await _model.GenerateContentAsync(fullPrompt);
                var responseText = result.Text.Trim().Replace("```json", "").Replace("```", "");
                var parsedJson = JsonSerializer.Deserialize<GpuSuggestionResponse>(responseText);
                var m = parsedJson?.suggested_gpus ?? new List<string>();
                Console.WriteLine("Parsed Suggested GPU IDs:");
                foreach (var name in m)
                {
                    Console.WriteLine(name + "--");
                }
                return m;
            }
            catch { return new List<string>(); }
        }
    }
}