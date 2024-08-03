using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MCTSAgent : Agent
{
    public bool simulationTimeMs;
    public float totalSims = 2500;
    public float c = Mathf.Sqrt(2.0f);

    public override int GetMove(Connect4State state)
    {
        NodeConnect4 root = new NodeConnect4(state.Clone());
        if (simulationTimeMs)
        {
            totalSims = 1000f;
            DateTime startTime = DateTime.Now;
            TimeSpan elapsed = DateTime.Now - startTime;
            while (elapsed.TotalMilliseconds < totalSims)
            {
                NodeConnect4 node = TreePolicy(root);
                float result = Simulation(node.state.Clone());
                Backpropagate(node, result);

                elapsed = DateTime.Now - startTime;
            }
            int bestMove = argMax(
                root.children.Select(child => (float)child.payoff / child.visits).ToArray()
            );
            return root.children[bestMove].move;
        }
        else
        {
            for (int i = 0; i < totalSims; i++)
            {
                NodeConnect4 node = TreePolicy(root);
                float result = Simulation(node.state.Clone());
                Backpropagate(node, result);
            }
            int bestMove = argMax(
                root.children.Select(child => (float)child.payoff / child.visits).ToArray()
            );
            return root.children[bestMove].move;
        }
    }

    public NodeConnect4 TreePolicy(NodeConnect4 node)
    {
        NodeConnect4 selectedNode = null;
        while (node != null)
        {
            selectedNode = node;
            if (node.children.Count < node.state.GetPossibleMoves().Count)
            {
                return Expand(node);
            }
            else
            {
                node = Selection(node);
            }
        }
        return selectedNode;
    }

    public NodeConnect4 Expand(NodeConnect4 node)
    {
        Connect4State state = node.state.Clone();
        List<int> possibleMoves = state.GetPossibleMoves();
        int randomMove = possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Count)];
        NodeConnect4 existingChild = node.children.Find(child => child.move == randomMove);
        while (existingChild != null)
        {
            randomMove = possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Count)];
            existingChild = node.children.Find(child => child.move == randomMove);
        }
        state.MakeMove(randomMove);
        NodeConnect4 childNode = new NodeConnect4(state);
        childNode.move = randomMove;
        node.children.Add(childNode);
        childNode.parent = node;
        return childNode;
    }

    public NodeConnect4 Selection(NodeConnect4 node)
    {
        if (node.children.Count == 0)
        {
            return null;
        }

        float totalVisits = node.visits;
        NodeConnect4 selectedChild = null;
        float bestUTCValue = float.MinValue;
        foreach (NodeConnect4 child in node.children)
        {
            float uctValue = child.UCTValue(totalVisits, c);
            if (uctValue > bestUTCValue)
            {
                bestUTCValue = uctValue;
                selectedChild = child;
            }
        }
        return selectedChild;
    }

    public float Simulation(Connect4State state)
    {
        Connect4State.Result gameResult = state.GetResult();

        while (gameResult == Connect4State.Result.Undecided)
        {
            List<int> possibleMoves = state.GetPossibleMoves();
            int randomMove = possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Count)];
            state.MakeMove(randomMove);
            gameResult = state.GetResult();
        }
        return Connect4State.ResultToFloat(gameResult);
    }

    private void Backpropagate(NodeConnect4 node, float result)
    {
        while (node != null)
        {
            node.visits++;
            float finalResult = 0;
            switch (result)
            {
                case 0:
                    if (node.state.GetPlayerTurn() == 1)
                    {
                        finalResult = 1;
                    }
                    else
                    {
                        finalResult = 0;
                    }
                    break;
                case 1:
                    if (node.state.GetPlayerTurn() == 0)
                    {
                        finalResult = 1;
                    }
                    else
                    {
                        finalResult = 0;
                    }
                    break;
                default:
                    finalResult = 0.5f;
                    break;
            }
            node.payoff += finalResult;
            node = node.parent;
        }
    }
}
