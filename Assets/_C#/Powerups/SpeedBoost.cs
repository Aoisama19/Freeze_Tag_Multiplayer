using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class SpeedBoost : PowerupsBase
{
    public string speedScreenVFXTag = "SpeedVFX";
    public bool isActivated = false;
    public float _originalSpeed;

    private VisualEffect _speedScreenVFX;
    private MeshTrail _trail;
    private ThirdPersonController _controller;
    private NavMeshAgent _agent;

    public SpeedBoost()
    {
        powerUpID = 1;
    }

    private void Awake()
    {
        if (this.TryGetComponent<ThirdPersonController>(out _controller))
        {
            _speedScreenVFX = GameObject.FindWithTag(speedScreenVFXTag).GetComponent<VisualEffect>();
            _trail = this.transform.GetChild(1).transform.GetChild(0).GetComponent<MeshTrail>();
            _originalSpeed = _controller._originalSpeed;
        }
        else if (this.TryGetComponent<NavMeshAgent>(out _agent))
        {
            _agent = GetComponent<NavMeshAgent>();
            _trail = this.transform.GetChild(0).GetComponent<MeshTrail>();
            _originalSpeed = _agent.speed;
        }
    }

    public override void Activate()
    {
        isActivated = true;

        if (_speedScreenVFX)
        {
            _speedScreenVFX.enabled = true;
        }

        StartCoroutine(PowerUpActivated());
    }

    IEnumerator PowerUpActivated()
    {
        float time = 0;
        
        // Play the mesh trail on the attached character
        if (_trail)
        {
            _trail.StartTrail();
        }
        
        // Change the speed of the attached character
        if (_controller)
        {
            _controller.SprintSpeed = _originalSpeed * 2f;
        }

        if (_agent)
        {
            _agent.speed = _originalSpeed * 2f;
        }

        while (time < activeTime)
        {
            time += Time.deltaTime;

            yield return new WaitForSeconds(refreshRate);
        }
        
        Deactivate();
    }

    protected override void Deactivate()
    {
        if (_speedScreenVFX)
        {
            _speedScreenVFX.enabled = false;
        }
        
        // Stop the mesh trail on the attached character
        if (_trail)
        {
            _trail.StopTrail();
        }
        
        // Reset the speed of the attached character
        if (_controller)
        {
            _controller.SprintSpeed = _originalSpeed;
        }
        
        if (_agent)
        {
            _agent.speed = _originalSpeed;
        }
        
        Destroy(this);
    }
}
