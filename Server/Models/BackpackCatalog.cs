using SPTarkov.Server.Core.Models.Common;

namespace BetterBackpacks.Models;

public static class BackpackCatalog
{
    public record Entry(string Name, string Tier, GridConfig[] Defaults);

    public static readonly (string Key, string Label)[] Tiers =
    [
        ("Tier 1", "Tier 1 — Easy Access (Trader LL1-2)"),
        ("Tier 2", "Tier 2 — Medium (Trader LL3, Common Flea)"),
        ("Tier 3", "Tier 3 — Hard to Obtain (Quest-locked, LL4, Boss Drops)"),
        ("Tier 4", "Tier 4 — Rare (FIR Only)"),
        ("Special", "Special — Filter Removal Only"),
    ];

    public static readonly Dictionary<string, Entry> All = new()
    {
        // =====================================================================
        // Tier 1 — Easy access (Trader LL1-2), ~20-25% buff
        // =====================================================================
        [ItemTpl.BACKPACK_TACTICAL_SLING_BAG_KHAKI] = new(
            "Tactical Sling Bag (Khaki)",
            "Tier 1",
            [new() { CellsH = 3, CellsV = 3 }]
        ),
        [ItemTpl.BACKPACK_VKBO_ARMY_BAG] = new(
            "VKBO Army Bag",
            "Tier 1",
            [new() { CellsH = 5, CellsV = 2 }]
        ),
        [ItemTpl.BACKPACK_TRANSFORMER_BAG] = new(
            "Transformer Bag",
            "Tier 1",
            [new() { CellsH = 4, CellsV = 3 }]
        ),
        [ItemTpl.BACKPACK_FLYYE_MBSS_BACKPACK_UCP] = new(
            "Flyye MBSS (UCP)",
            "Tier 1",
            [new() { CellsH = 4, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_WARTECH_BERKUT_BB102_BACKPACK_ATACS_FG] = new(
            "Berkut BB-102 (A-TACS FG)",
            "Tier 1",
            [new() { CellsH = 5, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_LBT8005A_DAY_PACK_BACKPACK_MULTICAM_BLACK] = new(
            "LBT-8005A Day Pack (MC Black)",
            "Tier 1",
            [new() { CellsH = 5, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_HAZARD_4_TAKEDOWN_SLING_BACKPACK_MULTICAM] = new(
            "Hazard 4 Takedown (MultiCam)",
            "Tier 1",
            [new() { CellsH = 4, CellsV = 8 }]
        ),
        [ItemTpl.BACKPACK_TEHINKOM_RKPT25_PATROL_BACKPACK_EMR] = new(
            "Tehinkom RK-PT-25 (EMR)",
            "Tier 1",
            [
                new() { CellsH = 1, CellsV = 2 },
                new() { CellsH = 1, CellsV = 2 },
                new() { CellsH = 1, CellsV = 3 },
                new() { CellsH = 1, CellsV = 3 },
                new() { CellsH = 4, CellsV = 2 },
                new() { CellsH = 4, CellsV = 3 },
            ]
        ),
        [ItemTpl.BACKPACK_OAKLEY_MECHANISM_HEAVY_DUTY_BACKPACK_BLACK] = new(
            "Oakley Mechanism (Black)",
            "Tier 1",
            [
                new() { CellsH = 5, CellsV = 5 },
                new() { CellsH = 2, CellsV = 2 },
                new() { CellsH = 2, CellsV = 2 },
                new() { CellsH = 2, CellsV = 2 },
                new() { CellsH = 2, CellsV = 2 },
            ]
        ),

        // =====================================================================
        // Tier 2 — Medium (Trader LL3, common flea), ~20-25% buff
        // =====================================================================
        [ItemTpl.BACKPACK_DUFFLE_BAG] = new(
            "Duffle Bag",
            "Tier 2",
            [new() { CellsH = 5, CellsV = 3 }]
        ),
        [ItemTpl.BACKPACK_LOLKEK_3F_TRANSFER_TOURIST] = new(
            "LolKek 3F Transfer Tourist",
            "Tier 2",
            [new() { CellsH = 4, CellsV = 4 }]
        ),
        [ItemTpl.BACKPACK_VERTX_READY_PACK_BACKPACK_RED] = new(
            "Vertx Ready Pack (Red)",
            "Tier 2",
            [new() { CellsH = 5, CellsV = 4 }]
        ),
        [ItemTpl.BACKPACK_HAZARD_4_PILLBOX_BACKPACK_BLACK] = new(
            "Hazard 4 Pillbox (Black)",
            "Tier 2",
            [new() { CellsH = 5, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_SCAV] = new(
            "Scav Backpack",
            "Tier 2",
            [new() { CellsH = 5, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_HAZARD_4_TAKEDOWN_SLING_BACKPACK_BLACK] = new(
            "Hazard 4 Takedown (Black)",
            "Tier 2",
            [new() { CellsH = 4, CellsV = 8 }]
        ),
        [ItemTpl.BACKPACK_GRUPPA_99_T20_BACKPACK_UMBER_BROWN] = new(
            "Gruppa 99 T20 (Umber Brown)",
            "Tier 2",
            [new() { CellsH = 4, CellsV = 5 }, new() { CellsH = 2, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_GRUPPA_99_T20_BACKPACK_MULTICAM] = new(
            "Gruppa 99 T20 (MultiCam)",
            "Tier 2",
            [new() { CellsH = 4, CellsV = 5 }, new() { CellsH = 2, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_HAZARD_4_DRAWBRIDGE_BACKPACK_COYOTE_TAN] = new(
            "Hazard 4 Drawbridge (Coyote)",
            "Tier 2",
            [new() { CellsH = 6, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_LBT1476A_3DAY_PACK_WOODLAND] = new(
            "LBT-1476A 3Day (Woodland)",
            "Tier 2",
            [new() { CellsH = 6, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_LBT1476A_3DAY_PACK_MULTICAM_ALPINE] = new(
            "LBT-1476A 3Day (Alpine)",
            "Tier 2",
            [new() { CellsH = 6, CellsV = 5 }]
        ),
        [ItemTpl.BACKPACK_EBERLESTOCK_F5_SWITCHBLADE_BACKPACK_DRY_EARTH] = new(
            "F5 Switchblade (Dry Earth)",
            "Tier 2",
            [new() { CellsH = 4, CellsV = 6 }, new() { CellsH = 2, CellsV = 6 }]
        ),
        [ItemTpl.BACKPACK_GRUPPA_99_T30_BACKPACK_BLACK] = new(
            "Gruppa 99 T30 (Black)",
            "Tier 2",
            [new() { CellsH = 6, CellsV = 6 }]
        ),
        [ItemTpl.BACKPACK_GRUPPA_99_T30_BACKPACK_MULTICAM] = new(
            "Gruppa 99 T30 (MultiCam)",
            "Tier 2",
            [new() { CellsH = 6, CellsV = 6 }]
        ),
        [ItemTpl.BACKPACK_CAMELBAK_TRIZIP_ASSAULT_BACKPACK_FOLIAGE] = new(
            "Camelbak Trizip (Foliage)",
            "Tier 2",
            [new() { CellsH = 6, CellsV = 6 }]
        ),
        [ItemTpl.BACKPACK_CAMELBAK_TRIZIP_ASSAULT_BACKPACK_MULTICAM] = new(
            "Camelbak Trizip (MultiCam)",
            "Tier 2",
            [new() { CellsH = 6, CellsV = 6 }]
        ),
        [ItemTpl.BACKPACK_ANA_TACTICAL_BETA_2_BATTLE_BACKPACK_OLIVE_DRAB] = new(
            "ANA Beta 2 (Olive Drab)",
            "Tier 2",
            [new() { CellsH = 6, CellsV = 7 }]
        ),
        [ItemTpl.BACKPACK_DIRECT_ACTION_DRAGON_EGG_MARK_II_BACKPACK_BLACK] = new(
            "DA Dragon Egg Mk II (Black)",
            "Tier 2",
            [
                new() { CellsH = 2, CellsV = 2 },
                new() { CellsH = 3, CellsV = 3 },
                new() { CellsH = 5, CellsV = 4 },
            ]
        ),
        [ItemTpl.BACKPACK_MYSTERY_RANCH_TERRAFRAME_BACKPACK_OLIVE_DRAB] = new(
            "MR Terraframe (Olive Drab)",
            "Tier 2",
            [new() { CellsH = 3, CellsV = 5 }, new() { CellsH = 4, CellsV = 6 }]
        ),
        [ItemTpl.BACKPACK_MYSTERY_RANCH_TERRAFRAME_BACKPACK_CHRISTMAS_EDITION] = new(
            "MR Terraframe (Christmas)",
            "Tier 2",
            [new() { CellsH = 3, CellsV = 5 }, new() { CellsH = 4, CellsV = 6 }]
        ),

        // =====================================================================
        // Tier 3 — Hard to obtain (Quest-locked, LL4, boss drops), ~25-35% buff
        // =====================================================================
        [ItemTpl.BACKPACK_SANITARS_BAG] = new(
            "Sanitar's Bag",
            "Tier 3",
            [
                new() { CellsH = 3, CellsV = 2 },
                new() { CellsH = 3, CellsV = 2 },
                new() { CellsH = 2, CellsV = 2 },
                new() { CellsH = 2, CellsV = 2 },
                new() { CellsH = 2, CellsV = 2 },
            ]
        ),
        [ItemTpl.BACKPACK_PARTISANS_BAG] = new(
            "Partisan's Bag",
            "Tier 3",
            [
                new() { CellsH = 6, CellsV = 2 },
                new() { CellsH = 1, CellsV = 2 },
                new() { CellsH = 1, CellsV = 2 },
                new() { CellsH = 2, CellsV = 2 },
                new() { CellsH = 1, CellsV = 2 },
                new() { CellsH = 1, CellsV = 2 },
            ]
        ),
        [ItemTpl.BACKPACK_3V_GEAR_PARATUS_3DAY_OPERATORS_TACTICAL_BACKPACK_FOLIAGE_GREY] = new(
            "3V Gear Paratus (Foliage Grey)",
            "Tier 3",
            [
                new() { CellsH = 6, CellsV = 5 },
                new() { CellsH = 1, CellsV = 2 },
                new() { CellsH = 1, CellsV = 2 },
                new() { CellsH = 3, CellsV = 3 },
            ]
        ),
        [ItemTpl.BACKPACK_EBERLESTOCK_G2_GUNSLINGER_II_BACKPACK_DRY_EARTH] = new(
            "G2 Gunslinger II (Dry Earth)",
            "Tier 3",
            [
                new() { CellsH = 4, CellsV = 5 },
                new() { CellsH = 2, CellsV = 8 },
                new() { CellsH = 3, CellsV = 3 },
            ]
        ),
        [ItemTpl.BACKPACK_MYSTERY_RANCH_SATL_BRIDGER_ASSAULT_PACK_FOLIAGE] = new(
            "MR SATL Bridger (Foliage)",
            "Tier 3",
            [
                new() { CellsH = 6, CellsV = 3 },
                new() { CellsH = 4, CellsV = 4 },
                new() { CellsH = 4, CellsV = 4 },
            ]
        ),
        [ItemTpl.BACKPACK_EBERLESTOCK_F4_TERMINATOR_LOAD_BEARING_BACKPACK_TIGER_STRIPE] = new(
            "F4 Terminator (Tiger Stripe)",
            "Tier 3",
            [
                new() { CellsH = 6, CellsV = 4 },
                new() { CellsH = 5, CellsV = 3 },
                new() { CellsH = 5, CellsV = 3 },
            ]
        ),

        // =====================================================================
        // Tier 4 — Rare (FIR only), top-end pushed to ~70 cells
        // =====================================================================
        [ItemTpl.BACKPACK_MYSTERY_RANCH_NICE_COMM_3_BVS_FRAME_SYSTEM_COYOTE] = new(
            "NICE COMM 3 BVS (Coyote)",
            "Tier 4",
            [
                new() { CellsH = 2, CellsV = 7, RemoveFilters = true },
                new() { CellsH = 2, CellsV = 7, RemoveFilters = true },
            ]
        ),
        [ItemTpl.BACKPACK_TASMANIAN_TIGER_TROOPER_35_BACKPACK_KHAKI] = new(
            "TT Trooper 35 (Khaki)",
            "Tier 4",
            [new() { CellsH = 6, CellsV = 7 }]
        ),
        [ItemTpl.BACKPACK_SSO_ATTACK_2_RAID_BACKPACK_KHAKI] = new(
            "SSO Attack 2 (Khaki)",
            "Tier 4",
            [new() { CellsH = 6, CellsV = 9 }]
        ),
        [ItemTpl.BACKPACK_SANTAS_BAG] = new(
            "Santa's Bag",
            "Tier 4",
            [new() { CellsH = 6, CellsV = 9 }]
        ),
        [ItemTpl.BACKPACK_PILGRIM_TOURIST] = new(
            "Pilgrim Tourist",
            "Tier 4",
            [new() { CellsH = 6, CellsV = 9 }]
        ),
        [ItemTpl.BACKPACK_MYSTERY_RANCH_BLACKJACK_50_BACKPACK_MULTICAM] = new(
            "Blackjack 50 (MultiCam)",
            "Tier 4",
            [new() { CellsH = 6, CellsV = 10 }]
        ),
        [ItemTpl.BACKPACK_6SH118_RAID_BACKPACK_EMR] = new(
            "6Sh118 Raid (EMR)",
            "Tier 4",
            [new() { CellsH = 6, CellsV = 12 }]
        ),
        [ItemTpl.BACKPACK_511_TACTICAL_RUSH_100_BACKPACK_BLACK] = new(
            "5.11 RUSH 100 (Black)",
            "Tier 4",
            [
                new() { CellsH = 2, CellsV = 2 },
                new() { CellsH = 2, CellsV = 3 },
                new() { CellsH = 2, CellsV = 3 },
                new() { CellsH = 6, CellsV = 9 },
            ]
        ),

        // =====================================================================
        // Special — Filter removal only (no size buff)
        // =====================================================================
        [ItemTpl.BACKPACK_LBT2670_SLIM_FIELD_MED_PACK_BLACK] = new(
            "LBT-2670 Med Pack (Black)",
            "Special",
            [new() { CellsH = 6, CellsV = 8, RemoveFilters = true }]
        ),
    };
}
