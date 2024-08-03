using System.Collections.Generic;
using UnityEngine;

public class MonteCarloAgent : Agent
{
    public int totalSims = 2500;

    public override int GetMove(Connect4State state)
    {
        // TODO: Override this method with the logic described in class.
        // Currently, it just returns a random move.
        List<int> moves = state.GetPossibleMoves();
        float[] scores = new float[moves.Count];
        for (int i = 0; i < moves.Count; i++)
        {
            int move = moves[i];
            float totalScore = 0;
            for (int j = 0; j < totalSims / moves.Count; j++)
            {
                Connect4State simState = state.Clone();
                simState.MakeMove(move);
                Connect4State.Result gameResult = simState.GetResult();
                while (gameResult == Connect4State.Result.Undecided)
                {
                    List<int> simMoves = simState.GetPossibleMoves();
                    int randomMove = simMoves[Random.Range(0, simMoves.Count)];
                    simState.MakeMove(randomMove);
                    gameResult = simState.GetResult();
                }
                float winner = ResultToFloat(simState);
                totalScore += winner;
            }
            scores[i] = totalScore;
        }
        int bestMoveIndex = argMax(scores);
        return moves[bestMoveIndex];
    }

    private float ResultToFloat(Connect4State state)
    {
        Connect4State.Result gameResult = state.GetResult();
        switch (gameResult)
        {
            case Connect4State.Result.YellowWin:
                return (playerIdx == 0) ? 1 : 0;
            case Connect4State.Result.RedWin:
                return (playerIdx == 1) ? 1 : 0;
            case Connect4State.Result.Draw:
                return 0.5f;
            default:
                return 0.5f;
        }
    }
}
