using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;
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
    public event Action<int> OnNewWave;

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
            if ((enemiesRemainngToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainngToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;
                StartCoroutine(SpawnEnemy());

            }
        }
        if (devMode)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                StopCoroutine("SpawnEnemy");
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
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
        // Spawning enemy
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position+ Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.onDeath += SpawnedEnemy_onDeath;
        spawnedEnemy.SetCharist(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }

    private void OnPlayerDeath()
    {
        isDisabled = true;
    }
    private void SpawnedEnemy_onDeath()
    {
        
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPos()
    {
        playerTrans.position = map.GetTileFromPos(Vector3.zero).position + Vector3.up*3;
    }

    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainngToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainngToSpawn;
            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPos();
        }


    }

    [Serializable]
    public class wave
    {

        public float timeBetweenSpawn;
        public bool infinite;
        public int enemyCount;
        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;

    }
}



   
