using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public wave[] waves;
    public Enemy enemy;
    wave currentWave;
    int currentWaveNumber;
    int enemiesRemainingAlive;

    LivingEntity playerEntity;
    Transform playerTrans;

    int enemiesRemainngToSpawn;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2f;
    float campThreashOldDistance = 1.5f;
    float nectCampCheckTime;
    Vector3 campPosHOld;
    bool isCamping;
    bool isDisabled;

    private void Start()
    {
        playerEntity = FindAnyObjectByType<Player>();
        playerTrans = playerEntity.transform;

        nectCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPosHOld = playerTrans.position;
        playerEntity.onDeath += OnPlayerDeath;

        map = FindAnyObjectByType<MapGenerator>();
        NextWave(); 
    }

    void Update()
    {
        if (!isDisabled)
        {
            if (enemiesRemainngToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainngToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;
                StartCoroutine(SpawnEnemy());

            }
        }
        
    }

    IEnumerator SpawnEnemy()
    {
        if (Time.time > nectCampCheckTime)
        {
            nectCampCheckTime = Time.time + timeBetweenCampingChecks;
            isCamping = (Vector3.Distance(playerTrans.position, campPosHOld) < campThreashOldDistance);
            campPosHOld = playerTrans.position;
        }
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPos(playerTrans.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;
        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed,1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position+ Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.onDeath += SpawnedEnemy_onDeath;
    }

    private void OnPlayerDeath()
    {
        isDisabled = true;
    }
    private void SpawnedEnemy_onDeath()
    {
        Debug.Log("Died Enemy");
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainngToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainngToSpawn;
        }

    }

    [Serializable]
    public class wave
    {
        public int enemyCount;
        public float timeBetweenSpawn;
    }
}



   
