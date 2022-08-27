using UnityEngine;

public class MazeChunk : MonoBehaviour
{
	[field: SerializeField] public bool GenerateMarkers { get; private set; }
	[field: SerializeField] public GameObject FreePrefab { get; private set; }
	[field: SerializeField] public GameObject FilledPrefab { get; private set; }

	public MazePieces MazePieces { get; private set; }
	public CornerPieces CornerPieces { get; private set; }

    public int[,] MazeData { get; private set; } = null;
	public Vector2Int ChunkPos { get; private set; }

	public void Init(MazePieces mazePieces, CornerPieces cornerPieces, int[,] data, Vector2Int worldPos) {
		MazePieces = mazePieces;
		CornerPieces = cornerPieces;
		
		MazeData = data;
		ChunkPos = worldPos;

		gameObject.name = $"MazeChunk ({worldPos.x}, {worldPos.y})";
	}

	public bool IsFilledAtPosition(int x, int y) {
		return MazeData[x, y] == 1;
	}

	public void DeconstructMaze() {
		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}
	}

	public void ConstructMaze() {
		if (MazeData == null) {
			Debug.LogError($"Cannot Construct Maze! MazeData is null for \"{gameObject.name}\"");
			return;
		}

		for (int x = 0; x <= MazeData.GetUpperBound(0); x++) {
			for (int y = 0; y <= MazeData.GetUpperBound(1); y++) {
				int cell = MazeData[x, y];

				if (GenerateMarkers) {
				GameObject marker = null;
					if (cell == 1) {
						marker = GameObject.Instantiate(FilledPrefab, transform);
					} else {
						marker = GameObject.Instantiate(FreePrefab, transform);
					}
					marker.transform.localPosition = new Vector3(x, 0, y);
				}

				if (cell == 1) continue;

				bool isUpFree = (y == MazeData.GetUpperBound(1) || MazeData[x, y + 1] == 0);
				bool isDownFree = (y == 0 || MazeData[x, y - 1] == 0);
				bool isLeftFree = (x == 0 || MazeData[x - 1, y] == 0);
				bool isRightFree = (x == MazeData.GetUpperBound(0) || MazeData[x + 1, y] == 0);

				CreateCell(x, y, isUpFree, isDownFree, isLeftFree, isRightFree);
			}
		}
	}

	void CreateCell(int x, int y, bool isUpFree, bool isDownFree, bool isLeftFree, bool isRightFree) {
		GameObject newCell = null;
		float cellRot = 0;

		if (isUpFree && !isDownFree && !isLeftFree && !isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.End, transform);
			cellRot = 180;
		} else if (!isUpFree && isDownFree && !isLeftFree && !isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.End, transform);
			cellRot = 0;
		} else if (isUpFree && isDownFree && !isLeftFree && !isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.Straight, transform);
			cellRot = 0;
		} else if (!isUpFree && !isDownFree && isLeftFree && !isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.End, transform);
			cellRot = 90;
		} else if (isUpFree && !isDownFree && isLeftFree && !isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.Corner, transform);
			cellRot = 0;
		} else if (!isUpFree && isDownFree && isLeftFree && !isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.Corner, transform);
			cellRot = 270;
		} else if (isUpFree && isDownFree && isLeftFree && !isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.TSection, transform);
			cellRot = 270;
		} else if (!isUpFree && !isDownFree && !isLeftFree && isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.End, transform);
			cellRot = 270;
		} else if (isUpFree && !isDownFree && !isLeftFree && isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.Corner, transform);
			cellRot = 90;
		} else if (!isUpFree && isDownFree && !isLeftFree && isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.Corner, transform);
			cellRot = 180;
		} else if (isUpFree && isDownFree && !isLeftFree && isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.TSection, transform);
			cellRot = 90;
		} else if (!isUpFree && !isDownFree && isLeftFree && isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.Straight, transform);
			cellRot = 90;
		} else if (isUpFree && !isDownFree && isLeftFree && isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.TSection, transform);
			cellRot = 0;
		} else if (!isUpFree && isDownFree && isLeftFree && isRightFree) {
			newCell = GameObject.Instantiate(MazePieces.TSection, transform);
			cellRot = 180;
		} else {
			return;
		}

		newCell.transform.localPosition = new Vector3(x, 0, y);
		newCell.transform.localEulerAngles = new Vector3(-90, 0, cellRot);
	}

	void CreateCorner(int x, int y, bool bottomLeft, bool bottomRight, bool topLeft, bool topRight) {
		GameObject newCorner = null;
		float cornerRot = 0;

		if (bottomLeft && !bottomRight && !topLeft && !topRight) {
			newCorner = GameObject.Instantiate(MazePieces.End, transform);
			cornerRot = 180;
		} else if (!bottomLeft && bottomRight && !topLeft && !topRight) {
			newCorner = GameObject.Instantiate(MazePieces.End, transform);
			cornerRot = 0;
		} else if (bottomLeft && bottomRight && !topLeft && !topRight) {
			newCorner = GameObject.Instantiate(MazePieces.Straight, transform);
			cornerRot = 0;
		} else if (!bottomLeft && !bottomRight && topLeft && !topRight) {
			newCorner = GameObject.Instantiate(MazePieces.End, transform);
			cornerRot = 90;
		} else if (bottomLeft && !bottomRight && topLeft && !topRight) {
			newCorner = GameObject.Instantiate(MazePieces.Corner, transform);
			cornerRot = 0;
		} else if (!bottomLeft && bottomRight && topLeft && !topRight) {
			newCorner = GameObject.Instantiate(MazePieces.Corner, transform);
			cornerRot = 270;
		} else if (bottomLeft && bottomRight && topLeft && !topRight) {
			newCorner = GameObject.Instantiate(MazePieces.TSection, transform);
			cornerRot = 270;
		} else if (!bottomLeft && !bottomRight && !topLeft && topRight) {
			newCorner = GameObject.Instantiate(MazePieces.End, transform);
			cornerRot = 270;
		} else if (bottomLeft && !bottomRight && !topLeft && topRight) {
			newCorner = GameObject.Instantiate(MazePieces.Corner, transform);
			cornerRot = 90;
		} else if (!bottomLeft && bottomRight && !topLeft && topRight) {
			newCorner = GameObject.Instantiate(MazePieces.Corner, transform);
			cornerRot = 180;
		} else if (bottomLeft && bottomRight && !topLeft && topRight) {
			newCorner = GameObject.Instantiate(MazePieces.TSection, transform);
			cornerRot = 90;
		} else if (!bottomLeft && !bottomRight && topLeft && topRight) {
			newCorner = GameObject.Instantiate(MazePieces.Straight, transform);
			cornerRot = 90;
		} else if (bottomLeft && !bottomRight && topLeft && topRight) {
			newCorner = GameObject.Instantiate(MazePieces.TSection, transform);
			cornerRot = 0;
		} else if (!bottomLeft && bottomRight && topLeft && topRight) {
			newCorner = GameObject.Instantiate(MazePieces.TSection, transform);
			cornerRot = 180;
		} else {
			return;
		}

		newCorner.transform.localPosition = new Vector3(x, 0, y);
		newCorner.transform.localEulerAngles = new Vector3(-90, 0, cornerRot);
	}
}