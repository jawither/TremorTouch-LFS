using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
    private Button button;
    public GameObject manager;

    public GameObject helpPanel;

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
        manager.GetComponent<Manager>().firstUse = true;
        helpPanel.SetActive(true);
    }
}
