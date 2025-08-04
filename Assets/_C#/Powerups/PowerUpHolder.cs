using System.Collections.Generic;
using UnityEngine;

public class PowerUpHolder : MonoBehaviour
{
    [SerializeField] private List<PowerupsBase> _powerups = new List<PowerupsBase>();
    [SerializeField] private int _powerCount = 0;
    
    private readonly int _maxPowerUps = 3;

    public bool AddPowerUp(int id)
    {
        if (_powerCount < _maxPowerUps)
        {
            ++_powerCount;
            switch (id)
            {
                case 1:
                    _powerups.Add(this.gameObject.AddComponent<SpeedBoost>());
                    break;
                case 2:
                    _powerups.Add(this.gameObject.AddComponent<Clone>());
                    break;
                case 3:
                    _powerups.Add(this.gameObject.AddComponent<Invisibility>());
                    break;
                default:
                    --_powerCount;
                    return false;
            }

            return true;
        }
        
        return false;
    }

    public void UsePowerUp(int key, int id)
    {
        if (key > -1)
        {
            if (key < _powerCount && _powerCount <= _maxPowerUps)
            {
                _powerups[key].Activate();
                _powerups.RemoveAt(key);
                --_powerCount;
            }
        }
        else
        {
            foreach (var powerup in _powerups)
            {
                if (powerup.powerUpID == id)
                {
                    powerup.Activate();
                    _powerups.Remove(powerup);
                    --_powerCount;
                    break;
                }
            }
        }
    }
}
