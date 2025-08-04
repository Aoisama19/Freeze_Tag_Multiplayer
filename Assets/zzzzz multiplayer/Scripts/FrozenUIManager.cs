using TMPro;
using UnityEngine;
using Mirror;
using System.Linq;

public class FrozenUIManager : MonoBehaviour
{
    public TextMeshProUGUI frozenText;

    void Update()
    {
        if (NetworkClient.active && NetworkClient.localPlayer != null)
        {
            var localFreeze = NetworkClient.localPlayer.GetComponent<PlayerFreezeManager>();

            if (localFreeze != null && localFreeze.playerRole == PlayerRole.Catcher)
            {
                int frozenCount = PlayerFreezeManager.allPlayers.Count(p => p.isFrozen && p != localFreeze);
                frozenText.text = $"Frozen: {frozenCount}";
                frozenText.gameObject.SetActive(true);
            }
            else
            {
                frozenText.gameObject.SetActive(false); // Only show for catchers
            }
        }
    }
}
