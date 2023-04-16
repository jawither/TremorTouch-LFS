using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;


public class SimpleSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool held = false;
    GameObject managerMean;
    Vector3 offset = Vector3.zero;

    public bool restrictX = false;
    public bool restrictY = false;

    float startX;
    float startY;

    public float value;
    public float minValue;
    public float maxValue;
    public bool useInt = false;

    static float physicalRange;
    TextMeshProUGUI valueText;

    // Start is called before the first frame update
    void Start()
    {
        print("Constructing SimpleDrag");
        managerMean = GameObject.Find("Manager").GetComponent<Manager>().mean;

        startX = transform.localPosition.x;
        startY = transform.localPosition.y;

        physicalRange = transform.parent.Find("Image").GetComponent<RectTransform>().rect.width;

        valueText = transform.parent.parent.Find("Name and value").Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (held)
        {
            transform.position =
                Vector3.Lerp(transform.position, managerMean.transform.position + offset, Time.deltaTime * 20f);
        }

        if (restrictX)
        {
            transform.localPosition = new Vector3(startX, transform.localPosition.y, transform.localPosition.z);
        }

        if (restrictY)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, startY, transform.localPosition.z);
        }

        float valLerp = (transform.localPosition.x + (physicalRange / 2)) / physicalRange;
        value = Mathf.Lerp(minValue, maxValue, valLerp);
        if (useInt)
            value = Mathf.Floor(value);
        valueText.text =(Mathf.Round(value * 10.0f) * 0.1f).ToString();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (held) return;

        held = true;
        offset = transform.position - managerMean.transform.position;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        held = false;
    }
}
