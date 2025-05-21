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

    private void Start()
    {
        playerTrees = new ITree[GameManager.Instance.GetPlayerCount()];
        playerScores = new int[GameManager.Instance.GetPlayerCount()];
        InitializeTrees();
        StartNewChallenge();
        StartCoroutine(SpawnTokensRoutine());
    }

    private void Update()
    {
        if (challengeActive)
        {
            CheckChallengeCompletion();
        }

        // Update tree visualizers
        for (int i = 0; i < playerTrees.Length; i++)
        {
            if (treeVisualizers[i] != null && playerTrees[i] != null)
            {
                treeVisualizers[i].UpdateTreeVisualization(playerTrees[i]);
            }
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

    public void CollectToken(int playerIndex, int tokenValue)
    {
        if (playerTrees[playerIndex] != null && challengeActive)
        {
            playerTrees[playerIndex].Insert(tokenValue);
            uiManager.ShowTokenCollected(playerIndex, tokenValue);
        }
    }
}