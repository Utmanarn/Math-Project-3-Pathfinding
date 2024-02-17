using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Dijkstras : MonoBehaviour
{
    private readonly List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };
    
    private readonly PriorityQueue<Tile> frontier = new PriorityQueue<Tile>();
    private readonly Dictionary<Tile, Tile> parents = new Dictionary<Tile, Tile>();

    private Tile current;
    
    private Board board;

    public bool manualStepThrough;
    
    public int maxSteps = 3;
    
    private int checkpointCount;
    private int visitedCheckpoints;

    private void OnEnable()
    {
        board = GetComponent<Board>();
    }

    private void Update()
    {
        RunPathFinding();
    }

    private void RunPathFinding()
    {
        if (!manualStepThrough)
        {
            while (frontier.Count > 0)
            {
                PathCheck();
            }
        }
        else
        {
            if (!Input.GetKeyDown(KeyCode.K)) return;
            if (frontier.Count > 0)
                PathCheck();
        }
    }

    private void PathCheck()
    {
        current = frontier.Dequeue();
        
        if (current.IsCheckPoint)
        {
            Vector3 keyVector3 = new Vector3(current.coordinate.x, 0.05f, current.coordinate.y);
            
            GameObject prefab = Instantiate(current.text, keyVector3, Quaternion.Euler(90, 0, 0));

            prefab.GetComponentInChildren<TextMesh>().text = current.priority.ToString();
            
            SavePath();
        }

        foreach (Tile next in GetNeighbors(current))
        {
            if (next == null) continue; // Expensive comparison
            if (next.IsStartPoint) continue; 
            if (next.IsBlocked) continue;
            if (next.IsObstacle(out int weight)) {}
            else weight = 0;
            if (parents.TryGetValue(next, out Tile par)) // If next already has a parent.
            {
                if (current.priority < par.priority) // This doesn't work it needs to store the priority otherwise it ignores the weight.
                    parents.Remove(next); // If the new distance is shorter than the old distance, remove the old parent.
                else
                    continue;
            }

            next.distanceFromStart = current.distanceFromStart + 1;
            next.priority = weight + next.distanceFromStart; // Priority needs to add 1 the further from start it gets.
            
            if (next.priority < maxSteps + 1)
            {
                next.CanBeReached = true;
            }

            Enqueue(next, next.priority);
            parents.Add(next, current);
            next.IsFrontier = true;
        }
    }

    public void Enqueue(Tile tile, double priority)
    {
        frontier.Enqueue(tile, priority);
    }
    
    // Gets all the neighbouring tiles and returns them as a list of tiles.
    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        if (current.IsPortal(out Vector2Int target))
            if (board.TryGetTile(target, out Tile portalOut)) 
                            neighbors.Add(portalOut); // If current is on a portal tile and the exit to the portal is not outside the board boundaries, add it to the list.
        
        for (int i = 0; i < directions.Count; i++) // Check each direction for a possible neighbor tile.
        {
            if (board.TryGetTile(tile.coordinate - directions[i], out Tile neighborTile))
            {
                neighbors.Add(neighborTile);
            }
        }

        return neighbors;
    }
    
    // Save the path to the goal by going up the list of parents till the start tile is found.
    private void SavePath()
    {

        while (!current.IsStartPoint)
        {
            current.OnPath = true;
            current = parents[current];
        }
        current.OnPath = true; // Sets the start tile to be on the path.
        visitedCheckpoints += 1;

        if (checkpointCount <= visitedCheckpoints)
        {
            frontier.Clear(); // When we have found all the checkpoints, we don't want to continue searching.
        }
    }

    public void IncrementVisitedCheckpoints()
    {
        checkpointCount += 1;
    }
    
    public void ResetValues()
    {
        frontier.Clear();
        parents.Clear();
        visitedCheckpoints = 0;
        checkpointCount = 0;
    }
}