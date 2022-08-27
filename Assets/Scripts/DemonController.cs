using UnityEngine;

using System.Collections;

[RequireComponent(typeof(Pathfind), typeof(AudioPoolPlayer))]
public class DemonController : MonoBehaviour {
	[field: SerializeField] public GameObject Visuals { get; private set; }
	[field: SerializeField] public AudioPool AmbientPool { get; private set; }
	[field: SerializeField] public AudioPool RevealPool { get; private set; }
	[field: SerializeField] public MazeController MazeController { get; private set; }
	[field: SerializeField] public PlayerController Player { get; private set; }
    [field: SerializeField] public LayerMask RaycastIgnore { get; private set; }
	[field: SerializeField] public float FreezeAfterSpawnDelay { get; private set; }
	[field: SerializeField] public float SpawnDistance { get; private set; }
	[field: SerializeField] public float SpawnDelay { get; private set; }
	[field: SerializeField] public float MoveSpawnDistance { get; private set; }
	[field: SerializeField] public float RevealDistance { get; private set; }

	public Pathfind Pathfind { get; private set; }
	public AudioPoolPlayer AudioPoolPlayer { get; private set; }
	public Renderer Renderer { get; private set; }
	public Collider Collider { get; private set; }

	public Vector2 WorldPosition { get { return Pathfind.WorldPosition; } }
	public Vector2 WorldPosition3D { get { return Pathfind.WorldPosition3D; } }

	public bool IsSpawned { get; private set; }
	public bool IsMovingSpawnPos { get; private set; }
	public float TimeUntilCanSpawn { get; private set; }

	IEnumerator MoveSpawnPosition() {
		if (!IsSpawned && !IsMovingSpawnPos) {
			IsMovingSpawnPos = true;

			do {
				Vector2 newPos = Player.WorldPosition + Random.insideUnitCircle.normalized * SpawnDistance;
				newPos.x = Mathf.RoundToInt(newPos.x);
				newPos.y = Mathf.RoundToInt(newPos.y);

				Pathfind.SetWorldPosition(newPos);
				Pathfind.CheckForTarget();
				yield return null;
			} while (!IsSpawned && !Pathfind.LineOfSight);

			IsMovingSpawnPos = false;
		}
	}

	void Awake() {
		Pathfind = GetComponent<Pathfind>();
		AudioPoolPlayer = GetComponent<AudioPoolPlayer>();
		Renderer = Visuals.GetComponent<Renderer>();
		Collider = GetComponent<Collider>();

		StartCoroutine(MoveSpawnPosition());
		TimeUntilCanSpawn = SpawnDelay;
	}

	void Start() {
		Despawn(false);
	}

	void Update() {
		if (!IsSpawned && TimeUntilCanSpawn <= 0){
			if (MazeController.IsCellOccupied(WorldPosition) || Vector2.Distance(WorldPosition, Player.WorldPosition) > MoveSpawnDistance) StartCoroutine(MoveSpawnPosition());
			if (Vector2.Distance(WorldPosition, Player.WorldPosition) <= RevealDistance) {
				RaycastHit hit;
				if (Physics.SphereCast(Player.Camera.transform.position, 0.1f, Player.Camera.transform.forward, out hit, 21, ~RaycastIgnore, QueryTriggerInteraction.Collide)) {
					if (hit.transform == transform) StartCoroutine(Reveal());
				}
			}
		} else {
			if (!Pathfind.DisablePathfinding && Pathfind.RemainingInsight <= 0 && Pathfind.Waypoints.Count == 0) Despawn();
		}

		TimeUntilCanSpawn = Mathf.Clamp(TimeUntilCanSpawn - Time.deltaTime, 0, SpawnDelay);
	}

	public IEnumerator Reveal(bool playAudio = true) {
		if (playAudio) AudioManager.PlayFromPool(RevealPool, 1, transform);
		Visuals.SetActive(true);
		IsSpawned = true;
		
		Pathfind.DisablePathfinding = true;
		AudioPoolPlayer.enabled = false;
		Collider.isTrigger = true;

		yield return new WaitForSeconds(FreezeAfterSpawnDelay);

		Pathfind.DisablePathfinding = false;
		AudioPoolPlayer.enabled = true;
		Collider.isTrigger = false;
	}

	public void Despawn(bool playAudio = true) {
		if (playAudio) AudioManager.PlayFromPool(AmbientPool, 1, transform.position);
		Visuals.SetActive(false);
		IsSpawned = false;

		Pathfind.DisablePathfinding = true;
		AudioPoolPlayer.enabled = false;
		Collider.isTrigger = true;

		TimeUntilCanSpawn = SpawnDelay;
	}
}