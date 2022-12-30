using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileImage : DialogBox
{
    public ComputeShader tile3D => (ComputeShader)Resources.Load("Tile3D");
    int tileX, tileY, tileZ;

    void OnEnable()
    {
        onValidate += CustomValidate;
        inputs = new List<DialogBox>();
        inputs.Add(null);

        startFunction += CustomStart2;
        runFunction += RunFunction;
    }

    void OnDisable()
    {
        onValidate -= CustomValidate;
        startFunction -= CustomStart2;
        runFunction -= RunFunction;
    }

    void CustomStart2()
    {
        AddField(new Field() { name = "Slice Length", type = Field.FieldType._uint, parameters = new List<string>() });
        tileX = tile3D.FindKernel("TileX");
        tileY = tile3D.FindKernel("TileY");
        tileZ = tile3D.FindKernel("TileZ");
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();
        output = new IOImage(inputs[0].output.image.width);
        try
        {
            int sliceLength = int.Parse(Read(fields[0]));
            int sqrtSliceLength = Mathf.RoundToInt(Mathf.Sqrt(sliceLength));
            int imageWidth = sliceLength * sqrtSliceLength;

            RenderTexture temp = new RenderTexture(imageWidth, imageWidth, 0);
            temp.enableRandomWrite = true;
            temp.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat;
            temp.Create();

            tile3D.SetInt("sliceWidth", sliceLength);
            tile3D.SetInt("sqrtSliceWidth", sqrtSliceLength);
            tile3D.SetInt("imageWidth", imageWidth);

            tile3D.SetTexture(tileX, "Result", output.image);
            tile3D.SetTexture(tileX, "Input", inputs[0].output.image);
            tile3D.Dispatch(tileX, Mathf.CeilToInt(imageWidth / 32f), Mathf.CeilToInt(imageWidth / 32f), 1);

            tile3D.SetTexture(tileY, "Result", output.image);
            tile3D.SetTexture(tileY, "Temp", temp);
            tile3D.Dispatch(tileY, Mathf.CeilToInt(imageWidth / 32f), Mathf.CeilToInt(imageWidth / 32f), 1);

            tile3D.SetTexture(tileZ, "Result", output.image);
            tile3D.SetTexture(tileZ, "Temp", temp);
            tile3D.Dispatch(tileZ, Mathf.CeilToInt(imageWidth / 32f), Mathf.CeilToInt(imageWidth / 32f), 1);

            temp.Release();
            DestroyImmediate(temp, true);

            output.state = IOImage.CompletionState.ready;
        }
        catch
        {
            output.state = IOImage.CompletionState.failed;
        }
    }

    void CustomValidate()
    {
        int.TryParse(Read(fields[0]), out int resolution);
        resolution = Mathf.Min(256, Mathf.RoundToInt(Mathf.Pow(4f, Mathf.RoundToInt(Mathf.Log(resolution + 1f, 4f)))));
        Write(fields[0], resolution.ToString());
    }
}
