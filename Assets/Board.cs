using BoardGame;
using UnityEngine;

[ExecuteInEditMode]
public class Board : BoardParent
{
    public Dijkstras pathFinding;

    // This function is called whenever the board or any tile inside the board
    // is modified.
    public override void SetupBoard() {
        pathFinding = GetComponent<Dijkstras>();
        pathFinding.ResetValues();

        GameObject[] text;
 
        text = GameObject.FindGameObjectsWithTag("Text");
 
        foreach(GameObject text1 in text)
        {
            DestroyImmediate(text1, true);
        }

        // Iterate over all tiles
        foreach (Tile tile in Tiles)
        {
            tile.ResetValues();
            
            if (tile.IsCheckPoint)
            {
                pathFinding.IncrementVisitedCheckpoints(); // Add 1 more checkpoint to find when we start searching for checkpoints.
            }

            if (!tile.IsStartPoint) continue;
            tile.IsFrontier = true;
            pathFinding.Enqueue(tile, 0);
        }
    }
}
