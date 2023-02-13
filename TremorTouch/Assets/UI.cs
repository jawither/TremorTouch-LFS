using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI : StandaloneInputModule
{

    public void ProcessClick(float x, float y)
    {
        var pointerData = GetTouchPointerEventData(new Touch()
        {
            position = new Vector2(x, y),
        }, out bool b, out bool bb);

        ProcessTouchPress(pointerData, true, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
