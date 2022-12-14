using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable()]
public class Field
{
    public enum FieldType
    {
        _float, _int, _uint, dropdown, text, displayText, colorArr, _bool
    }

    public FieldType type;
    public string name;
    public List<string> parameters;

    [System.NonSerialized]
    public GameObject fieldValueRead;
    [System.NonSerialized]
    public GameObject reference;

    public Field()
    {
    }

    public Field(Field original)
    {
        fieldValueRead = null;
        reference = null;
        parameters = new List<string>(original.parameters);
        name = original.name;
        type = original.type;
    }
}

[System.Serializable()]
public class IOImage
{
    public RenderTexture image;
    public CompletionState state = CompletionState.unready;

    public enum CompletionState
    {
        ready, unready, failed
    }

    public IOImage()
    {
        state = CompletionState.unready;
    }
    public IOImage(int resolution)
    {
        image = new RenderTexture(resolution, resolution, 0);
        image.enableRandomWrite = true;
        image.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat;
        image.Create();
        state = CompletionState.unready;
    }

    public void Clear()
    {
        state = CompletionState.unready;
        if (image != null)
            Object.DestroyImmediate(image, true);
    }
}

public class DialogBox : MonoBehaviour
{
    public List<DialogBox> inputs;
    public IOImage output;
    public bool readyInputs
    {
        get
        {
            for (int i = 0; i < inputs.Count; i++)
                if (inputs[i] == null || inputs[i].output.state != IOImage.CompletionState.ready)
                    return false;
            return true;
        }
    }

    public System.Action startFunction = () => { };
    public System.Action updateFunction = () => { };
    public System.Action onValidate = () => { };
    public System.Action runFunction = () => { };

    public static Color successColor = Color.Lerp(Color.white, Color.green, 0.4f);
    public static Color failureColor = Color.Lerp(Color.white, Color.red, 0.6f);
    public static Color unreadyColor = Color.white;

    public List<Field> fields;
    public GameObject prefabTextField => (GameObject)Resources.Load("TextField");
    public GameObject prefabDropDown => (GameObject)Resources.Load("DropDown");
    public GameObject prefabColorArr => (GameObject)Resources.Load("ColorArray");
    public GameObject prefabDisplayText => (GameObject)Resources.Load("TextDisplay");
    public GameObject prefabBoolean => (GameObject)Resources.Load("Toggle");
    public GameObject prefabInput => (GameObject)Resources.Load("NodeIn");
    public GameObject runtimeTransform;

    RectTransform rectTr;
    float fieldPositions;
    bool holdingDownMouse;
    [System.NonSerialized]
    public bool finished;
    Vector3 relMousePos, MousePos;
    Image dialogboxImg;

    public void SetInput(DialogBox reference, int index) => inputs[index] = reference;

    // Start is called before the first frame update
    void Start()
    {
        dialogboxImg = GetComponentInChildren<Image>();
        rectTr = GetComponent<RectTransform>();
        Initialize();
        onValidate += OnValidation;
        startFunction();
        InitializeInputs();
    }

    void InitializeInputs()
    {
        for (int i = 0; i < inputs.Count; i++)
        {
            GameObject g = Instantiate(prefabInput, runtimeTransform.transform);
            float y = -200f * (i - 0.5f * (inputs.Count - 1f)) / (inputs.Count + 1.0f);
            g.transform.SetAsFirstSibling();
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(-8, y);
            g.GetComponent<NodeInput>().index = i;
            g.GetComponentInChildren<Text>().text = (i + 1).ToString();
        }
    }

    void Initialize()
    {
        output = new IOImage();
        runtimeTransform = new GameObject("Procedural");
        runtimeTransform.transform.SetParent(transform);
        RectTransform r = runtimeTransform.AddComponent<RectTransform>();
        r.anchorMax = Vector2.one;
        r.anchorMin = Vector2.zero;
        r.sizeDelta = Vector2.zero;
        r.localScale = Vector3.one;

        runtimeTransform.transform.localPosition = Vector3.zero;

        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 108f + fields.Count * 30f);
        fieldPositions = 68f;
        Text txt = GetComponentInChildren<Text>();
        txt.text = name;
        foreach (Field f in fields)
            InitializeField(f);
    }

    public bool HasField(string name) => fields.FindAll(a => a.name == name).Count != 0;

    public void AddField(Field field)
    {
        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 108f + fields.Count * 30f);
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
        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 108f + fields.Count * 30f);
        fieldPositions -= 30f;
    }

    void InitializeField(Field field)
    {
        if (field.fieldValueRead != null || field.reference != null)
            return;

        if (field.type == Field.FieldType.displayText)
        {
            GameObject g = Instantiate(prefabDisplayText, runtimeTransform.transform);
            g.name = field.name;
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -fieldPositions);
            g.GetComponent<Text>().text = field.parameters[0];
            field.fieldValueRead = g;
            field.reference = g;
        }
        if (field.type == Field.FieldType._bool)
        {
            GameObject g = Instantiate(prefabBoolean, runtimeTransform.transform);
            g.name = field.name;
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(-22f, -fieldPositions);
            g.GetComponent<Text>().text = field.name + ":";
            field.fieldValueRead = g.GetComponentInChildren<Toggle>().gameObject;
            field.reference = g;
        }
        if (field.type == Field.FieldType.colorArr)
        {
            GameObject g = Instantiate(prefabColorArr, runtimeTransform.transform);
            g.name = field.name;
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(-110f, -fieldPositions);
            field.fieldValueRead = g;
            field.reference = g;
        }
        if (field.type == Field.FieldType._uint || field.type == Field.FieldType._int || field.type == Field.FieldType._float || field.type == Field.FieldType.text)
        {
            GameObject g = Instantiate(prefabTextField, runtimeTransform.transform);
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
            GameObject g = Instantiate(prefabDropDown, runtimeTransform.transform);
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

    public static void Write(Field field, string value)
    {
        if (field.type == Field.FieldType._uint || field.type == Field.FieldType._int || field.type == Field.FieldType._float || field.type == Field.FieldType.text)
            field.fieldValueRead.GetComponent<InputField>().text = value;
        switch (field.type)
        {
            case Field.FieldType.dropdown:
                int index = field.parameters.FindIndex(0, field.parameters.Count, a => a == value);
                field.fieldValueRead.GetComponent<Dropdown>().value = index;
                break;
            case Field.FieldType._bool:
                field.fieldValueRead.GetComponent<Toggle>().isOn = bool.Parse(value);
                break;
        }
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
            case Field.FieldType._bool:
                return field.fieldValueRead.GetComponent<Toggle>().isOn.ToString();
        }
        return "";
    }

    public static void Validate(Field field)
    {
        if (field.type == Field.FieldType.displayText || field.type == Field.FieldType.dropdown || field.type == Field.FieldType._bool)
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
        output.Clear();
        inputs.Clear();
    }

    public bool HoveringOver()
    {
        Vector3 delta = (Input.mousePosition - transform.position) / CanvasZoom.zoom;
        Vector3 delta2 = new Vector3();
        delta2.x = Mathf.Clamp(delta.x, 15f - .5f * rectTr.rect.width, .5f * rectTr.rect.width - 15f);
        delta2.y = Mathf.Clamp(delta.y, 15f - .5f * rectTr.rect.height, .5f * rectTr.rect.height - 15f);
        return (delta - delta2).magnitude < 15f;
    }

    void OnValidation()
    {
        foreach (Field f in fields)
            Validate(f);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && holdingDownMouse)
        {
            if (EditorLogic.selected == null || EditorLogic.layer < transform.GetSiblingIndex())
            {
                EditorLogic.selected = this;
                EditorLogic.layer = transform.GetSiblingIndex();
            }
        }
        else
        {
            if (!CanvasZoom.zooming && !Input.GetMouseButton(1))
                transform.position = RoundToTens(transform.position / CanvasZoom.zoom) * CanvasZoom.zoom;
            relMousePos = Input.mousePosition - transform.position;
        }

        if (Input.GetMouseButtonDown(0))
            holdingDownMouse = HoveringOver();
        holdingDownMouse &= !Input.GetMouseButtonUp(0) && !Input.GetKeyDown(KeyCode.Delete) && !Input.GetKeyDown(KeyCode.Backspace);

        if (Input.GetKeyDown(KeyCode.Return))
            onValidate();

        if (Input.GetMouseButton(1))
            transform.position += Input.mousePosition - MousePos;
        MousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(2) && HoveringOver())
            Duplicate();

        updateFunction();
    }

    void Duplicate()
    {
        DialogBox copy = Instantiate(gameObject, gameObject.transform.parent).GetComponent<DialogBox>();
        copy.transform.position += Vector3.down * 30f;
        copy.name = gameObject.name;
        DestroyImmediate(copy.runtimeTransform);
        copy.fields.Clear();
    }

    private void LateUpdate()
    {
        if (EditorLogic.selected == this)
        {
            transform.position = Input.mousePosition - relMousePos;
            transform.SetAsLastSibling();
        }
        if (EditorLogic.run && readyInputs && !finished)
        {
            onValidate();
            try
            {
                runFunction();
                output.state = IOImage.CompletionState.ready;
            }
            catch (System.Exception e)
            {
                output.state = IOImage.CompletionState.failed;
                Debug.LogError(e);
            }
            finished = true;
        }
        if (!EditorLogic.run)
        {
            if (finished)
                output.Clear();
            finished = false;
        }

        if (output == null || output.state == IOImage.CompletionState.unready)
            dialogboxImg.color = unreadyColor;
        else if (output.state == IOImage.CompletionState.ready)
            dialogboxImg.color = successColor;
        else
            dialogboxImg.color = failureColor;
    }

    Vector3 RoundToTens(Vector3 vector)
    {
        vector /= 10f;
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y)) * 10f;
    }
}
