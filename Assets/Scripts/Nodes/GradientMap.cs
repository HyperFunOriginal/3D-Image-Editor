using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientMap : VariableLengthNode
{
    new void OnEnable()
    {
        onValidate += CustomValidate;
        startFunction += CustomStart2;
        base.OnEnable();
    }

    new void OnDisable()
    {
        onValidate -= CustomValidate;
        startFunction -= CustomStart2;
        base.OnDisable();
    }

    void CustomStart2()
    {
        AddField(new Field() { name = "Number of points", type = Field.FieldType._uint, parameters = new List<string>() });
        addField = new Field() { name = "points", type = Field.FieldType.colorArr, parameters = new List<string>() };
    }

    void CustomValidate()
    {
        string val = Read(fields[0]);
        if (val != "")
        {
            numAddFields = System.Math.Min(15u, uint.Parse(val));
            Write(fields[0], numAddFields.ToString());
        }
    }
}
