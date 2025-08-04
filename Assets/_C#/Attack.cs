using System;
using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityStandardAssets.Characters.ThirdPerson;

public class Attack : MonoBehaviour
{
    private PlayerInput _playerInput;
    private FieldOfView _fov;
    private PowerUpHolder _powerUpHolder;
    private InputAction _attackAction;
    private InputAction _powerUp1Action;
    private InputAction _powerUp2Action;
    private InputAction _powerUp3Action;
    private bool _lock = false;
    private float _freezeCooldown = 1.0f;
    private float _lastFreezeTime = 0f;
    private float _unfreezeRange = 3f; // Closer range for unfreezing

    private void Start()
    {
        this.TryGetComponent<PowerUpHolder>(out _powerUpHolder);
        this.TryGetComponent<FieldOfView>(out _fov);
        _playerInput = GetComponent<PlayerInput>();
        _attackAction = _playerInput.actions["Attack"];
        _powerUp1Action = _playerInput.actions["Powerup1"];
        _powerUp2Action = _playerInput.actions["Powerup2"];
        _powerUp3Action = _playerInput.actions["Powerup3"];
    }

    private void Update()
    {
        if (_powerUpHolder != null)
        {
            if (_powerUp1Action.IsPressed() && _lock == false)
            {
                _powerUpHolder.UsePowerUp(0, -1);
                StartCoroutine(Delay());
            }
            else if (_powerUp2Action.IsPressed() && _lock == false)
            {
                _powerUpHolder.UsePowerUp(1, -1);
                StartCoroutine(Delay());
            }
            else if ( _powerUp3Action.IsPressed() && _lock == false)
            {
                _powerUpHolder.UsePowerUp(2, -1);
                StartCoroutine(Delay());
            }
        }

        // Check cooldown
        if (Time.time - _lastFreezeTime < _freezeCooldown)
            return;

        // For player chaser, only check attack input and range
        if (_fov.amChaser && _attackAction.IsPressed())
        {
            if (IsInFreezeRange())
            {
                Debug.Log("Freeze");
                FreezePlayer();
                _lastFreezeTime = Time.time;
            }
        }
        // For AI chaser, check both conditions
        else if (_fov.amChaser && _fov.canSeePlayer && _attackAction.IsPressed())
        {
            if (IsInFreezeRange())
            {
                Debug.Log("Freeze");
                FreezePlayer();
                _lastFreezeTime = Time.time;
            }
        }
        else if (_fov.amRunner && _fov.canSeeFreezeRunner && _attackAction.IsPressed())
        {
            if (IsInUnfreezeRange())
            {
                Debug.Log("Unfreeze");
                UnfreezePlayer();
                _lastFreezeTime = Time.time;
            }
        }
    }

    private bool IsInFreezeRange()
    {
        foreach (var target in _fov.targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= _fov.radius)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, direction);
                if (angle <= _fov.angle / 2)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsInUnfreezeRange()
    {
        foreach (var target in _fov.freezeRunners)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= _unfreezeRange)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, direction);
                if (angle <= _fov.angle / 2)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void FreezePlayer()
    {
        foreach (var t in _fov.targets)
        {
            if (IsInFreezeRange())
            {
                Actions.Baraf?.Invoke();
                t.layer = LayerMask.NameToLayer("Freeze");
                
                // Disable components
                if (t.TryGetComponent<Rigidbody>(out var rb))
                    rb.isKinematic = true;
                if (t.TryGetComponent<RunnerAI>(out var ai))
                    ai.enabled = false;
                if (t.TryGetComponent<NavMeshAgent>(out var agent))
                    agent.enabled = false;
                if (t.TryGetComponent<ThirdPersonCharacter>(out var character))
                    character.enabled = false;
                if (t.TryGetComponent<Animator>(out var animator))
                {
                    animator.enabled = false;
                    // Play freeze animation
                    animator.Play("Freeze");
                }
                if (t.TryGetComponent<FieldOfView>(out var fov))
                    fov.enabled = false;

                // Add visual effect
                StartCoroutine(FreezeEffect(t));
            }
        }
    }

    private void UnfreezePlayer()
    {
        foreach (var t in _fov.freezeRunners)
        {
            if (IsInUnfreezeRange())
            {
                Actions.Paani?.Invoke();
                t.layer = LayerMask.NameToLayer("Runner");
                
                // Enable components
                if (t.TryGetComponent<Rigidbody>(out var rb))
                    rb.isKinematic = false;
                if (t.TryGetComponent<RunnerAI>(out var ai))
                    ai.enabled = true;
                if (t.TryGetComponent<NavMeshAgent>(out var agent))
                    agent.enabled = true;
                if (t.TryGetComponent<ThirdPersonCharacter>(out var character))
                    character.enabled = true;
                if (t.TryGetComponent<Animator>(out var animator))
                {
                    animator.enabled = true;
                    // Play unfreeze animation
                    animator.Play("Unfreeze");
                }
                if (t.TryGetComponent<FieldOfView>(out var fov))
                    fov.enabled = true;

                // Add visual effect
                StartCoroutine(UnfreezeEffect(t));
            }
        }
    }

    private IEnumerator FreezeEffect(GameObject target)
    {
        // Add particle effect or visual feedback
        var renderer = target.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.blue;
            yield return new WaitForSeconds(0.5f);
            renderer.material.color = originalColor;
        }
    }

    private IEnumerator UnfreezeEffect(GameObject target)
    {
        // Add particle effect or visual feedback
        var renderer = target.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.white;
            yield return new WaitForSeconds(0.5f);
            renderer.material.color = originalColor;
        }
    }

    private IEnumerator Delay()
    {
        _lock = true;
        yield return new WaitForSeconds(0.5f);
        _lock = false;
    }
}
