using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackCollision : MonoBehaviour
{
    private void OnEnable() {
        StartCoroutine("AutoDisable");
    }

    private void OnTriggerEnter(Collider other) {

        if (other.tag == "Enemy") {
            other.GetComponent<EnemyController>().TakeDamage(10);
        }
    }

    public IEnumerator AutoDisable() {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}
