using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TickWheelManager {
	
	public int[] WheelLayerSlotNum = new int[] {256,128,128,128,64,64};
	public WheelLayer[] WheelLayers;
	public long currentTick = 0;

	private TickWheelManager () {
        Init();
    }

    private static TickWheelManager instance_;
	public static TickWheelManager instance {
        get
        {
            if (instance_ == null)
            {
                instance_ = new TickWheelManager();
            }

            return instance_;
        }
	}

    private void Init () {
		WheelLayers = new WheelLayer[WheelLayerSlotNum.Length];
        int offset = 0;
        for (int i = 0; i < WheelLayers.Length; i++)
		{
            WheelLayers[i] = new WheelLayer(0, WheelLayerSlotNum[i], offset);
            offset += GetPowerFromSlotNum(WheelLayerSlotNum[i]);
                
            if (i > 0) {
                WheelLayers[i].child = WheelLayers[i - 1];
                WheelLayers[i - 1].parent = WheelLayers[i];
            }
        }
	}

	int GetPowerFromSlotNum (int num) {
        if ((num & (num - 1)) != 0) {
            DebugUtils.LogError("slot数不是2的幂次方");
            return 0;
        }

        int p = 0;
		while (num >> p > 0) {
            p++;
        }

        return p - 1;
    }

	public void AddEvent (TickEvent e) {
		if (e.tickBit <= currentTick) {
            DebugUtils.LogError("Unable to add TickEvent to the past or now");
            return;
        }

        for (int i = WheelLayers.Length - 1; i >= 0; i--)
        {
            if (WheelLayers[i].pointer != WheelLayers[i].GetLayerTick(e.tickBit))
            {
                WheelLayers[i].AddEvent(e);
                return;
			}
		}

        DebugUtils.LogError ("Tick overflow");
	}

	public void Tick () {
		currentTick += 1;
		if (WheelLayers.Length > 0)
        	WheelLayers[0].Tick();
    }
}

public class WheelSlot {
	public List<TickEvent> events = new List<TickEvent>();
	public void AddEvent (TickEvent e) {
		events.Add (e);
	}

	public void Execute () {
		// TODO::这里如果事件太多，在1tick中可能会阻塞导致卡顿
		for (int i = events.Count - 1; i >= 0; i--)
		{
			if (events[i].callback != null)
            	events[i].callback();
            events.RemoveAt(i);
        }
	}

	public void Clear () {
        events.Clear();
    }

}

public class WheelLayer {
    public int layerIndex;	// 0-min
    public int slotNum;
	public List<WheelSlot> slots;
	public int pointer;	// this layer's tick currently
	public WheelLayer parent;	// 上层Wheel
	public WheelLayer child;	// 下层Wheel

	/// <summary>
	///	位位移
	/// </summary>
	private int layerOffset_ = 0;
	public int layerOffset { 
		get {
            return layerOffset_;
        }
		private set {
            layerOffset_ = value;
        }
	}

    private int layerMask_ = 0;
    public int layerMask {
		get {
            return layerMask_;
        }
		private set {
            layerMask_ = value;
        }
	}

	private int layerEventMask_ = 0;
    public int layerEventMask {
		get {
            return layerEventMask_;
        }
		private set {
            layerEventMask_ = value;
        }
	}

	public WheelLayer (int layerIndex, int slotNum, int layerOffset) {
        this.layerIndex = layerIndex;
        this.slotNum = slotNum;
        this.layerOffset_ = layerOffset;
        this.layerMask_ = (slotNum - 1);
        this.layerEventMask_ = layerMask_ << layerOffset;

        slots = new List<WheelSlot>(slotNum);
        for (int i = 0; i < slotNum; i++)
        {
            slots.Add(new WheelSlot());
        }
	}

	/// <summary>
	///	添加事件
	/// </summary>
	public bool AddEvent (TickEvent e) {
        int index = GetLayerTick(e.tickBit);
        slots[index].AddEvent (e);
        return true;
    }

	/// <summary>
	///	由parent调用，会不断向下层分发事件
	/// TODO::如果跨越多层分发事件可能导致卡顿
	/// </summary>
	public void RegisterEvents (List<TickEvent> events) {
		for (int i = events.Count - 1; i >= 0; i--)
		{
            AddEvent(events[i]);
        }

		if (child == null) {    // 当前为最底层，则开始执行事件
            ExecuteEvents();
        } else {    // 继续向下层分发事件
            DeliverEvents();
        }
	}

	void Clear () {
		for (int i = 0; i < slots.Count; i++)
		{
            slots[i].Clear();
        }
	}

	public void Tick () {
        pointer = (pointer + 1) & layerMask_;
		if (pointer == 0 && parent != null) {
            SharpTick();
        } else if (pointer != 0) {	// 正常指向下一格
			if (child != null) {    // 当前非最底层, 向下层分发事件
                DeliverEvents();
            } else {    // 当前为最底层
                ExecuteEvents();
            }
		}
    }

	/// <summary>
	///	整点重新归零
	/// </summary>
	void SharpTick () {
        Clear();
        parent.Tick();
	}

	/// <summary>
	///	向下层分发事件
	/// </summary>
	void DeliverEvents () {
		if (child == null)
            return;

        List<TickEvent> events = slots[pointer].events;
		if (Utils.IsNullOrEmpty(events))
            return;

        // TODO::这里如果事件太多，在1tick中可能会阻塞导致卡顿
        child.RegisterEvents(events);
        slots[pointer].Clear();
    }

	void ExecuteEvents () {
		slots[pointer].Execute();
	}

	/// <summary>
	///	返回TickEvent在当前层的tick数
	/// </summary>
	public int GetLayerTick (long tickBit) {
        return (int)((tickBit & layerEventMask) >> layerOffset);
    }
}

public class TickEvent {
	public Action callback {get;set;}
	public long tickBit {get;set;}	// 以二进制的形式储存各个layer上的时间

	private TickEvent () {

	}

	public static TickEvent Create (long tick, Action cb) {
		if (cb == null) {
            DebugUtils.LogError("Callback can not be empty");
            return null;
        }

        TickEvent e = new TickEvent() { tickBit = tick, callback = cb };
        return e;
    }

	public static TickEvent CreateAfter (long after, Action cb) {
		if (cb == null) {
            DebugUtils.LogError("Callback can not be empty");
            return null;
        }

        TickEvent e = new TickEvent() { tickBit = TickWheelManager.instance.currentTick + after, callback = cb };
        return e;
    }

	public static TickEvent CreateRepeat (long after, long interval, Action cb) {
		if (cb == null) {
            DebugUtils.LogError("Callback can not be empty");
            return null;
        }

        TickEvent e = new TickEvent() { tickBit = TickWheelManager.instance.currentTick + after};
        Action rpcb = () =>
        {
            cb();
            e.tickBit += interval;
            TickWheelManager.instance.AddEvent(e);
        };
        e.callback = rpcb;

        return e;
    }

	
}
