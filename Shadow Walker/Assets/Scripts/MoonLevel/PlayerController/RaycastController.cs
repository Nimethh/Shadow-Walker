using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PolygonCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask ground;
    public LayerMask ladder;

    public const float skinWidth = .05f;
    const float distanceBetweenRays = 0.05f;
    [HideInInspector]
    public int numberOfHorizontalRays;
    [HideInInspector]
    public int numberOfVerticalRays;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public PolygonCollider2D collider;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {
        collider = GetComponent<PolygonCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.middleBottom = new Vector2(bounds.center.x, bounds.min.y);
        raycastOrigins.middleTop = new Vector2(bounds.center.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        numberOfHorizontalRays = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
        numberOfVerticalRays = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);

        horizontalRaySpacing = bounds.size.y / (numberOfHorizontalRays - 1);
        verticalRaySpacing = bounds.size.x / (numberOfVerticalRays - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
        public Vector2 middleTop, middleBottom;
    }
}
