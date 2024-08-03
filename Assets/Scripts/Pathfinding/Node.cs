// Adapted from: https://github.com/SebLague/Pathfinding-2D

using UnityEngine;

public class Node : IHeapItem<Node> {
	
	public bool walkable;
	public Vector2 worldPosition;
	public int gridX;
	public int gridY;
	public bool underwater;
	public float gCost;
	public float hCost;
	public Node parent;
	int heapIndex;
	
	public Node(bool _walkable, Vector2 _worldPos, int _gridX, int _gridY, bool _underwater)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		underwater = _underwater;
	}

	public Node Clone()
	{
		return new Node(walkable, worldPosition, gridX, gridY, underwater);
	}

	public float fCost
	{
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex
	{
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
