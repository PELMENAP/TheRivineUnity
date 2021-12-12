using UnityEngine;
[ExecuteInEditMode]
public class DayCycle : MonoBehaviour
{
    [SerializeField] Gradient dirlightGradient;
    [SerializeField] Gradient ambientlightGradient;

    [SerializeField, Range(1, 3600)] float dayinSecond = 60;
    [SerializeField, Range(0f, 1f)] float timeProgress;
    [SerializeField] Light dirLight;

    void Update()
    {
        if (Application.isPlaying)
        {
            timeProgress += Time.deltaTime / dayinSecond;
        }
        if (timeProgress > 1f)
        {
            timeProgress = 0f;
        }
        dirLight.color = dirlightGradient.Evaluate(timeProgress);
        RenderSettings.ambientLight = ambientlightGradient.Evaluate(timeProgress);
    }
}
