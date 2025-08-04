using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Invisibility : PowerupsBase
{
    private DissolvingController[] _dissolvingController;
    private int _layerIndex;
    private string _tag;
    
    public Invisibility()
    {
        powerUpID = 3;
        activeTime = 1f;
    }

    private void Start()
    {
        _dissolvingController = this.TryGetComponent<NavMeshAgent>(out var temp) ? GetComponents<DissolvingController>() : GetComponentsInChildren<DissolvingController>();
        _dissolvingController[0].enabled = true;
        _layerIndex = this.gameObject.layer;
        _tag = this.gameObject.tag;
    }

    public override void Activate()
    {
        if (this.gameObject.TryGetComponent<bl_MiniMapEntity>(out var temp))
        {
            temp.enabled = false;
        }

        this.gameObject.layer = LayerMask.NameToLayer("Invisible");
        this.gameObject.tag = "Untagged";
        _dissolvingController[0].Dissolve();
        StartCoroutine(PowerUpActivated());
        Actions.AmInvisible?.Invoke(this.gameObject);
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
        if (this.gameObject.TryGetComponent<bl_MiniMapEntity>(out var temp))
        {
            temp.enabled = true;
        }

        this.gameObject.layer = _layerIndex;
        this.gameObject.tag = _tag;
        _dissolvingController[0].UnDissolve();
        Destroy(this, 1f);
    }
}
