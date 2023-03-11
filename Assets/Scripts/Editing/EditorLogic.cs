using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EditorLogic : MonoBehaviour
{
    public static EditorLogic instance;
    public static FollowCursor conSelected;
    public static BaseNode selected;
    public static List<NodeInput> hoveringOverInput;
    public static int layer;
    public static bool run;

    private void Start()
    {
        if (FindObjectsOfType<EditorLogic>().Length > 1)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(this);

        run = false;
        hoveringOverInput = new List<NodeInput>();
    }

    public void SaveSketch(string name)
    {
        string path = Application.dataPath;
        if (!Application.isEditor)
            path = path.Remove(path.LastIndexOfAny(new char[] { '\\', '/' }));
        if (!Directory.Exists(path + "/Saves/"))
            Directory.CreateDirectory(path + "/Saves/");
        path += "/Saves/" + name + ".s3d";

        Sketch s = VisualiserInterface.visualising ? VisualiserInterface.persistent : Sketch.Yield();
        Sketch.Save(path, s);
        Debug.Log("Saved Sketch to: " + path);
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

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S) && !VisualiserInterface.visualising)
            SaveSketch("Sketch");
    }
}