using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKitScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript player = other.GetComponent<PlayerScript>();
            if(player.currentHealth < 100)
            {
                player.HealUp(20);
                Destroy(this.gameObject);
            }
        }
    }

}
