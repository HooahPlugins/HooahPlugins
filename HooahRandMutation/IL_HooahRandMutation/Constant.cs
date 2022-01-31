using System.Collections.Generic;

public static class Constant
{
    public const string GUID = "hooh.rand.mutation";
    public const string NAME = "Character Random Mutation";
    public const string VERSION = "0.0.1";
    public const string NotLoaded = "Not Loaded";

    // todo: resolve this abomination
    //       ui? or just load from textfile?
    //       anyway in any method, gotta make this customizable
    //       and persistent.
    public static readonly HashSet<string> HorizontalFix = new HashSet<string>()
    {
        "p_cf_head_bone", "cf_J_FaceRoot", "cf_J_FaceBase", "cf_J_FaceLowBase", "cf_J_FaceLow_s",
        "cf_J_Chin_rs", "cf_J_ChinTip_s", "cf_J_ChinLow", "cf_J_MouthBase_tr",
        "N_Mouth", "cf_J_MouthMove", "cf_J_MouthLow", "cf_J_Mouthup",
        "cf_J_MouthBase_s", "cf_J_MouthCavity", "cf_J_FaceUp_ty",
        "cf_J_FaceUp_tz", "N_Head", "N_Head_top", "cf_J_NoseBase_trs", "cf_J_NoseBase_s",
        "cf_J_Nose_r", "cf_J_Nose_t", "cf_J_Nose_tip", "N_Nose",
        "cf_J_NoseBridge_t", "cf_J_megane", "N_Megane",
        "cf_J_NoseBridge_s", "N_Face", "cf_J_FaceRoot_s",
        "cf_J_Head", "cf_J_Head_s", "ct_head", "cf_head_bone",
        "cf_J_Tang_S_00",
        "cf_J_Neck", "cf_J_Neck_s",
        "ct_head", "o_eyelashes", "o_eyeshadow", "o_head", "o_namida", "o_tang", "o_tooth",
        "ColFace01", "ColFace02"
    };
}
