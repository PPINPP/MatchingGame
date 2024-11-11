using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using TMPro;
using UnityEngine;

public class ChangeOutline : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtfont;
    [SerializeField] Material fontmat;
    private Material matscript;
    // Start is called before the first frame update
    void Start()
    {
        Color ncolor = new Color(0.1f,0.1f,0.1f);
        txtfont.outlineColor = ncolor;
        // var rend = txtfont.GetComponent<Renderer>();
        // matscript = rend.material;
        // Color ncolor = new Color(0.1f,0.1f,0.1f);
        // matscript.SetColor("_OutlineColor", ncolor );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
