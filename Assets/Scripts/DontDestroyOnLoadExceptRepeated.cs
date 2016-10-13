using UnityEngine;
using System.Collections;

public class DontDestroyOnLoadExceptRepeated : MonoBehaviour {

	private static int counter = 0;
	// Use this for initialization
	void Awake () {
		if (counter != 0)
		{
			GameObject aux = GameObject.FindGameObjectWithTag(name);
			if (aux != null)
			{
				DestroyImmediate(gameObject);
			}
		}
		else
		{
			DontDestroyOnLoad(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
