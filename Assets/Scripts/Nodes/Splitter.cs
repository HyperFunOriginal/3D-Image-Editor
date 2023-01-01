using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : DialogBox
{
    void OnEnable()
    {
        inputs = new List<DialogBox>();
        inputs.Add(null);

        startFunction += CustomStart2;
        runFunction += RunFunction;
    }


    void CustomStart2()
    {
        AddField(new Field() { name = "Info", type = Field.FieldType.displayText, parameters = new List<string>() { "Splits an input into" } });
        AddField(new Field() { name = "Info", type = Field.FieldType.displayText, parameters = new List<string>() { "several output textures." } });
    }

    void OnDisable()
    {
        runFunction -= RunFunction;
        startFunction -= CustomStart2;
    }


    void RunFunction()
    {
        if (output != null)
            output.Clear();
        output = new IOImage(inputs[0].output.image.width);
        Graphics.CopyTexture(inputs[0].output.image, output.image);
    }
}
