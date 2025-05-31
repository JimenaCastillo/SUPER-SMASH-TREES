using UnityEngine;

public interface IPlayer
{
    int GetPlayerIndex();
    Vector2 GetFacingDirection();
    void AddForce(Vector2 force);
    void OnTokenCollected(int value);
}