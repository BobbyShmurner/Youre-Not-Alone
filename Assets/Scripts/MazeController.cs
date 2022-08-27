using UnityEngine;

using System.Threading;
using System.Collections.Generic;

public class MazeController : MonoBehaviour {
	[field: SerializeField] public MazePieces MazePieces { get; private set; }
	[field: SerializeField] public CornerPieces CornerPieces { get; private set; }

	[field: SerializeField] public Vector2Int ChunkSize { get; private set; } = new Vector2Int(5, 5);
	[field: SerializeField] [field: Range( 0f, 1f)] public float MinPlacementThreshold { get; private set; } = 0;
	[field: SerializeField] [field: Range( 0f, 1f)] public float MaxPlacementThreshold { get; private set; } = 0.95f;
	[field: SerializeField] [field: Range(-1f, 1f)] public float PlacementThresholdOverride { get; private set; } = -1;
	[field: SerializeField] public int ChunksToSwitchThreshold { get; private set; } = 20;

	public Dictionary<Vector2Int, int[,]> MazeData { get; private set; } = new Dictionary<Vector2Int, int[,]>();
	public float PlacementThreshold { get; private set; }

	int previousChunkAmountToSwitchThreshold;

	void Start() {
		transform.position -= new Vector3(ChunkSize.x * 0.5f - 0.5f, 0, ChunkSize.y * 0.5f - 0.5f);
		RegenerateAllChunks();
	}

	public void RegenerateAllChunks() {
		RandomisePlacementThreshold();

		int i = 0;
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				MazeChunk chunk = transform.GetChild(i).GetComponent<MazeChunk>();
				chunk.transform.localPosition = new Vector3(x * ChunkSize.x, 0, y * ChunkSize.y);

				ConstructChunkAtPosition(chunk, new Vector2Int(x, y), true);
				i++;
			}
		}
	}

	public Vector2Int GetChunkPosAtPosition(Vector2 worldPos) {

		Vector2 alignedWorldPos = GetChunkAlignedWorldPos(worldPos);

		return new Vector2Int(
			Mathf.FloorToInt((alignedWorldPos.x - (alignedWorldPos.x.Mod(ChunkSize.x))) / ChunkSize.x),
			Mathf.FloorToInt((alignedWorldPos.y - (alignedWorldPos.y.Mod(ChunkSize.y))) / ChunkSize.y)
		);
	}

	public Vector2 GetLocalChunkPos(Vector2 worldPos) {
		Vector2 alignedWorldPos = GetChunkAlignedWorldPos(worldPos);

		return new Vector2(
			alignedWorldPos.x.Mod(ChunkSize.x),
			alignedWorldPos.y.Mod(ChunkSize.y)
		);
	}

	public Vector2 GetChunkAlignedWorldPos(Vector2 worldPos) {
		return new Vector2(
			worldPos.x + ChunkSize.x * 0.5f,
			worldPos.y + ChunkSize.y * 0.5f
		);
	}

	public bool IsCellOccupied(Vector2 worldPos) {
		Vector2 localChunkPos = GetLocalChunkPos(worldPos);

		int[,] chunkData = GetChunkData(GetChunkPosAtPosition(worldPos));
		return chunkData[Mathf.FloorToInt(localChunkPos.x), Mathf.FloorToInt(localChunkPos.y)] == 1;
	}

	public void RandomisePlacementThreshold() {
		if (PlacementThresholdOverride != -1) PlacementThreshold = PlacementThresholdOverride;
		else PlacementThreshold = Mathf.Clamp(Random.value, MinPlacementThreshold, MaxPlacementThreshold);
	}

	public void MoveChunks(int xDir, int yDir) {
		foreach (Transform child in transform) {
			float deltaX = xDir * ChunkSize.x;
			float deltaY = yDir * ChunkSize.y;
			bool genChunk = false;

			MazeChunk chunk = child.GetComponent<MazeChunk>();
			if (chunk == null) continue;

			if (child.localPosition.x + xDir > ChunkSize.x) {
				deltaX = -ChunkSize.x * 2;
				genChunk = true;
			} else if  (child.localPosition.x + xDir < -ChunkSize.x) {
				deltaX = ChunkSize.x * 2;
				genChunk = true;
			}

			if (child.localPosition.z + yDir > ChunkSize.y) {
				deltaY = -ChunkSize.y * 2;
				genChunk = true;
			} else if  (child.localPosition.z + yDir < -ChunkSize.y) {
				deltaY = ChunkSize.y * 2;
				genChunk = true;
			}

			if (genChunk) ConstructChunkAtPosition(chunk, new Vector2Int(chunk.ChunkPos.x - xDir * 3, chunk.ChunkPos.y - yDir * 3));
			child.localPosition += new Vector3(deltaX, 0, deltaY);
		}
	}

	public int[,] GetChunkData(Vector2Int chunkPos, bool forceRegenerate = false) {
		if (!forceRegenerate && MazeData.ContainsKey(chunkPos)) return MazeData[chunkPos];

		return CreateDataForChunk(chunkPos);
	}

	public int[,] GenerateMazeData() {
        int[,] maze = new int[ChunkSize.x, ChunkSize.y];
		int xMax = maze.GetUpperBound(0);
		int yMax = maze.GetUpperBound(1);

		for (int x = 0; x <= xMax; x++) {
			for (int y = 0; y <= yMax; y++) {
				if (x % 2 != 0 || y % 2 != 0) continue;
				if (Random.value <= PlacementThreshold) continue;

				maze[x, y] = 1;

				int xAdjacent = Random.value < .5 ? 0 : (Random.value < .5 ? -1 : 1);
				int yAdjacent = xAdjacent != 0 ? 0 : (Random.value < .5 ? -1 : 1);

				if (x + xAdjacent > xMax || x + xAdjacent < 0 || y + yAdjacent > yMax || y + yAdjacent < 0) continue;
				maze[x + xAdjacent, y + yAdjacent] = 1;
			}
		}

        return maze;
    }

	int[,] CreateDataForChunk(Vector2Int chunkPos) {
		int[,] newData = GenerateMazeData();
		MazeData.Add(chunkPos, newData);

		if (MazeData.Count > previousChunkAmountToSwitchThreshold + ChunksToSwitchThreshold) {
			previousChunkAmountToSwitchThreshold = MazeData.Count;
			RandomisePlacementThreshold();
		}

		return newData;
	}
	
	void ConstructChunkAtPosition(MazeChunk chunk, Vector2Int chunkPos, bool forceRegenerate = false) {
		chunk.DeconstructMaze();
		chunk.Init(MazePieces, CornerPieces, GetChunkData(chunkPos, forceRegenerate), chunkPos);
		chunk.ConstructMaze();
	}
}