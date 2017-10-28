using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TickWheelManager : MonoBehaviour {
	
	public int[] WheelLayerSlotNum = new int[] {200,100,100,100,50,50};
	public WheelLayer[] WheelLayers;
	public int currentTick = 0;

	private void Init () {
		WheelLayers = new WheelLayer[WheelLayerSlotNum.Length];
		for (int i = 0; i < WheelLayers.Length; i++)
		{
			WheelLayers[i].Init (WheelLayerSlotNum[i]);
		}
	}

	public void AddEvent (TickEvent e) {
		int slotMul = 1;
		for (int i = 0; i < WheelLayerSlotNum.Length; i++)
		{
			slotMul *= WheelLayerSlotNum[i]; 
			if (e.tick < slotMul) {
				int index = e.tick / (slotMul / WheelLayerSlotNum[i]);
				WheelLayers[i].AddEvent (index, e);
				return;
			}
		}

		Debug.LogError ("Tick overflow");
	}

	public void Tick (int tic) {
		currentTick += tic;
		
	}

	// private const int WHEELTICKS3 = 100;
	// private const int WHEELTICKS2 = 100;
	// private const int WHEELTICKS1 = 100;
	// private const int WHEELTICKS0 = 100;

	// private TickTime mCurrentTick = new TickTime ();


	// private static TickWheelManager mInstance; 
	// public static TickWheelManager instance {
	// 	get {
	// 		if (mInstance == null) {
	// 			mInstance = new TickWheelManager();
	// 		}
	// 		return mInstance;
	// 	}
	// }

	// public void Init () {
	// 	mCurrentTick.SetTick ();
	// }

	// public void Tick () {
	// 	mCurrentTick.AddTick (1);
	// }

	// void CheckSlot (TickTime curTick) {
		
	// }

	// public sealed class TickTime : IComparable{
	// 	public int wheelTick3;
	// 	public int wheelTick2;
	// 	public int wheelTick1;
	// 	public int wheelTick0;

	// 	public TickTime (int wheel0 = 0, int wheel1 = 0, int wheel2 = 0, int wheel3 = 0) {
	// 		SetTick (wheel0, wheel1, wheel2, wheel3);
	// 	}

	// 	public void SetTick (int wheel0 = 0, int wheel1 = 0, int wheel2 = 0, int wheel3 = 0) {
	// 		wheelTick0 = wheel0;
	// 		wheelTick1 = wheel1;
	// 		wheelTick2 = wheel2;
	// 		wheelTick3 = wheel3;
	// 	}

	// 	public void AddTick (int wheel0 = 0, int wheel1 = 0, int wheel2 = 0, int wheel3 = 0) {
	// 		wheel0 += wheelTick0;
	// 		int totalW0 = wheel0 % WHEELTICKS0;
			
	// 		wheel1 += wheel0 / WHEELTICKS0 + wheelTick1;
	// 		int totalW1 = wheel1 % WHEELTICKS1;
			
	// 		wheel2 += wheel1 / WHEELTICKS1 + wheelTick2;
	// 		int totalW2 = wheel2 % WHEELTICKS2;
			
	// 		wheel3 += wheel2 / WHEELTICKS1 + wheelTick3;
	// 		int totalW3 = wheel3 % WHEELTICKS2;

	// 		wheelTick0 = totalW0;
	// 		wheelTick1 = totalW1;
	// 		wheelTick2 = totalW2;
	// 		wheelTick3 = totalW3;
	// 	}

    //     int IComparable.CompareTo(object obj)
    //     {
    //         if (obj is TickTime) {
	// 			TickTime newTick = obj as TickTime;
	// 			return newTick.wheelTick3 * WHEELTICKS3 + 
	// 		}

	// 		return 1;
    //     }


        // public TickTime AddTick (int wheel0 = 0, int wheel1 = 0, int wheel2 = 0, int wheel3 = 0) {
        // 	wheel0 += wheelTick0;
        // 	int totalW0 = wheel0 % WHEELTICKS0;

        // 	wheel1 += wheel0 / WHEELTICKS0 + wheelTick1;
        // 	int totalW1 = wheel1 % WHEELTICKS1;

        // 	wheel2 += wheel1 / WHEELTICKS1 + wheelTick2;
        // 	int totalW2 = wheel2 % WHEELTICKS2;

        // 	wheel3 += wheel2 / WHEELTICKS1 + wheelTick3;
        // 	int totalW3 = wheel3 % WHEELTICKS2;

        // 	TickTime tick = new TickTime (totalW0, totalW1, totalW2, totalW3);
        // 	return tick;
        // }
    // }
}

public class WheelSlot {
	public List<TickEvent> events = new List<TickEvent>();
	public void AddEvent (TickEvent e) {
		events.Add (e);
	}
}

public class WheelLayer {
	public int slotNum;
	public WheelLayer parent;
	public List<WheelSlot> slots;
	public int pointer;	// this layer's tick currently

	public void Init (int slotNum) {
		this.slotNum = slotNum;
		slots = new List<WheelSlot>(slotNum);
	}

	public void AddEvent (int index, TickEvent e) {
		slots[index].AddEvent (e);
	}

	public void Tick (int t) {

	}
}

public class TickEvent {
	public int tick { get; set; }	// 可能不用了？
	public Action callback {get;set;}
	public long tickBit {get;set;}	// 以二进制的形式储存各个layer上的时间

	private TickEvent () {

	}

	// public static void Create () {

	// }
}
