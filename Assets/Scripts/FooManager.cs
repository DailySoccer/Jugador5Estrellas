using Assets.Scripts.Physics;
using UnityEngine;

public class FooManager : MonoBehaviour
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
	

	#endregion  //End public methods

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//

	#region Monobehaviour methods

	/// <summary>
	/// 
	/// </summary>
	private void Awake()
	{
		AIAgent.SetBall(_ball.transform);
		//Camera.main.GetComponent<ResetParent>().SetNewParent(
		//	_ball.transform, 
		//	new Vector3(0, 3, 7), 
		//	new Vector3(50, 0, 0),
		//	60);
	}

	/// <summary>
	/// 
	/// </summary>
	private void Start()
	{
		_goalkeeper.Activate();
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

	[SerializeField] private PhysicBall _ball;
	[SerializeField] private AIAgent _goalkeeper;

	#endregion  //End private members
}
