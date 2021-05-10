using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public float attackDamage;
    public float attackSpeed;
    public GameObject attackParticles;
    public int[] summonCost = new int[4];
}
