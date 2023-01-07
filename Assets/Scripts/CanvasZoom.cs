using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasZoom : MonoBehaviour
{
    public static CanvasZoom instance;
    public static float zoom;
    public static bool zooming => Mathf.Abs(instance.lerp) > 0.0001f;

    float lerp;
    Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        zoom = 1f;
        canvas = GetComponent<Canvas>();
        lerp = 0f;
    }

    public static void ReinitializeZoom()
    {
        zoom = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        lerp += Input.mouseScrollDelta.y * .1f;
        lerp /= 2f;
        zoom *= Mathf.Exp(lerp);
        zoom = Mathf.Clamp(zoom, 0.3f, 2f);
        canvas.scaleFactor = zoom;
    }
}
