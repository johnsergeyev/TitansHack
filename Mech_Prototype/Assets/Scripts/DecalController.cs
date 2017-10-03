using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalController : MonoBehaviour {
	public int maxCount = 5;

	private Dictionary<string, List<GameObject>> pool;

	void Start () {
		pool = new Dictionary<string, List<GameObject>>();
	}

	public GameObject getDecal(string type) {
		if (!pool.ContainsKey(type))
			pool.Add(type, new List<GameObject>());
		
		GameObject go;

		if (pool[type].Count > maxCount) {
			go = pool [type] [0];
			pool [type].RemoveAt (0);
			pool [type].Add (go);

			return go;
		} else {
			go = (GameObject)Instantiate (Resources.Load("Prefabs/" + type));
			pool [type].Add (go);
			return go;
		}
	}
}
