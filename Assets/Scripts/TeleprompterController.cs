using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TeleprompterController : MonoBehaviour {
	public string sourceText = string.Empty;
	public float wpmRate;
	public Text statusText;
	public bool IsComplete;
	public Color startColor;
	public Color readyColor;

	private GameController gameController;
	private Text textPanel;
	private string enteredText;
	private int mistakes;
	private bool Ready;
	private System.Diagnostics.Stopwatch timer;

	// Use this for initialization
	void Start () {
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
		timer = new System.Diagnostics.Stopwatch();
		Ready = false;
	}

	void Awake () {
		textPanel = (Text)GetComponent(typeof(Text));
		readyColor = textPanel.color;
		//textPanel.color = startColor;
	}

	public void SetContent(string text) {
		sourceText = text;
		enteredText = string.Empty;
		mistakes = 0;
		IsComplete = false;
		textPanel.text = "<b>Get ready to start typing!</b>\nYou'll be typing " + sourceText.Length + " letters.";
	}

	public float Accuracy() {
		return 1.0f - ((mistakes / (float)enteredText.Length));
	}

	public void Begin() {
		IsComplete = false;
		textPanel.color = readyColor;
		Ready = true;
		textPanel.text = asRichText ();
		timer.Start ();
	}

	void Update() {
		if (Input.inputString == string.Empty) {
			return;
		}

		if (IsComplete == true || Ready != true) {
			return;
		}

		if (Debug.isDebugBuild && Input.GetKeyDown (KeyCode.Escape)) {
			enteredText = sourceText;
			textPanel.text = FinalStats ();
			IsComplete = true;
			timer.Stop ();
			Debug.Log ("Skipping ahead!");
			return;
		}

		textPanel.color = readyColor;
		foreach (char c in Input.inputString) {
			if (c == "\b" [0]) {
				Debug.Log ("Currently ignoring backspace. THERE IS ONLY FORWARD!");
			} else if (System.Char.IsWhiteSpace(c)) {
				// We have a whitespace press, advance while the sourceText is also a whitespace character
				int cursor = enteredText.Length;
				if (System.Char.IsWhiteSpace (sourceText [cursor])) {
					while (System.Char.IsWhiteSpace (sourceText [cursor])) {
						enteredText += sourceText [cursor];
						cursor++;
					}
				} else {
					// User typed a whitespace character when one shouldn't be there.
					enteredText += c;
					mistakes++;
				}
				//print ("User entered their name: " + currentBlock.enteredText);
			} else {
				enteredText += c;
				if (enteredText [enteredText.Length - 1] != sourceText [enteredText.Length - 1]) {
					mistakes++;
				}
			}
		}

		if (enteredText.Length + 1 >= sourceText.Length) {
			textPanel.text = FinalStats ();
			IsComplete = true;
			timer.Stop ();
		} else {
			statusText.text = "Accuracy: " + string.Format ("{0:P2}", Accuracy ()) + ", " + string.Format("{0}", WPM()) + " WPM";
			textPanel.text = asRichText ();
		}
	}

	public float WPM() {
		System.TimeSpan ts = timer.Elapsed;
		return Mathf.Round((enteredText.Length / wpmRate) / (float)ts.TotalMinutes);
	}

	string FinalStats() {
		return "<b>You did it!</b>\nYou typed " + string.Format("{0}", WPM()) + " words per minute.\nYou were <color=red>" + string.Format("{0:P2}", Accuracy()) + "</color> accurate!";
	}

	/*
	 * Return the list of text needing to be typed 
	 * errors on enteredText show up as red and correctly entered are green!
	 */
	string asRichText() {
		if (sourceText == string.Empty) {
			return "Loading...";
		}

		string output = string.Empty;
		int index = 0;

		foreach(char c in sourceText){
			// Haven't typed yet, just be the default color.
			if (index == enteredText.Length) {
				//output += "<i>" + c + "</i>";
				output += c;
			}
			else if (index > enteredText.Length - 1 ) {
				output += c;
			}
			else if ( System.Char.IsWhiteSpace(c) || System.String.IsNullOrEmpty(c.ToString ()) ) {
				output += c;
			}
			else {
				string textColor = (enteredText [index] == sourceText [index]) ? "green" : "red";
				//Debug.Log ("expected " + sourceText [index] + ", got " + enteredText [index]);

				output += "<color=" + textColor + ">" + c + "</color>";
			}

			index++;
		}

		return output;
	}
}
