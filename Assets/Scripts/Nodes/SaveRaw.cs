using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveRaw : BaseNode
{
    void OnEnable()
    {
        inputs = new List<BaseNode>();
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
        AddField(new Field() { name = "Filename", type = Field.FieldType.text, parameters = new List<string>() });
        name = "Save RAW data";
    }

    private Texture2D ReturnImg(RenderTexture rt)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D outputImage = new Texture2D(rt.width, rt.height, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        outputImage.ReadPixels(new Rect(0.0f, 0.0f, rt.width, rt.height), 0, 0);
        outputImage.Apply();
        RenderTexture.active = active;
        return outputImage;
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();

        SaveImageToFile(ReturnImg(inputs[0].output.image), Read(fields[0]));
    }

    private void SaveImageToFile(Texture2D img, string fileName)
    {
        byte[] png = img.GetRawTextureData();
        string path = Application.dataPath;

        if (!Application.isEditor)
            path = path.Remove(path.LastIndexOfAny(new char[] { '\\', '/' }));
        if (!Directory.Exists(path + "/Images/"))
            Directory.CreateDirectory(path + "/Images/");

        while (File.Exists(path + "/Images/" + fileName + ".rawImage"))
            fileName += " Copy";

        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(path + "/Images/" + fileName + ".rawImage")))
        {
            writer.Write(png);
            writer.Write(img.width);
            writer.Flush();
        }
        DestroyImmediate(img, true);
    }
}
