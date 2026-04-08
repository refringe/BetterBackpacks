using System.Linq;
using BetterBackpacks.Models;
using BetterBackpacks.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Web;

namespace BetterBackpacks;

public record ModMetadata : AbstractModMetadata, IModWebMetadata
{
    public override string ModGuid { get; init; } = "com.refringe.betterbackpacks";
    public override string Name { get; init; } = "BetterBackpacks";
    public override string Author { get; init; } = "Refringe";
    public override List<string>? Contributors { get; init; } = ["Josh Mate"];
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");

    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class BetterBackpacksPlugin(
    ISptLogger<BetterBackpacksPlugin> logger,
    DatabaseService databaseService,
    ItemHelper itemHelper,
    ConfigService configService,
    HttpServerHelper httpServerHelper
) : IOnLoad
{
    public Task OnLoad()
    {
        ApplyConfig();

        var backendUrl = httpServerHelper.GetBackendUrl();
        logger.Info($"[BetterBackpacks] Web config: {backendUrl}/better-backpacks");

        return Task.CompletedTask;
    }

    private const int MaxGridWidth = 6;

    public void ApplyConfig()
    {
        var items = databaseService.GetItems();
        var configuredCount = 0;
        var dynamicCount = 0;
        var config = configService.Config;
        var increasePercent = config.CustomIncreasePercent;
        var debug = config.Debug;

        foreach (var (id, item) in items)
        {
            if (!itemHelper.IsOfBaseclass(id, BaseClasses.BACKPACK))
            {
                continue;
            }

            if (item.Properties?.Grids == null || !item.Properties.Grids.Any())
            {
                continue;
            }

            var templateId = id.ToString();

            if (config.Backpacks.TryGetValue(templateId, out var backpackConfig))
            {
                if (ApplyConfiguredBackpack(id, item, backpackConfig))
                {
                    if (debug)
                    {
                        var grids = string.Join(", ", backpackConfig.Grids.Select(g => $"{g.CellsH}x{g.CellsV}"));
                        logger.Info($"[BetterBackpacks] {item.Name} ({templateId}): configured [{grids}]");
                    }
                    configuredCount++;
                }
            }
            else if (increasePercent > 0)
            {
                var before = debug
                    ? item.Properties.Grids.Select(g => $"{g.Properties?.CellsH}x{g.Properties?.CellsV}").ToList()
                    : null;

                if (ApplyDynamicIncrease(item, increasePercent))
                {
                    if (debug)
                    {
                        var after = string.Join(
                            ", ",
                            item.Properties.Grids.Select(g => $"{g.Properties?.CellsH}x{g.Properties?.CellsV}")
                        );
                        logger.Info(
                            $"[BetterBackpacks] {item.Name} ({templateId}): dynamic +{increasePercent}% [{string.Join(", ", before!)}] -> [{after}]"
                        );
                    }
                    dynamicCount++;
                }
            }
        }

        logger.Success($"[BetterBackpacks] Modified {configuredCount} configured + {dynamicCount} dynamic backpacks.");
    }

    private bool ApplyConfiguredBackpack(object id, TemplateItem item, BackpackConfig backpackConfig)
    {
        var gridConfigs = backpackConfig.Grids;

        if (gridConfigs.Count == 0)
        {
            return false;
        }

        var grids = item.Properties!.Grids!.ToList();

        // Add grids when config specifies more than vanilla (grid-split bags).
        if (grids.Count < gridConfigs.Count)
        {
            var template = grids[0];
            while (grids.Count < gridConfigs.Count)
            {
                grids.Add(
                    new Grid
                    {
                        Name = $"grid_{grids.Count}",
                        Id = $"{id}_grid_{grids.Count}",
                        Parent = template.Parent,
                        Prototype = template.Prototype,
                        Properties = new GridProperties
                        {
                            CellsH = 1,
                            CellsV = 1,
                            Filters = [],
                            MinCount = template.Properties?.MinCount,
                            MaxCount = template.Properties?.MaxCount,
                            MaxWeight = template.Properties?.MaxWeight,
                            IsSortingTable = template.Properties?.IsSortingTable,
                        },
                    }
                );
            }

            item.Properties.Grids = grids;
        }
        else if (grids.Count > gridConfigs.Count)
        {
            grids = grids.Take(gridConfigs.Count).ToList();
            item.Properties.Grids = grids;
        }

        for (var i = 0; i < grids.Count; i++)
        {
            var props = grids[i].Properties;
            if (props == null)
            {
                continue;
            }

            props.CellsH = gridConfigs[i].CellsH;
            props.CellsV = gridConfigs[i].CellsV;

            if (gridConfigs[i].RemoveFilters)
            {
                props.Filters = [];
            }
        }

        return true;
    }

    private static bool ApplyDynamicIncrease(TemplateItem item, int increasePercent)
    {
        var modified = false;

        foreach (var grid in item.Properties!.Grids!)
        {
            var props = grid.Properties;
            if (props?.CellsH == null || props.CellsV == null)
            {
                continue;
            }

            var (newH, newV) = CalculateIncreasedGrid(props.CellsH.Value, props.CellsV.Value, increasePercent);

            if (newH != props.CellsH || newV != props.CellsV)
            {
                props.CellsH = newH;
                props.CellsV = newV;
                modified = true;
            }
        }

        return modified;
    }

    /// <summary>
    /// Calculates new grid dimensions that increase total cells by at least the given
    /// percentage while keeping the width at or below <see cref="MaxGridWidth"/>.
    /// Picks the smallest total that meets the target across all valid widths.
    /// </summary>
    private static (int CellsH, int CellsV) CalculateIncreasedGrid(int cellsH, int cellsV, int increasePercent)
    {
        var currentCells = cellsH * cellsV;
        var targetCells = (int)Math.Ceiling(currentCells * (1.0 + increasePercent / 100.0));

        var bestW = cellsH;
        var bestV = cellsV;
        var bestTotal = int.MaxValue;

        for (var w = 1; w <= MaxGridWidth; w++)
        {
            var h = (int)Math.Ceiling((double)targetCells / w);
            var total = w * h;

            if (total >= targetCells && total < bestTotal)
            {
                bestW = w;
                bestV = h;
                bestTotal = total;
            }
        }

        return (bestW, bestV);
    }
}
