using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientMap : VariableLengthNode
{
    public ComputeShader gradientMap => (ComputeShader)Resources.Load("GradMapShader");

    new void OnEnable()
    {
        onValidate += CustomValidate;
        inputs = new List<DialogBox>();
        inputs.Add(null);

        startFunction += CustomStart2;
        runFunction += RunFunction;
        base.OnEnable();
    }

    new void OnDisable()
    {
        onValidate -= CustomValidate;
        startFunction -= CustomStart2;
        runFunction -= RunFunction;
        base.OnDisable();
    }

    void CustomStart2()
    {
        AddField(new Field() { name = "Luminance Type", type = Field.FieldType.dropdown, parameters = new List<string>() { "Sum", "Length" } });
        AddField(new Field() { name = "Number of points", type = Field.FieldType._uint, parameters = new List<string>() });
        addField = new Field() { name = "points", type = Field.FieldType.colorArr, parameters = new List<string>() };
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();
        output = new IOImage(inputs[0].output.image.width);
        try
        {
            bool type = Read(fields[0]) != "Sum";
            ColorValues.KeyColorPair[] pairings = new ColorValues.KeyColorPair[numAddFields];
            for (int i = 0; i < pairings.Length; i++)
                pairings[i] = new ColorValues.KeyColorPair(Read(additionalFields[i]));

            ComputeBuffer keyMap = new ComputeBuffer((int)numAddFields, sizeof(float) * 5);
            keyMap.SetData(pairings);
            gradientMap.SetBuffer(0, "Keys", keyMap);
            gradientMap.SetBool("sumMode", type);
            gradientMap.SetInt("keyCnt", (int)numAddFields);
            gradientMap.SetTexture(0, "Input", inputs[0].output.image);
            gradientMap.SetTexture(0, "Output", output.image);
            gradientMap.Dispatch(0, Mathf.CeilToInt(output.image.width / 32f), Mathf.CeilToInt(output.image.width / 32f), 1);
            keyMap.Dispose();
            output.state = IOImage.CompletionState.ready;
        }
        catch
        {
            output.state = IOImage.CompletionState.failed;
        }
    }

    void CustomValidate()
    {
        string val = Read(fields[1]);
        if (val != "")
        {
            numAddFields = System.Math.Min(15u, uint.Parse(val));
            Write(fields[1], numAddFields.ToString());
        }
    }
}
