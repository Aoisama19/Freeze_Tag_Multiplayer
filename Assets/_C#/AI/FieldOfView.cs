using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0, 360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool amRunner;
    public bool amChaser;
    
    public bool canSeePlayer;
    public bool canSeeFreezeRunner;

    public List<GameObject> targets;
    public List<GameObject> freezeRunners;
    
    private void Start()
    {
        string myTag = this.gameObject.tag;

        if (myTag == "Chaser")
        {
            amChaser = true;
            amRunner = false;
        }
        else
        {
            amChaser = false;
            amRunner = true;
        }

        if (amRunner)
            RunnerFOV();
        else if (amChaser)
            ChaserFOV();
    }

    private void ChaserFOV()
    {
        radius = 6f;
        angle = 100f;
        targetMask = LayerMask.GetMask("Runner");
        StartCoroutine(FOVRoutine());
    }

    private void RunnerFOV()
    {
        radius = 50f;
        angle = 100f;
        targetMask = LayerMask.GetMask("Chaser");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        var wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
            if (amRunner)
                FieldOfViewFreezeRunnerCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        var rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            foreach (var t in rangeChecks)
            {
                var target = t.transform;
                var directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    var position = transform.position;
                    var distanceToTarget = Vector3.Distance(position, target.position);

                    canSeePlayer = !Physics.Raycast(position, directionToTarget, distanceToTarget, obstructionMask);
                    
                    if (!targets.Contains(t.gameObject))
                        targets.Add(t.gameObject);
                }
                else
                {
                    canSeePlayer = false;
                    if (targets.Contains(t.gameObject))
                        targets.Remove(t.gameObject);
                }
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
            targets.Clear();
        }
    }

    private void FieldOfViewFreezeRunnerCheck()
    {
        var rangeChecks = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Freeze"));
        if (rangeChecks.Length != 0)
        {
            foreach (var t in rangeChecks)
            {
                var target = t.transform;
                var directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    var position = transform.position;
                    var distanceToTarget = Vector3.Distance(position, target.position);

                    canSeeFreezeRunner = !Physics.Raycast(position, directionToTarget, distanceToTarget, obstructionMask);

                    if (!freezeRunners.Contains(t.gameObject))
                        freezeRunners.Add(t.gameObject);
                }
                else
                {
                    canSeeFreezeRunner = false;
                    if (freezeRunners.Contains(t.gameObject))
                        freezeRunners.Remove(t.gameObject);
                }
            }
        }
        else if (canSeeFreezeRunner)
        {
            canSeeFreezeRunner = false;
            freezeRunners.Clear();
        }
    }

}
