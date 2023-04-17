using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CalibrateWelcome : MonoBehaviour
{

    public Button yourButton;
    public GameObject self;
    public GameObject manager;

    private RectTransform panelRectTransform;

    public GameObject calibrateWelcomePanel;

    public GameObject calibrationArena;

    // Start is called before the first frame update
    void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();
        panelRectTransform.anchoredPosition = new Vector2(353, 75);

        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(ButtonOnClick);

        self.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ButtonOnClick()
    {
        calibrationArena.SetActive(true);

        var asdf = calibrationArena.GetComponent<CalibrationArena>(); //.RunCalibration();

        asdf.RunCalibration();

        calibrateWelcomePanel.SetActive(false);
    }
}
