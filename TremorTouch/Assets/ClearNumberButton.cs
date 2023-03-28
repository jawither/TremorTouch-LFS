using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClearNumberButton : MonoBehaviour
{

    public Button button;
    public GameObject textField;

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
        TMP_Text text = textField.GetComponent<TMP_Text>();
        text.text = "";
    }

}
