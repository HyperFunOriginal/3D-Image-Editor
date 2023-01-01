using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileImage : DialogBox
{
    public ComputeShader tile3D => (ComputeShader)Resources.Load("Shaders/Tile3D");
    int tileX, tileY, tileZ;

    void OnEnable()
    {
        inputs = new List<DialogBox>();
        inputs.Add(null);

        startFunction += CustomStart2;
        runFunction += RunFunction;
    }

    void OnDisable()
    {
        startFunction -= CustomStart2;
        runFunction -= RunFunction;
    }

    void CustomStart2()
    {
        tileX = tile3D.FindKernel("TileX");
        tileY = tile3D.FindKernel("TileY");
        tileZ = tile3D.FindKernel("TileZ");
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();

        output = new IOImage(inputs[0].output.image.width);
        int sqrtSliceLength = Mathf.RoundToInt(Mathf.Pow(output.image.width, 0.333333333333333f));
        int sliceLength = sqrtSliceLength * sqrtSliceLength;
        int imageWidth = output.image.width;

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
    }
}
