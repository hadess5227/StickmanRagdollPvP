using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RagdollCreatures;
using UnityEditor;
public class TnTAction : MonoBehaviour
{
    public GameObject bulletExplosion;
    public float damageRadius = 0;
    public int damage = 15;
    public GameObject nextCollider;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.transform.root.gameObject;
        if (tag != "platform")
        {
            if (collision.gameObject.tag == "Bullet")
            {
                Destroy(collision.gameObject);
                if (transform.childCount > 0)
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    transform.GetChild(0).GetComponent<AudioSource>().Play();
                    transform.GetChild(0).parent = null;
                }
                Destroy(transform.GetComponent<BoxCollider2D>());
                bulletExplosion.GetComponent<BulletExplosion>().explosionRadius = damageRadius;
                bulletExplosion.GetComponent<BulletExplosion>().damage = damage;
                Instantiate(bulletExplosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                if (collision.gameObject.GetComponent<RagdollLimb>() && obj.GetComponent<RagdollCreature>().aiCont && obj.GetComponent<RagdollCreature>().isDead == false)
                {
                    RagdollCreature ragdollCreature = obj.GetComponent<RagdollCreature>();
                    Rigidbody2D centerOfMass = ragdollCreature.centerOfMass?.rigidbody;
                    centerOfMass.AddForce(new Vector2(0, Vector2.up.y) * 120, ForceMode2D.Impulse);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.transform.root.gameObject;
        if (tag == "platform")
        {
            if (collision.gameObject.GetComponent<RagdollLimb>() && obj.GetComponent<RagdollCreature>().aiCont && obj.GetComponent<RagdollCreature>().isDead == false)
            {
                RagdollCreature ragdollCreature = obj.GetComponent<RagdollCreature>();
                Rigidbody2D centerOfMass = ragdollCreature.centerOfMass?.rigidbody;
                centerOfMass.AddForce(new Vector2(0, Vector2.up.y) * 50, ForceMode2D.Impulse);
                //nextCollider.SetActive(false);
            }
        }
    }
}
