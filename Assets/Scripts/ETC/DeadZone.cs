using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour {
    private ActionController theActionController;
    private PlayerController thePlayerController;
    private float deadTime = 3f;
    private float remainDeadTime;
    private bool isDeadZone = false;

    void Start() {
        theActionController = FindObjectOfType<ActionController>();
        thePlayerController = FindObjectOfType<PlayerController>();
        remainDeadTime = deadTime;
    }

    void Update() { 
        if (isDeadZone && !GameManager.instance.isDied) {
            remainDeadTime -= Time.deltaTime;
            theActionController.WarningText.text = "위험지역 " + Mathf.Round(remainDeadTime) + "초 후 뒤짐";
            if (remainDeadTime <= 0 ) {
                isDeadZone = false;
                thePlayerController.Die();
                theActionController.WarningText.gameObject.SetActive(false);

            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Player" && !GameManager.instance.isDied) {
            isDeadZone = true;
            theActionController.WarningText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.tag == "Player" && !GameManager.instance.isDied) {
            remainDeadTime = deadTime;
            isDeadZone = false;
            theActionController.WarningText.gameObject.SetActive(false);
        }
    }
}
