using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviourPunCallbacks, IPunObservable
{
    public float moveSpeed = 5f;
    public float detectionRadius = 10f;
    public float attackRadius = 2f;
    public int maxHealth = 100;
    public Image fillImage;

    private GameObject player;
    private int currentHealth;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private bool isPlayerSpawned = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;
    }

    private void Update()
    {   
        if (photonView.IsMine)
        {   Debug.Log("1");
            if (player != null )
            {   Debug.Log("2");
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                // Если игрок находится в радиусе обнаружения, следуем за ним
                Debug.Log("3");
                if (distanceToPlayer >= detectionRadius )
                {
                    Vector3 direction = player.transform.position - transform.position;
                    direction.y = 0f;
                    direction.Normalize();
                    transform.position += direction * Time.deltaTime * moveSpeed;
                    transform.LookAt(player.transform);
                    Debug.Log("4");
                }
                // Если игрок в пределах радиуса атаки, атакуем его
                else if (distanceToPlayer <= attackRadius)
                {
                    SceneManager.LoadScene(0);
                    Debug.Log("5");
                }
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 5f);
            Debug.Log("5");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine && other.CompareTag("Player"))
        {
            isPlayerSpawned = true;
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}

