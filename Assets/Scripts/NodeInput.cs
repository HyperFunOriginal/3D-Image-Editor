using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInput : MonoBehaviour
{
    internal BaseNode reference;
    public long hashCode;
    internal int index;

    private bool MouseHoverOver(Vector3 pos)
    {
        Vector3 del = (transform.position - pos) / CanvasZoom.zoom;
        return del.x < 10f && Mathf.Abs(del.y) < 25f && del.x > -8f;
    }

    private void Start()
    {
        name = System.Convert.ToString(hashCode, 16).PadLeft(16, '0');
        reference = GetComponentInParent<BaseNode>();
    }
    // Update is called once per frame
    void Update()
    {
        if (MouseHoverOver(Input.mousePosition))
            EditorLogic.hoveringOverInput.Add(this);
    }
}
