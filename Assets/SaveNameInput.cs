using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveNameInput : MonoBehaviour {
    [SerializeField]
    private GameObject  saveButtonObj;
    [SerializeField]
    private Button      saveButton;
    [SerializeField]
    private Text        saveText;

	// Use this for initialization
	void Start () {
        saveButton.interactable = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnEndEdit()
    {
        if(saveText.text != "")
        {
            saveButton.interactable = true;
        }
        else
        {
            saveButton.interactable = false;
        }
    }
}
