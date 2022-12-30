using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInput : MonoBehaviour
{
    internal DialogBox reference;
    internal int index; // Needs initialization from DialogBox

    private bool MouseHoverOver(Vector3 pos)
    {
        Vector3 del = (transform.position - pos) / CanvasZoom.zoom;
        return del.x < 10f && Mathf.Abs(del.y) < 25f && del.x > -8f;
    }

    private void Start()
    {
        string hash = Random.Range(int.MinValue, int.MaxValue).ToString();
        hash += Random.Range(int.MinValue, int.MaxValue).ToString();
        name = hash;
        reference = GetComponentInParent<DialogBox>();
    }
    // Update is called once per frame
    void Update()
    {
        if (MouseHoverOver(Input.mousePosition))
            EditorLogic.hoveringOverInput.Add(this);
    }
}
