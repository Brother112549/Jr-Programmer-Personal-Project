using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject pickupPrefab;

    [SerializeField] private int m_waveNumber = 1;
    public int waveNumber { 
        get { return m_waveNumber; } 
        private set { m_waveNumber = value; }
    }

    private GameManager gameManager;
    private float spawnRangeX = 16;
    private float spawnRangeZ = 8;
    private float spawnOnX = 19;
    private float spawnOnZ = 10;
    private int enemyCount;
    private bool isSpawningEnemies = false;

    // Start is called before the first frame update
    void Start() {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        SpawnEnemyWave(waveNumber);
        StartCoroutine(SpawnObstacles());
    }

    // Update is called once per frame
    void Update() {
        if (!isSpawningEnemies) {
            enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (enemyCount == 0) {
                SpawnEnemyWave(++waveNumber);
            }
        }
    }

    private Vector3 GenerateEdgeSpawnPosition() {
        int axis = Random.Range(0, 2);
        int side = Random.Range(0, 2);
        if (axis == 0 && side == 0) {
            //spawn on top edge
            float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
            float spawnPosZ = spawnOnZ;
            Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
            return randomPos;
        }
        else if (axis == 0) {
            //spawn on bottom edge
            float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
            float spawnPosZ = -spawnOnZ;
            Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
            return randomPos;
        }
        else if (side == 0) {
            //spawn on left edge
            float spawnPosX = -spawnOnX;
            float spawnPosZ = Random.Range(-spawnRangeZ, spawnRangeZ);
            Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
            return randomPos;
        }
        else {
            //spawn on right edge
            float spawnPosX = spawnOnX;
            float spawnPosZ = Random.Range(-spawnRangeZ, spawnRangeZ);
            Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
            return randomPos;
        }
    }

    private Vector3 GenerateSpawnPosition() {
        float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
        float spawnPosZ = Random.Range(-spawnRangeZ, spawnRangeZ);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

    private void SpawnEnemyWave(int enemiesToSpawn) {
        //spawn new pickup
        Instantiate(pickupPrefab, GenerateSpawnPosition(), pickupPrefab.transform.rotation);
        //use game manager to update ui
        gameManager.UpdateWave(waveNumber);
        Debug.Log("Call Spawn Enemies");
        isSpawningEnemies = true;
        StartCoroutine(SpawnEnemiesWithDelay(enemiesToSpawn));
    }

    IEnumerator SpawnEnemiesWithDelay(int enemiesToSpawn) {
        Debug.Log("Spawn Enemies");
        Vector3 enemyPos = GenerateEdgeSpawnPosition();
        for (int i = 0; i < enemiesToSpawn; i++) {
            Instantiate(enemyPrefab, enemyPos, enemyPrefab.transform.rotation);
            yield return new WaitForSeconds(1);
        }
        isSpawningEnemies = false;
    }

    IEnumerator SpawnObstacles() {
        while (true) {
            float delay = Random.Range(1, 5);
            Vector3 obSpawnPos = GenerateEdgeSpawnPosition();
            Vector3 obSpawnRot = obstaclePrefab.transform.rotation.eulerAngles;
            if (obSpawnPos.z == spawnOnZ) {
                Instantiate(obstaclePrefab, obSpawnPos, Quaternion.Euler(obSpawnRot.x, 180, obSpawnRot.z));
            }
            else if (obSpawnPos.z == -spawnOnZ) {
                Instantiate(obstaclePrefab, obSpawnPos, Quaternion.Euler(obSpawnRot.x, 0, obSpawnRot.z));
            }
            else if (obSpawnPos.x == -spawnOnX) {
                Instantiate(obstaclePrefab, obSpawnPos, Quaternion.Euler(obSpawnRot.x, 90, obSpawnRot.z));
            }
            else {
                Instantiate(obstaclePrefab, obSpawnPos, Quaternion.Euler(obSpawnRot.x, -90, obSpawnRot.z));
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
