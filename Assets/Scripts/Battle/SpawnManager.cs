using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Transform[] _playerSpawnPoints;
    [SerializeField]
    private Transform[] _enemySpawnPoints;

    public (Transform player, Transform enemy) GetSpawnPoints()
    {
        int index = Random.Range(0, _playerSpawnPoints.Length);

        return (_playerSpawnPoints[index], _enemySpawnPoints[index]);
    }

    public (GameObject player, GameObject enemy) SpawnUnit(GameObject playerPrefab, GameObject enemyPrefab)
    {
        var (player, enemy) = GetSpawnPoints();

        GameObject playerGameobject = Instantiate(playerPrefab, player.position, playerPrefab.transform.rotation);
        playerGameobject.tag = "Player";
        playerGameobject.layer = LayerMask.NameToLayer("Stick");

        GameObject enemyGameObject = Instantiate(enemyPrefab, enemy.position, enemyPrefab.transform.rotation);
        enemyGameObject.tag = "Enemy";
        enemyGameObject.layer = LayerMask.NameToLayer("Stick");

        return (playerGameobject, enemyGameObject);
    }

}
