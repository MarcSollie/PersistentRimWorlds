using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;

namespace PersistentWorlds.Patches.UI
{
    /// <summary>
    /// This patch gets rid of "Starting Season" if New Colony is selected under the Persistent RimWorlds menu.
    /// </summary>
    [HarmonyPatch(typeof(Dialog_AdvancedGameConfig), "DoWindowContents")]
    public class Dialog_AdvancedGameConfig_DoWindowContents_Patch
    {
        #region Fields
        private static readonly MethodInfo DisposeMethod = AccessTools.Method(typeof(IDisposable), "Dispose");

        private static readonly MethodInfo
            NotNullMethod = AccessTools.Method(typeof(PersistentWorldManager), "NotNull");
        #endregion
        
        #region Methods
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr, ILGenerator ilGenerator)
        {
            var codes = new List<CodeInstruction>(instr);

            var label1 = ilGenerator.DefineLabel();

            codes[201].labels.Add(label1);
            
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode != OpCodes.Endfinally || codes[i - 1].opcode != OpCodes.Callvirt ||
                    codes[i - 1].operand != DisposeMethod) continue;

                var codesToInsert = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Call,
                        NotNullMethod),
                    new CodeInstruction(OpCodes.Brtrue_S, label1)
                };

                codes.InsertRange(i + 3, codesToInsert);
                
                break;
            }
            
            return codes.AsEnumerable();
        }
        #endregion
    }
}