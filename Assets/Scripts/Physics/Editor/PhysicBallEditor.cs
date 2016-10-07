using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Physics.Editor
{
	[CustomEditor(typeof(PhysicBall))]
	public class PhysicBallEditor : UnityEditor.Editor
	{
		public void Awake()
		{
			_ball = (PhysicBall) target;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Shoot"))
				_ball.AddVelocity(_ball.ShootRightValue * Vector3.right
				                  + _ball.ShootUpValue * Vector3.up 
				                  + _ball.ShootForwardValue * Vector3.forward);

			GUILayout.Label("Right: " + _ball.ShootRightValue);
			_ball.ShootRightValue   = GUILayout.HorizontalSlider(_ball.ShootRightValue,	-10f, 10f);

			GUILayout.Label("Up:" + _ball.ShootUpValue);
			_ball.ShootUpValue	  = GUILayout.HorizontalSlider(_ball.ShootUpValue, 0f,  10f);

			GUILayout.Label("Forward:" + _ball.ShootForwardValue);
			_ball.ShootForwardValue = GUILayout.HorizontalSlider(_ball.ShootForwardValue, -10f, 10f);

			EditorUtility.SetDirty(target);
		}

		private PhysicBall _ball;
	}
}

