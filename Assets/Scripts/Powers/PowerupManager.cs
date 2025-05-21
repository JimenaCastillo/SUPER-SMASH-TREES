using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupType
{
    ForcePush,
    Shield,
    AirJump
}

public class PowerupManager : MonoBehaviour
{
    [SerializeField] private GameObject forcePushEffectPrefab;
    [SerializeField] private GameObject shieldEffectPrefab;
    [SerializeField] private GameObject airJumpEffectPrefab;

    private Dictionary<int, List<PowerupType>> playerPowerups = new Dictionary<int, List<PowerupType>>();
    private Dictionary<int, bool> shieldActive = new Dictionary<int, bool>();
    private Dictionary<int, GameObject> shieldEffects = new Dictionary<int, GameObject>();
    private Dictionary<int, bool> airJumpAvailable = new Dictionary<int, bool>();

    private void Start()
    {
        int playerCount = GameManager.Instance.GetPlayerCount();
        for (int i = 0; i < playerCount; i++)
        {
            playerPowerups[i] = new List<PowerupType>();
            shieldActive[i] = false;
            airJumpAvailable[i] = false;
        }
    }

    public void GrantRandomPowerup(int playerIndex)
    {
        PowerupType powerup = (PowerupType)Random.Range(0, 3);
        playerPowerups[playerIndex].Add(powerup);

        // Notify the player
        string powerupName = GetPowerupName(powerup);
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowPowerupMessage(playerIndex, $"¡{powerupName} obtenido!");
        }
    }

    private string GetPowerupName(PowerupType powerup)
    {
        switch (powerup)
        {
            case PowerupType.ForcePush: return "Force Push";
            case PowerupType.Shield: return "Shield";
            case PowerupType.AirJump: return "Air Jump";
            default: return "Unknown Powerup";
        }
    }

    public bool HasPowerup(int playerIndex, PowerupType powerupType)
    {
        if (!playerPowerups.ContainsKey(playerIndex)) return false;
        return playerPowerups[playerIndex].Contains(powerupType);
    }

    public void UsePowerup(int playerIndex, PowerupType powerupType)
    {
        if (!HasPowerup(playerIndex, powerupType)) return;

        // Remove the powerup from the list
        playerPowerups[playerIndex].Remove(powerupType);

        // Apply powerup effect
        switch (powerupType)
        {
            case PowerupType.ForcePush:
                ApplyForcePush(playerIndex);
                break;
            case PowerupType.Shield:
                ApplyShield(playerIndex);
                break;
            case PowerupType.AirJump:
                EnableAirJump(playerIndex);
                break;
        }
    }

    private PlayerController GetPlayerByIndex(int playerIndex)
    {
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if (player.GetPlayerIndex() == playerIndex)
                return player;
        }
        return null;
    }

    private void ApplyForcePush(int playerIndex)
    {
        PlayerController player = GetPlayerByIndex(playerIndex);
        if (player == null) return;

        Vector2 pushDirection = player.GetFacingDirection() * 10f;
        float radius = 3f;

        // Create visual effect
        Instantiate(forcePushEffectPrefab, player.transform.position, Quaternion.identity);

        // Find nearby players
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, radius);
        foreach (Collider2D collider in colliders)
        {
            PlayerController otherPlayer = collider.GetComponent<PlayerController>();
            if (otherPlayer != null && otherPlayer.GetPlayerIndex() != playerIndex)
            {
                // Calculate direction from the player to the target
                Vector2 direction = (otherPlayer.transform.position - player.transform.position).normalized;

                // Apply force only if shield is not active
                if (!IsShieldActive(otherPlayer.GetPlayerIndex()))
                {
                    otherPlayer.AddForce(direction * 15f);
                }
                else
                {
                    // Shield absorbs the force
                    DeactivateShield(otherPlayer.GetPlayerIndex());
                }
            }
        }
    }

    private void ApplyShield(int playerIndex)
    {
        PlayerController player = GetPlayerByIndex(playerIndex);
        if (player == null) return;

        // Create shield visual
        GameObject shieldEffect = Instantiate(shieldEffectPrefab, player.transform);
        shieldEffects[playerIndex] = shieldEffect;

        // Activate shield
        shieldActive[playerIndex] = true;
    }

    private void EnableAirJump(int playerIndex)
    {
        airJumpAvailable[playerIndex] = true;
    }

    public bool IsShieldActive(int playerIndex)
    {
        if (!shieldActive.ContainsKey(playerIndex)) return false;
        return shieldActive[playerIndex];
    }

    public void DeactivateShield(int playerIndex)
    {
        if (shieldActive.ContainsKey(playerIndex) && shieldActive[playerIndex])
        {
            shieldActive[playerIndex] = false;

            if (shieldEffects.ContainsKey(playerIndex) && shieldEffects[playerIndex] != null)
            {
                Destroy(shieldEffects[playerIndex]);
                shieldEffects.Remove(playerIndex);
            }
        }
    }

    public bool UseAirJump(int playerIndex)
    {
        if (!airJumpAvailable.ContainsKey(playerIndex) || !airJumpAvailable[playerIndex])
            return false;

        airJumpAvailable[playerIndex] = false;

        PlayerController player = GetPlayerByIndex(playerIndex);
        if (player == null) return false;

        // Create air jump effect
        Instantiate(airJumpEffectPrefab, player.transform.position, Quaternion.identity);

        return true;
    }
}
