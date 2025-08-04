using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(NavMeshAgent))]
public class CatcherAI : MonoBehaviour
{
    public float updateSpeed = 0.1f;
    public float catchDistance = 2f;
    public float powerupCooldown = 5f;
    public float minDistanceForSpeedBoost = 6f;
    public float minDistanceForInvisibility = 8f;
    public float minDistanceForClone = 10f;

    [SerializeField] private Transform _target;
    private ThirdPersonCharacter _character;
    private NavMeshAgent _agent;
    private FieldOfView _fov;
    private PowerUpHolder _powerUpHolder;
    private bool _flag = false;
    private bool _lock = false;
    private float _lastPowerupTime = 0f;
    private float _lastTargetUpdateTime = 0f;
    private float _targetUpdateInterval = 1f;

    private void Awake()
    {
        _character = GetComponent<ThirdPersonCharacter>();
        _agent = GetComponent<NavMeshAgent>();
        _fov = GetComponent<FieldOfView>();
        _powerUpHolder = GetComponent<PowerUpHolder>();
    }

    private void OnEnable()
    {
        _agent.updateRotation = false;
        Actions.AmInvisible += RunnerInvisible;
        Actions.GameStart += GameStart;
    }

    private void OnDisable()
    {
        Actions.AmInvisible -= RunnerInvisible;
        Actions.GameStart -= GameStart;
    }

    private void GameStart()
    {
        _lock = true;
        StartCoroutine(FollowTarget());
    }

    private void Update()
    {
        if (!_lock) return;
        _character.Move(_agent.desiredVelocity, false, false);
    }

    private Transform GetTargetToFollow()
    {
        Transform closestTarget = null;
        float shortestDistance = Mathf.Infinity;
        GameObject[] runners = GameObject.FindGameObjectsWithTag("Runner");

        foreach (GameObject runner in runners)
        {
            if (runner.layer == LayerMask.NameToLayer("Freeze"))
                continue;

            float distance = Vector3.Distance(transform.position, runner.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestTarget = runner.transform;
            }
        }

        return closestTarget;
    }

    private Vector3 RandomNavMeshLocation()
    {
        var finalPosition = Vector3.zero;
        var randomPosition = Random.insideUnitSphere * 100f;
        randomPosition += transform.position;
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 100f, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private IEnumerator FollowTarget()
    {
        if (!_target)
        {
            _target = GetTargetToFollow();
        }

        WaitForSeconds wait = new WaitForSeconds(updateSpeed);
        float distance = 0f;

        while (true)
        {
            // Update target periodically
            if (Time.time - _lastTargetUpdateTime >= _targetUpdateInterval)
            {
                _target = GetTargetToFollow();
                _lastTargetUpdateTime = Time.time;
            }

            if (_fov.canSeePlayer)
            {
                _target = _fov.targets[0].transform;
            }

            if (_target)
            {
                _agent.SetDestination(_target.position);
                distance = Vector3.Distance(transform.position, _target.position);

                // Use Powerups strategically
                if (Time.time - _lastPowerupTime >= powerupCooldown)
                {
                    if (distance <= minDistanceForSpeedBoost)
                    {
                        _powerUpHolder.UsePowerUp(-1, 1); // SpeedBoost
                        _lastPowerupTime = Time.time;
                    }
                    else if (distance <= minDistanceForInvisibility)
                    {
                        _powerUpHolder.UsePowerUp(-1, 3); // Invisibility
                        _lastPowerupTime = Time.time;
                    }
                    else if (distance >= minDistanceForClone)
                    {
                        _powerUpHolder.UsePowerUp(-1, 2); // Clone
                        _lastPowerupTime = Time.time;
                    }
                }

                if (distance <= catchDistance)
                {
                    // Freeze the target
                    _target.gameObject.layer = LayerMask.NameToLayer("Freeze");

                    if (_target.TryGetComponent<NavMeshAgent>(out _))
                    {
                        _target.GetComponent<RunnerAI>().enabled = false;
                        _target.GetComponent<NavMeshAgent>().enabled = false;
                        _target.GetComponent<ThirdPersonCharacter>().enabled = false;
                    }
                    else
                    {
                        _target.GetComponent<PlayerInput>().enabled = false;
                        _target.GetComponent<ThirdPersonController>().enabled = false;
                        _target.GetComponent<Attack>().enabled = false;
                    }

                    _target.GetComponent<Animator>().enabled = false;
                    _target.GetComponent<FieldOfView>().enabled = false;

                    // Move on to the next target
                    _target = GetTargetToFollow();
                }
            }
            else
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _agent.SetDestination(RandomNavMeshLocation());
                    if (_flag == false)
                        StartCoroutine(Delay());
                }
            }

            yield return wait;
        }
    }

    private IEnumerator Delay()
    {
        _flag = true;
        while (true)
        {
            if (!_target)
            {
                _target = GetTargetToFollow();
                yield return new WaitForSeconds(5f);
            }
            else
            {
                _flag = false;
                StopCoroutine(Delay());
            }
        }
    }

    private void RunnerInvisible(GameObject obj)
    {
        if (_target && _target.gameObject == obj)
        {
            _target = GetTargetToFollow();
        }
    }
}
