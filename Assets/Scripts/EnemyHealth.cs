using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealth : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float hitPoints = 100f;

    bool isDead = false;

    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine) return;

        photonView.RPC("SyncDamage", RpcTarget.AllBuffered, damage);
    }

    [PunRPC]
    private void SyncDamage(float damage)
    {
        hitPoints -= damage;

        if (hitPoints <= 0)
        {
            photonView.RPC("Dead", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void Dead()
    {
        if (isDead) return;
        isDead = true;
        GetComponent<Animator>().SetTrigger("dead");

        // Уничтожаем объект врага через корутину, чтобы синхронизация завершилась
        StartCoroutine(DestroyEnemy());
    }

    private IEnumerator DestroyEnemy()
    {
        // Ждем некоторое время перед уничтожением объекта
        yield return new WaitForSeconds(2f);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hitPoints);
            stream.SendNext(isDead);
        }
        else
        {
            hitPoints = (float)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
        }
    }
}

