using UnityEngine;
using System.Collections;

public class OnTriggerParser : MonoBehaviour {

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	public AIAgent reference;
	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public void SetAIRef(AIAgent agent)
	{
		reference = agent;
	}
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void OnTriggerEnter(Collider other)
	{
		reference.TriggerEnter(other);
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
