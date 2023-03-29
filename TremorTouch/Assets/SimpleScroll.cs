using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleScroll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool held = false;
    GameObject managerMean;
    float offset = 0;
    Vector3 startMeanPos;

    // Start is called before the first frame update
    void Start()
    {
        print("Constructing SimpleDrag");
        managerMean = GameObject.Find("Manager").GetComponent<Manager>().mean;

    }

    // Update is called once per frame
    void Update()
    {
        if (held)
        {
            GetComponent<ScrollRect>().verticalNormalizedPosition = Mathf.Lerp(
                GetComponent<ScrollRect>().verticalNormalizedPosition, offset +
                (startMeanPos.y - managerMean.transform.position.y) /1000, Time.deltaTime * 8);
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (held) return;

        held = true;
        startMeanPos = managerMean.transform.position;
        offset = GetComponent<ScrollRect>().verticalNormalizedPosition;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        held = false;
    }
}
