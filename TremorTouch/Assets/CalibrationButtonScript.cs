using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationButtonScript : MonoBehaviour
{

    private Button button;
    public GameObject manager;
    public GameObject calibratePanel;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick()
    {
        manager.GetComponent<Manager>().calibrating = true;
        calibratePanel.SetActive(true);
    }

}
