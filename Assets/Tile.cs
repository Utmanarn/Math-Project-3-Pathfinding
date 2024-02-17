using BoardGame;
using UnityEngine;

[ExecuteInEditMode]
public class Tile : TileParent {
    
    // 1. TileParent extends MonoBehavior, so you can add member variables here
    // to store data.
    public Material regularMaterial;
    public Material blockedMaterial;
    public Material pathMaterial;
    public Material frontierMaterial;
    public Material canBeReachedMaterial;
    public Material obstacleMaterial;
    public Material checkpointMaterial;
    public Material portalMaterial;

    public GameObject text;

    public int distanceFromStart = 0;
    public int priority = 0;
    
    public bool OnPath = false;
    public bool IsFrontier = false;
    public bool CanBeReached = false;

    // This function is called when something has changed on the board. All 
    // tiles have been created before it is called.
    public override void OnSetup(Board board) {
    }

    // This function is called during the regular 'Update' step, but also gives
    // you access to the 'board' instance.
    public override void OnUpdate(Board board) {
        // Change the material color under certain conditions.
        if (TryGetComponent<MeshRenderer>(out var meshRenderer)) {
            if (IsBlocked) {
                meshRenderer.sharedMaterial = blockedMaterial;
            } else if (OnPath) {
                meshRenderer.sharedMaterial = pathMaterial;
            } else if (IsCheckPoint) {
                meshRenderer.sharedMaterial = checkpointMaterial;
            } else if (CanBeReached) {
                meshRenderer.sharedMaterial = canBeReachedMaterial;
            } else if (IsPortal(out Vector2Int t)) {
                meshRenderer.sharedMaterial = portalMaterial;
            } else if (IsObstacle(out int p)) {
                meshRenderer.sharedMaterial = obstacleMaterial;
            } else if (IsFrontier) {
                meshRenderer.sharedMaterial = frontierMaterial;
            } else {
                meshRenderer.sharedMaterial = regularMaterial;
            }
        }
    }

    public void ResetValues()
    {
        OnPath = false;
        IsFrontier = false;
        CanBeReached = false;
        priority = 0;
        distanceFromStart = 0;
    }
}