using System.Runtime.CompilerServices;
using UnityEngine;

using System.Collections;

[RequireComponent(typeof(Pathfind), typeof(AudioPoolPlayer))]
public class DemonController : MonoBehaviour {
	[field: SerializeField] public GameObject Visuals { get; private set; }
	[field: SerializeField] public AudioPool AmbientPool { get; private set; }
	[field: SerializeField] public AudioPool RevealPool { get; private set; }
	[field: SerializeField] public MazeController MazeController { get; private set; }
	[field: SerializeField] public PlayerController Player { get; private set; }
    [field: SerializeField] public LayerMask RaycastIgnore { get; private set; } = 768;
	[field: SerializeField] public float SpawnDelay { get; private set; }
	[field: SerializeField] public float SpawnDistance { get; private set; }
	[field: SerializeField] public float MoveSpawnDistance { get; private set; }
	[field: SerializeField] public float RevealDistance { get; private set; }

	public Pathfind Pathfind { get; private set; }
	public AudioPoolPlayer AudioPoolPlayer { get; private set; }

	public Vector2 WorldPosition { get { return Pathfind.WorldPosition; } }
	public Vector2 WorldPosition3D { get { return Pathfind.WorldPosition3D; } }

	public bool IsSpawned { get; private set; }

	public void MoveSpawnPosition() {
		Pathfind.SetWorldPosition(Player.WorldPosition + Random.insideUnitCircle.normalized * SpawnDistance);
	}

	void Awake() {
		Pathfind = GetComponent<Pathfind>();
		AudioPoolPlayer = GetComponent<AudioPoolPlayer>();
	}

	void Start() {
		Despawn(false);
		MoveSpawnPosition();
	}

	void Update() {
		if (!IsSpawned){
			if (MazeController.IsCellOccupied(WorldPosition) || Vector2.Distance(WorldPosition, Player.WorldPosition) > MoveSpawnDistance) MoveSpawnPosition();
			if (Vector2.Distance(WorldPosition, Player.WorldPosition) <= RevealDistance) {
				RaycastHit hit;
				if (Physics.Raycast(transform.position, Player.transform.position - transform.position, out hit, 21, ~RaycastIgnore)) {
					Debug.Log($"Hit: {hit.transform.name}");
					if (hit.transform.GetComponent<PlayerController>() != null) StartCoroutine(Reveal());
				}
			}
		}
	}

	public IEnumerator Reveal(bool playAudio = true) {
		if (playAudio) AudioManager.PlayFromPool(RevealPool, 1, transform);
		Visuals.SetActive(true);
		IsSpawned = true;
		
		Pathfind.DisablePathfinding = true;
		AudioPoolPlayer.enabled = false;

		yield return new WaitForSeconds(SpawnDelay);

		Pathfind.DisablePathfinding = false;
		AudioPoolPlayer.enabled = true;
	}

	public void Despawn(bool playAudio = true) {
		if (playAudio) AudioManager.PlayFromPool(AmbientPool, 1, transform);
		Visuals.SetActive(false);
		IsSpawned = false;

		Pathfind.DisablePathfinding = true;
		AudioPoolPlayer.enabled = false;
	}
}