using UnityEngine;
using System.Collections;
public class CM : MonoBehaviour
{
    public GameObject Player;
    private Vector3 offset;
    private Transform cameratrans;
    public float Velocity;
    public float MinDistance;
    void Start()
    {
        cameratrans = transform;
        offset = cameratrans.position - Player.transform.position;
    }

    void LateUpdate()
    {
        Raycast();
        var targetPos = Player.transform.position + offset;
        if (Player == null || Vector3.Distance(cameratrans.position, targetPos) < MinDistance)
        {
            return;
        }
        transform.Translate(transform.InverseTransformPoint(Vector3.Lerp(cameratrans.position, targetPos, Velocity * Time.fixedDeltaTime)));
    }

    void Raycast()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(cameratrans.position + new Vector3(0, 2, 0), transform.forward);
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject obj = hits[i].collider.gameObject;
            if (obj.tag == "Chost")
            {
                StartCoroutine(changeAlpha(0.3f, obj));
            }
        }
    }

    private IEnumerator changeAlpha(float a, GameObject obj)
    {
        Material mat = obj.GetComponent<Renderer>().material;
        Color color = mat.color;
        color.a = a;
        mat.color = color;
        yield return new WaitForSeconds(3);
        color.a = 1f;
        mat.color = color;
    }
}

