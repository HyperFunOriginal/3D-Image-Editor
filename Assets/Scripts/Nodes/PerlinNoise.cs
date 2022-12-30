using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : VariableLengthNode
{
    public ComputeShader perlinNoise => (ComputeShader)Resources.Load("PerlinGenerate");
    Field fractal;

    new void OnEnable()
    {
        onValidate += CustomValidate;
        inputs = new List<DialogBox>();
        inputs.Add(null);

        startFunction += CustomStart2;
        runFunction += RunFunction;
        updateFunction += CustomUpdate2;
        base.OnEnable();
    }

    new void OnDisable()
    {
        onValidate -= CustomValidate;
        startFunction -= CustomStart2;
        runFunction -= RunFunction;
        updateFunction -= CustomUpdate2;
        base.OnDisable();
    }

    void CustomUpdate2()
    {
        bool read = Read(fractal) == "Fractal";
        numAddFields = read ? 1u : 0u;
    }

    void CustomStart2()
    {
        AddField(new Field() { name = "Seed", type = Field.FieldType._int, parameters = new List<string>() });
        AddField(new Field() { name = "Frequency", type = Field.FieldType._float, parameters = new List<string>() });
        AddField(new Field() { name = "Persistence", type = Field.FieldType._float, parameters = new List<string>() });
        AddField(new Field() { name = "Lacunarity", type = Field.FieldType._float, parameters = new List<string>() });
        AddField(new Field() { name = "Force 0-1", type = Field.FieldType._bool, parameters = new List<string>() });
        fractal = new Field() { name = "Perlin Noise Type", type = Field.FieldType.dropdown, parameters = new List<string>() { "Simple", "Fractal" } };
        AddField(fractal);
        addField = new Field() { name = "Octaves", type = Field.FieldType._uint, parameters = new List<string>() };
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();
        output = new IOImage(inputs[0].output.image.width);
        try
        {
            bool fractalOrNot = Read(fractal) == "Fractal";
            perlinNoise.SetBool("fractalMode", fractalOrNot);
            perlinNoise.SetInt("seed", int.Parse(Read(fields[0])));
            perlinNoise.SetInt("octaves", fractalOrNot ? int.Parse(Read(additionalFields[0])) : 1);
            perlinNoise.SetFloat("frequency", float.Parse(Read(fields[1])));
            perlinNoise.SetFloat("persistence", float.Parse(Read(fields[2])));
            perlinNoise.SetFloat("lacunarity", float.Parse(Read(fields[3])));
            perlinNoise.SetBool("forceNorm", bool.Parse(Read(fields[4])));
            perlinNoise.SetTexture(0, "Input", inputs[0].output.image);
            perlinNoise.SetTexture(0, "Output", output.image);
            perlinNoise.Dispatch(0, Mathf.CeilToInt(output.image.width / 32f), Mathf.CeilToInt(output.image.width / 32f), 1);
            output.state = IOImage.CompletionState.ready;
        }
        catch
        {
            output.state = IOImage.CompletionState.failed;
        }
    }

    void CustomValidate()
    {
        float.TryParse(Read(fields[3]), out float value2);
        float.TryParse(Read(fields[2]), out float value);
        Write(fields[2], Mathf.Clamp01(value).ToString());
        Write(fields[3], Mathf.Max(1,value2).ToString());
    }
}
