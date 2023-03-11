using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadRaw : BaseNode
{
    void OnEnable()
    {
        inputs = new List<BaseNode>();

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
        name = "Load RAW data";
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();

        string path = Application.dataPath;
        if (!Application.isEditor)
            path = path.Remove(path.LastIndexOfAny(new char[] { '\\', '/' }));
        path += "/Images/" + Read(fields[0]) + ".rawImage";

        if (!File.Exists(path))
            throw new FileNotFoundException("Texture not found! Check that the fields have been entered correctly. Filepath entered: " + path);

        byte[] bytes; int resolution = 0;
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            bytes = reader.ReadBytes((int)reader.BaseStream.Length - 4);
            resolution = reader.ReadInt32();
            reader.Close();
        }

        Texture2D img = new Texture2D(resolution, resolution, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        img.LoadRawTextureData(bytes);
        img.Apply();
        output = new IOImage(img.width);

        RenderTexture.active = output.image;
        Graphics.Blit(img, output.image);
        DestroyImmediate(img, true);
    }
}