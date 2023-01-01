using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleUV : DialogBox
{
    public ComputeShader uvMap => (ComputeShader)Resources.Load("Shaders/BareUVGenerator");

    void OnEnable()
    {
        onValidate += CustomValidate;
        runFunction += RunFunction;
        startFunction += CustomStart2;
    }

    void CustomStart2()
    {
        AddField(new Field() { name = "Slice Length", type = Field.FieldType._uint, parameters = new List<string>() });
    }

    void OnDisable()
    {
        onValidate -= CustomValidate;
        runFunction -= RunFunction;
        startFunction -= CustomStart2;
    }

    void CustomValidate()
    {
        int.TryParse(Read(fields[0]), out int resolution);
        resolution = Mathf.Min(256, Mathf.RoundToInt(Mathf.Pow(4f, Mathf.RoundToInt(Mathf.Log(resolution + 1f, 4f)))));
        Write(fields[0], resolution.ToString());
    }

    void RunFunction()
    {
        if (output != null)
        output.Clear();
        int sliceLength = int.Parse(Read(fields[0]));
        int sqrtSliceLength = Mathf.RoundToInt(Mathf.Sqrt(sliceLength));
        int imageWidth = sliceLength * sqrtSliceLength;
        output = new IOImage(imageWidth);

        uvMap.SetTexture(0, "Result", output.image);
        uvMap.SetInt("sliceLength", sliceLength);
        uvMap.SetInt("sqrtSliceLength", sqrtSliceLength);
        uvMap.Dispatch(0, Mathf.CeilToInt(imageWidth / 32f), Mathf.CeilToInt(imageWidth / 32f), 1);
    }
}
