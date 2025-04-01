using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// User interface loading progress.
/// </summary>
using UnityEngine.Events;


public class UILoadingStreamer : MonoBehaviour
{
	[Tooltip ("List of streamers objects that should affect loading screen. Drag and drop here all your streamer objects from scene hierarchy which should be used in loading screen.")]
	/// <summary>
	/// The streamers.
	/// </summary>
	public Streamer[] streamers;

	/// <summary>
	/// The progress image.
	/// </summary>
	public Image progressImg;

	[Tooltip ("Time in seconds that you give your loading screen to get data from whole streamers about scene that they must load before loading screen will be switched off.")]
	/// <summary>
	/// The wait time after end of loading.
	/// </summary>
	public float waitTime = 2;

	public UnityEvent onDone;

	/// <summary>
	/// Awake this instance, and set fill to 0.
	/// </summary>
	void Awake ()
	{
		foreach (var item in streamers) {
			item.loadingStreamer = this;
			item.showLoadingScreen = true;
		}
		progressImg.fillAmount = 0;
	}

	/// <summary>
	/// Update this instance, and sets current progress of streaming.
	/// </summary>
	void Update ()
	{
		if (streamers.Length > 0) {
			
			bool initialized = true;

			progressImg.fillAmount = 0;
			foreach (var item in streamers) {
				progressImg.fillAmount += item.LoadingProgress / (float)streamers.Length;
				initialized = initialized && item.initialized;
			}
			if (initialized) {
				if (progressImg.fillAmount >= 1) {
					if (onDone != null)
						onDone.Invoke ();
					StartCoroutine (TurnOff ());
				}
			}

		} else
			Debug.Log ("No streamer Attached");
	}

	public IEnumerator TurnOff ()
	{
		yield return new WaitForSeconds (waitTime);
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Show progress bar and resets fill.
	/// </summary>
	public void Show ()
	{
		progressImg.fillAmount = 0;
		gameObject.SetActive (true);
	}

}
