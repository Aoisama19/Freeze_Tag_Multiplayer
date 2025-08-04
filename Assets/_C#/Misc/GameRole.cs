using UnityEngine;

public class GameRole : MonoBehaviour
{
    public enum Roles
    {
        Catcher,
        Runner
    }

    [SerializeField]
    private Roles selectedRole;

    public Roles SelectedRole
    {
        get => selectedRole;
        set => selectedRole = value;
    }
}
