using UnityEngine;
using System.Collections;

public class FOVAdjust : MonoBehaviour
{

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public void ChangeFOV(float newAngle)
	{
		transform.localScale = _initialLocalScale * (Mathf.Tan(newAngle * Mathf.Deg2Rad * 0.5f) * _initialTG);
	}
	#endregion  //End public methods

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//
	#region Monobehaviour methods

	// Use this for initialization
	void Start()
	{
		_initialTG = 1f / Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f);
		_initialLocalScale = transform.localScale;
	}

	// Update is called once per frame
	void Update()
	{

	}

	#endregion  //End monobehaviour methods

	//-----------------------------------------------------------//
	//                      PRIVATE METHODS                      //
	//-----------------------------------------------------------//
	#region Private methods
	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members
	private float _initialTG;
	private Vector3 _initialLocalScale;
	#endregion  //End private members
}
