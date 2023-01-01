using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRunButton : EditorObject
{
    public Sprite run;
    public Sprite pause;
    Image sprite;
    new void Start()
    {
        sprite = gameObject.GetComponent<Image>();
        base.Start();
    }
    public void OnClicked()
    {
        EditorLogic.run = !EditorLogic.run;
        if (EditorLogic.run)
            sprite.sprite = pause;
        else
            sprite.sprite = run;
    }
}
