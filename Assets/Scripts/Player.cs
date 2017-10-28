using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [HideInInspector]public bool isWalking = false;
    public float speed = 5;

    // Use this for initialization
    void Start () {
        TickWheelManager.instance.AddEvent(TickEvent.CreateAfter(300, ChangeColor));
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 move = new Vector3(Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
        if (move.sqrMagnitude > 0) {
            BeginWalk();
            transform.Translate(move * Time.deltaTime * speed);
        } else {
            EndWalk();
        }
	}

	public void BeginWalk () {
        isWalking = true;
    }

	public void EndWalk () {
        isWalking = false;
    }

	void ChangeColor () {
        this.GetComponent<MeshRenderer>().material.color = Color.red;
        DebugUtils.LogError(TickWheelManager.instance.currentTick + "");
    }
}
