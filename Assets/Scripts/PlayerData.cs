using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] playerStats;
    public float[] playerPositionAndRotation;

    public string[] inventoryContent;
    public string[] hotbarContent;

    public PlayerData(float[] _playerStats, float[] _playerPositionAndRotation, string[] _inventoryContent, string[] _hotbarContent)
    {
        playerStats = _playerStats;
        playerPositionAndRotation = _playerPositionAndRotation;
        inventoryContent = _inventoryContent;
        hotbarContent = _hotbarContent;
    }
}