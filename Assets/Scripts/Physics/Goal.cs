
using UnityEngine;

public class Goal : MonoBehaviour
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
	/// /
	/// </summary>
	/// <param name="other"></param>
	protected virtual void OnTriggerEnter(Collider other)
	{
		Debug.Log("<color=Yellow><b>Goal::OnTriggerEnter</b></color>");
		InteractiveMatch.NotifyResult(InteractiveMatch.GameAction.Goal);
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

	#endregion  //End private members
}
