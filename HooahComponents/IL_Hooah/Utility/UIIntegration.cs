using System.Collections;
using System.Linq;
using System.Reflection;
using AdvancedStudioUI;
using HooahComponents.Configuration;
using HooahComponents.Hooks;
using HooahUtility.Controller.ContentManagers;
using HooahUtility.Service;
using UnityEngine;
#if AI || HS2
using KKAPI.Studio;
using KKAPI.Studio.UI;
using KKAPI.Utilities;
using Studio;

#endif

namespace HooahComponents.Utility
{
    public static class UIIntegration
    {
#if AI || HS2
        public static bool Loaded;
        public static ToolbarToggle ToolbarButton { get; private set; }
        public static RectTransform WindowTransform;

        public static IEnumerator InitializeCoroutine()
        {
            yield return new WaitUntil(() => StudioAPI.StudioLoaded);
            yield return new WaitUntil(AssetManager.LoadAssetBundles);
            yield return new WaitUntil(CanvasManager.InitializeCanvas);

            var asset = AssetManager.UIAssetBundle.LoadAsset<GameObject>("TabWindow");
            var instance = Object.Instantiate(asset, CanvasManager.CanvasGameObject.transform);
            instance.SetActive(false);
            WindowTransform = instance.GetComponent<RectTransform>();

            StudioItemControl.OnInitialize += (self, content) =>
            {
                InitializeStudioTab(self, content);
                StudioMacros.InitializeMacroTab(self, content);
                InitializeSceneTab(self, content);
                InitializeConfigTab(self, content);
            };

            yield return new WaitUntil(() =>
            {
                // Load assets and assign new instance as singleton reference
                StudioItemControl.Instance = instance.GetComponent<StudioItemControl>();
                return StudioItemControl.Instance != null;
            });

            ToolbarButton = CustomToolbarButtons.AddLeftToolbarToggle(
                ResourceUtils.GetEmbeddedResource("icon.png", Assembly.GetAssembly(typeof(UIIntegration)))
                    .LoadTexture(),
                false, ToggleStudioItemControlWindow
            );

            Hooks.Hooks.RegisterEvents();
            Loaded = true;
        }


        public static void ToggleStudioItemControlWindow(bool value)
        {
            var instance = StudioItemControl.Instance;
            if (Input.GetKey(KeyCode.LeftShift))
                instance.gameObject.GetComponent<RectTransform>().position = Vector3.zero;

            if (!instance.internalUpdate) instance.gameObject.SetActive(value);
        }

        public static void InitializeStudioTab(StudioItemControl self, TabbedContentControl content)
        {
            var form = self.CreateTab("studioObject", "Object");
            self.Form = form;
        }


        public static void InitializeConfigTab(StudioItemControl self, TabbedContentControl content)
        {
            var form = self.CreateTab("globalConfig", "Config");
            SetupConfigTabContent(form);
        }

        public static void SetupConfigTabContent(SerializedDataForm form)
        {
            form.AddButton("Save Global Config", Serialization.SaveAllConfigs);
            form.AddButton("Load Global Config", () =>
            {
                form.RemoveAllFields();
                Serialization.LoadAllConfigs();
                SetupConfigTabContent(form);
            });

            var hooahConfigInstance = new HooahConfigManger();
            hooahConfigInstance.AddForm(form, null, () => { hooahConfigInstance.UpdateConfig(); });

            var instance = AuraConfigManager.Instance;
            instance.AddForm(form, null, () => { instance.UpdateConfig(); });
        }

        public static void InitializeSceneTab(StudioItemControl self, TabbedContentControl content)
        {
            var form = self.CreateTab("scene", "Scene");
            // todo: remake this  working good
            // var instance = new SceneConfigManager();
            // form.AddButton("Set Selected Folder as Camera Folder", () =>
            // {
            //     var folder = Studio.Studio.Instance.treeNodeCtrl.selectObjectCtrl.OfType<OCIFolder>().FirstOrDefault();
            //     if (folder != null)
            //     {
            //         instance.Config.MainCameraFolder.AssignReference(folder);
            //     }
            // });

            form.AddButton("Collide with Character Colliders", () =>
            {
                var item = Studio.Studio.Instance.treeNodeCtrl.selectObjectCtrl.OfType<OCIItem>().FirstOrDefault();
                if (item == null) return;

                var colliders = Studio.Studio.Instance.treeNodeCtrl.selectObjectCtrl.OfType<OCIChar>()
                    .SelectMany(x => x.charInfo.animBody.gameObject.GetComponentsInChildren<DynamicBoneColliderBase>())
                    .ToList();

                foreach (var dynamicBone in item.objectItem.GetComponentsInChildren<DynamicBone>())
                    dynamicBone.m_Colliders = colliders;
            });

            // form.AddButton("Tone down ", () =>
            // {
            //     var materials = Studio.Studio.Instance.treeNodeCtrl.selectObjectCtrl.OfType<OCIChar>()
            //         .SelectMany(x => x.charInfo.animBody.gameObject.GetComponentsInChildren<Renderer>())
            //         .SelectMany(x => x.materials)
            //         .Where(x => x.shader.name.Contains("hair"))
            //         .ToList();
            //
            //     foreach (var material in materials)
            //     {
            //         // Never go back
            //         material.SetFloat("_HighLight", 9999999f);
            //         material.SetFloat("_Rimpower", 9999999f);
            //     }
            // });
        }
#endif
    }
}