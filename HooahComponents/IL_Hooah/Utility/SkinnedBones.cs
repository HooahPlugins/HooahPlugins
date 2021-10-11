#if AI || HS2
using System.Collections.Generic;
using AIChara;
using UnityEngine;
#endif

namespace HooahComponents.Utility
{
    public static class SkinnedBones
    {
#if AI || HS2
        private static readonly Dictionary<ChaControl, Dictionary<string, Transform>> TransformCache =
            new Dictionary<ChaControl, Dictionary<string, Transform>>();

        // Skip unnecessary skinned bones to make traverse more efficient.
        // Containment O(1)
        private static readonly HashSet<string> Blacklist = new HashSet<string>
        {
            // "cf_J_Root", 
            "cf_t_root", "N_Neck", "N_Chest_f", "N_Chest", "N_Tikubi_L",
            "N_Tikubi_R", "N_Back", "N_Back_L", "N_Back_R", "N_Waist", "N_Waist_f", "N_Waist_b",
            "N_Waist_L", "N_Waist_R", "N_Leg_L", "N_Leg_R", "N_Knee_L", "N_Knee_R", "N_Ankle_L",
            "N_Ankle_R", "N_Foot_L", "N_Foot_R", "N_Shoulder_L", "N_Shoulder_R", "N_Elbo_L", "N_Elbo_R",
            "N_Arm_L", "N_Arm_R", "N_Wrist_L", "N_Wrist_R", "N_Hand_L", "N_Hand_R", "N_Index_L",
            "N_Index_R", "N_Middle_L", "N_Middle_R", "N_Ring_L", "N_Ring_R", "N_Dan",
            "N_Kokan", "N_Ana", "N_Hair_pony", "N_Hair_twin_L", "N_Hair_twin_R", "N_Hair_pin_L", "N_Hair_pin_R",
            "N_Head_top", "N_Head", "N_Hitai", "N_Face", "N_Megane",
            "N_Earring_L", "N_Earring_R", "N_Nose", "N_Mouth"
        };

        public static bool TryGetSkinnedBones(ChaControl chaControl, out Dictionary<string, Transform> result)
        {
            if (chaControl == null || ReferenceEquals(chaControl.objBodyBone, null))
            {
                result = null;
                return false;
            }

            if (TransformCache.TryGetValue(chaControl, out var dict))
            {
                result = dict;
                return true;
            }

            dict = new Dictionary<string, Transform>();
            GetBonesRecursive(dict, chaControl.objBodyBone.transform);

            if (dict.Count <= 0)
            {
                result = null;
                return false;
            }

            TransformCache[chaControl] = dict;
            result = TransformCache[chaControl];
            return true;
        }


        // reject cf_J_Roots to prevent duplicated bone collections
        private static void GetBonesRecursive(IDictionary<string, Transform> dict, Transform targetTransform)
        {
            dict[targetTransform.name] = targetTransform;

            for (var i = 0; i < targetTransform.childCount; i++)
            {
                var childTransform = targetTransform.GetChild(i);
                if (!Blacklist.Contains(childTransform.name)) GetBonesRecursive(dict, childTransform);
            }
        }

        public static void CleanUpCache(ChaControl chaControl)
        {
            // reserved for unknown cleanup task.
            if (TransformCache.ContainsKey(chaControl)) TransformCache.Remove(chaControl);
        }
#endif
    }
}