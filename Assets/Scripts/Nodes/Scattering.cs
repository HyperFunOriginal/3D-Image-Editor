using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scattering : BaseNode
{
    public ComputeShader scatterCompute => (ComputeShader)Resources.Load("Shaders/ScatteringCompute");

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
        AddField(new Field() { name = "Ray Count", type = Field.FieldType._uint, parameters = new List<string>() });
        AddField(new Field() { name = "Maximum Iterations", type = Field.FieldType._uint, parameters = new List<string>() });
        AddField(new Field() { name = "Step Size", type = Field.FieldType._float, parameters = new List<string>() });
        AddField(new Field() { name = "Scattering Coeff.", type = Field.FieldType._float, parameters = new List<string>() });
        AddField(new Field() { name = "Density Multiplier", type = Field.FieldType._float, parameters = new List<string>() });
        name = "Compute Scattering";
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();

        int.TryParse(Read(fields[0]), out int value);
        int.TryParse(Read(fields[1]), out int value2);
        float.TryParse(Read(fields[2]), out float value3);
        float.TryParse(Read(fields[3]), out float value4);
        float.TryParse(Read(fields[4]), out float value5);

        output = new IOImage(inputs[0].output.image.width);
        int sqrtSliceLength = Mathf.RoundToInt(Mathf.Pow(output.image.width, 0.333333333333333f));
        int sliceLength = sqrtSliceLength * sqrtSliceLength;
        int imageWidth = output.image.width;

        scatterCompute.SetFloats("stepSize", value3, value5);
        scatterCompute.SetFloat("scatteringCoeff", value4 / value);
        scatterCompute.SetInt("rayCount", value);
        scatterCompute.SetInt("maxIterations", value2);
        scatterCompute.SetInt("sliceLength", sliceLength);
        scatterCompute.SetInt("sqrtsliceLength", sqrtSliceLength);
        scatterCompute.SetTexture(0, "Input", inputs[0].output.image);
        scatterCompute.SetTexture(0, "Output", output.image);
        scatterCompute.Dispatch(0, Mathf.CeilToInt(imageWidth / 32f), Mathf.CeilToInt(imageWidth / 32f), 1);
    }
    void CustomValidate()
    {
        int.TryParse(Read(fields[0]), out int value);
        int.TryParse(Read(fields[1]), out int value2);
        float.TryParse(Read(fields[2]), out float value3);
        float.TryParse(Read(fields[3]), out float value4);
        float.TryParse(Read(fields[4]), out float value5);

        Write(fields[0], Mathf.Clamp(value, 1, 16).ToString());
        Write(fields[1], Mathf.Clamp(value2, 1, 10).ToString());
        Write(fields[2], Mathf.Clamp(value3, 0f, .5f).ToString("f4"));
        Write(fields[3], Mathf.Clamp(value4, 0f, 1f).ToString("f4"));
        Write(fields[4], Mathf.Max(value5, 0f).ToString("f3"));
    }
}
