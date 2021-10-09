using UnityEngine;
using System.Collections;
public class MoveCube : MonoBehaviour
{
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 endPosition;
    [SerializeField] private float progress;
    [SerializeField] private GameObject cube;
    private float step = 0.0001f;
    private bool acr = true;

    private void Start()
    {
        startPosition = cube.transform.position;
        endPosition = new Vector2(startPosition.x + Random.Range(-10, 10), startPosition.y + Random.Range(-10, 10));
        cube.transform.position = startPosition;
        StartCoroutine(Inionni());
    }

    private IEnumerator Inionni()
    {
        acr = false;
        endPosition = new Vector2(startPosition.x + Random.Range(-10, 10), startPosition.y + Random.Range(-10, 10));
        progress = 0;
        yield return new WaitForSeconds(3);
        acr = true;
    }
    private void FixedUpdate()
    {
        if (acr == true)
        {
            if (Mathf.RoundToInt(startPosition.x) != Mathf.RoundToInt(endPosition.x) || Mathf.RoundToInt(startPosition.y) != Mathf.RoundToInt(endPosition.y))
            {
                if (progress != 0)
                {
                    startPosition = Vector2.Lerp(startPosition, endPosition, progress);
                }
                cube.transform.position = startPosition;
                progress += step;
            }
            else
            {
                StartCoroutine(Inionni());
            }
        }
    }

}