using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    // Tunable parameters
    static int cacheSize = 8;
    static int minTaps = 2;
    static float maxTimeBetweenTaps = 0.8f;

    enum Algorithm
    {
        Base,
        Weighted
    }
    Algorithm alg;


    public Toggle AlgorithmToggle;
    public Toggle settingsToggle;
    public Slider cacheSizeSlider;
    public Slider minTapsSlider;
    public Slider maxTimeBetweenTapsSlider;
    bool visible = false;

    // static int minTapsForHold = 10;
    // static int minTapsForSwipe = 10;

    // For hold/swipe, numberOfTaps has to be greater than a threshold, should that threshold be less than the cache size?


    // Manager vars
    float timeSinceLastTap = 0f;
    List<GameObject> locations;
    public GameObject locationPrefab;
    public GameObject meanPrefab;
    GameObject mean;
    Color waiting;


    // Start: Called before the first frame update by Unity.
    void Start()
    {
        locations = new List<GameObject>(cacheSize);
        mean = GameObject.Instantiate(meanPrefab, transform.position, Quaternion.identity);
        waiting = new Color(255f, 0f, 0f, 0.5f);


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

    // Update: Called once per frame by Unity.
    void Update()
    {
        // Receive user input
        if (Input.GetButtonDown("Fire1")) ReceiveUserTap();

        // Exit if cache is empty
        if (locations.Count == 0) return;

        // Exit if clock hasn't yet expired
        timeSinceLastTap += Time.deltaTime;
        if (timeSinceLastTap < maxTimeBetweenTaps) return;

        // Reset if clock expired but not enough taps
        if (locations.Count < minTaps)
        {
            Reset();
            return;
        }

        // Issue tap if clock expired and enough taps
        if (locations.Count <= cacheSize)
        {
            IssueTapToSystem();
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

        // If cache at capacity, remove oldest tap
        if (locations.Count == cacheSize)
        {
            GameObject.Destroy(locations[0]);
            locations.RemoveAt(0);
        }

        // Get screen tap location coordinates
        Vector3 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        destination.z = Camera.main.nearClipPlane;
        transform.position = destination;

        // Create new tap location at coordinates
        GameObject newLocation = Instantiate(locationPrefab, destination, Quaternion.identity);
        locations.Add(newLocation);

        // Make mean location visible and update its location iff new tap count > minTaps
        if (locations.Count < minTaps)
        {
            mean.GetComponent<SpriteRenderer>().color = Color.clear;
        }
        else
        {
            mean.GetComponent<SpriteRenderer>().color = waiting;
            mean.transform.position = GetMeanPosition();
        }
    }


    // GetMeanPosition: Returns 2d coordinates that is the mean of all positions
    // of taps in the cache.

    Vector2 GetMeanPosition()
    {
        float x = 0;
        float y = 0;

        foreach (GameObject location in locations)
        {
            x += location.transform.position.x;
            y += location.transform.position.y;
        }

        return new Vector2(x / locations.Count, y / locations.Count);
    }


    // Reset: Empties cache and makes mean invisible.

    void Reset()
    {
        mean.GetComponent<SpriteRenderer>().color = Color.clear;
        ClearCache();
    }


    // TODO
    // IssueTapToSystem: Activate UI elements at the location of the mean.

    void IssueTapToSystem()
    {
        mean.GetComponent<SpriteRenderer>().color = Color.red;
        ClearCache();
    }


    // ClearCache: clears cache and deallocates memory for stored lcoations.

    void ClearCache()
    {
        foreach (GameObject g in locations)
        {
            GameObject.Destroy(g);
        }

        locations.Clear();
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

}
