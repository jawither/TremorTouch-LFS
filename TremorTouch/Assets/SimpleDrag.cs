using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SimpleDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool held = false;
    GameObject managerMean;
    Vector3 offset = Vector3.zero;

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
            transform.position =
                Vector3.Lerp(transform.position, managerMean.transform.position + offset, Time.deltaTime * 100f);
        }
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
