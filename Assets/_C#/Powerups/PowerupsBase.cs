using UnityEngine;

public class PowerupsBase : MonoBehaviour
{
    public int powerUpID;
    public float activeTime = 0.5f;
    public float refreshRate = 0.1f;

    public virtual void Activate()
    {
        Debug.LogError("Base Class Activated!");
    }
    
    protected virtual void Deactivate()
    {
        Debug.LogError("Base Class Deactivated!");
    }

}
