using System.Text.Json;

namespace BetterBackpacks.Models;

public class ModConfig
{
    public bool Debug { get; set; }
    public int UnconfiguredIncreasePercent { get; set; } = 25;
    public Dictionary<string, BackpackConfig> Backpacks { get; set; } = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static ModConfig FromJson(string json)
    {
        return JsonSerializer.Deserialize<ModConfig>(json, JsonOptions) ?? CreateDefault();
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, JsonOptions);
    }

    public static ModConfig CreateDefault()
    {
        var config = new ModConfig();
        foreach (var (id, entry) in BackpackCatalog.All)
        {
            config.Backpacks[id] = new BackpackConfig
            {
                Grids = entry
                    .Defaults.Select(d => new GridConfig
                    {
                        CellsH = d.CellsH,
                        CellsV = d.CellsV,
                        RemoveFilters = d.RemoveFilters,
                    })
                    .ToList(),
            };
        }
        return config;
    }
}

public class BackpackConfig
{
    public List<GridConfig> Grids { get; set; } = [];
}

public class GridConfig
{
    public int CellsH { get; set; }
    public int CellsV { get; set; }
    public bool RemoveFilters { get; set; }
}
