using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System.Threading;

public class Manager : MonoBehaviour
{

    //Types of algorithm
    public enum Algorithm
    {
        Base,
        Weighted
    }

    //Types of input
    enum InputType
    {
        Tap,
        Scroll
    }

    // Settings variables to change from within app
    public Algorithm alg;

    public SimpleSlider cacheSizeSlider;
    public SimpleSlider minTapsSlider;
    public SimpleSlider maxTimeBetweenTapsSlider;
    public SimpleToggle algToggle;

    bool visible = false;
    bool holdFunctionality = false;

    // Tunable parameters
    static int cacheSize = 8;
    static int minTaps = 3;
    static float maxTimeBetweenTaps = 2f;
    static float timeBetweenTapAndHold = 5f;
    static float holdDuration = 4f;

    //Option flags
    bool colorTapsOnRecencyFlag = true;
    bool weightedModeFlag = true;

    // Manager vars
    public bool firstUse = true;
    public bool calibrating = false;
    float timeSinceLastTap = 0f;

    List<GameObject> cache;
    public GameObject locationPrefab;
    public GameObject meanPrefab;
    public GameObject mean;
    Color waiting;
    HashSet<GameObject> heldObjects;
    bool makingMeanClear = false;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    GameObject canvas;
    public CanvasGroup settingsCanvas;
    public SimpleToggle settingsToggle;




    // Awake: Called before the first frame update by Unity, and before
    // other scripts' Start() calls.
    void Awake()
    {
        Assert.IsTrue(minTaps >= 1);
        alg = Algorithm.Base;

        canvas = GameObject.Find("Canvas");
        cache = new List<GameObject>(cacheSize);
        heldObjects = new HashSet<GameObject>();

        waiting = new Color(255f, 0f, 0f, 0.5f);

        m_Raycaster = canvas.GetComponent<GraphicRaycaster>();
        m_EventSystem = canvas.GetComponent<EventSystem>();
        

        mean = GameObject.Instantiate(meanPrefab, transform.position,
            Quaternion.identity, canvas.transform);

        mean.GetComponent<Canvas>().overrideSorting = true;
        mean.GetComponent<Canvas>().sortingOrder = 5;

        Reset();
        
    }


    // Update: Called once per frame by Unity.
    void Update()
    {

        if (firstUse || calibrating)
        {
            //return;
        }

        UpdateSettings();

        // Receive user input
        if (Input.GetButtonDown("Fire1"))
        {
            ReceiveUserTap();
        }

        // Exit if cache is empty
        if (cache.Count == 0) return;

        if (cache.Count >= cacheSize)
        {
            IssueHoldToSystem();
        }

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
        if (cache.Count < cacheSize)
        {
            IssueTapToSystem();
            return;
        }

        // Reset if clock expired and enough taps (system was holding)
        if (cache.Count >= cacheSize)
        {
            Reset();
            StartCoroutine(ReleaseHeldObjects(true));
            return;
        }

        // We should never get here
        Assert.IsTrue(false);
    }



    // ReceiveUserTap: called on frame that user taps the screen.
    // Updates Manager metadata.

    void ReceiveUserTap()
    {

        makingMeanClear = false;

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

    IEnumerator ReleaseHeldObjects(bool force)
    {

        yield return new WaitForSeconds(0.1f);

        // For each item that is held, check whether the mean is above it,
        // if not, issue a mouseup. (mouseup no matter what if force=true)

        List<RaycastResult> results = GetObjectsAtPosition(mean.transform.position);

        foreach (GameObject obj in heldObjects.ToList())
        {
            bool heldItemIsInRaycastResults = false;
            foreach (RaycastResult r in results)
            {
                if (obj == r.gameObject)
                {
                    heldItemIsInRaycastResults = true;
                    break;
                }
            }

            if (force || !heldItemIsInRaycastResults)
            {
                ExecuteEvents.Execute(obj,
                    new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
                heldObjects.Remove(obj);
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
        float weightMultiplier = -cache.Count / 2;

        foreach (GameObject location in cache)
        {
            x += location.transform.position.x * (weightedModeFlag ? (1 + weight * weightMultiplier) : 1);
            y += location.transform.position.y * (weightedModeFlag ? (1 + weight * weightMultiplier) : 1);

            if (cache.Count % 2 == 0 && (weightMultiplier == -1)) weightMultiplier += 1;
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
        List<RaycastResult> results = GetObjectsAtPosition(mean.transform.position);

        // Issue a click to each of those UI elements
        foreach (RaycastResult result in results)
        {
            ExecuteEvents.Execute(result.gameObject,
                new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }

        // Make mean red and empty cache
        SetMeanColor(Color.red);
        StartCoroutine(MakeMeanClearAfterDuration());
        ClearCache();
    }

    List<RaycastResult> GetObjectsAtPosition(Vector3 position)
    {
        // Get list of all UI elements at mean's location
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = position;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        return results;
    }

    void IssueHoldToSystem()
    {
        List<RaycastResult> results = GetObjectsAtPosition(mean.transform.position);

        // Issue a mousedown to each of those UI elements
        foreach (RaycastResult result in results)
        {
            ExecuteEvents.Execute(result.gameObject,
                new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);


            // Keep track of what objects we are holding so we can
            // un-hold them later.
            heldObjects.Add(result.gameObject);
        }

        SetMeanColor(Color.green);
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
        for (var i = cache.Count - 1; i >= 0; i--)
        {
            var val = 1 - weight * (cache.Count - i);
            cache[i].GetComponent<Image>().color = new Color(1, 1, 1, val);
        }
    }

    // UI toggle for changing algorithm from base to weighted

    void AlgorithmValueChanged()
    {
        if (alg == Algorithm.Base)
        {
            alg = Algorithm.Weighted;
        }
        else if (alg == Algorithm.Weighted)
        {
            alg = Algorithm.Base;
        }
    }


    // UI toggle that sets visibility for sliders on and off.


    // UI slider for changing cache size (range from 2-8)

    void UpdateCacheSize()
    {
        cacheSize = (int)cacheSizeSlider.value;
    }


    // UI slider for changing minimum number of taps (range from 1-3)

    void UpdateMinTaps()
    {
        minTaps = (int)minTapsSlider.value;
    }


    // UI slider for changing max time between taps (range from 0.5-2)
    // lower time means the mean location turns yellow faster

    void UpdateMaxTimeBetweenTaps()
    {
        maxTimeBetweenTaps = maxTimeBetweenTapsSlider.value;
    }

    void UpdateSettings()
    {
        UpdateCacheSize();
        UpdateMinTaps();
        UpdateMaxTimeBetweenTaps();

        if(algToggle.value) { alg = Algorithm.Weighted; }
        else { alg = Algorithm.Base; }
        
        if(settingsToggle.value)
        {
            settingsCanvas.interactable = true;
            settingsCanvas.blocksRaycasts = true;
            settingsCanvas.alpha = 1f;
        }
        else
        {
            settingsCanvas.interactable = false;
            settingsCanvas.blocksRaycasts = false;
            settingsCanvas.alpha = 0f;
        }
    }

    InputType AnalyzeInput()
    {

        var minx = 10000.0f;
        var maxx = -10000.0f;

        var miny = 10000.0f;
        var maxy = -10000.0f;

        foreach(var input in cache)
        {
            var x = input.transform.position.x;
            var y = input.transform.position.y;

            if(x > maxx)
            {
                maxx = x;
            }

            if(x < minx)
            {
                minx = x;
            }

            if(y > maxy)
            {
                maxy = y;
            }

            if(y < miny)
            {
                miny = y;
            }
        }

        var yrange = maxy - miny;
        var xrange = maxx - minx;


        if(yrange > 3 * xrange)
        {
            print("SCROLL");
            return InputType.Scroll;
        }
        else
        {
            print("TAP");
            return InputType.Tap;
        }
    }

    IEnumerator MakeMeanClearAfterDuration()
    {
        makingMeanClear = true;
        float time = 0.75f;
        for (float elapsed = 0; elapsed < time; elapsed += Time.deltaTime)
        {
            yield return null;
        }

        if (makingMeanClear)
        {
            SetMeanColor(Color.clear);
        }
        makingMeanClear = false;
    }
}
