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

    int enemiesRemainngToSpawn;
    float nextSpawnTime;

    private void Start()
    {
        NextWave(); 
    }

    void Update()
    {
        if(enemiesRemainngToSpawn>0 && Time.time > nextSpawnTime)
        {
            enemiesRemainngToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnedEnemy.onDeath += SpawnedEnemy_onDeath;
        }
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


    // Start is called before the first frame update
   

    // Update is called once per frame
   
