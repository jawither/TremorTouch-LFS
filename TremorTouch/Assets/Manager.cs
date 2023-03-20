using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class Manager : MonoBehaviour
{

    //Types of algorithm
    enum Algorithm
    {
        Base,
        Weighted
    }

    // Settings variables to change from within app
    Algorithm alg;

    public Toggle AlgorithmToggle;
    public Toggle settingsToggle;
    public Slider cacheSizeSlider;
    public Slider minTapsSlider;
    public Slider maxTimeBetweenTapsSlider;
    bool visible = false;

    // Tunable parameters
    static int cacheSize = 8;
    static int minTaps = 3;
    static float maxTimeBetweenTaps = 0.8f;

    //Option flags
    bool colorTapsOnRecencyFlag = true;
    bool weightedModeFlag = true;

    // Manager vars
    float timeSinceLastTap = 0f;
    int totalTaps = 0;
    int numTapsOnExecute = 0;
    float timeSinceExecutedTap = 0f;
    bool toExecuteTap = false;
    List<GameObject> cache;
    public GameObject locationPrefab;
    public GameObject meanPrefab;
    GameObject mean;
    Color waiting;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    GameObject canvas;


    // Start: Called before the first frame update by Unity.
    void Start()
    {
        Assert.IsTrue(minTaps >= 1);
        alg = Algorithm.Base;

        canvas = GameObject.Find("Canvas");
        cache = new List<GameObject>(cacheSize);
        waiting = new Color(255f, 0f, 0f, 0.5f);

        m_Raycaster = canvas.GetComponent<GraphicRaycaster>();
        m_EventSystem = canvas.GetComponent<EventSystem>();

        mean = GameObject.Instantiate(meanPrefab, transform.position,
            Quaternion.identity, canvas.transform);

        mean.GetComponent<Canvas>().overrideSorting = true;
        mean.GetComponent<Canvas>().sortingOrder = 5;

        Settings();

        Reset();
    }


      // Update: Called once per frame by Unity.
    void Update()
    {
        // Receive user input
        if (Input.GetButtonDown("Fire1")){
            totalTaps += 1;
            ReceiveUserTap();
        }

        // Exit if cache is empty
        if (cache.Count == 0) return;

        // Exit if clock hasn't yet expired
        timeSinceLastTap += Time.deltaTime;
        if (timeSinceLastTap < maxTimeBetweenTaps) return;

        // Reset if clock expired but not enough taps
        if (cache.Count < minTaps)
        {
            Reset();
            return;
        }

        // Issue tap if clock expired and enough taps
        if (cache.Count <= cacheSize)
        {
            if (!toExecuteTap){
                timeSinceExecutedTap = timeSinceLastTap;
                numTapsOnExecute = totalTaps;
                toExecuteTap = true;
            }
            else if(timeSinceLastTap - timeSinceExecutedTap >= 3f){
                if (totalTaps > numTapsOnExecute){
                    IssueHoldToSystem();
                }
                else{
                    IssueTapToSystem();
                }
                toExecuteTap = false;
            }
            return;
        }


        // We should never get here
        Assert.IsTrue(false);
    }



    // ReceiveUserTap: called on frame that user taps the screen.
    // Updates Manager metadata.

    void ReceiveUserTap()
    {
        // Reset clock
        timeSinceLastTap = 0f;

        // If cache at capcity, remove oldest tap
        if (cache.Count == cacheSize)
        {
            GameObject.Destroy(cache[0]);
            cache.RemoveAt(0);
        }

        // Create new tap location at mouse
        Vector3 destination = Input.mousePosition;
        GameObject newLocation = Instantiate(locationPrefab, destination,
            Quaternion.identity, canvas.transform);
        cache.Add(newLocation);

        // Make mean location visible and update its location iff new tap count > minTaps
        if (cache.Count < minTaps)
        {
            SetMeanColor(Color.clear);
        }
        else
        {
            SetMeanColor(waiting);
            if(colorTapsOnRecencyFlag) ColorTapsOnRecency();
            switch (alg)
            {
                case Algorithm.Base:
                    mean.transform.position = GetMeanPosition();
                    break;
                case Algorithm.Weighted:
                    mean.transform.position = GetWeightedMeanPosition();
                    break;
            }
        }

    }


    // GetMeanPosition: Returns 2d coordinates that is the mean of all positions
    // of taps in the cache.

    Vector2 GetMeanPosition()
    {
        float x = 0;
        float y = 0;

        foreach (GameObject location in cache)
        {
            x += location.transform.position.x;
            y += location.transform.position.y;
        }

        return new Vector2(x / cache.Count, y / cache.Count);
    }

    Vector2 GetWeightedMeanPosition()
    {

        var items = Enumerable.Range(1, cache.Count);

        float sum = (float)items.Select(i => 1.0 / i).Sum();


        float x = 0;
        float y = 0;

        int Denominator = cache.Count;

        foreach (GameObject location in cache)
        {
            x += (location.transform.position.x / (Denominator * sum));
            y += (location.transform.position.y / (Denominator * sum));
            Denominator -= 1;
        }

        return new Vector2(x, y);
    }

    Vector2 GetWeightedMeanPositionSecondImplementation()
    {
        float x = 0;
        float y = 0;

        float weight = 0.1f;
        float weightMultiplier = - cache.Count / 2;

        foreach (GameObject location in cache)
        {
            x += location.transform.position.x * (weightedModeFlag ? (1 + weight * weightMultiplier) : 1);
            y += location.transform.position.y * (weightedModeFlag ? (1 + weight * weightMultiplier) : 1);

            if(cache.Count % 2 == 0 && (weightMultiplier  ==  -1)) weightMultiplier += 1;
            weightMultiplier += 1;
        }

        return new Vector2(x / cache.Count, y / cache.Count);
    }


    // Reset: Empties cache and makes mean invisible.

    void Reset()
    {
        SetMeanColor(Color.clear);
        ClearCache();
    }


    // IssueTapToSystem: Activate UI elements at the location of the mean.

    void IssueTapToSystem()
    {

        // Get list of all UI elements at mean's location
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = mean.transform.position;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        // Issue a click to each of those UI elements
        foreach (RaycastResult result in results)
        {
            ExecuteEvents.Execute(result.gameObject,
                new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }

        // Make mean red and empty cache
        SetMeanColor(Color.red);
        ClearCache();
    }

    void IssueHoldToSystem()
    {
        SetMeanColor(Color.green);
        ClearCache();
    }


    // ClearCache: clears cache and deallocates memory for stored lcoations.

    void ClearCache()
    {
        foreach (GameObject g in cache)
        {
            GameObject.Destroy(g);
        }

        cache.Clear();
    }


    // SetMeanColor: Sets mean to a color.

    void SetMeanColor(Color color)
    {
        mean.GetComponent<Image>().color = color;
    }

    // ColorTapsOnRecency: Makes more recent taps have higher opacity

    void ColorTapsOnRecency()
    {
        float weight = 1f / (cacheSize * 2);
        for(var i = cache.Count - 1; i >= 0; i--)
        {
            var val = 1 - weight * (cache.Count - i);
            cache[i].GetComponent<Image>().color = new Color(1, 1, 1, val);
        }
    }

    // UI toggle for changing algorithm from base to weighted

    void AlgorithmValueChanged()
    {
        if (alg == Algorithm.Base) {
            alg = Algorithm.Weighted;
        }
        else if (alg == Algorithm.Weighted) {
            alg = Algorithm.Base;
        }
    }


    // UI toggle that sets visibility for sliders on and off.

    void SettingValueChanged() {
        if (visible == false)
        {
        cacheSizeSlider.gameObject.SetActive(true);
        minTapsSlider.gameObject.SetActive(true);
        maxTimeBetweenTapsSlider.gameObject.SetActive(true);
        visible = true;
        }
        else {
        cacheSizeSlider.gameObject.SetActive(false);
        minTapsSlider.gameObject.SetActive(false);
        maxTimeBetweenTapsSlider.gameObject.SetActive(false);
        visible = false;
        }

    }


    // UI slider for changing cache size

    void UpdateCacheSize()
    {
        cacheSize = Mathf.RoundToInt(cacheSizeSlider.value * 8);
        print(cacheSize);
    }


    // UI slider for changing minimum number of taps

    void UpdateMinTaps()
    {
        minTaps = Mathf.CeilToInt(minTapsSlider.value * 2);
        print(minTaps);
    }


    // UI slider for changing max time between taps

    void UpdateMaxTimeBetweenTaps()
    {
        maxTimeBetweenTaps = maxTimeBetweenTapsSlider.value * 0.8f;
        print(maxTimeBetweenTaps);
    }

    void Settings()
    {
        AlgorithmToggle.onValueChanged.AddListener( delegate {AlgorithmValueChanged();});
        settingsToggle.onValueChanged.AddListener( delegate {SettingValueChanged();});

        cacheSizeSlider.value = cacheSize;
        cacheSizeSlider.minValue = 0.2f;
        cacheSizeSlider.onValueChanged.AddListener( delegate {UpdateCacheSize();});
        cacheSizeSlider.gameObject.SetActive(false);

        minTapsSlider.value = minTaps;
        minTapsSlider.minValue = 0.5f;
        minTapsSlider.onValueChanged.AddListener( delegate {UpdateMinTaps();});
        minTapsSlider.gameObject.SetActive(false);

        maxTimeBetweenTapsSlider.value = maxTimeBetweenTaps;
        maxTimeBetweenTapsSlider.minValue = 0.5f;
        maxTimeBetweenTapsSlider.onValueChanged.AddListener( delegate {UpdateMaxTimeBetweenTaps();});
        maxTimeBetweenTapsSlider.gameObject.SetActive(false);
    }
}
