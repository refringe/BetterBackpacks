using System.Linq;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace BetterBackpacks;

public record ModMetadata : AbstractModMetadata
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
public class BetterBackpacks(ISptLogger<BetterBackpacks> logger, DatabaseService databaseService, ItemHelper itemHelper)
    : IOnLoad
{
    private record GridConfig(int CellsH, int CellsV, bool RemoveFilters = false);

    // Backpack grid configurations keyed by item template ID.
    // Each entry specifies dimensions for every grid (in order) on that backpack.
    // When configs specify more grids than vanilla, additional grids are created from a template.
    private static readonly Dictionary<MongoId, GridConfig[]> Backpacks = new()
    {
        // =====================================================================
        // Tier 1 — Easy access (Trader LL1-2), ~20-25% buff
        // =====================================================================

        // Tactical Sling Bag (Khaki) — Ragman LL1, 6,660 RUB
        // default 3x2 (6) - updated 3x3 (9), +50%
        [ItemTpl.BACKPACK_TACTICAL_SLING_BAG_KHAKI] = [new(3, 3)],

        // VKBO Army Bag — Jaeger LL1, 8,400 RUB
        // default 4x2 (8) - updated 5x2 (10), +25%
        [ItemTpl.BACKPACK_VKBO_ARMY_BAG] = [new(5, 2)],

        // Transformer Bag — Ragman LL1, 11,489 RUB
        // default 3x3 (9) - updated 4x3 (12), +33%
        [ItemTpl.BACKPACK_TRANSFORMER_BAG] = [new(4, 3)],

        // Flyye MBSS (UCP) — Ragman LL2, 20,336 RUB
        // default 4x4 (16) - updated 4x5 (20), +25%
        [ItemTpl.BACKPACK_FLYYE_MBSS_BACKPACK_UCP] = [new(4, 5)],

        // Berkut BB-102 (A-TACS FG) — Ragman LL2, 24,509 RUB
        // default 4x5 (20) - updated 5x5 (25), +25%
        [ItemTpl.BACKPACK_WARTECH_BERKUT_BB102_BACKPACK_ATACS_FG] = [new(5, 5)],

        // LBT-8005A Day Pack (MC Black) — PK LL2, ~27k RUB
        // default 4x5 (20) - updated 5x5 (25), +25%
        [ItemTpl.BACKPACK_LBT8005A_DAY_PACK_BACKPACK_MULTICAM_BLACK] = [new(5, 5)],

        // Hazard 4 Takedown (MultiCam) — Ragman LL2 (quest: "Blood of War Part 1")
        // default 3x8 (24) - updated 4x8 (32), +33%
        [ItemTpl.BACKPACK_HAZARD_4_TAKEDOWN_SLING_BACKPACK_MULTICAM] = [new(4, 8)],

        // Tehinkom RK-PT-25 (EMR) — Ragman LL2, 30,360 RUB — 6 grids
        // default 1x2,1x2,1x3,1x3,3x2,3x3 (25) - updated 1x2,1x2,1x3,1x3,4x2,4x3 (30), +20%
        [ItemTpl.BACKPACK_TEHINKOM_RKPT25_PATROL_BACKPACK_EMR] =
        [
            new(1, 2),
            new(1, 2),
            new(1, 3),
            new(1, 3),
            new(4, 2),
            new(4, 3),
        ],

        // Oakley Mechanism (Black) — Ragman LL2, 122,629 RUB — 5 grids
        // default 4x4,2x2,2x2,2x2,2x2 (32) - updated 5x5,2x2,2x2,2x2,2x2 (41), +28%
        [ItemTpl.BACKPACK_OAKLEY_MECHANISM_HEAVY_DUTY_BACKPACK_BLACK] =
        [
            new(5, 5),
            new(2, 2),
            new(2, 2),
            new(2, 2),
            new(2, 2),
        ],

        // =====================================================================
        // Tier 2 — Medium (Trader LL3, common flea), ~20-25% buff
        // =====================================================================

        // Duffle Bag — Flea only, ~30,671 RUB
        // default 4x3 (12) - updated 5x3 (15), +25%
        [ItemTpl.BACKPACK_DUFFLE_BAG] = [new(5, 3)],

        // LolKek 3F Transfer Tourist — Flea only, ~41,549 RUB
        // default 3x4 (12) - updated 4x4 (16), +33%
        [ItemTpl.BACKPACK_LOLKEK_3F_TRANSFER_TOURIST] = [new(4, 4)],

        // Vertx Ready Pack (Red) — Flea only, ~21,927 RUB
        // default 4x4 (16) - updated 5x4 (20), +25%
        [ItemTpl.BACKPACK_VERTX_READY_PACK_BACKPACK_RED] = [new(5, 4)],

        // Hazard 4 Pillbox (Black) — Flea only, ~22,444 RUB
        // default 4x5 (20) - updated 5x5 (25), +25%
        [ItemTpl.BACKPACK_HAZARD_4_PILLBOX_BACKPACK_BLACK] = [new(5, 5)],

        // Scav Backpack — Flea only, ~24,176 RUB
        // default 4x5 (20) - updated 5x5 (25), +25%
        [ItemTpl.BACKPACK_SCAV] = [new(5, 5)],

        // Hazard 4 Takedown (Black) — Ragman LL3, 30,094 RUB
        // default 3x8 (24) - updated 4x8 (32), +33%
        [ItemTpl.BACKPACK_HAZARD_4_TAKEDOWN_SLING_BACKPACK_BLACK] = [new(4, 8)],

        // Gruppa 99 T20 (Umber Brown) — Ragman LL3, 32,646 RUB — SPLIT into 2 grids
        // default 5x5 (25) - updated 4x5+2x5 (30), +20%
        [ItemTpl.BACKPACK_GRUPPA_99_T20_BACKPACK_UMBER_BROWN] = [new(4, 5), new(2, 5)],

        // Gruppa 99 T20 (MultiCam) — Ragman LL3, 32,646 RUB — SPLIT into 2 grids
        // default 5x5 (25) - updated 4x5+2x5 (30), +20%
        [ItemTpl.BACKPACK_GRUPPA_99_T20_BACKPACK_MULTICAM] = [new(4, 5), new(2, 5)],

        // Hazard 4 Drawbridge (Coyote) — Flea only, ~34,839 RUB
        // default 5x5 (25) - updated 6x5 (30), +20%
        [ItemTpl.BACKPACK_HAZARD_4_DRAWBRIDGE_BACKPACK_COYOTE_TAN] = [new(6, 5)],

        // LBT-1476A 3Day (Woodland) — Flea only, ~39,860 RUB
        // default 5x5 (25) - updated 6x5 (30), +20%
        [ItemTpl.BACKPACK_LBT1476A_3DAY_PACK_WOODLAND] = [new(6, 5)],

        // LBT-1476A 3Day (Alpine) — Flea only, ~26,821 RUB
        // default 5x5 (25) - updated 6x5 (30), +20%
        [ItemTpl.BACKPACK_LBT1476A_3DAY_PACK_MULTICAM_ALPINE] = [new(6, 5)],

        // F5 Switchblade (Dry Earth) — Flea only, ~71,526 RUB — SPLIT into 2 grids
        // default 5x6 (30) - updated 4x6+2x6 (36), +20%
        [ItemTpl.BACKPACK_EBERLESTOCK_F5_SWITCHBLADE_BACKPACK_DRY_EARTH] = [new(4, 6), new(2, 6)],

        // Gruppa 99 T30 (Black) — Flea only, ~98,049 RUB
        // default 5x6 (30) - updated 6x6 (36), +20%
        [ItemTpl.BACKPACK_GRUPPA_99_T30_BACKPACK_BLACK] = [new(6, 6)],

        // Gruppa 99 T30 (MultiCam) — Flea only, ~117,522 RUB
        // default 5x6 (30) - updated 6x6 (36), +20%
        [ItemTpl.BACKPACK_GRUPPA_99_T30_BACKPACK_MULTICAM] = [new(6, 6)],

        // Camelbak Trizip (Foliage) — Ragman LL3 (quest: "Inventory Check"), 122,528 RUB
        // default 5x6 (30) - updated 6x6 (36), +20%
        [ItemTpl.BACKPACK_CAMELBAK_TRIZIP_ASSAULT_BACKPACK_FOLIAGE] = [new(6, 6)],

        // Camelbak Trizip (MultiCam) — Ragman LL3 (quest: "Inventory Check"), 61,264 RUB
        // default 5x6 (30) - updated 6x6 (36), +20%
        [ItemTpl.BACKPACK_CAMELBAK_TRIZIP_ASSAULT_BACKPACK_MULTICAM] = [new(6, 6)],

        // ANA Beta 2 (Olive Drab) — Ragman LL3, 136,816 RUB (very expensive)
        // default 5x6 (30) - updated 6x7 (42), +40%
        [ItemTpl.BACKPACK_ANA_TACTICAL_BETA_2_BATTLE_BACKPACK_OLIVE_DRAB] = [new(6, 7)],

        // DA Dragon Egg Mk II (Black) — Ragman LL3, 33,772 RUB — 3 grids
        // default 2x2,3x2,5x3 (25) - updated 2x2,3x3,5x4 (33), +32%
        [ItemTpl.BACKPACK_DIRECT_ACTION_DRAGON_EGG_MARK_II_BACKPACK_BLACK] = [new(2, 2), new(3, 3), new(5, 4)],

        // MR Terraframe (Olive Drab) — Flea only, ~37,273 RUB — 2 grids
        // default 2x5,4x5 (30) - updated 3x5,4x6 (39), +30%
        [ItemTpl.BACKPACK_MYSTERY_RANCH_TERRAFRAME_BACKPACK_OLIVE_DRAB] = [new(3, 5), new(4, 6)],

        // MR Terraframe (Christmas) — Flea only, ~95,725 RUB — 2 grids
        // default 2x5,4x5 (30) - updated 3x5,4x6 (39), +30%
        [ItemTpl.BACKPACK_MYSTERY_RANCH_TERRAFRAME_BACKPACK_CHRISTMAS_EDITION] = [new(3, 5), new(4, 6)],

        // =====================================================================
        // Tier 3 — Hard to obtain (Quest-locked, LL4, boss drops), ~25-35% buff
        // =====================================================================

        // Sanitar's Bag — Flea only, ~39,598 RUB (boss drop: Sanitar) — SPLIT into 5 grids
        // default 4x4 (16) - updated 3x2,3x2,2x2,2x2,2x2 (24), +50%
        [ItemTpl.BACKPACK_SANITARS_BAG] = [new(3, 2), new(3, 2), new(2, 2), new(2, 2), new(2, 2)],

        // Partisan's Bag — Flea only, ~116,764 RUB (boss bag) — SPLIT into 6 grids
        // default 5x4 (20) - updated 6x2,1x2,1x2,2x2,1x2,1x2 (24), +20%
        [ItemTpl.BACKPACK_PARTISANS_BAG] = [new(6, 2), new(1, 2), new(1, 2), new(2, 2), new(1, 2), new(1, 2)],

        // 3V Gear Paratus (Foliage Grey) — PK LL4, ~120k RUB — 4 grids
        // default 5x5,1x2,1x2,3x2 (35) - updated 6x5,1x2,1x2,3x3 (43), +23%
        [ItemTpl.BACKPACK_3V_GEAR_PARATUS_3DAY_OPERATORS_TACTICAL_BACKPACK_FOLIAGE_GREY] =
        [
            new(6, 5),
            new(1, 2),
            new(1, 2),
            new(3, 3),
        ],

        // G2 Gunslinger II (Dry Earth) — Ragman LL4, 135,420 RUB — 3 grids
        // default 3x5,2x7,3x2 (35) - updated 4x5,2x8,3x3 (45), +29%
        [ItemTpl.BACKPACK_EBERLESTOCK_G2_GUNSLINGER_II_BACKPACK_DRY_EARTH] = [new(4, 5), new(2, 8), new(3, 3)],

        // MR SATL Bridger (Foliage) — Ragman LL4, 159,972 RUB — 3 grids
        // default 6x2,3x4,3x4 (36) - updated 6x3,4x4,4x4 (50), +39%
        [ItemTpl.BACKPACK_MYSTERY_RANCH_SATL_BRIDGER_ASSAULT_PACK_FOLIAGE] = [new(6, 3), new(4, 4), new(4, 4)],

        // F4 Terminator (Tiger Stripe) — Ragman LL4, 180,780 RUB — 3 grids
        // default 5x4,5x2,5x2 (40) - updated 6x4,5x3,5x3 (54), +35%
        [ItemTpl.BACKPACK_EBERLESTOCK_F4_TERMINATOR_LOAD_BEARING_BACKPACK_TIGER_STRIPE] =
        [
            new(6, 4),
            new(5, 3),
            new(5, 3),
        ],

        // =====================================================================
        // Tier 4 — Rare (FIR only), top-end pushed to ~70 cells
        // =====================================================================

        // NICE COMM 3 BVS (Coyote) — Flea only, ~115,118 RUB (boss bag) — SPLIT into 2 grids
        // default 2x7 (14) - updated 2x7+2x7 (28), +100%, filters removed
        [ItemTpl.BACKPACK_MYSTERY_RANCH_NICE_COMM_3_BVS_FRAME_SYSTEM_COYOTE] =
        [
            new(2, 7, RemoveFilters: true),
            new(2, 7, RemoveFilters: true),
        ],

        // TT Trooper 35 (Khaki) — FIR only, ~51,314 RUB (common FIR)
        // default 5x7 (35) - updated 6x7 (42), +20%
        [ItemTpl.BACKPACK_TASMANIAN_TIGER_TROOPER_35_BACKPACK_KHAKI] = [new(6, 7)],

        // SSO Attack 2 (Khaki) — FIR only, ~54,219 RUB
        // default 5x7 (35) - updated 6x9 (54), +54%
        [ItemTpl.BACKPACK_SSO_ATTACK_2_RAID_BACKPACK_KHAKI] = [new(6, 9)],

        // Santa's Bag — FIR only, ~38,453 RUB (boss/event drop)
        // default 5x7 (35) - updated 6x9 (54), +54%
        [ItemTpl.BACKPACK_SANTAS_BAG] = [new(6, 9)],

        // Pilgrim Tourist — FIR only, ~48,979 RUB
        // default 5x7 (35) - updated 6x9 (54), +54%
        [ItemTpl.BACKPACK_PILGRIM_TOURIST] = [new(6, 9)],

        // Blackjack 50 (MultiCam) — FIR only, ~78,554 RUB
        // default 6x7 (42) - updated 6x10 (60), +43%
        [ItemTpl.BACKPACK_MYSTERY_RANCH_BLACKJACK_50_BACKPACK_MULTICAM] = [new(6, 10)],

        // 6Sh118 Raid (EMR) — FIR only, ~89,280 RUB (king)
        // default 6x8 (48) - updated 6x12 (72), +50%
        [ItemTpl.BACKPACK_6SH118_RAID_BACKPACK_EMR] = [new(6, 12)],

        // 5.11 RUSH 100 (Black) — FIR only, ~75,886 RUB — 4 grids
        // default 1x2,2x2,2x2,5x7 (45) - updated 2x2,2x3,2x3,6x9 (70), +56%
        [ItemTpl.BACKPACK_511_TACTICAL_RUSH_100_BACKPACK_BLACK] = [new(2, 2), new(2, 3), new(2, 3), new(6, 9)],

        // =====================================================================
        // Special — Filter removal only (no size buff)
        // =====================================================================

        // LBT-2670 Med Pack (Black) — Flea only, ~39,323 RUB
        // default 6x8 (48) - updated 6x8 (48), +0%, filters removed
        [ItemTpl.BACKPACK_LBT2670_SLIM_FIELD_MED_PACK_BLACK] = [new(6, 8, RemoveFilters: true)],
    };

    public Task OnLoad()
    {
        var items = databaseService.GetItems();
        var count = 0;

        foreach (var (id, item) in items)
        {
            if (!itemHelper.IsOfBaseclass(id, BaseClasses.BACKPACK))
            {
                continue;
            }

            if (!Backpacks.TryGetValue(id, out var gridConfigs))
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
            if (grids.Count < gridConfigs.Length)
            {
                var template = grids[0];
                while (grids.Count < gridConfigs.Length)
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
            else if (grids.Count > gridConfigs.Length)
            {
                logger.Warning(
                    $"[BetterBackpacks] {item.Name}: expected {gridConfigs.Length} grids, found {grids.Count}. Skipping."
                );
                continue;
            }

            for (var i = 0; i < grids.Count; i++)
            {
                if (grids[i].Properties == null)
                {
                    continue;
                }

                grids[i].Properties.CellsH = gridConfigs[i].CellsH;
                grids[i].Properties.CellsV = gridConfigs[i].CellsV;

                if (gridConfigs[i].RemoveFilters)
                {
                    grids[i].Properties.Filters = [];
                }
            }

            count++;
        }

        logger.Success($"[BetterBackpacks] Modified {count} backpacks.");

        return Task.CompletedTask;
    }
}
