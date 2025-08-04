using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChracterPannel : MonoBehaviour
{
    public Transform InstentiatedChracters;
    GameObject ChracterToinstentiate;
    public Transform ChracterPoint;
    public Transform ChracterBtnsParent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        for (int i = 0; i < InstentiatedChracters.childCount; i++)
        {
            Destroy(InstentiatedChracters.GetChild(i).gameObject);
        }
        ChracterToinstentiate = Instantiate(ChracterBtnsParent.GetChild(0).gameObject.GetComponent<AnimationManage>().Chracter, InstentiatedChracters);
        ChracterToinstentiate.transform.position = ChracterPoint.position;
        ChracterToinstentiate.transform.rotation = ChracterPoint.rotation;
    }
    private void OnDisable()
    {
        for (int i = 0; i < InstentiatedChracters.childCount; i++)
        {
            if (InstentiatedChracters.GetChild(i))
                Destroy(InstentiatedChracters.GetChild(i).gameObject);
        }
    }
}
