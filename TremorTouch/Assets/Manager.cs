using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    int cacheSize = 8;
    float maxTimeBetweenTaps = 1.2f;
    float timeSinceLAstTap = 0f;
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
        timeSinceLAstTap += Time.deltaTime;

        if (Input.GetButtonDown("Fire1"))
        {
            timeSinceLAstTap = 0f;

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

            if (locations.Count > 2)
            {
                mean.GetComponent<SpriteRenderer>().color = Color.red;
                mean.transform.position = GetMean();
            }
            else
            {
                mean.GetComponent<SpriteRenderer>().color = Color.clear;
            }
        }

        if (timeSinceLAstTap >= maxTimeBetweenTaps)
        {
            foreach (GameObject g in locations) {
                GameObject.Destroy(g);
            }

            mean.GetComponent<SpriteRenderer>().color = Color.clear;
            locations.Clear();
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
