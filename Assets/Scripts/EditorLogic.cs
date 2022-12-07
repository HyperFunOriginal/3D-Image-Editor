using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorLogic : MonoBehaviour
{
    public static EditorLogic instance;
    public static FollowCursor conSelected;
    public static DialogBox selected;
    public static List<GameObject> hoveringOverInput;
    public static int layer;
    public static bool run;

    private void Start()
    {
        instance = this;
        run = false;
        hoveringOverInput = new List<GameObject>();
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            selected = null;
            layer = -1;
        }
        if (selected != null && (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)))
            Destroy(selected.gameObject);
        hoveringOverInput.Clear();
    }
}
