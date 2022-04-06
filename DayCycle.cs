using UnityEngine;

[ExecuteInEditMode]
public class DayCycle : MonoBehaviour
{
    public static bool isday = true;
    [SerializeField] Gradient sunGradient;
    [SerializeField] private Transform player;
    [SerializeField, Range(1, 3600)] private int dayinSecond = 60;
    [SerializeField, Range(0f, 1f)] private float time;
    [SerializeField, Range(1, 200)] private int radius;
    [SerializeField, Range(1, 10)] private int speed = 3;
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D globalLight;
    private UnityEngine.Rendering.Universal.Light2D sun;
    
    private void Awake(){
        sun = this.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            time += Time.deltaTime / dayinSecond;
            sun.shadowIntensity = (-2.4f * time * time + 2.4f * time) / 2;
        }
        if (time > 1f)
        {
            time = 0f;
        }
        sun.color = globalLight.color = sunGradient.Evaluate(time);
        if(time >= 0.2f && time <= 0.8f){
            sun.transform.position = new Vector2(-Mathf.Cos((time - 0.2f) / 0.6f * speed) * radius, Mathf.Sin((time - 0.2f) / 0.6f  * speed) * radius) + new Vector2(player.position.x, player.position.y);
            sun.intensity = globalLight.intensity = (- 280 / 9 * time * time + 280 / 9 * time - 43 / 9 - 0.8f) / 4;
            isday = true;
        }
        else{
            isday = false;
        }
    }
}
