using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject canvas;
    public GameObject sceneCam;
    public float enemySpawnRadius = 10f;
    public int numEnemiesToSpawn = 5;
    public float spawnInterval = 3f;

    public static GameManager instance = null;

    void Awake()
    {
        instance = this;
        canvas.SetActive(true);
    }

    public void SpawnPlayer()
    {
        float randomValue = Random.Range(5, -5);
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(playerPrefab.transform.position.x * randomValue, playerPrefab.transform.position.y), Quaternion.identity, 0);
        canvas.SetActive(false);
        sceneCam.SetActive(true);
        
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < numEnemiesToSpawn; i++)
        {
            Vector3 randomPosition = GetRandomPosition();
            PhotonNetwork.Instantiate(enemyPrefab.name, randomPosition, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetRandomPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(0f, enemySpawnRadius);

        float x = Mathf.Cos(angle) * distance;
        float z = Mathf.Sin(angle) * distance;

        Vector3 randomPosition = new Vector3(x, 0f, z);

        return randomPosition;
    }
}
