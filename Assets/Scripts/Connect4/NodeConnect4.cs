using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeConnect4
{
    public float visits;
    public float payoff;
    public int move;
    public Connect4State state;
    public List<NodeConnect4> children;
    public NodeConnect4 parent;
    public NodeConnect4(Connect4State _state)
    {
        state = _state;
        visits = 0;
        payoff = 0;
        children = new List<NodeConnect4>();
    }

    public float UCTValue(float totalVisits, float c)
    {
        if (visits == 0){
            visits = 0.00000001f;
        }
            
        return (payoff / visits) + c * Mathf.Sqrt(Mathf.Log(totalVisits) / visits);
    }
}
