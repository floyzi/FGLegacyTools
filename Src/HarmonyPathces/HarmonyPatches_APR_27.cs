#if APR_27
using FG.Common.CMS;
using HarmonyLib;
using Mediatonic.Tools.ParsingUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGLegacyTools.HarmonyPathces
{
    public static partial class HarmonyPatches
    {
        [HarmonyPatch(typeof(CMSLoader), nameof(CMSLoader.Fallback)), HarmonyPrefix]
        static bool Fallback(CMSLoader __instance, string error)
        {
            return false;
        }


        [HarmonyPatch(typeof(Parsing), nameof(Parsing.ParseDictionaries)), HarmonyPrefix]
        static bool ParseDictionaries(Il2CppSystem.Object obj, Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object> data, CMSReferenceCache referenceCache)
        {
            FLZ_CMSParser.ParseBase(obj.Cast<FallGuysCMSData>(), data);
            return false;
        }
    }
}
#endif