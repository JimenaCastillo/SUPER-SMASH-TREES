using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private PlayerController[] players;
    [SerializeField] private float gameTime = 300f; // 5 minutos

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public int GetPlayerCount()
    {
        return players.Length;
    }

    public PlayerController GetPlayer(int index)
    {
        if (index >= 0 && index < players.Length)
        {
            return players[index];
        }
        return null;
    }

    public float GetGameTime()
    {
        return gameTime;
    }
}