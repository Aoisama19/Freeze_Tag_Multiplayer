using UnityEngine;
using Mirror;
using System.Collections.Generic;

public enum PlayerRole { Catcher, Runner }

public class PlayerFreezeManager : NetworkBehaviour
{
    [SyncVar]
    public PlayerRole playerRole = PlayerRole.Runner;

    [SyncVar(hook = nameof(OnFrozenChanged))]
    public bool isFrozen = false;

    public static List<PlayerFreezeManager> allPlayers = new List<PlayerFreezeManager>();

    private Renderer rend;
    private ThirdPersonCharacterNetwork movementScript;
    private GameObject frozenTagUI;

    // ✅ Add to list
    void Awake()
    {
        allPlayers.Add(this);
    }

    void OnDestroy()
    {
        allPlayers.Remove(this);
    }

    // ✅ SERVER: Assign roles properly
    public override void OnStartServer()
    {
        // Host is Catcher
        if (NetworkServer.connections.Count == 1)
            playerRole = PlayerRole.Catcher;
        else
            playerRole = PlayerRole.Runner;
    }

    public override void OnStopServer()
    {
        allPlayers.Remove(this);
    }

    // ✅ CLIENT: Setup local components
    public override void OnStartLocalPlayer()
{
    rend = GetComponentInChildren<Renderer>();
    movementScript = GetComponent<ThirdPersonCharacterNetwork>();
}

// ✅ Called for all clients (including host)
public override void OnStartClient()
{
    frozenTagUI = transform.Find("FrozenUI/Canvas/FrozenTag")?.gameObject;
    if (frozenTagUI != null)
        frozenTagUI.SetActive(isFrozen); // Show if already frozen
}

    void Update()
    {
        if (!isLocalPlayer) return;

        // 🟥 Color logic
        if (rend != null)
        {
            if (playerRole == PlayerRole.Catcher)
                rend.material.color = Color.red;
            else if (isFrozen)
                rend.material.color = Color.cyan;
            else
                rend.material.color = Color.green;
        }

        // 🟦 Movement control
        if (movementScript != null)
        {
            movementScript.enabled = !isFrozen;
        }

        // 🟩 Freeze/unfreeze on F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryFreezeOrUnfreeze();
        }
    }

    void TryFreezeOrUnfreeze()
    {
        if (playerRole == PlayerRole.Catcher)
        {
            foreach (var p in allPlayers)
            {
                if (p == this || !p.isActiveAndEnabled) continue;
                if (p.playerRole == PlayerRole.Runner && !p.isFrozen && InRange(p))
                {
                    CmdFreeze(p.netIdentity);
                }
            }
        }
        else if (playerRole == PlayerRole.Runner)
        {
            foreach (var p in allPlayers)
            {
                if (p == this || !p.isActiveAndEnabled) continue;
                if (p.playerRole == PlayerRole.Runner && p.isFrozen && InRange(p))
                {
                    CmdUnfreeze(p.netIdentity);
                }
            }
        }
    }

    bool InRange(PlayerFreezeManager target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance < 3f;
    }

    [Command]
    void CmdFreeze(NetworkIdentity targetId)
    {
        var p = targetId.GetComponent<PlayerFreezeManager>();
        if (p != null && p.playerRole == PlayerRole.Runner)
            p.Freeze();
    }

    [Command]
    void CmdUnfreeze(NetworkIdentity targetId)
    {
        var p = targetId.GetComponent<PlayerFreezeManager>();
        if (p != null && p.playerRole == PlayerRole.Runner)
            p.Unfreeze();
    }

    [Server]
    public void Freeze()
    {
        isFrozen = true;
    }

    [Server]
    public void Unfreeze()
    {
        isFrozen = false;
    }

    // ✅ Called on all clients when frozen changes
    void OnFrozenChanged(bool oldValue, bool newValue)
    {
        if (movementScript != null)
            movementScript.SetFrozen(newValue);

        // ✅ Toggle FROZEN text UI
        if (frozenTagUI != null)
            frozenTagUI.SetActive(newValue);
    }
}
