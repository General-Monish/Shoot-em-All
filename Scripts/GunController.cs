using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    Gun equipedGun;
    public Gun firstGun;

    private void Start()
    {
        if (firstGun != null)
        {
            EquipGun(firstGun);
        }
    }

    public void EquipGun(Gun GunToEquip)
    {
        if (equipedGun != null)
        {
            Destroy(equipedGun.gameObject);
        }
        equipedGun = Instantiate(GunToEquip,weaponHold.position, weaponHold.rotation) as Gun;
        equipedGun.transform.parent = weaponHold;
    }

    public void OntriggerHold()
    {
        if (equipedGun != null)
        {
            equipedGun.OntriggerHold();
        }
    }

    public void OntriggerReleased()
    {
        if (equipedGun != null)
        {
            equipedGun.OntriggerReleased();
        }
    }

    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }

    
}
