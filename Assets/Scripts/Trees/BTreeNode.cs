using System;
using System.Collections.Generic;
using UnityEngine;

public class BTreeNode
{
    public int[] Keys { get; set; }
    public BTreeNode[] Children { get; set; }
    public int KeyCount { get; set; }
    public bool IsLeaf { get; set; }
    public int Order { get; set; }

    public BTreeNode(int order, bool isLeaf)
    {
        Order = order;
        IsLeaf = isLeaf;
        Keys = new int[2 * order - 1];
        Children = new BTreeNode[2 * order];
        KeyCount = 0;
    }
}

public class BTree : ITree
{
    private BTreeNode root;
    private int order;
    private List<TreeNode> convertedNodes; // For visualization compatibility

    public BTree(int order = 3)
    {
        this.order = order;
        root = new BTreeNode(order, true);
        convertedNodes = new List<TreeNode>();
    }

    public TreeType GetTreeType()
    {
        return TreeType.BTree;
    }

    public void Insert(int value)
    {
        // If root is full, split it
        if (root.KeyCount == 2 * order - 1)
        {
            BTreeNode newRoot = new BTreeNode(order, false);
            newRoot.Children[0] = root;
            SplitChild(newRoot, 0);
            root = newRoot;
            InsertNonFull(root, value);
        }
        else
        {
            InsertNonFull(root, value);
        }

        // Update converted nodes for visualization
        UpdateConvertedNodes();
    }

    private void InsertNonFull(BTreeNode node, int value)
    {
        int i = node.KeyCount - 1;

        if (node.IsLeaf)
        {
            // Find the position to insert the key
            while (i >= 0 && value < node.Keys[i])
            {
                node.Keys[i + 1] = node.Keys[i];
                i--;
            }

            // Insert the key
            node.Keys[i + 1] = value;
            node.KeyCount++;
        }
        else
        {
            // Find the child which is going to have the new key
            while (i >= 0 && value < node.Keys[i])
            {
                i--;
            }
            i++;

            // If the child is full, split it
            if (node.Children[i].KeyCount == 2 * order - 1)
            {
                SplitChild(node, i);
                if (value > node.Keys[i])
                {
                    i++;
                }
            }
            InsertNonFull(node.Children[i], value);
        }
    }

    private void SplitChild(BTreeNode parent, int index)
    {
        BTreeNode child = parent.Children[index];
        BTreeNode newChild = new BTreeNode(order, child.IsLeaf);
        newChild.KeyCount = order - 1;

        // Copy the last (t-1) keys of child to newChild
        for (int j = 0; j < order - 1; j++)
        {
            newChild.Keys[j] = child.Keys[j + order];
        }

        // Copy the last t children of child to newChild
        if (!child.IsLeaf)
        {
            for (int j = 0; j < order; j++)
            {
                newChild.Children[j] = child.Children[j + order];
            }
        }

        // Reduce the number of keys in child
        child.KeyCount = order - 1;

        // Create space for new child in parent
        for (int j = parent.KeyCount; j > index; j--)
        {
            parent.Children[j + 1] = parent.Children[j];
        }

        // Link the new child to parent
        parent.Children[index + 1] = newChild;

        // Move the middle key of child to parent
        for (int j = parent.KeyCount - 1; j >= index; j--)
        {
            parent.Keys[j + 1] = parent.Keys[j];
        }
        parent.Keys[index] = child.Keys[order - 1];
        parent.KeyCount++;
    }

    private void UpdateConvertedNodes()
    {
        convertedNodes.Clear();
        ConvertToTreeNodes(root, null, 0);
    }

    private void ConvertToTreeNodes(BTreeNode bNode, TreeNode parent, int depth)
    {
        if (bNode == null) return;

        // Create a simple representation for visualization
        TreeNode node = new TreeNode(bNode.Keys[0]);
        convertedNodes.Add(node);

        if (!bNode.IsLeaf)
        {
            for (int i = 0; i <= bNode.KeyCount; i++)
            {
                ConvertToTreeNodes(bNode.Children[i], node, depth + 1);
            }
        }
    }

    public bool ValidateChallenge(string challengeType, int parameter)
    {
        switch (challengeType)
        {
            case "height":
                return GetHeight(root) >= parameter;
            case "nodes":
                return CountNodes(root) >= parameter;
            default:
                return false;
        }
    }

    private int GetHeight(BTreeNode node)
    {
        if (node == null || node.IsLeaf)
            return 0;

        int maxHeight = 0;
        for (int i = 0; i <= node.KeyCount; i++)
        {
            int height = GetHeight(node.Children[i]);
            maxHeight = Math.Max(maxHeight, height);
        }
        return maxHeight + 1;
    }

    private int CountNodes(BTreeNode node)
    {
        if (node == null)
            return 0;

        int count = 1;
        if (!node.IsLeaf)
        {
            for (int i = 0; i <= node.KeyCount; i++)
            {
                count += CountNodes(node.Children[i]);
            }
        }
        return count;
    }

    public List<TreeNode> GetNodes()
    {
        return convertedNodes;
    }
}