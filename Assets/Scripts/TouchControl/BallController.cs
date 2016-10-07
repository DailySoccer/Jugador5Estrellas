using System;
using System.Linq;
using Assets.Scripts.Physics;
using Assets.Scripts.TouchControl;
using Newtonsoft.Json.Utilities;
using UnityEngine;


[RequireComponent(typeof(PhysicBall))]
public class BallController : MonoBehaviour
{
	[Serializable]
	public struct BallSetup
	{
		public InteractiveType InteractiveType;
		[SerializeField, Range(1f, 1000f)]
		public float SpeedMin;
		[SerializeField, Range(1f, 1000f)]
		public float SpeedMax;
		[SerializeField, Range(0f, 90f)]
		public float DegreesMin;
		[SerializeField, Range(0f, 90f)]
		public float DegreesMax;
	}

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members

	/// <summary>
	/// 
	/// </summary>
	/// <param name="action"></param>
	public void Setup(InteractiveType interactiveType = InteractiveType.All)
	{
		int index = _setups.IndexOf(s => s.InteractiveType == interactiveType);
		_setup = index >= 0 ? _setups[index] :
			_setups.FirstOrDefault(s => s.InteractiveType == InteractiveType.All);
	}


	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods

	/// <summary>
	/// 
	/// </summary>
	/// <param name="swipeVector"></param>
	/// <param name="speedRatio"></param>
	public void Shoot(Vector2 swipeVector, float speedRatio)
	{
		Vector3 shootVelocity = CalcShootVelocity(swipeVector, speedRatio);
		Debug.Log("BallController::Shoot>> Velocity=" + shootVelocity);

		_ball.AddVelocity(shootVelocity);
	}

	/// <summary>
	/// 
	/// </summary>
	public void ResetPosition()
	{
		_ball.SetPosition(_startPosition);
		_ball.Stop();
	}

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
		if (_ball == null)
			_ball = GetComponent<PhysicBall>();

		_groundLayerMask = LayerMask.GetMask(_groundLayerName);

		Setup();
	}
	
	/// <summary>
	/// 
	/// </summary>
	private void OnDestroy()
	{
		_input = null;
		_ball = null;
	}
	/// <summary>
	/// 
	/// </summary>
	private void OnEnable()
	{
		if (_input != null) {
			_input.Swipe += OnInputSwipe;
			_input.DoubleTap += OnInputDoubleTap;
		}

		_ball.SimulationEnd += OnBallStopMoving;
	}

	/// <summary>
	/// 
	/// </summary>
	private void OnDisable()
	{
		_ball.SimulationEnd -= OnBallStopMoving;

		if (_input != null) {
			_input.Swipe -= OnInputSwipe;
			_input.DoubleTap -= OnInputDoubleTap;
		}
	}
	/// <summary>
	/// 
	/// </summary>
	private void Start()
	{
		_startPosition = transform.position;
	}
	

	#endregion  //End monobehaviour methods

	//-----------------------------------------------------------//
	//                      PRIVATE METHODS                      //
	//-----------------------------------------------------------//
	#region Private methods

	/// <summary>
	/// 
	/// </summary>
	/// <param name="obj"></param>
	private void OnInputDoubleTap(Vector2 obj)
	{
		// TODO.... ??
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="swipeVector"></param>
	/// <param name="speedRatio"></param>
	private void OnInputSwipe(Vector2 swipeVector, float speedRatio)
	{
		Shoot(swipeVector, speedRatio);
	}
	

	/// <summary>
	/// 
	/// </summary>
	/// <param name="obj"></param>
	private void OnBallStopMoving(Vector3 obj)
	{
		if (_resetPositionAfterShoot)
			ResetPosition();
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="swipeVector"></param>
	/// <param name="swipeSpeedRatio"></param>
	/// <returns></returns>
	private Vector3 CalcShootVelocity(Vector2 swipeVector, float swipeSpeedRatio)
	{
		if (swipeVector.y <= 0f)
			return Vector3.zero;

		Vector2 ballPos2D   = Camera.main.WorldToScreenPoint(transform.position);
		RaycastHit groundInfo;

		int retries = CalcRetriesMax;
		bool groundFound;
		do {
			Vector2 point = ballPos2D + swipeVector * retries / CalcRetriesMax;
			Ray rayToGround = Camera.main.ScreenPointToRay(point);
			groundFound = Physics.Raycast(rayToGround, out groundInfo, GroundDistanceMax, _groundLayerMask);
		} while (!groundFound && --retries > 0);

		if (!groundFound) {
			Debug.LogWarning("BallController::CalcShootVelocity>> Ground not found!!");
			return Vector3.zero;
		}

		float speed = _setup.SpeedMin + swipeSpeedRatio * (_setup.SpeedMax - _setup.SpeedMin);

		Vector3 up = groundInfo.normal;
		Vector3 forward =  groundInfo.point - transform.position;
		forward -= Vector3.Project(forward, up);
		forward.Normalize();

		float shootDegrees = _setup.DegreesMin + (_setup.DegreesMax - _setup.DegreesMin) 
			* (speed - _setup.SpeedMin)/(_setup.SpeedMax - _setup.SpeedMin);

		float sin = Mathf.Sin(Mathf.Deg2Rad * shootDegrees);
		float cos = Mathf.Sqrt(1f - sin * sin);
		Vector3 direction = cos * forward + sin * up;

		return speed * direction;
	}

	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members

	private const int CalcRetriesMax = 4;
	private const float GroundDistanceMax = 100;

	[SerializeField] private string _groundLayerName = "Cancha";
	private int _groundLayerMask;

	[SerializeField] private TouchInputManager _input;
	[SerializeField] private PhysicBall _ball;
	[SerializeField] private BallSetup[] _setups;
	[SerializeField] private bool _resetPositionAfterShoot;
	private Vector3 _startPosition;
	private BallSetup _setup;

	#endregion  //End private members


}
