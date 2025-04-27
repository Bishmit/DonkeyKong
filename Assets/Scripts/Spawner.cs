using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject prefab;
    public float minTime = 3f;
    public float maxTime = 5f;

    private void OnEnable()
    {
        Invoke(nameof(Spawn), minTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Spawn()
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
        Invoke(nameof(Spawn), Random.Range(minTime, maxTime));
    }
}
