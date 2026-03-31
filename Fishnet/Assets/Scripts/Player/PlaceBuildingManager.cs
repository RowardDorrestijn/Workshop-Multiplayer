using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceBuildingManager : NetworkBehaviour
{
    [Header("Settings")]
    public GameObject buildingPrefab;
    public GameObject ghostPrefab;
    public float spawnDistance = 10f;
    public LayerMask groundMask;
    public LayerMask obstacleMask;

    [Header("Validation")]
    public float maxPlacementSlope = 15f;
    public Vector3 buildingHalfExtents = new Vector3(1f, 1f, 1f);
    public Color validColor = new Color(0, 1, 0, 0.5f);
    public Color invalidColor = new Color(1, 0, 0, 0.5f);

    [Header("Rotation Settings")]
    public float rotationSpeed = 0.1f;
    private float manualRotationY = 0f;

    private GameObject currentGhost;

    private List<Renderer> ghostRenderers = new List<Renderer>();

    private bool isBuildingMode = false;
    private bool canPlace = false;
    private InputSystem_Actions actions;

    private void Awake()
    {
        actions = new InputSystem_Actions();
        actions.Player.Attack.performed += ctx => PlaceBuilding();
    }

    private void OnEnable() => actions.Enable();
    private void OnDisable() => actions.Disable();

    void Update()
    {
        if(!IsOwner) return;

        if (Keyboard.current.bKey.wasPressedThisFrame) ToggleBuildMode();

        if (isBuildingMode && currentGhost != null)
        {
            UpdateGhostPosition();
            ValidatePlacement();
        }
    }

    void UpdateGhostPosition()
    {
        Vector3 targetPos = transform.position + (transform.forward * spawnDistance);

        if (Physics.Raycast(targetPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 20f, groundMask))
        {
            currentGhost.transform.position = hit.point + Vector3.up * 0.01f;

            float scrollValue = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scrollValue) > 0.1f)
            {
                // This will rotate the building as you scroll
                manualRotationY += scrollValue * rotationSpeed;
            }

            Quaternion finalRot = Quaternion.Euler(0, transform.eulerAngles.y + manualRotationY, 0);
            currentGhost.transform.rotation = finalRot;

            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            Vector3 boxCenter = hit.point + Vector3.up * buildingHalfExtents.y;

            bool isOverlapping = Physics.CheckBox(boxCenter, buildingHalfExtents, finalRot, obstacleMask);
            canPlace = (slopeAngle <= maxPlacementSlope) && !isOverlapping;
        }
    }

    void ValidatePlacement()
    {
        foreach (Renderer ren in ghostRenderers)
        {
            if (ren != null)
            {
                ren.material.color = canPlace ? validColor : invalidColor;
            }
        }
    }

    void ToggleBuildMode()
    {
        isBuildingMode = !isBuildingMode;
        if (isBuildingMode)
        {
            manualRotationY = 0f;
            currentGhost = Instantiate(ghostPrefab);

            ghostRenderers.Clear();
            ghostRenderers.AddRange(currentGhost.GetComponentsInChildren<Renderer>());

            Collider[] colliders = currentGhost.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }
        else
        {
            Destroy(currentGhost);
        }
    }

    void PlaceBuilding()
    {
        if (isBuildingMode && canPlace)
        {
            PlaceBuildingServerRpc(currentGhost.transform.position, currentGhost.transform.rotation);
            ToggleBuildMode();
        }
    }

    [ServerRpc]
    private void PlaceBuildingServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject house = Instantiate(buildingPrefab, position, rotation);
        Spawn(house);
    }

    private void OnDrawGizmos()
    {
        if (currentGhost != null)
        {
            Gizmos.color = canPlace ? Color.green : Color.red;
            Vector3 boxCenter = currentGhost.transform.position + Vector3.up * (buildingHalfExtents.y);
            Gizmos.DrawWireCube(boxCenter, buildingHalfExtents * 2);
        }
    }
}
