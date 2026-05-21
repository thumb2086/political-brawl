using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private BaseCharacter character;
    private bool isLocalPlayer;

    void Start()
    {
        character = GetComponent<BaseCharacter>();
        if (character == null)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (character.IsDead() || GameManager.Instance.currentState != GameState.Playing)
            return;

        if (InputManager.Instance == null) return;

        Vector2 moveDir = InputManager.Instance.GetMoveDirection();
        character.SetMoveDirection(moveDir);

        if (moveDir != Vector2.zero)
        {
            if (InputManager.Instance.GetAimDirection() == Vector2.zero)
                character.SetAimDirection(moveDir);
        }

        Vector2 aimDir = InputManager.Instance.GetAimDirection();
        if (aimDir != Vector2.zero)
        {
            character.SetAimDirection(aimDir);
        }

        if (InputManager.Instance.IsAttacking() && aimDir != Vector2.zero)
        {
            character.TryAttack();
        }

        if (InputManager.Instance.IsSuperPressed())
        {
            character.TryUseSuper();
        }

        if (InputManager.Instance.IsGadget1Pressed())
        {
            character.UseGadget1();
        }

        if (InputManager.Instance.IsGadget2Pressed())
        {
            character.UseGadget2();
        }
    }
}
