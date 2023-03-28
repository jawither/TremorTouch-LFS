using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class Welcome : MonoBehaviour
{
	public Button yourButton;
	public GameObject self;
	public GameObject manager;

	void Start()
	{
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
		manager.GetComponent<Manager>().firstUse = false;
		self.SetActive(false);
	}

	void Update()
    {
        
    }
}
