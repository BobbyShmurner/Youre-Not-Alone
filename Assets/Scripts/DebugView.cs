using System.Text;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class DebugView : MonoBehaviour {
	[field: SerializeField] public MazeController MazeController { get; private set; }
	[field: SerializeField] public PlayerController Player { get; private set; }
	[field: SerializeField] public DemonController Monster { get; private set; }

	public Text Text { get; private set; }
	public float Fps { get; private set; }

	void Awake() {
		Text = GetComponent<Text>();
	}

	void Start() {
		StartCoroutine(UpdateFPS());
	}

	void LateUpdate() {
		if (Input.GetKeyDown(KeyCode.F3)) Text.enabled = !Text.enabled;
		Vector2Int chunkPos = MazeController.GetChunkPosAtPosition(Player.WorldPosition);

		StringBuilder stringBuilder = new StringBuilder();

		stringBuilder.Append($"FPS: {Fps:0.00}");
		stringBuilder.Append($"\nWorldPos: {Player.WorldPosition.x:0.00}, {Player.WorldPosition.y:0.00}");
		stringBuilder.Append($"\nChunkPos: {chunkPos.x}, {chunkPos.y}");
		stringBuilder.Append($"\nMosterPos: {Monster.WorldPosition.x:0.00}, {Monster.WorldPosition.y:0.00}");
		stringBuilder.Append($"\nMonsterNextWaypoint: {(Monster.Pathfind.Waypoints.Count > 0 ? $"{Monster.Pathfind.Waypoints[0].x:0.00}, {Monster.Pathfind.Waypoints[0].y:0.00}" : "<color=red>None</color>")}");
		stringBuilder.Append($"\nMonsterRemainingInsight: {(Monster.Pathfind.RemainingInsight > 0 ? $"{Monster.Pathfind.RemainingInsight:0.00}" : "<color=red>0.00</color>")}");
		stringBuilder.Append($"\nMonsterWaypointCount: {(Monster.Pathfind.Waypoints.Count > 0 ? $"{Monster.Pathfind.Waypoints.Count}" : "<color=red>0</color>")}");
		stringBuilder.Append($"\nMonsterTimeUntilCanSpawn: {(Monster.TimeUntilCanSpawn > 0 ? $"<color=red>{Monster.TimeUntilCanSpawn:0.00}</color>" : "<color=green>0.00</color>")}");
		stringBuilder.Append($"\nIsCellOccupied: {(MazeController.IsCellOccupied(Player.WorldPosition) ? "<color=green>True</color>" : "<color=red>False</color>")}");
		stringBuilder.Append($"\nIsMonsterUsingInsight: {(Monster.Pathfind.RemainingInsight != Monster.Pathfind.InsightTime ? "<color=green>True</color>" : "<color=red>False</color>")}");
		stringBuilder.Append($"\nIsPlayerInLineOfSight: {(Monster.Pathfind.LineOfSight ? "<color=green>True</color>" : "<color=red>False</color>")}");

		Text.text = stringBuilder.ToString();
	}

	IEnumerator UpdateFPS() {
		while (true) {
			Fps = 1 / Time.deltaTime;
			yield return new WaitForSecondsRealtime(0.1f);
		}
	}
}