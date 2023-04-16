using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class CalibrationArena : MonoBehaviour
{

    private RectTransform panelRectTransform;
    public GameObject self;
    public GameObject manager;

    public float time = 0;
    private bool recievingCalibratingTaps = false;

    private int numInputs = 0;

    private Button btn;

    private List<GameObject> CalibrateTaps = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();
        panelRectTransform.anchoredPosition = new Vector2(0, 0);

        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        self.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(recievingCalibratingTaps)
        {
            
            if(numInputs >= 20)
            {
                recievingCalibratingTaps = false;

                Vector2 meanResult = GetMeanPosition();
                Vector2 weightedResult = GetWeightedMeanPosition();

                float meanDistance = Vector2.Distance(self.transform.position, meanResult);
                float weightedDistance = Vector2.Distance(self.transform.position, weightedResult);

                Debug.Log("MEAN");
                Debug.Log(meanDistance);
                Debug.Log("WEIGHTED");
                Debug.Log(weightedDistance);

                if(meanDistance <= weightedDistance) //mean is better for user
                {
                    manager.GetComponent<Manager>().alg = Manager.Algorithm.Base;

                    float xOffset = Mathf.Abs(meanResult.x - self.transform.position.x);
                    float yOffset = Mathf.Abs(meanResult.y - self.transform.position.y);
                }
                else //weighted is better for user
                {
                    manager.GetComponent<Manager>().alg = Manager.Algorithm.Weighted;

                    float xOffset = Mathf.Abs(weightedResult.x - self.transform.position.x);
                    float yOffset = Mathf.Abs(weightedResult.y - self.transform.position.y);
                }

                CalibrateTaps.Clear();
                numInputs = 0;


                manager.GetComponent<Manager>().calibrating = false;
                self.SetActive(false);
            }

        }
        //time += Time.deltaTime;
        //Debug.Log(time);
    }

    public void RunCalibration()
    {
        recievingCalibratingTaps = true;
    }

    void OnClick()
    {
        numInputs = numInputs + 1;

        // Create new tap location at mouse
        GameObject asdf = new GameObject();
        Vector3 destination = Input.mousePosition;
        GameObject newLocation = Instantiate(asdf, destination,
            Quaternion.identity, self.transform);
        CalibrateTaps.Add(newLocation);

        Debug.Log(numInputs);
    }

    Vector2 GetWeightedMeanPosition()
    {

        var items = Enumerable.Range(1, CalibrateTaps.Count);

        float sum = (float)items.Select(i => 1.0 / i).Sum();


        float x = 0;
        float y = 0;

        int Denominator = CalibrateTaps.Count;

        foreach (GameObject location in CalibrateTaps)
        {
            x += (location.transform.position.x / (Denominator * sum));
            y += (location.transform.position.y / (Denominator * sum));
            Denominator -= 1;
        }

        return new Vector2(x, y);
    }

    Vector2 GetMeanPosition()
    {
        float x = 0;
        float y = 0;

        foreach (GameObject location in CalibrateTaps)
        {
            x += location.transform.position.x;
            y += location.transform.position.y;
        }

        return new Vector2(x / CalibrateTaps.Count, y / CalibrateTaps.Count);
    }
}
