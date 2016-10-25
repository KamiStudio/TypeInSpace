using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Question {
	public string question;
	public string[] options;
	public int correctAnswer;
}

[System.Serializable]
public class Story {
	[SerializeField]
	public string title;

	[SerializeField]
	public string url;

	[SerializeField]
	public TextAsset content;

	public Question[] questions;

	public void OnGUI () {
		title = EditorGUILayout.TextField( "Title", title);
		content = EditorGUILayout.ObjectField (content, typeof(TextAsset), false) as TextAsset;
	}

	public string Content() {
		return content.text;
	}
}

public class GameController : MonoBehaviour {
	public GameObject hazard;
	public TextAsset[] stories;
	public Story[] stories2;

	public Vector3 spawnValues;
	public int hazardCount;

	public float m_StartDelay = 3f;
	private WaitForSeconds m_startWait;

	public float spawnWait;
	public float waveWait;

	private TeleprompterController teleprompter;
	private Text statusText;

	private bool restart;
	private int selectedStory;
	private GameObject[] storyButtons;

	void Start () {
		selectedStory = -1;
		restart = false;
		m_startWait = new WaitForSeconds (m_StartDelay);
		Debug.Log ("Beginning game");

		teleprompter = GameObject.Find ("Teleprompter").GetComponent<TeleprompterController>();
		statusText = GameObject.FindWithTag ("Status").GetComponent<Text> ();
		storyButtons = GameObject.FindGameObjectsWithTag ("StoryButton");

		StartCoroutine (GameLoop ());
	}

	public void SetStory(int storyIndex) {
		Debug.Log ("Clicked button! Kapow: " + storyIndex);
		selectedStory = storyIndex;

		foreach (GameObject button in storyButtons) {
			button.SetActive (false);
		}
	}

	void Update() {
		//Debug.Log (teleprompter.Accuracy ());
		if (restart) {
			if (Input.GetKeyDown (KeyCode.R)) {
				SceneManager.LoadScene ("Main");
				foreach (GameObject button in storyButtons) {
					button.SetActive (true);
				}
				restart = false;
			}
		}
	}

	private IEnumerator GameLoop () {
		yield return StartCoroutine (RoundStarting ());

		yield return StartCoroutine (RoundPlaying ());

		yield return StartCoroutine (RoundEnding());
	}

	private IEnumerator RoundStarting () {
		statusText.text = "Get ready!";

		while (selectedStory == -1) {
			yield return null;
		}

		teleprompter.SetContent (stories2 [selectedStory].Content ());
		statusText.text = string.Empty;
		teleprompter.statusText = statusText;

		yield return m_startWait;
	}

	private IEnumerator RoundPlaying () {
		statusText.text = "Start typing!";

		teleprompter.Begin ();

		while (!teleprompter.IsComplete) {
			for (int i = 0; i < hazardCount; i++) {
				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
			//yield return new WaitForSeconds (waveWait);
			yield return null;
		}
	}

	private IEnumerator RoundEnding () {
		statusText.text = "All done! Press 'R' to play again.";
		restart = true;
		yield return m_startWait;
	}
}
