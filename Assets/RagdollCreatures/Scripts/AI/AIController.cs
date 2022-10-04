using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public string AIName = "";
    public float attackRange = 30.0f;
    public GameObject enemy;
    public Interact interact;
    public GameObject[] weapons = new GameObject[6];
    // Start is called before the first frame update
    void Start()
    {
        int index = Random.RandomRange(0, 5);
        interact.currentInteractable = weapons[index];
        weaponSelected(index);
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void weaponSelected(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
        weapons[index].SetActive(true);
    }
    public bool EnemyInAttackRange()
    {
        enemy = null;
        Vector2 point = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, attackRange);
        foreach (Collider2D ehit in colliders)
        {
            if (IsMe(ehit.gameObject) == false && IsTeam(ehit.gameObject) == false && ehit.GetComponent<RagdollLimb>())
            {
                enemy = ehit.transform.root.gameObject;
                return true;
            }
        }
        return false;
    }
    bool IsMe(GameObject obj)
    {
        foreach (Transform trans in transform.parent)
        {
            if (trans.gameObject == obj)
            {
                return true;
            }
        }
        return false;
    }

    bool IsTeam(GameObject obj)
    {
        RagdollCreature ragdollCreature = obj.transform.root.GetComponent<RagdollCreature>();
        if (ragdollCreature == null)
        {
            return true;
        }
        else
        {
            return ragdollCreature.aiCont;
        }
    }
}
