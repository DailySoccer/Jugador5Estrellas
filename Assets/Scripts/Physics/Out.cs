using UnityEngine;

public class Out : MonoBehaviour
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

	private void OnTriggerEnter(Collider other)
	{
		InteractiveMatch.NotifyResult(InteractiveMatch.GameAction.Out);
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
