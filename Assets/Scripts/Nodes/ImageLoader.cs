using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageLoader : DialogBox
{
    void OnEnable()
    {
        inputs = new List<DialogBox>();

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
        AddField(new Field() { name = "Filename", type = Field.FieldType.text, parameters = new List<string>() });
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();

        string path = Application.dataPath;
        if (!Application.isEditor)
            path = path.Remove(path.LastIndexOfAny(new char[] { '\\', '/' }));
        path += "/Images/" + Read(fields[0]) + ".png";

        if (!File.Exists(path))
            throw new FileNotFoundException("Texture not found! Check that the fields have been entered correctly. Filepath entered: " + path);
        byte[] bytes = File.ReadAllBytes(path);

        Texture2D img = new Texture2D(1,1, TextureFormat.ARGB32, false);
        ImageConversion.LoadImage(img, bytes);
        output = new IOImage(img.width);

        RenderTexture.active = output.image;
        Graphics.Blit(img, output.image);
        DestroyImmediate(img, true);
    }
}