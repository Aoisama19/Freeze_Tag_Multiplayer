using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpPickUp : MonoBehaviour
{
    public enum PowerUp
    {
        SpeedBoost,
        Clone,
        Invisibility
    }
    
    public PowerUp powerUp = PowerUp.SpeedBoost;
    
    private void OnTriggerEnter(Collider other)
    {
        PowerUpHolder temp;
        if (other.TryGetComponent<PowerUpHolder>(out temp))
        {
            int id = 0;
            switch (powerUp)
            {
                case PowerUp.SpeedBoost:
                    id = 1;
                    break;
                case PowerUp.Clone:
                    id = 2;
                    break;
                case PowerUp.Invisibility:
                    id = 3;
                    break;
                default:
                    Debug.Log("No PowerUp Given");
                    break;
            }

            if (temp.AddPowerUp(id))
            {
                StartCoroutine(WaitBeforeVisible());
            }
        }
    }

    private IEnumerator WaitBeforeVisible()
    {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(10f);
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
    }
}
