using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    UILine line;
    public string followName;
    bool set;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<UILine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followName == "cursor")
        {
            line.pointD = (Input.mousePosition - transform.position) / CanvasZoom.zoom;
            line.pointC = line.pointD - Vector3.right * (30f + Mathf.Abs(line.pointD.x - 10f) * 0.5f);
            line.pointB = Vector3.right * (40f + Mathf.Abs(line.pointD.x - 10f) * 0.5f);
            line.pointA = Vector3.right * 10f;
            line.thickness = 10f;
            line.SetVerticesDirty();
            set = false;
        }
        else if (followName != "") {
            GameObject target = GameObject.Find(followName);
            line.pointD = target.transform.position - transform.position;
            line.pointC = line.pointD - Vector3.right * (30f + Mathf.Abs(line.pointD.x - 10f) * 0.5f);
            line.pointB = Vector3.right * (40f + Mathf.Abs(line.pointD.x - 10f) * 0.5f);
            line.pointA = Vector3.zero;
            line.thickness = 10f;
            line.SetVerticesDirty();
            set = false;
        }
        else if (!set)
        {
            line.pointD = Vector3.zero;
            line.pointC = Vector3.zero;
            line.pointB = Vector3.zero;
            line.pointA = Vector3.zero;
            line.thickness = 0f;
            line.SetVerticesDirty();
            set = true;
        }
    }

    private bool MouseHoverOver(Vector3 pos)
    {
        Vector3 del = (transform.position - pos) / CanvasZoom.zoom;
        return del.x < 0f && Mathf.Abs(del.y) < 25f && del.x > -13f;
    }

    private void LateUpdate()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        if (MouseHoverOver(Input.mousePosition))
        {
            if (EditorLogic.conSelected != null)
                EditorLogic.conSelected.followName = "";
            EditorLogic.conSelected = this;
            followName = "cursor";
        }
        else if (EditorLogic.conSelected == this)
        {
            if (EditorLogic.hoveringOverInput.Count > 0)
                followName = EditorLogic.hoveringOverInput[0].name;
            else
                followName = "";
            EditorLogic.conSelected = null;
        }
    }
}
