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
            var chaControlClass = typeof(ChaControl);
            charShapeFaceField = chaControlClass.GetProperty("sibFace", AccessTools.all);
            charShapeBodyField = chaControlClass.GetProperty("sibBody", AccessTools.all);
        }

        private static ShapeInfoBase GetFaceShape(this ChaControl chaControl)
        {
            return charShapeFaceField.GetValue(chaControl) as ShapeInfoBase;
        }

        private static ShapeInfoBase GetBodyShape(this ChaControl chaControl)
        {
            return charShapeBodyField.GetValue(chaControl) as ShapeInfoBase;
        }

        public static ShapeHeadInfoFemale GetFemaleBodyShape(this ChaControl femaleControl)
        {
            return femaleControl.GetBodyShape() as ShapeHeadInfoFemale;
        }

        public static ShapeHeadInfoMale GetMaleBodyShape(this ChaControl femaleControl)
        {
            return femaleControl.GetBodyShape() as ShapeHeadInfoMale;
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
