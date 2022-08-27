using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class CullManager : MonoBehaviour {
	[field: SerializeField] public int RayDistance { get; private set; } = 25;
	[field: SerializeField] public int RaysToShoot { get; private set; } = 30;

	public Camera Camera { get; private set; }

	void Awake() {
		Camera = GetComponent<Camera>();
	}

	void Start() {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)  {
        if (camera != Camera) return;

		float angle = 0;
		for (int i = 0; i < RaysToShoot; i++) {
			float x = Mathf.Sin(angle) * RayDistance;
			float z = Mathf.Cos(angle) * RayDistance;
			angle += 2 * Mathf.PI / RaysToShoot;
		
			Vector3 dir = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
			Debug.DrawLine (transform.position, dir, Color.red);

			RaycastHit hit;
			if (Physics.Raycast(transform.position, dir, out hit)) {
				CullObject cullObject = hit.transform.GetComponent<CullObject>();
				if (cullObject && cullObject.enabled) cullObject.Renderer.enabled = true;
			}
		}
    }

    void OnDestroy() {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
}