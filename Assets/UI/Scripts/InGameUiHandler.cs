using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUiHandler : MonoBehaviour
{
    public string[] ChractersNames;
    public GameObject[] PowerUpsObj;
    public GameObject ChractersParrent;
    // Start is called before the first frame update
    void Start()
    {
        // Enable powerups in both modes
        foreach (GameObject PowerUpsObjects in PowerUpsObj)
        {
            PowerUpsObjects.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        for (int i = 0; i < ChractersParrent.transform.childCount; i++)
        {
            ChractersParrent.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < ChractersParrent.transform.childCount; i++)
        {
            if (PlayerPrefs.GetString("Chracter") == ChractersNames[i])
            {
                ChractersParrent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
