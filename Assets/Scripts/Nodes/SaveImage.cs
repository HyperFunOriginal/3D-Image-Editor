using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveImage : DialogBox
{
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
        AddField(new Field() { name = "Filename", type = Field.FieldType.text, parameters = new List<string>()});
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();

        SaveImageToFile(ReturnImg(inputs[0].output.image), Read(fields[0]));
    }

    private Texture2D ReturnImg(RenderTexture rt)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D outputImage = new Texture2D(rt.width, rt.height);
        outputImage.ReadPixels(new Rect(0.0f, 0.0f, rt.width, rt.height), 0, 0);
        outputImage.Apply();
        RenderTexture.active = active;
        return outputImage;
    }

    private void SaveImageToFile(Texture2D img, string fileName)
    {
        byte[] png = img.EncodeToPNG();
        string path = Application.dataPath;
        if (!Application.isEditor)
            path = path.Remove(path.LastIndexOfAny(new char[] { '\\', '/' }));
        if (!Directory.Exists(path + "/Images/"))
            Directory.CreateDirectory(path + "/Images/");

        while (File.Exists(path + "/Images/" + fileName + ".png"))
            fileName += " Copy";

        File.WriteAllBytes(path + "/Images/" + fileName + ".png", png);
        DestroyImmediate(img, true);
    }
}
