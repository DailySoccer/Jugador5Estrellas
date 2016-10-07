using UnityEngine;
using System.Collections;

public class ResetParent : MonoBehaviour
{

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//

	#region Public members

	public float Pitch = 10;
	public Vector3 LocalPosition = Vector3.zero;
	public Vector3 LookPoint = Vector3.zero;
	public float FieldOfView = 60;

	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//

	#region Public methods

	public void SetNewParent(Transform newParent, Vector3 localPosition, Vector3 lookTo, float fieldOfView, Transform rotRef = null)
	{
		if (newParent != null)
		{
			_parentRef = newParent;
			if (rotRef != null)
			{
				transform.SetParent(rotRef);
				transform.localPosition = localPosition;
				transform.SetParent(null);
			}
			_localPosition = transform.position - newParent.position;
			transform.localPosition = newParent.position + _localPosition;
			transform.rotation = Quaternion.LookRotation(lookTo - transform.position, Vector3.up);
			Camera meAsCam = GetComponent<Camera>();
			if (meAsCam != null)
			{
				meAsCam.fieldOfView = fieldOfView;
			}
		}
		FOVAdjust Label = GameObject.FindObjectOfType<FOVAdjust>();
		if (Label != null)
		{
			Label.ChangeFOV(fieldOfView);
		}
	}

	#endregion  //End public methods

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//

	#region Monobehaviour methods

	void Awake()
	{

	}
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	private void LateUpdate()
	{
		if (_parentRef != null)
		{
			transform.position = _parentRef.position + _localPosition;
		}
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

	private Transform _parentRef;
	private Vector3 _localPosition;

	#endregion  //End private members
}
