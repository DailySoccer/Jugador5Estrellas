using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class PointOfInterest {
		
		public GameObject Balon;
		public GameObject Source;
		public GameObject Target;
		
		public  BallMotor BallMotor = null;
		private MatchManager mMatchManager;
		
		public PointOfInterest() {
			Balon = GameObject.FindGameObjectWithTag ("Balon");
			BallMotor = Balon.GetComponentInChildren<BallMotor>();
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
		}
		
		public void Update() {
			if ( BallMotor ) {
				InteractiveActions actions = mMatchManager.InteractiveActions;
				Source = BallMotor.NewPropietary;
				Target = actions.Target;
            }
        }
	};
}
