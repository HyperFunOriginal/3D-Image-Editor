using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorObject : MonoBehaviour
{
    RectTransform rt;
    Vector2 normalPos;
    Vector3 normalScale;
    // Start is called before the first frame update
    internal virtual void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
        normalPos = rt.anchoredPosition;
        normalScale = rt.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = normalPos / CanvasZoom.zoom;
        rt.localScale = normalScale / CanvasZoom.zoom;
        transform.SetAsLastSibling();
    }
}
