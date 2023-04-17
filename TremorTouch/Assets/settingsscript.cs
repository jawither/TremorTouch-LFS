using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingsscript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform panelRectTransform;
        panelRectTransform = GetComponent<RectTransform>();
        panelRectTransform.anchoredPosition = new Vector2(335, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
