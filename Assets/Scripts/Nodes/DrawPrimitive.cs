using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPrimitive : BaseNode
{
    public ComputeShader drawPrimitive => (ComputeShader)Resources.Load("Shaders/DrawPrimitive");
    int SolidColor, DrawCube, DrawSphere, DrawCylinderX, DrawCylinderY, DrawCylinderZ;

    FieldSetDictionary<string> fieldSets;
    Field useFade, fadeDistance;
    void OnEnable()
    {
        inputs = new List<BaseNode>();

        addFieldsMethod += CustomStart2;
        runFunction += RunFunction;
        updateFunction += CustomUpdate;
        onValidate += CustomValidate;

        SolidColor = drawPrimitive.FindKernel("SolidColor");
        DrawCube = drawPrimitive.FindKernel("DrawCube");
        DrawSphere = drawPrimitive.FindKernel("DrawSphere");
        DrawCylinderX = drawPrimitive.FindKernel("DrawCylinderX");
        DrawCylinderY = drawPrimitive.FindKernel("DrawCylinderY");
        DrawCylinderZ = drawPrimitive.FindKernel("DrawCylinderZ");

        name = "Draw Primitive";
    }

    void OnDisable()
    {
        addFieldsMethod -= CustomStart2;
        runFunction -= RunFunction;
        updateFunction -= CustomUpdate;
        onValidate -= CustomValidate;
    }

    void CustomValidate()
    {
        int.TryParse(Read(fields[0]), out int resolution);
        resolution = Mathf.Min(256, Mathf.RoundToInt(Mathf.Pow(4f, Mathf.RoundToInt(Mathf.Log(resolution + 1f, 4f)))));
        Write(fields[0], resolution.ToString());
    }

    void CustomUpdate()
    {
        fieldSets.SetActive(Read(fields[3]));
    }

    void CustomStart2()
    {
        AddField(new Field() { name = "Slice Length", type = Field.FieldType._uint, parameters = new List<string>() });
        AddField(new Field("", Field.FieldType.displayText, "Base Color, Ignore arrow"));
        AddField(new Field("", Field.FieldType.colorArr));
        AddField(new Field() { name = "Primitive Type", type = Field.FieldType.dropdown, parameters = new List<string>() { "Solid Color", "Sphere", "Cylinder", "Cube" } });

        fieldSets = new FieldSetDictionary<string>(this);
        useFade = new Field("Use S.D.F. Fade", Field.FieldType._bool);
        fadeDistance = new Field("Fade Distance", Field.FieldType._float);

        fieldSets.Add(new FieldSet<string>("Solid Color"));
        fieldSets.Add(new FieldSet<string>("Sphere", new Field("Radius", Field.FieldType._float), useFade, fadeDistance));
        fieldSets.Add(new FieldSet<string>("Cylinder", new Field("Radius", Field.FieldType._float), new Field("Length", Field.FieldType._float), new Field("Direction", Field.FieldType.dropdown, "X", "Y", "Z"), useFade, fadeDistance));
        fieldSets.Add(new FieldSet<string>("Cube", new Field("Length", Field.FieldType._float), new Field("Width", Field.FieldType._float), new Field("Height", Field.FieldType._float), useFade, fadeDistance));
    }

    void RunFunction()
    {
        if (output != null)
            output.Clear();

        int resolution = int.Parse(Read(fields[0]));
        ColorValues.KeyColorPair baseColor = new ColorValues.KeyColorPair(Read(fields[2]));

        output = new IOImage(resolution * Mathf.RoundToInt(Mathf.Sqrt(resolution)));
        string option = Read(fields[3]);

        drawPrimitive.SetFloats("baseColor", baseColor.color.x, baseColor.color.y, baseColor.color.z, baseColor.color.w);
        if (option == "Solid Color")
        {
            drawPrimitive.SetTexture(SolidColor, "Result", output.image);
            drawPrimitive.Dispatch(SolidColor, Mathf.CeilToInt(output.image.width / 32f), Mathf.CeilToInt(output.image.width / 32f), 1);
            return;
        }

        float SDFBlurAmt = bool.Parse(Read(useFade)) ? Mathf.Max(float.Parse(Read(fadeDistance)), 0.001f) : 0.001f;
        drawPrimitive.SetInt("sliceLength", resolution);
        drawPrimitive.SetInt("sqrtSliceLength", Mathf.RoundToInt(Mathf.Sqrt(resolution)));
        drawPrimitive.SetFloat("sdfBlurAmt", SDFBlurAmt);
        if (option == "Cube")
        {
            drawPrimitive.SetTexture(DrawCube, "Result", output.image);
            float x = float.Parse(Read(fieldSets.activeFieldSet.containedFields[0]));
            float y = float.Parse(Read(fieldSets.activeFieldSet.containedFields[2]));
            float z = float.Parse(Read(fieldSets.activeFieldSet.containedFields[1]));
            drawPrimitive.SetFloats("dimensions", x, y, z);
            drawPrimitive.Dispatch(DrawCube, Mathf.CeilToInt(output.image.width / 32f), Mathf.CeilToInt(output.image.width / 32f), 1);
            return;
        }
        drawPrimitive.SetFloat("radius", float.Parse(Read(fieldSets.activeFieldSet.containedFields[0])));
        if (option == "Sphere")
        {
            drawPrimitive.SetTexture(DrawSphere, "Result", output.image);
            drawPrimitive.Dispatch(DrawSphere, Mathf.CeilToInt(output.image.width / 32f), Mathf.CeilToInt(output.image.width / 32f), 1);
            return;
        }
        if (option == "Cylinder")
        {
            drawPrimitive.SetFloat("lengthCylinder", float.Parse(Read(fieldSets.activeFieldSet.containedFields[1])));
            string direction = Read(fieldSets.activeFieldSet.containedFields[2]);
            if (direction == "X")
            {
                drawPrimitive.SetTexture(DrawCylinderX, "Result", output.image);
                drawPrimitive.Dispatch(DrawCylinderX, Mathf.CeilToInt(output.image.width / 32f), Mathf.CeilToInt(output.image.width / 32f), 1);
                return;
            }
            else if (direction == "Y")
            {
                drawPrimitive.SetTexture(DrawCylinderY, "Result", output.image);
                drawPrimitive.Dispatch(DrawCylinderY, Mathf.CeilToInt(output.image.width / 32f), Mathf.CeilToInt(output.image.width / 32f), 1);
                return;
            }
            else if (direction == "Z")
            {
                drawPrimitive.SetTexture(DrawCylinderZ, "Result", output.image);
                drawPrimitive.Dispatch(DrawCylinderZ, Mathf.CeilToInt(output.image.width / 32f), Mathf.CeilToInt(output.image.width / 32f), 1);
                return;
            }
            throw new System.Exception("Error! Direction not chosen. (Neither X, nor Y, nor Z).");
        }
        throw new System.Exception("Error! Primitive type not chosen.");
    }
}
