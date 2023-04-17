using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SimpleToggle : MonoBehaviour
{
    GameObject managerMean;
    Vector3 offset = Vector3.zero;

    public bool restrictX = false;
    public bool restrictY = false;

    float startX;
    float startY;

    public bool value = false;

    public Color offColor;
    public Color onColor;

    // Start is called before the first frame update
    void Start()
    {

        managerMean = GameObject.Find("Manager").GetComponent<Manager>().mean;

        startX = transform.position.x;
        startY = transform.position.y;

        Set(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Set(bool val)
    {
        print("Setting to " + val);
        value = val;

        Color dest = offColor;
        if(val) { dest = onColor; }

        var colors = GetComponent<Button>().colors;
        colors.normalColor = dest;
        GetComponent<Button>().colors = colors;
    }

    public void Toggle(int x)
    {
        if(value) { Set(false); }
        else { Set(true); }
    }

}
