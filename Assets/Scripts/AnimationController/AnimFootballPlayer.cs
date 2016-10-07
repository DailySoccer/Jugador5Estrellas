using System;
using UnityEngine;

namespace Assets.Scripts.AnimationController
{
	[RequireComponent(typeof(Animator))]
	public class AnimFootballPlayer : MonoBehaviour
	{
		//-----------------------------------------------------------//
		//                      PUBLIC MEMBERS                       //
		//-----------------------------------------------------------//
		#region Public members
		public Transform BallPlacer;
		public const float STOP_DISTANCE = 1;
		public const float MAX_SPEED = 2;
		public const float ACCEL = 2f;
		public float ROT_SPEED = 180;
		public const float DESP_SPEED = 4;
		public const float ANGLE_MARGIN = 5;
		public bool UpdateRotation = true;

		/// <summary>
		/// 
		/// </summary>
		public bool IsGoalkeeper
		{
			get { return _isGoalkeeper;  }
			set {
				if (_animator != null) {
					if (value)
						_animator.SetTrigger("Goalkeeper");
					else
						_animator.ResetTrigger("Goalkeeper");
				}
				_isGoalkeeper = value;
			}
		}

		#endregion  //End public members

		//-----------------------------------------------------------//
		//                      PUBLIC METHODS                       //
		//-----------------------------------------------------------//
		#region Public methods
		public void Reset()
		{
			_currentSpeed = 0;
			_currentTarget = transform.position;
			if (_animator != null)
			{
				_animator.SetFloat("AngleToTarget", 0);
				_animator.SetFloat("Speed", _currentSpeed);
				_animator.ResetTrigger("Goalkeeper");
				_animator.speed = 1f;
			}
		}

		public void GoTo(Vector3 target)
		{
			if (target != _currentTarget)
			{
				_currentTarget = target;
			}
		}
		public void Tackling()
		{
			_animator.SetTrigger("Tackling");
		}
		public void Shoot()
		{
			_animator.SetTrigger("Shoot");
		}
		public void Dribbling()
		{
			_animator.SetTrigger("Dribbling");
		}


		public void ClearBall(int q, int l, bool isRight, float speed = 1f)
		{
			Debug.Log("AnimFootballPlayer::ClearBall>> <b> L" + l + "Q" + q + (isRight ? "_DER" : "_IZQ </b>"));

			_animator.speed = speed;
			_animator.SetTrigger(string.Format(_lateralAnimFormat, q));
			_animator.SetTrigger(string.Format(_verticalAnimFormat, l));
			_animator.SetTrigger(isRight ? _rightAnim : _leftAnim);
		}

		public void FixHeight()
		{
			if (!_fixHeight)
			{
				ChangeFixHeight(true);
			}
		}

		public void DoNotFixHeight()
		{
			if (_fixHeight)
			{
				ChangeFixHeight(false);
			}
		}

		#endregion  //End public methods

		//-----------------------------------------------------------//
		//                  MONOBEHAVIOUR METHODS                    //
		//-----------------------------------------------------------//
		#region Monobehaviour methods
		void Awake()
		{
			_animator = GetComponent<Animator>();
			IsGoalkeeper = IsGoalkeeper;
		}
		// Use this for initialization
		void Start()
		{
			_currentSpeed = 0;
		}

		// Update is called once per frame
		public void OwnerUpdate()
		{
			Vector3 diff = _currentTarget - transform.position;
			float angleDiff = Vector3.Angle(transform.forward, diff) * Mathf.Sign(Vector3.Dot(transform.right, diff));
			float angleStep = ROT_SPEED * Time.deltaTime * Mathf.Sign(angleDiff);
			bool rotStep = Mathf.Abs(angleStep) < Mathf.Abs(angleDiff);
			_animator.SetFloat("AngleToTarget", 0);
			if (diff.sqrMagnitude < STOP_DISTANCE)
			{
				_currentSpeed -= ACCEL * Time.deltaTime;
			}
			else
			{
				_currentSpeed += ACCEL * Time.deltaTime;
				if (UpdateRotation)
				{
					transform.RotateAround(transform.position, transform.up, rotStep ? angleStep : angleDiff);
				}
			}
			_currentSpeed = Mathf.Clamp(_currentSpeed, 0, MAX_SPEED);
			_animator.SetFloat("Speed", _currentSpeed);

			//Fix on height
			if (_fixHeight)
			{
				transform.position -= Vector3.Scale(transform.position, Vector3.up);
			}
		}

		#endregion  //End monobehaviour methods

		//-----------------------------------------------------------//
		//                      PRIVATE METHODS                      //
		//-----------------------------------------------------------//
		#region Private methods

		private void ChangeFixHeight(bool fixHeight)
		{
			_fixHeight = fixHeight;
		}

		#endregion  //End private methods

		//-----------------------------------------------------------//
		//                      PRIVATE MEMBERS                      //
		//-----------------------------------------------------------//
		#region Private members

		[SerializeField] private string _lateralAnimFormat = "Clear_Q{0}";
		[SerializeField] private string _verticalAnimFormat = "Clear_L{0}";
		[SerializeField] private string _leftAnim = "Clear_IZQ";
		[SerializeField] private string _rightAnim = "Clear_DER";

		private Animator _animator;
		private float _currentSpeed;
		private Vector3 _currentTarget;
		private bool _fixHeight;
		private bool _isGoalkeeper;

	

		#endregion  //End private members


	}
}
