#if AI || HS2
using MessagePack;
using HooahUtility.Serialization.Attributes;
using HooahUtility.Serialization.StudioReference;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using HooahUtility.Model;
#endif

namespace HooahComponents.Configuration
{
#if AI || HS2
    [MessagePackObject]
    public class SceneConfig : IFormData
    {
        [StudioReference(StudioReferenceAttribute.ReferenceType.Folder)]
        public StudioReference MainCameraFolder;
    }

    public class SceneConfigManager : ConfigManager<SceneConfigManager, SceneConfig>
    {
        public SceneConfigManager()
        {
            Config.MainCameraFolder = new StudioReference();
            Studio.Studio.Instance
                .LateUpdateAsObservable()
                .Subscribe(_ => { UpdateCameraFolder(); });
        }

        public void UpdateConfig()
        {
        }

        public void UpdateCameraFolder()
        {
            Camera cam = Camera.current;
            if (Config.MainCameraFolder.ReferenceTransform == null || cam == null) return;
            var transform = cam.transform;
            Config.MainCameraFolder.ReferenceTransform.position = transform.position;
            Config.MainCameraFolder.ReferenceTransform.eulerAngles = transform.eulerAngles;
        }
    }
#endif
}