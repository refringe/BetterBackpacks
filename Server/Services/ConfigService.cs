using System.Reflection;
using BetterBackpacks.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;

namespace BetterBackpacks.Services;

[Injectable(InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader + 1)]
public class ConfigService(ISptLogger<ConfigService> logger, ModHelper modHelper) : IOnLoad
{
    public ModConfig Config { get; private set; } = new();

    public Task OnLoad()
    {
        ReloadFromDisk();
        return Task.CompletedTask;
    }

    public void ReloadFromDisk()
    {
        try
        {
            var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
            var configPath = Path.Combine(modPath, "config.json");

            if (!File.Exists(configPath))
            {
                logger.Warning("[BetterBackpacks] No config.json found, using defaults.");
                Config = ModConfig.CreateDefault();
                return;
            }

            var json = File.ReadAllText(configPath);
            Config = ModConfig.FromJson(json);
            logger.Info("[BetterBackpacks] Loaded config.json.");
        }
        catch (Exception ex)
        {
            logger.Error($"[BetterBackpacks] Failed to load config: {ex.Message}");
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
            var json = Config.ToJson();
            await File.WriteAllTextAsync(Path.Combine(modPath, "config.json"), json);
            logger.Success("[BetterBackpacks] Configuration saved via web UI.");
        }
        catch (Exception ex)
        {
            logger.Error($"[BetterBackpacks] Failed to save config: {ex.Message}");
            throw;
        }
    }
}
