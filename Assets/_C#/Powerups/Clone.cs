using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : PowerupsBase
{
    private readonly int _maxClones = 1;
    
    [SerializeField] 
    private GameObject[] _clones;

    private DissolvingController[] _cloneDissolvingControllers;

    public Clone()
    {
        powerUpID = 2;
        activeTime = 1f;
    }

    private void Awake()
    {
        _cloneDissolvingControllers = new DissolvingController[_maxClones];
        
        // Check if the script is attached to either runner or catcher
        var parentRole = this.GetComponent<GameRole>().SelectedRole;
        // Get two clone AI from the object pool
        _clones = GameObject.FindGameObjectsWithTag("Clone");
        // Store both clones in an array
        for (int i = 0; i < _maxClones; i++)
        {
            _clones[i].GetComponent<GameRole>().SelectedRole = parentRole;
            _cloneDissolvingControllers[i] =  _clones[i].GetComponent<DissolvingController>();
            
            _cloneDissolvingControllers[i].Dissolve();
        }
    }

    public override void Activate()
    {
        for (int i = 0; i < _maxClones; i++)
        {
            _clones[i].transform.GetChild(0).gameObject.SetActive(true);
            _clones[i].GetComponent<CapsuleCollider>().enabled = true;
           _cloneDissolvingControllers[i].UnDissolve();
        }
        
        var pos = this.gameObject.transform.position;
        pos.x += 4;
        pos.z += 2;
        _clones[0].transform.position = pos;
        
        StartCoroutine(PowerUpActivated());
    }

    private IEnumerator PowerUpActivated()
    {
        float time = 0f;

        while (time < activeTime)
        {
            time += Time.deltaTime;

            yield return new WaitForSeconds(refreshRate);
        }
        
        Deactivate();
    }
    
    protected override void Deactivate()
    {
        for (int i = 0; i < _maxClones; i++)
        {
            _cloneDissolvingControllers[i].Dissolve();
            _clones[i].GetComponent<Collider>().enabled = false;
        }
        
        Destroy(this, 1f);
    }
}
