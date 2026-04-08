using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace BetterBackpacks.Patches
{
    /// <summary>
    /// Forces backpacks to use <see cref="GeneratedGridsView"/> instead of static
    /// <see cref="TemplatedGridsView"/> layouts. Templated layouts bake grid positions into
    /// Unity prefabs, which means they cannot adapt when grid dimensions are modified at
    /// runtime. By intercepting <see cref="ContainedGridsView.CreateGrids"/> for backpacks
    /// that have a <see cref="GridLayoutComponent"/>, we force instantiation of the dynamic
    /// template so that grid views are created to match actual grid data.
    /// </summary>
    internal class CreateGridsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(
                typeof(ContainedGridsView),
                nameof(ContainedGridsView.CreateGrids),
                new[] { typeof(Item), typeof(ContainedGridsView) }
            );
        }

        /// <summary>
        /// Intercepts grid creation for backpacks that would normally load a static rig
        /// layout prefab. Instead, instantiates the dynamic <see cref="GeneratedGridsView"/>
        /// template which creates one <see cref="GridView"/> per grid at runtime.
        /// </summary>
        [PatchPrefix]
        private static bool Prefix(
            Item item,
            ContainedGridsView containedGridsTemplate,
            ref ContainedGridsView __result
        )
        {
            // Only backpacks; everything else is out of scope for this mod.
            if (item is not BackpackItemClass)
            {
                return true;
            }

            // If the backpack has no static layout component, it already uses the dynamic
            // template. Let the original method handle it.
            var layoutComponent = item.GetItemComponent<GridLayoutComponent>();
            if (layoutComponent == null)
            {
                return true;
            }

            // Instantiate the dynamic template, bypassing the static rig layout prefab.
            __result = Object.Instantiate(containedGridsTemplate);
            return false;
        }
    }
}
