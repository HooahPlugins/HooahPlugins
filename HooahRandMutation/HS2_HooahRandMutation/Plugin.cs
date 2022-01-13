using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HooahRandMutation.IL_HooahRandMutation;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UnityEngine.Events;

// todo: add check if the template exists.
// if there is no template, just use the current character as template for once.
namespace HooahRandMutation
{
    [BepInPlugin(Constant.GUID, Constant.NAME, Constant.VERSION)]
    public class HooahRandMutationPlugin : BaseUnityPlugin
    {
        private static ManualLogSource _logger;
        private static HooahRandMutationPlugin _instance;

        private void Start()
        {
            _instance = this;
            _logger = Logger;
            Harmony.CreateAndPatchAll(typeof(HooahRandMutationPlugin));
            MakerAPI.RegisterCustomSubCategories += (sender, e) =>
            {
                // todo: maybe use dropdown for this one?
                var all = MakerConstants.Body.All;
                var category = new MakerCategory(all.CategoryName, "MakerRandomMutation", all.Position + 5,
                    "RandomMutation");
                e.AddSubCategory(category);
                e.AddControl(new MakerButton("Set current character as template", category, _instance)).OnClick
                    .AddListener(delegate
                    {
                        MakerAPI.GetCharacterControl().Remember();
                        MakerAPI.GetCharacterControl().SetTemplate();
                    });


                /*==
                 Slider Blending
                 ==*/
                var deviationCheekSlider =
                    e.AddControl(new MakerSlider(category, "Cheek Deviation", 0f, 1f, 0.1f, _instance));
                var deviationChinSlider =
                    e.AddControl(new MakerSlider(category, "Chin Deviation", 0f, 1f, 0.1f, _instance));
                var deviationEarSlider =
                    e.AddControl(new MakerSlider(category, "Ear Deviation", 0f, 1f, 0.1f, _instance));
                var deviationEyesSlider =
                    e.AddControl(new MakerSlider(category, "Eyes Deviation", 0f, 1f, 0.1f, _instance));
                var deviationEyesAngleSlider =
                    e.AddControl(new MakerSlider(category, "Eyes Angle Deviation", 0f, 1f, 0.1f, _instance));
                var deviationMouthSlider =
                    e.AddControl(new MakerSlider(category, "Mouth Deviation", 0f, 1f, 0.1f, _instance));
                var deviationNoseSlider =
                    e.AddControl(new MakerSlider(category, "Nose Deviation", 0f, 1f, 0.1f, _instance));
                var deviationHeadSlider =
                    e.AddControl(new MakerSlider(category, "Head Deviation", 0f, 1f, 0.1f, _instance));
                var randomizeHeadSlider = new UnityAction(() =>
                {
                    var chara = MakerAPI.GetCharacterControl();

                    chara.MutateRangeCombined(
                        deviationHeadSlider.Value,
                        deviationChinSlider.Value,
                        deviationCheekSlider.Value,
                        deviationEyesSlider.Value,
                        deviationEyesAngleSlider.Value,
                        deviationNoseSlider.Value,
                        deviationMouthSlider.Value,
                        deviationEarSlider.Value
                    );

                    chara.UpdateShapeFaceValueFromCustomInfo();
                });

                e.AddControl(new MakerButton("Randomize Slider", category, _instance)).OnClick
                    .AddListener(randomizeHeadSlider);

                /*==
                 ABMX Blending
                 ===*/
                var abmxPositionDeviation = e.AddControl(new MakerSlider(category, "ABMX Position Deviation",
                    0f, 1f, 0.1f, _instance));
                var abmxAngleDeviation = e.AddControl(new MakerSlider(category, "ABMX Angle Deviation",
                    0f, 1f, 0.1f, _instance));
                var abmxScaleDeviation = e.AddControl(new MakerSlider(category, "ABMX Scale Deviation",
                    0f, 1f, 0.1f, _instance));
                var abmxLengthDeviation = e.AddControl(new MakerSlider(category, "ABMX Length Deviation",
                    0f, 1f, 0.1f, _instance));

                var abmxAbsolute = e.AddControl(new MakerToggle(category, "Use Absolute Scale", false, _instance));

                var randomizeABMX = new Action<HashSet<string>>(filters =>
                {
                    MakerAPI.GetCharacterControl().RandomizeABMX(abmxPositionDeviation.Value,
                        abmxAngleDeviation.Value, abmxScaleDeviation.Value, abmxLengthDeviation.Value,
                        abmxAbsolute.Value, filters);
                });

                e.AddControl(new MakerButton("Randomize All", category, _instance)).OnClick
                    .AddListener(() => randomizeABMX.Invoke(null));
                e.AddControl(new MakerButton("Randomize Only Head", category, _instance)).OnClick
                    .AddListener(() => randomizeABMX.Invoke(ABMXMutation.HeadBoneNames));
                e.AddControl(new MakerButton("Randomize Head (Slider & ABMX)", category, _instance)).OnClick
                    .AddListener(() =>
                    {
                        randomizeHeadSlider.Invoke();
                        randomizeABMX.Invoke(ABMXMutation.HeadBoneNames);
                    });


                var blendingCategory = new MakerCategory(all.CategoryName, "MakerBlendingMutation", all.Position + 5,
                    "Blending Mutation");
            };
        }
    }
}
