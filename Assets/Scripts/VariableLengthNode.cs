using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableLengthNode : DialogBox
{
    public Field addField;
    public uint numAddFields;
    internal List<Field> additionalFields;
    // Start is called before the first frame update
    public virtual void OnEnable()
    {
        numAddFields = 0;
        updateFunction += CustomUpdate;
        startFunction += CustomStart;
    }

    public virtual void OnDisable()
    {
        updateFunction -= CustomUpdate;
        startFunction -= CustomStart;
    }

    void CustomStart()
    {
        additionalFields = new List<Field>();
    }

    void CustomUpdate()
    {
        for (int i = additionalFields.Count - 1; i >= numAddFields; i--)
        {
            RemoveField(additionalFields[i]);
            additionalFields.RemoveAt(i);
        }   
        for (int i = 0; i < numAddFields - additionalFields.Count; i++)
        {
            Field cpy = new Field(addField);
            additionalFields.Add(cpy);
            AddField(cpy);
        }
    }
}
