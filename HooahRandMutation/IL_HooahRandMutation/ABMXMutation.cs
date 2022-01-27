using System;
using System.Collections.Generic;
using System.Linq;
using AIChara;
using KKABMX.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HooahRandMutation
{
    public static class ABMXMutation
    {

        public static readonly HashSet<string> BadDragons = new HashSet<string>()
        {
            "cf_J_Chin_rs", "cf_J_FaceBase", "cf_J_FaceRoot", "cf_J_FaceLowBase", "cf_J_FaceLow_s", "cf_J_NoseBase_trs",
            "cf_J_MouthBase_tr", "cf_J_FaceUp_ty","cf_J_FaceUp_tz",
            "cf_J_CheekLow_L", "cf_J_CheekUp_L", "cf_J_CheekUp_R",
            "cf_J_Chin_rs", "cf_J_ChinTip_s", "cf_J_ChinLow", "cf_J_MouthBase_tr"
        };

        public static readonly HashSet<string> HeadBoneNames = new HashSet<string>()
        {
            "p_cf_head_bone", "cf_J_FaceRoot", "cf_J_FaceBase", "cf_J_FaceLowBase", "cf_J_FaceLow_s", "cf_J_CheekLow_L",
            "cf_J_CheekUp_L", "cf_J_CheekUp_R", "cf_J_Chin_rs", "cf_J_ChinTip_s", "cf_J_ChinLow", "cf_J_MouthBase_tr",
            "N_Mouth", "cf_J_MouthMove", "cf_J_Mouth_L", "cf_J_Mouth_R", "cf_J_MouthLow", "cf_J_Mouthup",
            "cf_J_MouthBase_s", "cf_J_CheekLow_R", "cf_J_MouthCavity", "cf_J_FaceUp_ty",
            "cf_J_EarBase_s_L", "cf_J_EarLow_L", "cf_J_EarRing_L", "N_Earring_L",
            "cf_J_EarUp_L", "cf_J_EarBase_s_R", "cf_J_EarLow_R", "cf_J_EarRing_R",
            "N_Earring_R", "cf_J_EarUp_R", "cf_J_FaceUp_tz", "cf_J_Eye_t_L",
            "cf_J_Eye_s_L", "cf_J_Eye_r_L", "cf_J_Eye01_L", "cf_J_Eye01_s_L",
            "cf_J_Eye02_L", "cf_J_Eye02_s_L", "cf_J_Eye03_L", "cf_J_Eye03_s_L",
            "cf_J_Eye04_L", "cf_J_Eye04_s_L", "cf_J_EyePos_rz_L", "cf_J_look_L",
            "cf_J_eye_rs_L", "cf_J_pupil_s_L", "cf_J_Eye_t_R", "cf_J_Eye_s_R",
            "cf_J_Eye_r_R", "cf_J_Eye01_R", "cf_J_Eye01_s_R", "cf_J_Eye02_R",
            "cf_J_Eye02_s_R", "cf_J_Eye03_R", "cf_J_Eye03_s_R", "cf_J_Eye04_R",
            "cf_J_Eye04_s_R", "cf_J_EyePos_rz_R", "cf_J_look_R", "cf_J_eye_rs_R",
            "cf_J_pupil_s_R", "cf_J_Mayu_L", "cf_J_MayuMid_s_L", "cf_J_MayuTip_s_L",
            "cf_J_Mayu_R", "cf_J_MayuMid_s_R", "cf_J_MayuTip_s_R", "N_Hitai",
            "N_Head", "N_Head_top", "cf_J_NoseBase_trs", "cf_J_NoseBase_s",
            "cf_J_Tang_S_00",
            "cf_J_Neck", "cf_J_Neck_s",
            "cf_J_Nose_r", "cf_J_Nose_t", "cf_J_Nose_tip", "N_Nose",
            "cf_J_Head", "cf_J_Head_s", "ct_head", "cf_head_bone",
            "cf_J_NoseWing_tx_L", "cf_J_NoseWing_tx_R", "cf_J_NoseBridge_t", "cf_J_megane",
            "N_Megane", "cf_J_NoseBridge_s", "N_Face", "cf_J_FaceRoot_s",
            "ct_head", "o_eyebase_L", "o_eyebase_R", "o_eyelashes",
            "o_eyeshadow", "o_head", "o_namida", "o_tang", "o_tooth", "ColFace01", "ColFace02"
        };

        public static void SetTemplate(this ChaControl control, int index = 0)
        {
            if (CharacterData.Templates == null || index < 0 ||
                index > CharacterData.Templates.Length) return;
            CharacterData.Templates[index] = control.GetCharacterSnapshot();
        }

        public static void TrySaveSlot(int index = 0)
        {
            // invalid
            if (CharacterData.Templates == null || index < 0 ||
                index > CharacterData.Templates.Length) return;
            CharacterData.Templates.ElementAt(index).Save();
        }

        public static void TryLoadSlot(int index = 0)
        {
            if (CharacterData.Templates == null || index < 0 ||
                index > CharacterData.Templates.Length) return;
            CharacterData.CharacterSliders.TryLoad(template =>
                CharacterData.Templates[index] = template);
        }
    }
}
