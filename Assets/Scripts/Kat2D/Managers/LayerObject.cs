using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class LayerObject : MonoBehaviour {
	public abstract void destroy();
	public abstract void addItem(ItemData item);
}