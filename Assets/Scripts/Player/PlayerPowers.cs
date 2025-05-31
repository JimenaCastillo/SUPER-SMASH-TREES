using UnityEngine;

public class PlayerPowers : MonoBehaviour
{
    [SerializeField] private int playerIndex = 0;

    private PowerupManager powerupManager;

    private string forcePushButton;
    private string shieldButton;
    private string airJumpButton;

    private Rigidbody2D rb;

    [SerializeField] private float airJumpForce = 9f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        powerupManager = FindObjectOfType<PowerupManager>();

        // Set up input based on player index
        if (playerIndex == 0)
        {
            forcePushButton = "Fire1";
            shieldButton = "Fire2";
            airJumpButton = "Fire3";
        }
        else if (playerIndex == 1)
        {
            forcePushButton = "Fire1P2";
            shieldButton = "Fire2P2";
            airJumpButton = "Fire3P2";
        }
    }

    private void Update()
    {
        // Force Push
        if (Input.GetButtonDown(forcePushButton))
        {
            powerupManager.UsePowerup(playerIndex, PowerupType.ForcePush);
        }

        // Shield
        if (Input.GetButtonDown(shieldButton))
        {
            powerupManager.UsePowerup(playerIndex, PowerupType.Shield);
        }

        // Air Jump
        if (Input.GetButtonDown(airJumpButton))
        {
            TryAirJump();
        }
    }

    private void TryAirJump()
    {
        if (powerupManager.UseAirJump(playerIndex))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, airJumpForce);

            Animator anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("AirJump");
            }
        }
    }
}
