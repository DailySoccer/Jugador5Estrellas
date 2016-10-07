using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Common;

namespace FootballStar.Match3D {
	
	public class DirectionGesture : MonoBehaviour {
	
		public float MinLenght = 5f;
        public static Vector3 mLastTarget;
        public static bool mValidGesture;
        public static Vector3 mLastEffect;

        // Angulo máximo de diferencia permitido con respecto a la dirección (para dar por válido el gesto)
        [HideInInspector]
        public float AngleMax = 90f;
			
		private bool mTap = false;
		
		public enum EResult {
			NONE,
			FAIL,
			SUCCESS
		};
		private bool mResultCached = false;
		private EResult mResult = EResult.NONE;
		public EResult Result {
			get { 
				if ( !mResultCached ) {
					mResult = VerifyResult ();
                    if(mResult!=EResult.NONE)
					    mResultCached = true;
				}
				return mResult; 
			}
		}
        // CACA: Calculo de factor suerte.
        public bool Lucky { get { return UnityEngine.Random.value > 0.5f;  } }
		
		private bool mActivated = false;
		public bool IsActivated { get { return mActivated; } }
		public bool IsFinished 	{ get { return !mActivated; } }

		public float ActionTime;
		private float StartTime;
		
		public Vector3 UserDirection {
			get {
				Vector3 direction = Vector3.zero;
				if ( mPositions.Count > 1 ) {
					Vector3 start = mPositions[0];
					Vector3 end = mPositions[ mPositions.Count - 1 ];
					direction = (end - start).normalized;
				}
				return direction;
			}
		}

        private List<Vector3> mPositions = new List<Vector3>();

//        private Vector3 mDirectionSuccess;
		private float mAngleDirectionSuccess;
		public float AngleDirectionSuccess {
			get { return mAngleDirectionSuccess; }
		}

		public Vector3 GuiCenter = new Vector3(0.5f, 0.5f, 0);
		
		void Start () {
		}
		
		void Update () {
			if ( mActivated ) {				
				if ( mTap ) {
					mTap = !GameInput.IsTouchUp();					
					// Hemos terminado?
					if ( !mTap ) { 
						Deactivate();
					}
				}
				else {
					mTap = GameInput.IsTouchDown();					
				}
				
				if ( mTap ) {
					AddPoint ( GameInput.TouchPosition );
				}
			}
		}
		
		void AddPoint ( Vector3 point ) {
			bool insert = true;
            point.z = 100;
            if ( mPositions.Count == 0 ) {
                mLastTarget = Vector3.zero;
                // Registramos el tiempo en el que se ha tardado en comenzar la accion
                ActionTime = Time.time - StartTime;
			}
			if ( mPositions.Count > 0 ) {
				// Únicamente insertamos las posiciones que tengan algún desplazamiento (con respecto a la última posición)
				Vector3 pointEnd = mPositions[ mPositions.Count - 1];
                float val = (point - pointEnd).sqrMagnitude;
                float min = Screen.width * 0.02f;
                insert = ( val > min*min);
			}


            if ( insert ) {
                mPositions.Add ( point );
                mResultCached = false;
			}
		}
		
		EResult VerifyResult () {
            EResult result = EResult.NONE;
            // Tiro como el culo.
            mLastTarget = new Vector3(52.5f, 2.6f + Random.value * 3f, 8f * Random.value - 4f);
            mValidGesture = false;
            if ( mPositions.Count > 1 && !mActivated) {
				Vector3 start = mPositions[0];
				Vector3 end = mPositions[ mPositions.Count - 1 ];				
				// Evaluamos si se ha desplazado...
				if ( (end - start).sqrMagnitude > (MinLenght * MinLenght) ) {
                    mValidGesture = true;
                    var match_manager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
                    switch (match_manager.InteractiveActions.CurrentAction.Action  )
                    {
                        case ActionType.CHUT:
                            result = GoalShot() ? EResult.SUCCESS : EResult.FAIL;
                            break;
                        case ActionType.PASE:
                     result = EResult.FAIL;//PassShot(match_manager, match_manager.InteractiveActions.CurrentAction.Entrenador.transform.position) ? EResult.SUCCESS : EResult.FAIL;
                            break;
                        case ActionType.REGATE:
                            Vector3 dir2D = (end - start).normalized;
                            float angleDirection = Mathf.Atan2(dir2D.x, dir2D.y) * Mathf.Rad2Deg;
                            float diffAngle = Mathf.Abs(angleDirection - mAngleDirectionSuccess);
                            if (diffAngle > 180f)
                            {
                                diffAngle = 360f - diffAngle;
                            }
                            result = (diffAngle < AngleMax && Lucky ) ? EResult.SUCCESS : EResult.FAIL;
                            break;
                    }
                }
			}
			return result;
		}

        public bool GoalShot() {
            // Proyecto el trazo contra la camara
            Vector3 ws = Camera.main.transform.position;            
            Vector3 we = Camera.main.ScreenToWorldPoint(mPositions[mPositions.Count - 1]);
            Ray r = new Ray(ws, (we - ws).normalized);
            Plane p = new Plane(Vector3.right, new Vector3(52.5f, 0, 0));
            float enter;
            if (p.Raycast(r, out enter)) {
                mLastTarget = r.GetPoint(enter);
                return Lucky;
            }
            return false;
        }

        public bool PassShot(MatchManager manager, Vector3 player) {
            // Proyecto el trazo contra la camara
            Vector3 org = mPositions[0];
            org.z = 10;
            Vector3 dst = mPositions[mPositions.Count - 1];
            dst.z = 10 + (mPositions.Count - 1) * 2;

            Vector3 ws = Camera.main.ScreenToWorldPoint(org);
            Vector3 we = Camera.main.ScreenToWorldPoint(dst);
            Debug.DrawLine(ws, we, Color.yellow, 5);

            ws.y = 0;
            we.y = 0;
            Debug.DrawLine(ws, we, Color.red, 5);

            Vector3 dif = (dst - org);
            dif.z = 0;
            float dis = (dif.magnitude / Screen.width)*50.0f;

            mLastTarget = player + (we - ws).normalized * dis;
            Debug.DrawLine(player, mLastTarget, Color.blue, 5);
            mLastTarget.y = 0;

            // Miramos que jugador va a recoger la pelota.
            const float maxVel = 7;
            float minAttackerDiff = float.MaxValue;
            /*Entrenador minAttacker = null;
            foreach (var ply in manager.Customization.Attackers) {
                float d;

                BallMotor bm = GameObject.FindGameObjectWithTag("Balon").GetComponent<BallMotor>();
                string name = bm.NewPropietary.name;
                if (!name.Contains( ply.name ) && ply.CanReach(mLastTarget, 1.5f * maxVel, out d)) {
                    if (d < minAttackerDiff)
                    {
                        minAttackerDiff = d;
                        minAttacker = ply;
                    }
                }
            }*/

            /*float minDefenderDiff = float.MaxValue;
            Entrenador minDefender = null;
            foreach (var ply in manager.Customization.Defenders) {
                float d;
                if (ply.CanReach(mLastTarget, 1.5f * maxVel, out d))
                    if (d < minDefenderDiff) {
                        minDefenderDiff = d;
                        minDefender = ply;
                    }
            }*/

            /*if (minAttackerDiff > minDefenderDiff || !Lucky) {
                PasarBalon_Goal.bestReceiver = minDefender;
                PasarBalon_Goal.bestPresser = minAttacker;
                return false;
            }*/

            /*PasarBalon_Goal.bestReceiver = minAttacker;
            PasarBalon_Goal.bestPresser = minDefender;*/
            return true;
        }


        public void Activate ( float angle ) {
			mActivated = true;
			mResultCached = false;
			mResult = EResult.NONE;
			mTap = GameInput.IsTouchDown();
			StartTime = Time.time;
			ActionTime = -1;
			mPositions.Clear();
            mAngleDirectionSuccess = angle;
		} 
		
		public void Deactivate() {
			mActivated = false;
		}
	}
}