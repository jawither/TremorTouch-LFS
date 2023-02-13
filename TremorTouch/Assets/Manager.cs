using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    // Tunable parameters
    static int cacheSize = 8;
    static int minTaps = 3;
    static float maxTimeBetweenTaps = 0.8f;


    // Manager vars
    float timeSinceLastTap = 0f;
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

        canvas = GameObject.Find("Canvas");
        cache = new List<GameObject>(cacheSize);
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
        // Receive user input
        if (Input.GetButtonDown("Fire1")) ReceiveUserTap();

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
            mean.transform.position = GetMeanPosition();
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

}
