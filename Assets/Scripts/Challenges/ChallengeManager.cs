using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    [SerializeField] private TokenSpawner tokenSpawner;
    [SerializeField] private TreeVisualizer[] treeVisualizers;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int maxTokensOnScreen = 10;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private PowerupManager powerupManager;

    private Challenge currentChallenge;
    private ITree[] playerTrees;
    private int[] playerScores;
    private bool challengeActive = false;
    private int winningPlayer = -1;

    private Challenge[] possibleChallenges = new Challenge[]
    {
        new Challenge(TreeType.BST, "depth", 4),
        new Challenge(TreeType.BST, "size", 7),
        new Challenge(TreeType.AVL, "balanced", 1),
        new Challenge(TreeType.AVL, "size", 5),
        new Challenge(TreeType.BTree, "height", 2),
        new Challenge(TreeType.BTree, "nodes", 6)
    };


    public static ChallengeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        int playerCount = 2;
        if (GameManager.Instance != null)
            playerCount = GameManager.Instance.GetPlayerCount();

        playerTrees = new ITree[playerCount];
        playerScores = new int[playerCount];
        InitializeTrees();
    }
    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        
        StartNewChallenge();
        StartCoroutine(SpawnTokensRoutine());
    }

    private void Update()
    {
        if (!challengeActive || playerTrees == null || treeVisualizers == null)
            return;

        CheckChallengeCompletion();

        if (treeVisualizers == null)
        {
            Debug.LogError("treeVisualizers es null!");
            return;
        }

        for (int i = 0; i < playerTrees.Length; i++)
        {
            if (i >= treeVisualizers.Length)
            {
                Debug.LogWarning($"treeVisualizers no tiene índice {i}. Longitud: {treeVisualizers.Length}");
                continue;
            }

            if (treeVisualizers[i] == null)
            {
                Debug.LogWarning($"treeVisualizers[{i}] es null.");
                continue;
            }

            if (playerTrees[i] == null)
            {
                Debug.LogWarning($"playerTrees[{i}] es null.");
                continue;
            }

            treeVisualizers[i].UpdateTreeVisualization(playerTrees[i]);
        }
    }

    private void InitializeTrees()
    {
        for (int i = 0; i < playerTrees.Length; i++)
        {
            // Start with empty trees - they'll get initialized properly when a challenge starts
            playerTrees[i] = new BinarySearchTree();
        }
    }

    public void StartNewChallenge()
    {
        // Pick a random challenge
        currentChallenge = possibleChallenges[Random.Range(0, possibleChallenges.Length)];

        // Initialize trees based on challenge type
        for (int i = 0; i < playerTrees.Length; i++)
        {
            switch (currentChallenge.treeType)
            {
                case TreeType.BST:
                    playerTrees[i] = new BinarySearchTree();
                    break;
                case TreeType.AVL:
                    playerTrees[i] = new AVLTree();
                    break;
                case TreeType.BTree:
                    playerTrees[i] = new BTree();
                    break;
            }
        }

        challengeActive = true;
        winningPlayer = -1;
        uiManager.UpdateChallengeText(currentChallenge.description);
        tokenSpawner.StartSpawning();
    }

    private void CheckChallengeCompletion()
    {
        for (int i = 0; i < playerTrees.Length; i++)
        {
            if (playerTrees[i].ValidateChallenge(currentChallenge.challengeType, currentChallenge.parameter))
            {
                CompleteChallenge(i);
                break;
            }
        }
    }

    private void CompleteChallenge(int playerIndex)
    {
        challengeActive = false;
        winningPlayer = playerIndex;
        playerScores[playerIndex] += 100;
        uiManager.UpdateScores(playerScores);
        uiManager.ShowChallengeCompleteMessage($"¡Jugador {playerIndex + 1} completó el reto!");
        tokenSpawner.StopSpawning();

        // Grant power-up to winning player
        powerupManager.GrantRandomPowerup(playerIndex);

        // Start a new challenge after a delay
        StartCoroutine(StartNewChallengeDelayed(3f));
    }

    private IEnumerator StartNewChallengeDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNewChallenge();
    }

    private IEnumerator SpawnTokensRoutine()
    {
        while (true)
        {
            if (challengeActive && tokenSpawner.GetActiveTokenCount() < maxTokensOnScreen)
            {
                tokenSpawner.SpawnToken(Random.Range(1, 100));
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void CollectToken(int playerIndex, int value)
{
        if (playerTrees == null)
        {
            Debug.LogError("playerTrees no ha sido inicializado.");
            return;
        }

        if (playerIndex < 0 || playerIndex >= playerTrees.Length)
        {
            Debug.LogWarning($"Índice de jugador inválido: {playerIndex}");
            return;
        }

        if (playerTrees[playerIndex] == null)
        {
            Debug.LogError($"El árbol del jugador {playerIndex} es null.");
            return;
        }

        playerTrees[playerIndex].Insert(value);

        

        if (playerIndex < treeVisualizers.Length && treeVisualizers[playerIndex] != null)
        {
            treeVisualizers[playerIndex].UpdateTreeVisualization(playerTrees[playerIndex]);

        }
        else
        {
            Debug.LogWarning($"No hay TreeVisualizer asignado para el jugador {playerIndex}");
        }
        playerScores[playerIndex] += 10;
        Debug.Log($"Jugador {playerIndex} ha ganado 10 puntos. Total: {playerScores[playerIndex]}");
        uiManager.UpdateScores(playerScores);
    }
    
}