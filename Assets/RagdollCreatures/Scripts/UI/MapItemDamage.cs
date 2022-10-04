using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RagdollCreatures;
public class MapItemDamage : MonoBehaviour, IWeapon
{
    // Start is called before the first frame update
    public int damage = 0;
    private WeaponType weaponType = WeaponType.Meele;
    public GameObject _parent;
    public GameObject _GetParent()
    {
        return _parent;
    }
    public void SetParent(GameObject parent)
    {
        _parent = parent;
    }
    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    
    public void SetWeaponType(WeaponType weaponType)
    {
        this.weaponType = weaponType;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
