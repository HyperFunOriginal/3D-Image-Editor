using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorValues : MonoBehaviour
{
    public string text => L.text + "_" + R.text + "_" + G.text + "_" + B.text + "_" + A.text;
    public InputField L;
    public InputField R;
    public InputField G;
    public InputField B;
    public InputField A;

    public void Validate()
    {
        float tryReadFloat;
        if (L.text != "")
        {
            float.TryParse(L.text, out tryReadFloat);
            L.text = tryReadFloat.ToString();
        }
        if (R.text != "")
        {
            float.TryParse(R.text, out tryReadFloat);
            R.text = tryReadFloat.ToString();
        }
        if (G.text != "")
        {
            float.TryParse(G.text, out tryReadFloat);
            G.text = tryReadFloat.ToString();
        }
        if (B.text != "")
        {
            float.TryParse(B.text, out tryReadFloat);
            B.text = tryReadFloat.ToString();
        }
        if (A.text != "")
        {
            float.TryParse(A.text, out tryReadFloat);
            A.text = tryReadFloat.ToString();
        }
    }
}
