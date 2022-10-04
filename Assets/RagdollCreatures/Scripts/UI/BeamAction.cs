using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RagdollCreatures;
public class BeamAction : MonoBehaviour
{
    public float forceSize = 50;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<RagdollLimb>())
        {
            RagdollCreature ragdollCreature = collision.transform.root.GetComponent<RagdollCreature>();
            //ragdollCreature.deactivateMusclesInAir = true;
            Rigidbody2D centerOfMass = ragdollCreature.centerOfMass?.rigidbody;
            //centerOfMass.velocity = Vector2.up * 100;
            centerOfMass.AddForce(Vector2.up * forceSize, ForceMode2D.Impulse);

            if(tag == "end" && collision.transform.root.GetComponent<RagdollCreature>().aiCont == true)
            {
                ragdollCreature.GetComponent<RagdollCreatureController>().controller.horizontalMove = new Vector2(-2.0f, 0);
            }
        }
    }
    /*
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<RagdollLimb>())
        {
            if(!forced)
            {
                RagdollCreature ragdollCreature = collision.transform.root.GetComponent<RagdollCreature>();
                //ragdollCreature.deactivateMusclesInAir = true;
                Rigidbody2D centerOfMass = ragdollCreature.centerOfMass?.rigidbody;
                //centerOfMass.velocity = Vector2.up * 100;
                centerOfMass.AddForce(Vector2.up * 50, ForceMode2D.Impulse);
            }

            if (forced)
                forced = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<RagdollLimb>())
        {
            RagdollCreature ragdollCreature = collision.transform.root.GetComponent<RagdollCreature>();
            //ragdollCreature.deactivateMusclesInAir = true;
            Rigidbody2D centerOfMass = ragdollCreature.centerOfMass?.rigidbody;
            //centerOfMass.velocity = Vector2.up * 100;
            centerOfMass.AddForce(Vector2.up * 50, ForceMode2D.Impulse);
        }
    }
    */
}
