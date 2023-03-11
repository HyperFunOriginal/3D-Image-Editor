using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorValues : MonoBehaviour
{
    public string text
    {
        get
        {
            return L.text + "_" + R.text + "_" + G.text + "_" + B.text + "_" + A.text;
        }
        set
        {
            KeyColorPair k = new KeyColorPair(value);
            L.text = k.value.ToString();
            R.text = k.color.x.ToString();
            G.text = k.color.y.ToString();
            B.text = k.color.z.ToString();
            A.text = k.color.w.ToString();
        }
    }
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

    public struct KeyColorPair
    {
        public readonly float value;
        public readonly Vector4 color;

        public KeyColorPair(string key)
        {
            string[] sep = key.Split('_');
            float.TryParse(sep[0], out value);
            Vector4 v = new Vector4();
            for (int i = 0; i < 4; i++)
            {
                float.TryParse(sep[i + 1], out float _0);
                v[i] = _0;
            }
            color = v;
        }

        public override string ToString()
        {
            return value + ": " + color.ToString();
        }
    }
}
