using System;
using System.Collections.Generic;
using UnityEngine;

public class BinarySearchTree : ITree
{
    private TreeNode root;

    public BinarySearchTree()
    {
        root = null;
    }

    public TreeType GetTreeType()
    {
        return TreeType.BST;
    }

    public void Insert(int value)
    {
        root = InsertRec(root, value);
    }

    private TreeNode InsertRec(TreeNode root, int value)
    {
        if (root == null)
            return new TreeNode(value);

        if (value < root.Value)
            root.Left = InsertRec(root.Left, value);
        else if (value > root.Value)
            root.Right = InsertRec(root.Right, value);

        return root;
    }

    public bool ValidateChallenge(string challengeType, int parameter)
    {
        switch (challengeType)
        {
            case "depth":
                return GetDepth(root) >= parameter;
            case "size":
                return CountNodes(root) >= parameter;
            default:
                return false;
        }
    }

    private int GetDepth(TreeNode node)
    {
        if (node == null)
            return 0;
        else
        {
            int leftDepth = GetDepth(node.Left);
            int rightDepth = GetDepth(node.Right);

            return Math.Max(leftDepth, rightDepth) + 1;
        }
    }

    private int CountNodes(TreeNode node)
    {
        if (node == null)
            return 0;
        else
            return CountNodes(node.Left) + CountNodes(node.Right) + 1;
    }

    public List<TreeNode> GetNodes()
    {
        List<TreeNode> nodes = new List<TreeNode>();
        InOrderTraversal(root, nodes);
        return nodes;
    }

    private void InOrderTraversal(TreeNode node, List<TreeNode> nodes)
    {
        if (node != null)
        {
            InOrderTraversal(node.Left, nodes);
            nodes.Add(node);
            InOrderTraversal(node.Right, nodes);
        }
    }
}