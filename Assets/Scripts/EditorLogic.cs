using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorLogic : MonoBehaviour
{
    public static EditorLogic instance;
    public static FollowCursor conSelected;
    public static DialogBox selected;
    public static List<NodeInput> hoveringOverInput;
    public static int layer;
    public static bool run;

    private void Start()
    {
        instance = this;
        run = false;
        hoveringOverInput = new List<NodeInput>();
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
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl))
            run = !run;
    }
}
