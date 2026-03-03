using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class LevelManager : MonoBehaviour {
	public static LevelManager Instance{ get; private set;}

	public Player Player{ get; private set;}
	CameraFollow Camera;

	private List<CheckPoint> _checkpoints;
	private int _currentCheckpointIndex;

	public bool isLastLevelOfWorld = false;
//	public CheckPoint DebugSpawn;
	[Header("Level Paremeter")]
	public string LevelName = "World 01-01";
	public string nextLevelName;

	public int timer = 120;
	public int currentTimer{ get; set; }
	int saveTimerCheckPoint;

	public int alarmTimeLess = 60;
	public AudioClip soundCheckpoint;
	[Range(0,1)]
	public float soundCheckpointVolume = 0.5f;
	public AudioClip soundTimeLess;
	[Range(0,1)]
	public float soundTimeLessVolume = 0.5f;
	public AudioClip soundTimeUp;
	[Range(0,1)]
	public float soundTimeUpVolume = 0.5f;


	void Awake(){
		Instance = this;
		currentTimer = timer;
		saveTimerCheckPoint = timer;


		_checkpoints = FindObjectsOfType<CheckPoint> ().OrderBy (t => t.transform.position.x).ToList ();
		_currentCheckpointIndex = _checkpoints.Count > 0 ? 0 : -1;
		//
	
	}
	// Use this for initialization
	void Start () {

		Player = FindObjectOfType<Player> ();
		Camera = FindObjectOfType<CameraFollow> ();

		var listener = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
		//		var listener = (IPlayerRespawnListener)FindObjectsOfType(typeof(IPlayerRespawnListener));

		foreach (var _listener in listener) {
			for (int i = _checkpoints.Count - 1; i >= 0; i--) {
				var distance = ((MonoBehaviour)_listener).transform.position.x - _checkpoints [i].transform.position.x;
				if (distance >= 0) {
					_checkpoints [i].AssignOnjectToCheckPoint (_listener);
					break;
				}
			}
		}

		StartCoroutine (BeginGameAfterCo (0.1f));
	
	}
	
	// Update is called once per frame
	void Update () {
		if (_currentCheckpointIndex + 1 >= _checkpoints.Count)
			return;

		var distanceToNextCheckPoint = _checkpoints [_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;
		if (distanceToNextCheckPoint >= 0)
			return;

		_currentCheckpointIndex++;
		_checkpoints [_currentCheckpointIndex].PlayerLeftCheckPoint ();
		_checkpoints [_currentCheckpointIndex].PlayerHitCheckPoint ();

		saveTimerCheckPoint = currentTimer;
		SoundManager.PlaySfx (soundCheckpoint, soundCheckpointVolume);

		GameManager.Instance.SaveCheckPoint ();
	}

	IEnumerator BeginGameAfterCo(float time){
		yield return new WaitForSeconds (time);

//		#if UNITY_EDITOR
//		if(DebugSpawn !=null){
//			DebugSpawn.SpawnPlayer(Player);
//		}else if(_currentCheckpointIndex != -1){
//			_checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
//		}
//		#else
		if(_currentCheckpointIndex!=-1)
		_checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
//		#endif 


	}

	public void StartGame(){
		StartCoroutine (CountDownTimer ());
	}

	public void KillPlayer(){
		Player.Kill ();
		Camera.isFollowing = false;

		GameManager.Instance.GameOver ();
	}

	public void GotoCheckPoint(){
		_checkpoints [_currentCheckpointIndex].SpawnPlayer (Player);
		Camera.isFollowing = true;
		currentTimer = saveTimerCheckPoint;

		StartCoroutine (CountDownTimer ());
	}

	IEnumerator CountDownTimer(){
		yield return new WaitForSeconds (1);

		if (GameManager.Instance.State != GameManager.GameState.Playing)
			yield break;

		currentTimer--;
		if (currentTimer == alarmTimeLess)
			SoundManager.PlaySfx (soundTimeLess, soundTimeLessVolume);
		else if (currentTimer <= 0) {
			SoundManager.PlaySfx (soundTimeUp, soundTimeUpVolume);
			KillPlayer ();
		}

		if (currentTimer > 0)
			StartCoroutine (CountDownTimer ());
	}
}
