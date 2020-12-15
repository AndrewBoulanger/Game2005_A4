using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TextSetter : MonoBehaviour
{
    public Text textbox;
    // Start is called before the first frame update
    void Start()
    {
        textbox = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setText(float val)
    {
        textbox.text = val.ToString();
    }
}
