using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarOfTanks;

/// <summary>
/// Caméra RTS :
///  - Au démarrage, se centre automatiquement sur les tanks du Layer "TeamA"
///  - Déplacement par edge-scrolling (souris aux bords)
///  - Zoom molette
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Edge Scrolling")]
    [Range(1f, 20f)] public float scrollSpeed = 8f;
    [Range(0f, 60f)] public float edgeThreshold = 20f; // pixels depuis le bord

    [Header("Zoom")]
    public float zoomSpeed = 2f;
    public float minZoom = 3f;
    public float maxZoom = 15f;

    [Header("Limites de la map (optionnel)")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    [Header("Centrage initial")]
    [Tooltip("Layer des tanks joueur (LayerMask Unity)")]
    public LayerMask playerLayer;
    public string playerTag = "Tank";

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        CenterOnPlayerTanks();
    }

    void Update()
    {
        HandleEdgeScroll();
        HandleZoom();
        ClampPosition();
    }

    // ── Centrage initial ─────────────────────────────────────────────────

    void CenterOnPlayerTanks()
    {
        // Centrer sur la base du joueur
        var playerBase = FindObjectsByType<Spawn>(FindObjectsSortMode.None)
            .Where(s => s.team == GameManager.Instance.playerTeam).First();
        var basePosition = playerBase.transform.position;

        // Positionne la caméra (garde la coordonnée Z)
        transform.position = new Vector3(basePosition.x, basePosition.y, transform.position.z);
    }

    // ── Edge scrolling ───────────────────────────────────────────────────

    void HandleEdgeScroll()
    {
        // Todo: faire que la vitesse du scrolling soit proportionnelle au zoom
        // scrollSpeed *= cam.orthographicSize / 15;

        Vector3 move = Vector3.zero;
        Vector2 mouse = Input.mousePosition;

        if (mouse.x < edgeThreshold)                        move.x = -1f;
        else if (mouse.x > Screen.width - edgeThreshold)    move.x =  1f;

        if (mouse.y < edgeThreshold)                        move.y = -1f;
        else if (mouse.y > Screen.height - edgeThreshold)   move.y =  1f;

        transform.position += move * scrollSpeed * Time.deltaTime;
    }

    // ── Zoom ─────────────────────────────────────────────────────────────

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.001f) return;

        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    // ── Clamp ────────────────────────────────────────────────────────────

    void ClampPosition()
    {
        if (!useBounds) return;

        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, minBounds.x, maxBounds.x);
        p.y = Mathf.Clamp(p.y, minBounds.y, maxBounds.y);
        transform.position = p;
    }

    // ── API publique ─────────────────────────────────────────────────────

    /// <summary>Recentre manuellement sur les tanks joueur (ex: touche Home)</summary>
    public void RecenterOnPlayers()
    {
        CenterOnPlayerTanks();
    }
}