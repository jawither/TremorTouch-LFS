using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    int cacheSize = 8;
    List<GameObject> locations;
    public GameObject locationPrefab;
    public GameObject meanPrefab;

    GameObject mean;

    // Start is called before the first frame update
    void Start()
    {
        locations = new List<GameObject>(cacheSize);
        mean = GameObject.Instantiate(meanPrefab, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (locations.Count == cacheSize)
            {
                GameObject.Destroy(locations[0]);
                locations.RemoveAt(0);
            }

            Vector3 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            destination.z = Camera.main.nearClipPlane;
            transform.position = destination;

            GameObject newLocation = Instantiate(locationPrefab, destination, Quaternion.identity);
            locations.Add(newLocation);

            mean.transform.position = GetMean();

        }
    }

    Vector2 GetMean()
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
}
