using UnityEngine;
using Mirror;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class ThirdPersonCharacterNetwork : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float jumpPower = 6f;
    [Range(1f, 4f)] [SerializeField] float gravityMultiplier = 2f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float groundCheckDistance = 0.3f;

    Rigidbody rb;
    Animator animator;
    CapsuleCollider capsule;
    PlayerFreezeManager freezeManager;

    Vector3 groundNormal;
    bool isGrounded;
    float origGroundCheckDistance;
    bool isFrozen = false;

   void Start()
{
    rb = GetComponent<Rigidbody>();
    animator = GetComponent<Animator>();
    capsule = GetComponent<CapsuleCollider>();
    freezeManager = GetComponent<PlayerFreezeManager>();
    origGroundCheckDistance = groundCheckDistance;

    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

    if (!isLocalPlayer)
    {
        rb.isKinematic = true;
        enabled = false;
        return;
    }

    CinemachineFreeLook cineCam = FindObjectOfType<CinemachineFreeLook>();
    if (cineCam != null)
    {
        cineCam.Follow = transform;
        cineCam.LookAt = transform;
    }

    rb.isKinematic = false;
}


    void Update()
    {
        if (!isLocalPlayer || isFrozen) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool jump = Input.GetButton("Jump");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * v + camRight * h;
        if (move.magnitude > 1f)
            move.Normalize();

        Move(move, jump);
    }

    void Move(Vector3 move, bool jump)
    {
        CheckGroundStatus();

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (isGrounded)
        {
            Vector3 moveVelocity = move * moveSpeed;
            moveVelocity.y = rb.velocity.y;
            rb.velocity = moveVelocity;

            if (jump && animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
                isGrounded = false;
                animator.applyRootMotion = false;
                groundCheckDistance = 0.1f;
            }
        }
        else
        {
            Vector3 extraGravity = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rb.AddForce(extraGravity);
            groundCheckDistance = rb.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
        }

        UpdateAnimator(move);
    }

    void UpdateAnimator(Vector3 move)
    {
        float forwardAmount = move.magnitude;

       
            animator.SetFloat("Forward", forwardAmount);
            animator.SetBool("OnGround", isGrounded);

            if (!isGrounded)
                animator.SetFloat("Jump", rb.velocity.y);
        
    }

    void CheckGroundStatus()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float rayLength = capsule.bounds.extents.y + groundCheckDistance;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayLength))
        {
            if (rb.velocity.y <= 0.1f)
            {
                groundNormal = hit.normal;
                isGrounded = true;
                animator.applyRootMotion = true;
            }
            else
            {
                isGrounded = false;
                animator.applyRootMotion = false;
            }
        }
        else
        {
            isGrounded = false;
            animator.applyRootMotion = false;
            groundNormal = Vector3.up;
        }

        Debug.DrawRay(origin, Vector3.down * rayLength, isGrounded ? Color.green : Color.red);
    }

    public void OnAnimatorMove()
    {
        if (!isLocalPlayer || !isGrounded || Time.deltaTime <= 0f) return;

        Vector3 v = (animator.deltaPosition) / Time.deltaTime;
        v.y = rb.velocity.y;
        rb.velocity = v;
    }

    // Freeze logic
    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;

        if (frozen)
        {
            rb.velocity = Vector3.zero;
            animator.SetFloat("Forward", 0);
            animator.SetBool("OnGround", true);
        }
    }

    public void ForceStop()
    {
        rb.velocity = Vector3.zero;
    }
}
