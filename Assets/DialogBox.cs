using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable()]
public class Field
{
    public enum FieldType
    {
        _float, _int, _uint, dropdown, slider, text, button, displayText, colorArr
    }

    public FieldType type;
    public string name;
    public List<string> parameters;

    [System.NonSerialized]
    public GameObject fieldValueRead;
    [System.NonSerialized]
    public GameObject reference;
}

[System.Serializable()]
public class IOImage
{
    public RenderTexture image;
    public DialogBox reference;
    public bool ready;

    public IOImage(int resolution, DialogBox refer)
    {
        image = new RenderTexture(resolution, resolution, 0);
        image.enableRandomWrite = true;
        image.Create();
        ready = false;
        reference = refer;
    }

    public void Clear()
    {
        ready = false;
        if (image != null)
            Object.DestroyImmediate(image, true);
    }
}

public class DialogBox : MonoBehaviour
{
    public List<IOImage> inputs;
    public IOImage output;
    public bool readyInputs
    {
        get
        {
            for (int i = 0; i < inputs.Count; i++)
                if (!inputs[i].ready || inputs[i] == null)
                    return false;
            return true;
        }
    }

    public System.Action startFunction = () => { };
    public System.Action updateFunction = () => { };

    public List<Field> fields;
    public GameObject prefabTextField;
    public GameObject prefabDropDown;
    public GameObject prefabColorArr;
    public GameObject prefabDisplayText;

    RectTransform rectTr;
    float fieldPositions;
    bool holdingDownMouse;
    Vector3 relMousePos;

    // Start is called before the first frame update
    void Start()
    {
        rectTr = GetComponent<RectTransform>();
        Initialize();
        startFunction();
    }

    void Initialize()
    {
        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 110f + fields.Count * 30f);
        fieldPositions = 80f;
        Text txt = GetComponentInChildren<Text>();
        txt.text = name;
        foreach (Field f in fields)
            InitializeField(f);
    }

    public void AddField(Field field)
    {
        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 110f + fields.Count * 30f);
        fields.Add(field);
        InitializeField(field);
    }
    
    public void RemoveField(Field field)
    {
        int index = fields.FindIndex(0, fields.Count, a => a == field);
        for (int i = index; i < fields.Count; i++)
            fields[i].reference.transform.position += Vector3.up * 30f;
        fields.Remove(field);
        DestroyImmediate(field.reference);
        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 110f + fields.Count * 30f);
    }

    void InitializeField(Field field)
    {
        if (field.fieldValueRead != null || field.reference != null)
            return;

        if (field.type == Field.FieldType.displayText)
        {
            GameObject g = Instantiate(prefabDisplayText, transform);
            g.name = field.name;
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -fieldPositions);
            g.GetComponent<Text>().text = field.parameters[0];
            field.fieldValueRead = g;
            field.reference = g;
        }
        if (field.type == Field.FieldType.colorArr)
        {
            GameObject g = Instantiate(prefabColorArr, transform);
            g.name = field.name;
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(-110f, -fieldPositions);
            field.fieldValueRead = g;
            field.reference = g;
        }
        if (field.type == Field.FieldType._uint || field.type == Field.FieldType._int || field.type == Field.FieldType._float || field.type == Field.FieldType.text)
        {
            GameObject g = Instantiate(prefabTextField, transform);
            g.name = field.name;
            g.GetComponent<Text>().text = field.name + ":";
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(-57f, -fieldPositions);
            InputField f = g.GetComponentInChildren<InputField>();
            Text[] texts = g.GetComponentsInChildren<Text>();
            field.fieldValueRead = f.gameObject;
            switch (field.type)
            {
                case Field.FieldType._float:
                    f.contentType = InputField.ContentType.DecimalNumber;
                    break;
                case Field.FieldType._uint:
                    f.contentType = InputField.ContentType.IntegerNumber;
                    break;
                case Field.FieldType._int:
                    f.contentType = InputField.ContentType.IntegerNumber;
                    break;
            }
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].gameObject.name == "Placeholder")
                    texts[i].text = "Enter " + field.type.ToString().TrimStart('_') + "...";
            }
            field.reference = g;
        }
        if (field.type == Field.FieldType.dropdown)
        {
            GameObject g = Instantiate(prefabDropDown, transform);
            g.name = field.name;
            g.GetComponent<Text>().text = field.name + ":";
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(-57f, -fieldPositions);
            Dropdown drp = g.GetComponentInChildren<Dropdown>();
            field.fieldValueRead = drp.gameObject;
            foreach (string s in field.parameters)
                drp.options.Add(new Dropdown.OptionData(s));
            field.reference = g;
        }
        fieldPositions += 30f;
    }

    public static string Read(Field field)
    {
        if (field.type == Field.FieldType._uint || field.type == Field.FieldType._int || field.type == Field.FieldType._float || field.type == Field.FieldType.text)
            return field.fieldValueRead.GetComponent<InputField>().text;
        switch (field.type)
        {
            case Field.FieldType.dropdown:
                return field.parameters[field.fieldValueRead.GetComponent<Dropdown>().value];
            case Field.FieldType.displayText:
                return field.parameters[0];
            case Field.FieldType.colorArr:
                return field.fieldValueRead.GetComponent<ColorValues>().text;
            case Field.FieldType.slider:
                return field.fieldValueRead.GetComponent<Slider>().value.ToString();
        }
        return "";
    }

    public static void Validate(Field field)
    {
        if (field.type == Field.FieldType.slider || field.type == Field.FieldType.displayText || field.type == Field.FieldType.dropdown || field.type == Field.FieldType.button)
            return; // No need to validate; no input to be processed.
        if (field.type == Field.FieldType.colorArr)
        {
            field.fieldValueRead.GetComponent<ColorValues>().Validate();
            return;
        }
        InputField toRead = field.fieldValueRead.GetComponent<InputField>();
        if (toRead.text == "")
            return;
        switch (field.type)
        {
            case Field.FieldType._float:
                float tryReadFloat;
                float.TryParse(toRead.text, out tryReadFloat);
                toRead.text = tryReadFloat.ToString();
                break;
            case Field.FieldType._int:
                int tryReadInt;
                int.TryParse(toRead.text, out tryReadInt);
                toRead.text = tryReadInt.ToString();
                break;
            case Field.FieldType._uint:
                uint tryReadUInt;
                uint.TryParse(toRead.text, out tryReadUInt);
                toRead.text = tryReadUInt.ToString();
                break;
        }
    }

    private void OnDestroy()
    {
        foreach (IOImage i in inputs)
            i.Clear();
        output.Clear();
        inputs.Clear();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && holdingDownMouse)
            transform.position = Input.mousePosition - relMousePos;
        else
            relMousePos = Input.mousePosition - transform.position;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 delta = Input.mousePosition - transform.position;
            Vector3 delta2 = new Vector3();
            delta2.x = Mathf.Clamp(delta.x, 52f - .5f * rectTr.rect.width, .5f * rectTr.rect.width - 52f);
            delta2.y = Mathf.Clamp(delta.y, 52f - .5f * rectTr.rect.height, .5f * rectTr.rect.height - 52f);
            holdingDownMouse = (delta - delta2).magnitude < 52f;
        }
        holdingDownMouse &= !Input.GetMouseButtonUp(0);
        transform.position -= new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * 20f;

        if (Input.GetKeyDown(KeyCode.Return))
            foreach (Field f in fields)
                Validate(f);

        updateFunction();
    }
}
