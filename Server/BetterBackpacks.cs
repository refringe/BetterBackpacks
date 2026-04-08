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

    public void ApplyConfig()
    {
        if (!configService.Config.Enabled)
        {
            logger.Info("[BetterBackpacks] Mod is disabled via configuration.");
            return;
        }

        var items = databaseService.GetItems();
        var count = 0;

        foreach (var (id, item) in items)
        {
            if (!itemHelper.IsOfBaseclass(id, BaseClasses.BACKPACK))
            {
                continue;
            }

            if (
                !configService.Config.Backpacks.TryGetValue(
                    id.ToString(),
                    out var backpackConfig
                )
            )
            {
                continue;
            }

            var gridConfigs = backpackConfig.Grids;

            if (gridConfigs.Count == 0)
            {
                continue;
            }

            if (item.Properties?.Grids == null || !item.Properties.Grids.Any())
            {
                logger.Warning($"[BetterBackpacks] {item.Name}: no grids found. Skipping.");
                continue;
            }

            var grids = item.Properties.Grids.ToList();

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

            count++;
        }

        logger.Success($"[BetterBackpacks] Modified {count} backpacks.");
    }
}
