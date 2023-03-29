using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NumberButton : MonoBehaviour
{

    public int buttonNumber;
    public Button buttonReference;
    public GameObject textField;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = buttonReference.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnClick()
    {
        TMP_Text text = textField.GetComponent<TMP_Text>();
        text.text = text.text + buttonNumber;
    }
}
