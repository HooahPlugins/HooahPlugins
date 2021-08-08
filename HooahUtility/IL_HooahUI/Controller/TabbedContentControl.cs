using System.Collections.Generic;
using HooahUtility.Controller;
using HooahUtility.Controller.ContentManagers;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TabbedContentControl
{
    [System.Serializable]
    public struct TabbedContent
    {
        public Button uiButtonTab;
        public RectTransform uiRectTransformView;
        public ContentManager contentManager;
        public T Manager<T>() where T : ContentManager => (T) contentManager;
    }

    private RectTransform _uiRectTabParent;
    private ButtonList _buttonList;
    private Dictionary<string, TabbedContent> _instancedTabs = new Dictionary<string, TabbedContent>();
    private bool _isEmpty = true;

    public TabbedContentControl(RectTransform uiRectContentParent, RectTransform uiRectTabParent,
        GameObject tabButtonObject)
    {
        _uiRectTabParent = uiRectContentParent;
        _buttonList = new ButtonList(uiRectTabParent, tabButtonObject);
    }

    public TabbedContent AddTab<T>(string key, string title) where T : ContentManager, new()
    {
        var tabObject = new GameObject($"tab_{key}", typeof(RectTransform));
        tabObject.SetActive(_isEmpty);
        tabObject.transform.SetParent(_uiRectTabParent, false);

        var rect = tabObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.right * 5;
        rect.offsetMax = Vector2.right * -5;
        rect.pivot = new Vector2(0.5f, 1);

        var pair = new TabbedContent()
        {
            uiRectTransformView = rect,
            contentManager = new T()
            {
                uiRectTransformParent = rect,
                uiRectTransformParentWrapper = _uiRectTabParent,
            }
        };
        pair.uiButtonTab = _buttonList.AddButton(title, () =>
        {
            SetActive(key);
            pair.contentManager.SyncHeight();
        });
        
        _instancedTabs.Add(key, pair);
        if (_isEmpty) _isEmpty = false;
        return pair;
    }

    public bool TryGetTabManager<T>(string key, out T manager) where T : ContentManager
    {
        manager = null;
        if (!_instancedTabs.TryGetValue(key, out var pairs)) return false;
        manager = (T) pairs.contentManager;
        return true;
    }

    public void SetActive(string key)
    {
        if (!_instancedTabs.TryGetValue(key, out var targetSt)) return;
        foreach (var st in _instancedTabs.Values) st.uiRectTransformView.gameObject.SetActive(false);
        // var delta = targetSt.uiRectTransformView.sizeDelta;
        // _uiRectTabParent.sizeDelta = delta;
        targetSt.uiRectTransformView.gameObject.SetActive(true);
    }
}