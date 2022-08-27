using UnityEngine;

public class MazeObject : MonoBehaviour {
	public Vector2 WorldPosition { get; private set; }
	public Vector3 WorldPosition3D { get { return new Vector3(WorldPosition.x, 0, WorldPosition.y); } }
	public Vector3 LastPosition { get; private set; }

	Vector2 manualMoveDelta;

	protected virtual void Start() {
		GameManager.Instance.OnMazeMove.AddListener(OnMazeMove);
	}

	public void SetWorldPosition(Vector2 newPos) {
		Vector2 deltaPos = newPos - WorldPosition;
		
		transform.position += new Vector3(deltaPos.x, 0, deltaPos.y);
		manualMoveDelta += deltaPos;
		WorldPosition += deltaPos;
	}

	void OnMazeMove(Vector2 deltaPos) {
		transform.position += new Vector3(deltaPos.x, 0, deltaPos.y);
		manualMoveDelta += deltaPos;
	}

	protected virtual void LateUpdate() {
		UpdateWorldPosition();

		LastPosition = transform.position;
	}

	void OnDestroy() {
		GameManager.Instance.OnMazeMove.RemoveListener(OnMazeMove);
	}

	void UpdateWorldPosition() {
		Vector3 deltaPos = transform.position - LastPosition;
		WorldPosition += new Vector2(deltaPos.x, deltaPos.z);
		WorldPosition -= manualMoveDelta;
		manualMoveDelta = Vector2.zero;
	}
}