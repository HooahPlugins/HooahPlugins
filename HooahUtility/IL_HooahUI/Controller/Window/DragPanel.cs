using UnityEngine;
using UnityEngine.EventSystems;

// todo: clamp the drag panel window.
public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector2 _pointerOffset;
    private RectTransform _canvasRectTransform;
    private RectTransform _panelRectTransform;

    void Awake()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        _canvasRectTransform = canvas.transform as RectTransform;
        _panelRectTransform = transform.parent as RectTransform;
    }

    public void OnPointerDown(PointerEventData data)
    {
        _panelRectTransform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _panelRectTransform, data.position, data.pressEventCamera, out _pointerOffset
        );
    }

    public void OnDrag(PointerEventData data)
    {
        if (_panelRectTransform == null)
            return;

        var pointerPostion = ClampToWindow(data);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform, pointerPostion, data.pressEventCamera, out var localPointerPosition
        ))
        {
            _panelRectTransform.localPosition = localPointerPosition - _pointerOffset;
        }
    }

    Vector2 ClampToWindow(PointerEventData data)
    {
        var rawPointerPosition = data.position;

        var canvasCorners = new Vector3[4];
        _canvasRectTransform.GetWorldCorners(canvasCorners);

        var clampedX = Mathf.Clamp(rawPointerPosition.x, canvasCorners[0].x, canvasCorners[2].x);
        var clampedY = Mathf.Clamp(rawPointerPosition.y, canvasCorners[0].y, canvasCorners[2].y);

        var newPointerPosition = new Vector2(clampedX, clampedY);
        return newPointerPosition;
    }
}