using UnityEngine;

[CreateAssetMenu(fileName = "New Building Data", menuName = "Wrecked Tide/Building Data")]
public class BuildingData : ScriptableObject
{
    [Tooltip("The name displayed in the UI.")]
    public string buildingName;

    [Tooltip("The size of the building on the grid (Width x Height).")]
    public Vector2Int size = new Vector2Int(1, 1);

    [Header("Functionality")]
    public bool isMotor = false;

    // We can add more data here later, like resource costs
    // public int plasticCost;
    // public int metalCost;

    [Tooltip("The 3D model that will be placed in the world.")]
    public GameObject prefab;
    public float constructionTime;
    public int metalCost;
    public int plasticCost;
    public int weight;
    public int powerProvided = 4;

}