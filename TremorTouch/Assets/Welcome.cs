using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class Welcome : MonoBehaviour
{
	public Button yourButton;
	public GameObject self;
	public GameObject manager;

	private RectTransform panelRectTransform;

	void Start()
	{
		panelRectTransform = GetComponent<RectTransform>();
		panelRectTransform.anchoredPosition = new Vector2(0, 0);

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
