using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    private GameObject playerCam;
    [SerializeField] float moveSpeed = 5f;
    public float rotationSpeed = 1f;
    private Rigidbody tankRigidbody;
    public Transform forwardIndicator;
    private Quaternion targetRotation;
    public PhotonView photonview;

    // private bool isMovingForward = true;
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float bulletForce = 10f;

    void Awake()
    {
        tankRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            CheckInputs();
        }
    }

    private void CheckInputs()
    {
        float moveInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        // Получаем направление переда танка
        Vector3 forwardDirection = forwardIndicator.position - transform.position;
        forwardDirection.Normalize();

        Vector3 movement = forwardDirection * moveInput * moveSpeed * Time.deltaTime;

        tankRigidbody.MovePosition(tankRigidbody.position + movement);

        Quaternion rotation = Quaternion.Euler(0f, rotationInput * rotationSpeed * Time.deltaTime, 0f);
        tankRigidbody.MoveRotation(tankRigidbody.rotation * rotation);

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Создаем пулю из префаба
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, spawnPoint.position, spawnPoint.rotation);
        
        // Получаем компонент Rigidbody2D пули
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // Применяем силу выстрела к пуле
        rb.AddForce(spawnPoint.forward * bulletForce, ForceMode.Impulse);
    }
}
