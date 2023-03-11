using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFunction : BaseNode
{
    public ComputeShader math => (ComputeShader)Resources.Load("Shaders/ImageMath");
    static List<string> functions = new List<string>() { "Add", "Subtract", "Multiply", "Divide", "Minimum", "Maximum", "Abs. Diff" };

    void OnEnable()
    {
        inputs = new List<BaseNode>();
        inputs.Add(null);
        inputs.Add(null);

        addFieldsMethod += CustomStart2;
        runFunction += RunFunction;
    }

    void OnDisable()
    {
        addFieldsMethod -= CustomStart2;
        runFunction -= RunFunction;
    }

    void CustomStart2()
    {
        AddField(new Field() { name = "Function", type = Field.FieldType.dropdown, parameters = new List<string>() { "Add", "Subtract", "Multiply", "Divide", "Minimum", "Maximum", "Abs. Diff" } });
        AddField(new Field() { name = "Ignore Alpha", type = Field.FieldType._bool, parameters = new List<string>() { } });
        name = "Maths Function";
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();
        output = new IOImage(inputs[0].output.image.width);
        string target = Read(fields[0]);
        int mode = functions.FindIndex(0, s => s == target);
        math.SetInt("mode", mode);
        math.SetBool("ignoreAlpha", bool.Parse(Read(fields[1])));
        math.SetTexture(0, "Input1", inputs[0].output.image);
        math.SetTexture(0, "Input2", inputs[1].output.image);
        math.SetTexture(0, "Output", output.image);
        math.Dispatch(0, Mathf.CeilToInt(output.image.width / 32f), Mathf.CeilToInt(output.image.width / 32f), 1);
    }
}
