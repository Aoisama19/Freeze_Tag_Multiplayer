using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NPC : MonoBehaviour
{
    private class BoneTransform
    {
        public Vector3 Position { get; set; }
        
        public Quaternion Rotation { get; set; }
    }
    private enum ZombieState
    {
        Walking,
        Ragdoll,
        StandingUp,
        ResettingBones
    }
    
    [Range(0, 100)] public float speed;
    [Range(0, 100)] public float walkRadius;

    private NavMeshAgent _agent;

    [SerializeField] private string _standUpStateName;
    [SerializeField] private string _faceDownStandUpStateName;
    [SerializeField] private string _standUpClipName;
    [SerializeField] private string _faceDownStandUpClipName;
    [SerializeField] private float _timeTakenToResetBones;
    
    private Rigidbody[] _ragdollRigidBodies;
    [SerializeField]
    private ZombieState _currentState = ZombieState.Walking;
    private Animator _anim;
    private float _timeToWakeUp;
    private Transform _hipsBone;

    private BoneTransform[] _standUpBoneTransforms;
    private BoneTransform[] _faceDownStandUpBoneTransforms;
    private BoneTransform[] _ragdollBoneTransforms;
    private Transform[] _bones;
    private float _elapsedResetBonesTime;
    private bool _isFacingUp;
    
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        _hipsBone = _anim.GetBoneTransform(HumanBodyBones.Hips);

        _bones = _hipsBone.GetComponentsInChildren<Transform>();
        _standUpBoneTransforms= new BoneTransform[_bones.Length];
        _faceDownStandUpBoneTransforms= new BoneTransform[_bones.Length];
        _ragdollBoneTransforms = new BoneTransform[_bones.Length];

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _standUpBoneTransforms[boneIndex] = new BoneTransform();
            _faceDownStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollBoneTransforms[boneIndex] = new BoneTransform();
        }
        
        PopulateAnimationStartBoneTransform(_standUpClipName, _standUpBoneTransforms);
        PopulateAnimationStartBoneTransform(_faceDownStandUpClipName, _faceDownStandUpBoneTransforms);
        
        DisableRagdoll();
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (!_agent) return;
        _agent.speed = speed;
        _agent.SetDestination(RandomNavMeshLocation());
    }

    private void Update()
    {
        switch (_currentState)
        {
            case ZombieState.Walking:
                WalkingBehaviour();
                break;
            case ZombieState.Ragdoll:
                RagdollBehaviour();
                break;
            case ZombieState.StandingUp:
                StandUpBehaviour();
                break;
            case ZombieState.ResettingBones:
                ResettingBonesBehaviour();
                break;
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

    private void DisableRagdoll()
    {
        foreach (var rb in _ragdollRigidBodies)
        {
            rb.isKinematic = true;
        }
        
        _anim.enabled = true;
    }

    private void EnableRagdoll()
    {
        _agent.enabled = false;
        //_agent.isStopped = true;
        
        foreach (var rb in _ragdollRigidBodies)
        {
            rb.isKinematic = false;
        }

        _anim.enabled = false;
    }

    private void WalkingBehaviour()
    {
        if (_agent.remainingDistance < _agent.stoppingDistance)
        {
            _agent.SetDestination(RandomNavMeshLocation());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!other.CompareTag("Chaser") && !other.CompareTag("Runner")) return;
        if (other.TryGetComponent<FieldOfView>(out _))
        {
            EnableRagdoll();
            _currentState = ZombieState.Ragdoll;
            _timeToWakeUp = Random.Range(3, 5);
        }
    }

    private void RagdollBehaviour()
    {
        _timeToWakeUp -= Time.deltaTime;

        if (_timeToWakeUp <= 0f)
        {
            _isFacingUp = _hipsBone.forward.y > 0;
            
            AlignRotationToHips();
            AlignPositionToHips();
            
            PopulateBoneTransforms(_ragdollBoneTransforms);

            _currentState = ZombieState.ResettingBones;
            _elapsedResetBonesTime = 0f;
        }
    }

    private void StandUpBehaviour()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName(GetStandUpStateName()) == false)
        {
            _currentState = ZombieState.Walking;
            _agent.enabled = true;
            //_agent.isStopped = false;
            _agent.SetDestination(RandomNavMeshLocation());
        }
    }

    private void ResettingBonesBehaviour()
    {
        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / _timeTakenToResetBones;

        BoneTransform[] standUpBoneTransforms = GetStandUpBoneTransform();
        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _bones[boneIndex].localPosition = Vector3.Lerp(
                _ragdollBoneTransforms[boneIndex].Position,
                standUpBoneTransforms[boneIndex].Position,
                elapsedPercentage);
            
            _bones[boneIndex].localRotation = Quaternion.Lerp(
                _ragdollBoneTransforms[boneIndex].Rotation,
                standUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage);
            
        }

        if (elapsedPercentage >= 1)
        {
            _currentState = ZombieState.StandingUp;
            DisableRagdoll();
            
            _anim.Play(GetStandUpStateName(), 0, 0);
        }
    }

    private void AlignRotationToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        Quaternion originalHipsRotation = _hipsBone.rotation;

        Vector3 desiredDirection = _hipsBone.up;

        if (_isFacingUp)
            desiredDirection *= -1f;
        
        desiredDirection.y = 0f;
        desiredDirection.Normalize();

        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
        transform.rotation *= fromToRotation;

        _hipsBone.position = originalHipsPosition;
        _hipsBone.rotation = originalHipsRotation;
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        transform.position = _hipsBone.position;

        Vector3 positionOffset = GetStandUpBoneTransform()[0].Position;
        positionOffset.y = 0f;
        positionOffset = transform.rotation * positionOffset;
        transform.position -= positionOffset;
        
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }
        
        _hipsBone.position = originalHipsPosition;
    }

    private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = _bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }

    private void PopulateAnimationStartBoneTransform(string clipName, BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;
        
        foreach (AnimationClip clip in _anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0f);
                PopulateBoneTransforms(boneTransforms);
                break;
            }
        }

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }

    private string GetStandUpStateName()
    {
        return _isFacingUp ? _standUpStateName : _faceDownStandUpStateName;
    }

    private BoneTransform[] GetStandUpBoneTransform()
    {
        return _isFacingUp ? _standUpBoneTransforms : _faceDownStandUpBoneTransforms;
    }
}
