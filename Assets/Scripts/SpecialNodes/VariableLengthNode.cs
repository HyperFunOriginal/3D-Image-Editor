using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableLengthNode : BaseNode
{
    public Field addField;
    public uint numAddFields;
    public List<Field> additionalFields;
    // Start is called before the first frame update
    public virtual void OnEnable()
    {
        numAddFields = 0;
        additionalFields = new List<Field>();
        updateFunction += CustomUpdate;
    }

    public virtual void OnDisable()
    {
        updateFunction -= CustomUpdate;
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
