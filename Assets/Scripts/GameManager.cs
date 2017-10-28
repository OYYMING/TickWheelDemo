using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Player player;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (CanTick()) {
        	TickWheelManager.instance.Tick();
        	TickWheelManager.instance.Tick();
        	TickWheelManager.instance.Tick();
			DebugUtils.Log(TickWheelManager.instance.currentTick + "");
		}
    }

	bool CanTick () {
        return player != null && player.isWalking;
    }

}
