using System;
using UnityEngine;

namespace Assets.Scripts.TouchControl
{

	/// <summary>
	/// 
	/// </summary>
	public class TouchInputManager : MonoBehaviour
	{
		private struct InputData
		{
			public enum InputPhase
			{
				None   = 0,
				Start  = 1,
				Stay   = 2,
				Stop   = 3
			}

			public Vector2 Position;
			public float Seconds;
			public InputPhase Phase;

			public float Speed {
				get { return Position.magnitude / (Screen.dpi * Seconds); }
			}

			public InputData(Vector2 position, float seconds, InputPhase phase = InputPhase.None)
			{
				Phase	 = phase;
				Position = position;
				Seconds	 = seconds;
			}

			public static InputData operator-(InputData x, InputData y)
			{
				return new InputData(x.Position - y.Position, x.Seconds - y.Seconds);
			}
		}

		#region Public members

		/// <summary>
		/// Returns swipe vector in inches/sec
		/// </summary>
		public event Action<Vector2, float> Swipe;

		/// <summary>
		/// Returns double tap position in inches
		/// </summary>
		public event Action<Vector2> DoubleTap;

		#endregion

		#region MonoBehaviour methods
		
		/// <summary>
		/// 
		/// </summary>
		private void Start()
		{			
		}

		/// <summary>
		/// 
		/// </summary>
		private void Update()
		{
			InputData input = ReadInput();

			switch (input.Phase)
			{

			case InputData.InputPhase.Start:						
				if(_beginInput.HasValue && _endInput.HasValue 
					&& IsDoubleTap(input - _beginInput.Value))
				{
					OnDoubleTap(_beginInput.Value.Position);
				}
				else
				{
					_beginInput = input;
					_endInput = null;
				}
				break;


			case InputData.InputPhase.Stop:

				if (!_beginInput.HasValue)
					return;

				_endInput = input;

				InputData swipeData = _endInput.Value - _beginInput.Value;
				if (IsSwipe(swipeData))
					OnSwipe(swipeData.Position, 
						Mathf.Clamp01(swipeData.Speed / _swipeInchesPerSecMax));

				break;


			default:

				if (_beginInput.HasValue && Time.time - _beginInput.Value.Seconds > _doubleTapSecsMax)
				{
					if (!_endInput.HasValue)
					{
						InputData swipeInput = input - _beginInput.Value;
						if (IsSwipe(swipeInput))
							OnSwipe(swipeInput.Position, 
								Mathf.Clamp01(swipeInput.Speed / _swipeInchesPerSecMax));
					}
					else
					{
						_beginInput = null;
						_endInput = null;
					}
				}

				break;
			}
		}






		#endregion

		#region Public methods
		#endregion

		#region Private methods


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private InputData ReadInput()
		{
#if UNITY_EDITOR
			InputData input;
			input.Phase = Input.GetMouseButtonDown(0)	? InputData.InputPhase.Start :
						  Input.GetMouseButtonUp(0)		? InputData.InputPhase.Stop  :
						  Input.GetMouseButton(0)		? InputData.InputPhase.Stay :
														  InputData.InputPhase.None;
			input.Position = Input.mousePosition;
			input.Seconds = Time.time;
#else
			if (Input.touchCount == 0)
				return new InputData();

			InputData input;
			Touch touch = Input.GetTouch(0);

			switch (touch.phase)
			{
				case TouchPhase.Began:
					input.Phase = InputData.InputPhase.Start;
					break;
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					input.Phase = InputData.InputPhase.Stop;
					break;
				default:
					input.Phase = InputData.InputPhase.Stay;
					break;
			}	
			
			input.Position = touch.position;
			input.Seconds = Time.time;
#endif
			return input;
		}
		
	
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private bool IsDoubleTap(InputData data)
		{
			return data.Seconds < _doubleTapSecsMax && !IsSwipe(data);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private bool IsSwipe(InputData data)
		{
			return data.Position.sqrMagnitude > 
				_doubleTapInchesSqrMax * Screen.dpi * Screen.dpi;
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnSwipe(Vector2 swipeVector, float speedRatio)
		{
			_beginInput = null;
			_endInput = null;

			//TODO: Trigger swipe
			Debug.Log("Swipe>> " + swipeVector + " @" + speedRatio);

			var e = Swipe;
			if (e != null)
				e(swipeVector, speedRatio);
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void OnDoubleTap(Vector2 position)
		{
			_beginInput = null;
			_endInput = null;

			Debug.Log("DoubleTap>> " + position);

			var e = DoubleTap;
			if (e != null)
				e(position);	
		}

#endregion

#region Private members

		[SerializeField, Range(0.01f, 1f)]	private float _doubleTapSecsMax = 0.5f;
		[SerializeField, Range(0f, .5f)]		private float _doubleTapInchesSqrMax = .1f;
		[SerializeField, Range(1f, 100f)]	private float _swipeInchesPerSecMax = 40f;

		private InputData? _beginInput;
		private InputData? _endInput;
		

		#endregion
	}
}
