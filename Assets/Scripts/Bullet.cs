using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    public float bulletSpeed = 10f;
    public float bulletLifetime = 1f;
    public int damage = 50;
    public GameObject hitVFXPrefab;

    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        
        bulletRigidbody.velocity = transform.forward * bulletSpeed;

        // уничтожаем пулю со временем
        Destroy(gameObject, bulletLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider is owned by the local player 
        if (!other.CompareTag("Enemy"))
            return;

        EnemyHealth targetHealth = other.GetComponent<EnemyHealth>();

        //если есть компонент EnemyHealth - получаем урон
        if (targetHealth != null)
        {
            // Call the TakeDamage method on the target's EnemyHealth script using Photon RPC
            targetHealth.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);

            // VFX 
            Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);
        }

        // Уничтожаем пулю при столкновении
        PhotonNetwork.Destroy(gameObject);
    }
}


