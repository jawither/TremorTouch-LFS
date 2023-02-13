using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    // Tunable parameters
    static int cacheSize = 8;
    static int minTaps = 2;
    static float maxTimeBetweenTaps = 0.8f;


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
    }

    // Update: Called once per frame by Unity.
    void Update()
    {
        // Update clock
        timeSinceLastTap += Time.deltaTime;

        // Receive user input
        if (Input.GetButtonDown("Fire1"))
        {
            ReceiveUserTap();
        }

        // Exit if clock hasn't yet expired
        if (timeSinceLastTap < maxTimeBetweenTaps)
        {
            return;
        }

        // Reset if clock expired but not enough taps
        if (locations.Count < minTaps)
        {
            Reset();
            return;
        }

        // Issue tap if clock expired and enough taps
        if (locations.Count < cacheSize)
        {
            IssueTapToSystem();
            return;
        }

        print("Should not see this atm");
        Reset();
    }


    // ReceiveUserTap: called on frame that user taps the screen.
    // Updates Manager metadata.

    void ReceiveUserTap()
    {
        // Reset clock
        timeSinceLastTap = 0f;

        // If cache at capcity, remove oldest tap
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

}
