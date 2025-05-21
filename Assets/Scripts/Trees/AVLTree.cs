using System;
using System.Collections.Generic;
using UnityEngine;

public class AVLTree : ITree
{
    private TreeNode root;

    public AVLTree()
    {
        root = null;
    }

    public TreeType GetTreeType()
    {
        return TreeType.AVL;
    }

    public void Insert(int value)
    {
        root = InsertRec(root, value);
    }

    private TreeNode InsertRec(TreeNode node, int value)
    {
        if (node == null)
            return new TreeNode(value);

        if (value < node.Value)
            node.Left = InsertRec(node.Left, value);
        else if (value > node.Value)
            node.Right = InsertRec(node.Right, value);
        else
            return node; // Duplicate values are not allowed

        // Update height
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

        // Get balance factor
        int balance = GetBalance(node);

        // Left Left Case
        if (balance > 1 && value < node.Left.Value)
            return RightRotate(node);

        // Right Right Case
        if (balance < -1 && value > node.Right.Value)
            return LeftRotate(node);

        // Left Right Case
        if (balance > 1 && value > node.Left.Value)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }

        // Right Left Case
        if (balance < -1 && value < node.Right.Value)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        return node;
    }

    private int GetHeight(TreeNode node)
    {
        if (node == null)
            return 0;
        return node.Height;
    }

    private int GetBalance(TreeNode node)
    {
        if (node == null)
            return 0;
        return GetHeight(node.Left) - GetHeight(node.Right);
    }

    private TreeNode RightRotate(TreeNode y)
    {
        TreeNode x = y.Left;
        TreeNode T2 = x.Right;

        // Perform rotation
        x.Right = y;
        y.Left = T2;

        // Update heights
        y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
        x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

        return x;
    }

    private TreeNode LeftRotate(TreeNode x)
    {
        TreeNode y = x.Right;
        TreeNode T2 = y.Left;

        // Perform rotation
        y.Left = x;
        x.Right = T2;

        // Update heights
        x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
        y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

        return y;
    }

    public bool ValidateChallenge(string challengeType, int parameter)
    {
        switch (challengeType)
        {
            case "balanced":
                return IsBalanced(root);
            case "size":
                return CountNodes(root) >= parameter;
            default:
                return false;
        }
    }

    private bool IsBalanced(TreeNode node)
    {
        if (node == null)
            return true;

        int balance = GetBalance(node);
        return Math.Abs(balance) <= 1 && IsBalanced(node.Left) && IsBalanced(node.Right);
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