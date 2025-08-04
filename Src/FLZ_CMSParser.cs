#if APR_27
#define CUSTOM_PARSER
#endif

#if CUSTOM_PARSER
using FG.Common.CMS;
using Il2CppInterop.Runtime;
using Il2CppSystem.Reflection;
using Mediatonic.Tools.ParsingUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Cinemachine.CinemachineCore;
using static FG.Common.CMS.RoundPool;
using BindingFlags = Il2CppSystem.Reflection.BindingFlags;
using Stage = FG.Common.CMS.RoundPool.Stage;

namespace FGLegacyTools
{
    internal static class FLZ_CMSParser
    {
        internal static void ParseBase(FallGuysCMSData CMSData, Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object> cms)
        {
            if (cms == null) throw new ArgumentNullException(nameof(cms));

            var rounds = cms["levels_round"].Cast<Il2CppSystem.Collections.Generic.List<Il2CppSystem.Object>>();
            var typeRound = Il2CppType.Of<Round>();
            var res = new RoundsData();

            foreach (var roundObj in rounds)
            {
                var roundContent = roundObj.TryCast<Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object>>();
                var inst = new Round();
                var id = roundContent["id"].ToString();

                foreach (var prop in typeRound.GetProperties((BindingFlags.Public | BindingFlags.Instance)))
                {
                    var cmsField = prop.GetCustomAttribute<CMSField>();
                    if (cmsField == null) continue;

                 
                    if (roundContent.TryGetValue(cmsField.Name, out var jsonRes) && jsonRes != null)
                    {
                        switch (prop.PropertyType.Name)
                        {
                            case "Int32":
                                prop.SetValue(inst, (Il2CppSystem.Object)Parsing.ParseInt(jsonRes));
                                break;
                            case "String":
                                prop.SetValue(inst, (Il2CppSystem.Object)Parsing.ParseString(jsonRes));
                                break;
                            case "Boolean":
                                prop.SetValue(inst, (Il2CppSystem.Object)Parsing.ParseBool(jsonRes));
                                break;
                        }
                    }
                }

                if (!res.ContainsKey(id))
                    res.Add(id, inst);
            }

            CMSData.Rounds = res;
        }
    }
}
#endif