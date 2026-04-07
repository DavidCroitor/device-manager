using Application.Interfaces;
using Domain.Models;
using GroqApiLibrary;
using System.Text.Json.Nodes;

namespace Infrastructure.AI;

public class AiDeviceDescriptorGenerator : IDeviceDescriptionGenerator
{
    private readonly IGroqApiClient _groqApiClient;
    public AiDeviceDescriptorGenerator(IGroqApiClient groqApiClient)
    {
        _groqApiClient = groqApiClient as GroqApiClient ?? throw new ArgumentException("Invalid GroqApiClient instance.");
    }
    public async Task<string> GenerateDescriptionAsync(Device device)
    {
        string prompt = $"Your task is to write a human-readable, easy-to-understand description for the following device based on its technical specifications:\n" +
                                $"- Name: {device.Name}\n" +
                                $"- Manufacturer: {device.Manufacturer}\n" +
                                $"- Type: {device.Type}\n" +
                                $"- OS: {device.OS}\n" +
                                $"- Processor: {device.Processor}\n" +
                                $"- RAM: {device.RamGB}\n\n" +
                                $"Please keep the description to maximum 2-3 senteces long, suitable for a catalog. Output only the description";

        JsonObject request = new()
        {
            ["model"] = "llama-3.1-8b-instant",
            ["temperature"] = 0.3,
            ["max_tokens"] = 100,
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"]="system",
                    ["content"]="You are an expert technical writer."
                },
                new JsonObject
                {
                    ["role"]="user",
                    ["content"]= prompt
                }
            }
        };

        try
        {
            var response = await _groqApiClient.CreateChatCompletionAsync(request);
            var description = response?["choices"]?[0]?["message"]?["content"]?.ToString();

            return description?.Trim() ?? "Description could not be generated";
        }
        catch(Exception ex)
        {
            return $"An error occurred while generating the description: {ex.Message}";
        }
    
    }
}
