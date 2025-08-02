#if !APR_27
using FG.Common;
using FGClient;
using HarmonyLib;
using MPG.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThatOneRandom3AMProject.HarmonyPathces
{
    public static partial class HarmonyPatches
    {
        [HarmonyPatch(typeof(SubMenuNavigation), nameof(SubMenuNavigation.HandleConfigureRequestFailed))]
        [HarmonyPrefix]
        static bool HandleConfigureRequestFailed(SubMenuNavigation __instance, CustomisationSelections previousSelections, bool isEmotes)
        {
            return false;
        }

        [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.ReportEndOfRoundMetrics))]
        [HarmonyPrefix]
        static bool ReportEndOfRoundMetrics(SubMenuNavigation __instance, ClientGameSession.ClientGameSessionCompleteEvent e)
        {
            return false;
        }
    }
}
#endif