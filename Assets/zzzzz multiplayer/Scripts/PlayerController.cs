using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SyncVar] public bool isCatcher = false;
    [SyncVar] public bool isFrozen = false;

    public float moveSpeed = 5f;
    private Renderer rend;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        UpdateColor();
    }

    void Update()
    {
        if (!isLocalPlayer || isFrozen) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        // Face direction
        if (move != Vector3.zero)
            transform.forward = move;

        if (isCatcher && Input.GetKeyDown(KeyCode.F))
            TryFreeze();

        if (!isCatcher && Input.GetKeyDown(KeyCode.U))
            TryUnfreeze();
    }

    void TryFreeze()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, 2f);
        foreach (var col in hit)
        {
            if (col.gameObject == this.gameObject) continue;
            var pc = col.GetComponent<PlayerController>();
            if (pc && !pc.isCatcher && !pc.isFrozen)
            {
                CmdFreeze(col.gameObject);
                break;
            }
        }
    }

    void TryUnfreeze()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, 2f);
        foreach (var col in hit)
        {
            if (col.gameObject == this.gameObject) continue;
            var pc = col.GetComponent<PlayerController>();
            if (pc && pc.isFrozen)
            {
                CmdUnfreeze(col.gameObject);
                break;
            }
        }
    }

    [Command]
    void CmdFreeze(GameObject target)
    {
        PlayerController pc = target.GetComponent<PlayerController>();
        if (pc) pc.isFrozen = true;
    }

    [Command]
    void CmdUnfreeze(GameObject target)
    {
        PlayerController pc = target.GetComponent<PlayerController>();
        if (pc) pc.isFrozen = false;
    }

    void UpdateColor()
    {
        if (rend == null) return;

        if (isCatcher) rend.material.color = Color.red;
        else if (isFrozen) rend.material.color = Color.cyan;
        else rend.material.color = Color.green;
    }

    public override void OnStartServer()
    {
        if (NetworkServer.connections.Count == 1)
            isCatcher = true;
    }

    public override void OnStartClient()
    {
        UpdateColor();
    }

    public override void OnStartLocalPlayer()
    {
        // Attach camera
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 10, -10);
        Camera.main.transform.localEulerAngles = new Vector3(45, 0, 0);
    }
}
