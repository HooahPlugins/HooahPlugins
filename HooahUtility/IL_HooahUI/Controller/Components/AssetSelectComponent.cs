using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if AI || HS2
using BepInEx;
using HooahUtility.Utility;
using KKAPI.Utilities;
using UniRx;

#endif
namespace HooahUtility.Controller.Components
{
    public class AssetSelectComponent : FormComponentBase
    {
        public Button uiButton;

        public enum AssetType { ExternalReference, AssetReference, BinaryData }

        public override void AssignValues()
        {
            // assign nothing
            uiButton.onClick.AddListener(LoadFile);
        }


#if AI || HS2
        private string GetDir() => Path.Combine(Paths.GameRootPath, @"userdata\textures");
#else
        private string GetDir() => Path.Combine(Application.dataPath, @"userdata\textures");
#endif

        public const string Filter = "Image (*.png)|*.png|All files|*.*";
        public const string FileExtension = ".png";

        public void LoadFile()
        {
#if AI || HS2
            OpenFileDialog.Show(ApplyFile, "Select Image File", GetDir(), Filter, FileExtension);
#else
            // load designated file for testing.
            ApplyFile(new[] {Path.Combine("D:\\testcase\\texture", "test.png")});
#endif
        }

#if AI || HS2
        private const uint SizeLimit = 8192 * 8192;

        private void LoadTexture(string path)
        {
            if (!File.Exists(path)) return;
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length <= 0)
            {
                Debug.LogError("The file has no content.");
                return;
            }

            if (bytes.Length > SizeLimit)
            {
                Debug.LogError("Image Size is too big.");
                return;
            }

            // loading...
            Observable.ReturnUnit()
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    tex.LoadImage(bytes);

                    if (path.Contains("ies"))
                    {
                        var l = tex.height;
                        var cube = new Cubemap(tex.height, TextureFormat.ARGB32, false);
                        for (var i = 0; i < ImageUtility.LinearCubemap.Length; i++)
                        {
                            cube.SetPixels(
                                tex.GetPixels(i * tex.height, 0, tex.height, tex.height), ImageUtility.LinearCubemap[i]
                            );
                        }

                        SetRawValue(cube, () => { });
                    }
                    else
                    {
                        // complete
                        SetRawValue(tex, () => { });
                    }
                });
        }
#elif UNITY_EDITOR
        private void Editor(string path)
        {
            // todo: implement preset
            // todo: implement advanced mode
            var bytes = File.ReadAllBytes(path);
            var tex = new Texture2D(512, 512, TextureFormat.RGBAHalf, false, true);
            tex.filterMode = FilterMode.Trilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.LoadImage(bytes);
            SetRawValue(tex, () => { });
        }
#endif


        protected void ApplyFile(string[] obj)
        {
            var filePath = obj.FirstOrDefault();
            if (filePath == null || filePath.Length <= 0) return;
#if AI || HS2
            if (MemberType.IsAssignableFrom(typeof(Texture)))
            {
                LoadTexture(filePath);
            }
            // else if (MemberType.IsAssignableFrom(typeof(Material)))
            // load material from asset bundles.
#elif UNITY_EDITOR
            Editor(filePath);
#endif
        }
    }
}