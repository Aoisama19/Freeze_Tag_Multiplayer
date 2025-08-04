using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityStandardAssets.Characters.ThirdPerson;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class RunnerAI : MonoBehaviour
{
    public NavMeshAgent agent;
    [Range(0, 100)] public float speed;
    [Range(0, 100)] public float walkRadius;
    public float unfreezeRange = 3f;
    public float evasionDistance = 5f;
    public float powerupCooldown = 5f;
    public float minDistanceForSpeedBoost = 8f;
    public float minDistanceForInvisibility = 10f;
    public float minDistanceForClone = 12f;

    private ThirdPersonCharacter _character;
    private FieldOfView _fov;
    private PowerUpHolder _powerUpHolder;
    private bool _temp = false;
    private bool _startRun = false;
    private float _lastPowerupTime = 0f;
    private float _lastTargetUpdateTime = 0f;
    private float _targetUpdateInterval = 1f;
    private Vector3 _lastSafePosition;
    private float _stuckTime = 0f;
    private float _maxStuckTime = 3f;

    private void OnEnable()
    {
        Actions.GameStart += GameStart;
    }

    private void OnDisable()
    {
        Actions.GameStart -= GameStart;
    }

    public void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        _character = this.GetComponent<ThirdPersonCharacter>();
        _fov = this.GetComponent<FieldOfView>();
        _powerUpHolder = this.GetComponent<PowerUpHolder>();
        _lastSafePosition = transform.position;
    }

    private void GameStart()
    {
        if (!agent) return;
        agent.SetDestination(RandomNavMeshLocation());
        _startRun = true;
    }

    private void Update()
    {
        if (!_startRun) return;

        // Check if stuck
        if (Vector3.Distance(transform.position, _lastSafePosition) < 0.1f)
        {
            _stuckTime += Time.deltaTime;
            if (_stuckTime >= _maxStuckTime)
            {
                agent.SetDestination(RandomNavMeshLocation());
                _stuckTime = 0f;
            }
        }
        else
        {
            _stuckTime = 0f;
            _lastSafePosition = transform.position;
        }

        // Update target periodically
        if (Time.time - _lastTargetUpdateTime >= _targetUpdateInterval)
        {
            UpdateBehavior();
            _lastTargetUpdateTime = Time.time;
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            _character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            agent.SetDestination(RandomNavMeshLocation());
            _character.Move(Vector3.zero, false, false);
        }
    }

    private void UpdateBehavior()
    {
        if (_temp == false)
        {
            if (_fov.canSeeFreezeRunner)
            {
                // Prioritize unfreezing teammates
                agent.SetDestination(_fov.freezeRunners[0].transform.position);
                _temp = true;

                if (Vector3.Distance(transform.position, _fov.freezeRunners[0].transform.position) < unfreezeRange)
                {
                    UnfreezeRunner();
                }
            }
            else if (_fov.canSeePlayer)
            {
                // Evade chaser
                Vector3 directionToChaser = transform.position - _fov.targets[0].transform.position;
                Vector3 evasionPoint = transform.position + directionToChaser.normalized * evasionDistance;
                
                // Use powerups when being chased
                if (Time.time - _lastPowerupTime >= powerupCooldown)
                {
                    float distanceToChaser = Vector3.Distance(transform.position, _fov.targets[0].transform.position);
                    
                    if (distanceToChaser <= minDistanceForSpeedBoost)
                    {
                        _powerUpHolder.UsePowerUp(-1, 1); // SpeedBoost
                        _lastPowerupTime = Time.time;
                    }
                    else if (distanceToChaser <= minDistanceForInvisibility)
                    {
                        _powerUpHolder.UsePowerUp(-1, 3); // Invisibility
                        _lastPowerupTime = Time.time;
                    }
                    else if (distanceToChaser <= minDistanceForClone)
                    {
                        _powerUpHolder.UsePowerUp(-1, 2); // Clone
                        _lastPowerupTime = Time.time;
                    }
                }

                agent.SetDestination(evasionPoint);
                _temp = true;
            }
            else
            {
                // Wander if no immediate threats or objectives
                agent.SetDestination(RandomNavMeshLocation());
            }

            StartCoroutine(Delay());
        }
    }

    private Vector3 RandomNavMeshLocation()
    {
        var finalPosition = Vector3.zero;
        var randomPosition = Random.insideUnitSphere * walkRadius;
        randomPosition += transform.position;
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, walkRadius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private void UnfreezeRunner()
    {
        var _target = _fov.freezeRunners[0];

        _target.gameObject.layer = LayerMask.NameToLayer("Runner");

        if (_target.TryGetComponent<NavMeshAgent>(out _))
        {
            _target.GetComponent<RunnerAI>().enabled = true;
            _target.GetComponent<NavMeshAgent>().enabled = true;
            _target.GetComponent<ThirdPersonCharacter>().enabled = true;
        }
        else
        {
            _target.GetComponent<PlayerInput>().enabled = true;
            _target.GetComponent<ThirdPersonController>().enabled = true;
            _target.GetComponent<Attack>().enabled = true;
        }

        _target.GetComponent<Animator>().enabled = true;
        _target.GetComponent<FieldOfView>().enabled = true;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
        _temp = false;
    }
}
