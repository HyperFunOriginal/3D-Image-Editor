using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : BaseNode
{
    public ComputeShader lightingCompute => (ComputeShader)Resources.Load("Shaders/LightingCompute");

    void OnEnable()
    {
        inputs = new List<BaseNode>();
        inputs.Add(null);

        addFieldsMethod += CustomStart2;
        runFunction += RunFunction;
        onValidate += CustomValidate;
    }

    void OnDisable()
    {
        addFieldsMethod -= CustomStart2;
        runFunction -= RunFunction;
        onValidate -= CustomValidate;
    }

    void CustomStart2()
    {
        AddField(new Field() { name = "Source Position (x)", type = Field.FieldType._float, parameters = new List<string>()});
        AddField(new Field() { name = "Source Position (y)", type = Field.FieldType._float, parameters = new List<string>()});
        AddField(new Field() { name = "Source Position (z)", type = Field.FieldType._float, parameters = new List<string>()});
        AddField(new Field() { name = "Source Intensity", type = Field.FieldType._float, parameters = new List<string>()});
        AddField(new Field() { name = "Step Size", type = Field.FieldType._float, parameters = new List<string>()});
        AddField(new Field() { name = "Density Multiplier", type = Field.FieldType._float, parameters = new List<string>()});
        name = "Compute Lighting";
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();

        float.TryParse(Read(fields[0]), out float value);
        float.TryParse(Read(fields[1]), out float value2);
        float.TryParse(Read(fields[2]), out float value3);
        float.TryParse(Read(fields[3]), out float value4);
        float.TryParse(Read(fields[4]), out float value5);
        float.TryParse(Read(fields[5]), out float value6);

        output = new IOImage(inputs[0].output.image.width);
        int sqrtSliceLength = Mathf.RoundToInt(Mathf.Pow(output.image.width, 0.333333333333333f));
        int sliceLength = sqrtSliceLength * sqrtSliceLength;
        int imageWidth = output.image.width;

        lightingCompute.SetFloats("lightInfo", value, value2, value3, value4);
        lightingCompute.SetFloats("stepSize", value5, value6);
        lightingCompute.SetInt("sliceLength", sliceLength);
        lightingCompute.SetInt("sqrtsliceLength", sqrtSliceLength);
        lightingCompute.SetTexture(0, "Input", inputs[0].output.image);
        lightingCompute.SetTexture(0, "Output", output.image);
        lightingCompute.Dispatch(0, Mathf.CeilToInt(imageWidth / 32f), Mathf.CeilToInt(imageWidth / 32f), 1);
    }
    void CustomValidate()
    {
        float.TryParse(Read(fields[0]), out float value);
        float.TryParse(Read(fields[1]), out float value2);
        float.TryParse(Read(fields[2]), out float value3);
        float.TryParse(Read(fields[3]), out float value4);
        float.TryParse(Read(fields[4]), out float value5);
        float.TryParse(Read(fields[5]), out float value6);

        Write(fields[0], Mathf.Clamp(value, -2f, 2f).ToString("f3"));
        Write(fields[1], Mathf.Clamp(value2, -2f, 2f).ToString("f3"));
        Write(fields[2], Mathf.Clamp(value3, -2f, 2f).ToString("f3"));
        Write(fields[3], Mathf.Clamp(value4, 0f, 5f).ToString("f3"));
        Write(fields[4], Mathf.Clamp(value5, 0f, .5f).ToString("f3"));
        Write(fields[5], Mathf.Max(value6, 0f).ToString("f3"));
    }
}
