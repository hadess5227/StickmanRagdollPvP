using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PUN2_RigidbodySync : MonoBehaviourPun, IPunObservable
{
    Rigidbody2D r;
    Vector3 latestPos;
    Quaternion latestRot;
    Vector2 velocity;
    float angularVelocity;

    bool valuesReceived = false;
    PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        
        /*
        if(photonView.IsMine)
        {
            gameObject.tag = "Player";
            r.isKinematic = true;
        }
        */
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(r.velocity);
            stream.SendNext(r.angularVelocity);
        }
        else
        {
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            velocity = (Vector2)stream.ReceiveNext();
            angularVelocity = (float)stream.ReceiveNext();

            valuesReceived = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(!photonView.IsMine && valuesReceived)
        {
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
            r.velocity = velocity;
            r.angularVelocity = angularVelocity;
        }
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if(!photonView.IsMine)
        {
            Transform collisionObjectRoot = collision.transform.root;
            if(collisionObjectRoot.CompareTag("Player"))
            {
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }
    }
    */
}
