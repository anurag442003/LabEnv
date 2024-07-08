using UnityEngine;

/// <summary>
/// Manages the stone on the body
/// </summary>
public class StoneArea : MonoBehaviour
{
    // The stone in this stone area
    public Stone stone;

    /// <summary>
    /// Reset the stone
    /// </summary>
    public void ResetStone()
    {
        // Reset the stone
        stone.ResetStone();
    }

    /// <summary>
    /// Gets the <see cref="Stone"/> that the collider belongs to
    /// </summary>
    /// <param name="collider">The collider</param>
    /// <returns>The matching stone</returns>
    public Stone GetStoneFromCollider(Collider collider)
    {
        // Check if the collider belongs to the stone
        return stone != null && stone.stoneCollider == collider ? stone : null;
    }

    /// <summary>
    /// Called when the area wakes up
    /// </summary>
    private void Awake()
    {
        // Ensure the stone reference is set
        if (stone == null)
        {
            Debug.LogError("Stone not set in StoneArea");
        }
    }
}
