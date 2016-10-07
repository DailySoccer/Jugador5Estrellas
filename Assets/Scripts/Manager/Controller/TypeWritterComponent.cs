
using UnityEngine;
using System.Collections;
using FootballStar.Audio;

namespace FootballStar.Manager {

	public class TypeWritterComponent : MonoBehaviour {
		
		public float letterPause = 0.015f;
		public UILabel TextLabel;
		
		void Awake() {
			mAudioGameController = GameObject.FindGameObjectWithTag("GameModel").GetComponent<AudioInGameController>();
		}
		
		// Use this for initialization
		void Start () {
			message = TextLabel.text;
			TextLabel.text = "";
			StartCoroutine ("TypeText");
		}

		IEnumerator TypeText () {
			string newText = "";
			bool isColorCode = false;
			foreach (char letter in message.ToCharArray()) 
			{
				if( char.Equals(letter,'[') )
					isColorCode = true;					
					
				newText += letter;

				if( isColorCode )
				{
					if ( newText.Substring(newText.Length - 1, 1) == "]" )
					{
						PlaySound();
						TextLabel.text += newText;
						isColorCode = false;
						newText ="";
						yield return new WaitForSeconds (letterPause);
					}
				}
				else
				{
					PlaySound();
					TextLabel.text += newText;
					newText ="";
					yield return new WaitForSeconds (letterPause);
				}
			}      
		}

		private void PlaySound()
		{
		 if (mAudioGameController)
				mAudioGameController.PlayDefinition(SoundDefinitions.TYPE_WRITER);
		}
		
		SmallTheater mSmallTheater;
		
		private UILabel mTextLabel;
		private string message;
		private AudioInGameController mAudioGameController;
	}
}
