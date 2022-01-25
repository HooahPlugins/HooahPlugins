using System.Reflection;
using System.Runtime.CompilerServices;
using AIChara;
using HarmonyLib;

namespace HooahRandMutation
{
    public static class FieldRefs
    {
        private static readonly PropertyInfo charShapeFaceField;
        private static readonly PropertyInfo charShapeBodyField;

        static FieldRefs()
        {
            var chaControlClass = typeof(AIChara.ChaControl);
            charShapeFaceField = chaControlClass.GetProperty("sibFace", AccessTools.all);
        }

        public static ShapeInfoBase GetFaceShape(this ChaControl chaControl)
        {
            return charShapeFaceField.GetValue(chaControl) as ShapeInfoBase;
        }

        public static ShapeHeadInfoFemale GetFemaleFaceShape(this ChaControl femaleControl)
        {
            return femaleControl.GetFaceShape() as ShapeHeadInfoFemale;
        }

        public static ShapeHeadInfoMale GetMaleFaceShape(this ChaControl femaleControl)
        {
            return femaleControl.GetFaceShape() as ShapeHeadInfoMale;
        }
    }
}
