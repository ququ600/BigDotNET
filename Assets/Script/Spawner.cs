using System.Collections;
using UnityEngine;
using System;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Wave[] waves;

    public int currentWaveIndex;
    public Wave currentWave;
    public int waitSpawnNum;//这一波还剩下多少敌人没有生成，等于0以后不再生成新的敌人
    public int spawnAliveNum;//这一波的敌人还存活了多少个，少于0的话，进行下一波
    public float nextSpawnTime;

    private MapGenerator mapGenerator;

    private bool isDisabled;
    private LivingEntity playerEntity;

    public event Action<int> onNewWave;//这个事件，将会在NextWave内部逻辑这个方法中触发。由于每一波的序号不同，所以这里使用的是Action<int>

    private void Start()
    {
        playerEntity = FindObjectOfType<LivingEntity>();
        playerEntity.onDeath += PlayerDeath;
        mapGenerator = FindObjectOfType<MapGenerator>();
        NextWave();
    }


    private void Update()
    {
        if (!isDisabled)
        {
            if ((waitSpawnNum > 0) /*&& Time.time > nextSpawnTime*/)
            {
                waitSpawnNum--;
                nextSpawnTime = Time.time + 1;

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    private void NextWave()
    {
        currentWaveIndex++;
        Debug.Log(string.Format("[Current Wave] : {0}", currentWaveIndex));
        currentWave = waves[currentWaveIndex - 1];
        waitSpawnNum = currentWave.enemyNum;
        spawnAliveNum = currentWave.enemyNum;

    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1.0f;
        float tileFlashSpeed = 4;

        Transform randomTile = mapGenerator.GetRandomOpenTile();

        #region
        Material tileMat = randomTile.GetComponent<MeshRenderer>().material;
        Color originalColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;
        #endregion

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        //GameObject spawnEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        GameObject spawnEnemy = Instantiate(enemyPrefab, randomTile.position + Vector3.up, Quaternion.identity);
        spawnEnemy.GetComponent<Enemy>().onDeath += EnemyDeath;//CORE

        // spawnEnemy.GetComponent<Enemy>().SetDifficulty(currentWave.enemySpeed, currentWave.enemyDamage, currentWave.enemyHealth, currentWave.enemySkinColor);
    }

    private void EnemyDeath()
    {
        spawnAliveNum--;
        if (spawnAliveNum <= 0)
            NextWave();
    }

    private void PlayerDeath()
    {
        isDisabled = true;
    }

}
