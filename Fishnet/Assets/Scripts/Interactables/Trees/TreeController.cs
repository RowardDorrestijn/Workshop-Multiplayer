using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;

public class TreeController : NetworkBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;
    public int amountOfHitsNeededForBranch = 5;
    public GameObject branchPrefab;

    private Vector3 originalPos;
    private bool isShaking = false;

    private readonly SyncVar<int> currentAmountOfHitsNeededForBranch = new SyncVar<int>();

    void Awake()
    {
        originalPos = transform.localPosition;
        currentAmountOfHitsNeededForBranch.OnChange += OnHitsChanged;
    }   

    public override void OnStartServer()
    {
        currentAmountOfHitsNeededForBranch.Value = amountOfHitsNeededForBranch;
    }

    private void OnHitsChanged(int prev, int next, bool asServer)
    {
        StartCoroutine(Shake());
    }

    [ServerRpc(RequireOwnership = false)]
    public void Hit(Transform playerTransform)
    {
        if (!isShaking)
        {
            currentAmountOfHitsNeededForBranch.Value--;

            if (currentAmountOfHitsNeededForBranch.Value <= 0)
            {
                DropBranch(playerTransform);
                currentAmountOfHitsNeededForBranch.Value = amountOfHitsNeededForBranch;
            }
        }
    }

    private void DropBranch(Transform player)
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        Vector3 dropDirection = Vector3.Cross(Vector3.up, dirToPlayer).normalized;

        Vector3 spawnPos = transform.position + (dropDirection * 1f) + (Vector3.up * 1f);

        GameObject branch = Instantiate(branchPrefab, spawnPos, Quaternion.identity);

        if (branch.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(dropDirection * 2f + Vector3.up * 2f, ForceMode.Impulse);
        }

        Spawn(branch);
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float z = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y, originalPos.z + z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        isShaking = false;
    }
}
