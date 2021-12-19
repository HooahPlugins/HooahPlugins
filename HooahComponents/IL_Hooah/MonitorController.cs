using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;
#if AI || HS2
using System.IO;
using BepInEx;
using HooahUtility.Model;
using HooahUtility.Model.Attribute;
using KKAPI.Utilities;
using MessagePack;
#endif

// todo: find a way to unscrew the unity native video issue
//       (the game crashes when the video has audio track...)
#if AI || HS2
public class MonitorController : MonoBehaviour, IFormData
#else
public class MonitorController : MonoBehaviour
#endif
{
    private RenderTexture _renderTexture;
    private VideoPlayer _videoPlayer;
    public string materialShaderProperty;

    public void Start()
    {
        _renderTexture = new RenderTexture(1024, 1024, 0)
        {
            volumeDepth = 1,
            dimension = TextureDimension.Tex2D,
            bindTextureMS = false,
            useDynamicScale = true,
            autoGenerateMips = false,
            format = RenderTextureFormat.ARGB32,
            wrapMode = TextureWrapMode.Repeat
        };

        _videoPlayer = gameObject.AddComponent<VideoPlayer>();
        _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        _videoPlayer.targetTexture = _renderTexture;
        _videoPlayer.playOnAwake = false;
        _videoPlayer.isLooping = true;
        _videoPlayer.targetMaterialProperty = materialShaderProperty;
        _videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        if (_videoPlayer.targetMaterialRenderer == null)
            _videoPlayer.targetMaterialRenderer = GetComponentInChildren<Renderer>();
        _videoPlayer.targetMaterialRenderer.material.SetTexture(materialShaderProperty, _renderTexture);
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        _videoPlayer.errorReceived += (source, message) =>
        {
            // todo: should i do something?
            Debugger.Break();
        };
        _videoPlayer.prepareCompleted += source =>
        {
            source.Play();
            for (ushort i = 0; i < _videoPlayer.audioTrackCount; i++)
                _videoPlayer.EnableAudioTrack(i, false);
        };
    }

#if AI || HS2
    [Key(0)] public bool loop = true;

    [Key(1)] public string url = "";

    private string GetDir() => Path.Combine(Paths.GameRootPath, @"userdata\textures");
    private const string Filter = "Video (*.mp4)";
    private const string FileExtension = ".mp4";

    [RuntimeFunction("Select Video")]
    public void TryPlayVideo()
    {
        OpenFileDialog.Show((files) =>
        {
            var file = files.FirstOrDefault();
            PlayVideo(file);
        }, "Select Video File", GetDir(), Filter, FileExtension);
    }
#else
    public string url = "";
#endif

    public void PlayVideo(string u)
    {
        _videoPlayer.Stop();
        _videoPlayer.url = null; // unset the variable to wish it to be cleared.
        _videoPlayer.Stop();
        url = u;
        _videoPlayer.url = u;
        _videoPlayer.Prepare();
    }
}
