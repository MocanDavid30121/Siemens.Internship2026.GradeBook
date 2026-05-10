using System.Text.Json;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public sealed class ExternalGradeRepository : IGradeRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ExternalGradeRepository(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IReadOnlyCollection<Grade>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var endpointUrl = _configuration["GradeData:EndpointUrl"];

        if (string.IsNullOrWhiteSpace(endpointUrl))
        {
            throw new InvalidOperationException("Grade data endpoint URL is not configured.");
        }

        using var response = await _httpClient.GetAsync(endpointUrl, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException("The external endpoint returned an empty response.");
        }

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            return DeserializeGrades(root);
        }

        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var propertyName in new[] { "data", "Data", "grades", "Grades", "items", "Items", "results", "Results" })
            {
                if (root.TryGetProperty(propertyName, out var property) &&
                    property.ValueKind == JsonValueKind.Array)
                {
                    return DeserializeGrades(property);
                }
            }

            foreach (var property in root.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    return DeserializeGrades(property.Value);
                }
            }
        }

        var preview = json.Length > 300 ? json[..300] : json;

        throw new InvalidOperationException(
            $"The external endpoint did not return a supported grade collection. Response starts with: {preview}");
    }

    public async Task<Grade?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var grades = await GetAllAsync(cancellationToken);

        return grades.FirstOrDefault(grade => grade.Id == id);
    }

    private static IReadOnlyCollection<Grade> DeserializeGrades(JsonElement jsonElement)
    {
        var grades = JsonSerializer.Deserialize<List<Grade>>(
            jsonElement.GetRawText(),
            JsonOptions);

        return grades ?? [];
    }
}