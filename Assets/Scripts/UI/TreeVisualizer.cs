using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreeVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject nodeTemplate;
    [SerializeField] private RectTransform treeContainer;
    [SerializeField] private float horizontalSpacing = 50f;
    [SerializeField] private float verticalSpacing = 40f;
    [SerializeField] private LineRenderer lineRendererPrefab;

    private List<GameObject> nodeObjects = new List<GameObject>();
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    public void UpdateTreeVisualization(ITree tree)
    {
        ClearVisualization();

        if (tree == null) return;

        List<TreeNode> nodes = tree.GetNodes();

        // For BST and AVL visualization, use a recursive approach
        if (tree.GetTreeType() == TreeType.BST || tree.GetTreeType() == TreeType.AVL)
        {
            // Extract the root node (if available)
            TreeNode root = nodes.Count > 0 ? nodes[0] : null;
            if (root != null)
            {
                // Calculate positions for each node recursively
                Dictionary<TreeNode, Vector2> nodePositions = new Dictionary<TreeNode, Vector2>();
                CalculateNodePositions(root, 0, 0, nodePositions);

                // Create visual nodes and connections
                CreateVisualNodes(root, nodePositions);
            }
        }
        // For B-Tree, use a level-based approach
        else if (tree.GetTreeType() == TreeType.BTree)
        {
            // Special visualization for B-Trees
            if (nodes.Count > 0)
            {
                float yPos = 0;
                float xPos = 0;

                foreach (TreeNode node in nodes)
                {
                    GameObject nodeObj = CreateNodeObject(node.Value.ToString(), new Vector2(xPos, yPos));
                    nodeObjects.Add(nodeObj);

                    xPos += horizontalSpacing;
                    if (xPos > treeContainer.rect.width - horizontalSpacing)
                    {
                        xPos = 0;
                        yPos -= verticalSpacing;
                    }
                }
            }
        }
    }

    private void CalculateNodePositions(TreeNode node, int level, int position, Dictionary<TreeNode, Vector2> nodePositions)
    {
        if (node == null) return;

        float xPos = position * horizontalSpacing;
        float yPos = -level * verticalSpacing;

        nodePositions[node] = new Vector2(xPos, yPos);

        // Calculate positions for children
        if (node.Left != null)
        {
            CalculateNodePositions(node.Left, level + 1, position * 2 - 1, nodePositions);
        }

        if (node.Right != null)
        {
            CalculateNodePositions(node.Right, level + 1, position * 2 + 1, nodePositions);
        }
    }

    private void CreateVisualNodes(TreeNode node, Dictionary<TreeNode, Vector2> nodePositions)
    {
        if (node == null) return;

        Vector2 nodePos = nodePositions[node];
        GameObject nodeObj = CreateNodeObject(node.Value.ToString(), nodePos);
        nodeObjects.Add(nodeObj);

        // Create lines to children
        if (node.Left != null)
        {
            Vector2 leftPos = nodePositions[node.Left];
            CreateLine(nodePos, leftPos);
            CreateVisualNodes(node.Left, nodePositions);
        }

        if (node.Right != null)
        {
            Vector2 rightPos = nodePositions[node.Right];
            CreateLine(nodePos, rightPos);
            CreateVisualNodes(node.Right, nodePositions);
        }
    }

    private GameObject CreateNodeObject(string value, Vector2 position)
    {
        GameObject nodeObj = Instantiate(nodeTemplate, treeContainer);
        RectTransform rectTransform = nodeObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        TextMeshProUGUI valueText = nodeObj.GetComponentInChildren<TextMeshProUGUI>();
        if (valueText != null)
        {
            valueText.text = value;
        }

        nodeObj.SetActive(true);
        return nodeObj;
    }

    private void CreateLine(Vector2 start, Vector2 end)
    {
        LineRenderer line = Instantiate(lineRendererPrefab, treeContainer);
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(start.x, start.y, 0));
        line.SetPosition(1, new Vector3(end.x, end.y, 0));

        lineRenderers.Add(line);
    }

    private void ClearVisualization()
    {
        foreach (GameObject nodeObj in nodeObjects)
        {
            Destroy(nodeObj);
        }
        nodeObjects.Clear();

        foreach (LineRenderer line in lineRenderers)
        {
            Destroy(line.gameObject);
        }
        lineRenderers.Clear();
    }
}