using UnityEngine;

public class AudioController : MonoBehaviour
{

    [SerializeField]
    private AudioSource[] audioSource_music;

    [SerializeField]
    private AudioSource[] audioSource_nature;

    [SerializeField]
    private AudioClip[] audioClip;

    private bool isFade = false;
    private int audioSourceNext = 0;
    private float speedFade = 0.001f;

    // Use this for initialization
    void Start()
    {

        // for (int i = 0; i < audioSource_music.Length; i++)
        // {

        //     if (audioSource_music[i].isPlaying == false)
        //     {

        //         audioSource_music[i].clip = audioClip[0];
        //         audioSource_music[i].Play();
        //         break;
        //     }

        // }

    }

    void Update()
    {

        if (isFade)
        {

            if (audioSourceNext == 0)
            {

                audioSource_music[0].volume += speedFade;
                audioSource_music[1].volume -= speedFade;

                if (audioSource_music[1].volume == 0)
                {
                    audioSource_music[1].Stop();
                }

            }
            else
            {

                audioSource_music[0].volume -= speedFade;
                audioSource_music[1].volume += speedFade;

                if (audioSource_music[0].volume == 0)
                {
                    audioSource_music[0].Stop();
                }
            }

        }


        if (Input.GetKeyDown(KeyCode.F))
        {


            for (int i = 0; i < audioSource_music.Length; i++)
            {

                if (audioSource_music[i].isPlaying == false)
                {

                    int s = Random.Range(0, audioClip.Length);

                    print("s=" + s);

                    audioSource_music[i].clip = audioClip[s];
                    audioSource_music[i].Play();
                    audioSource_music[i].volume = 0;
                    isFade = true;
                    audioSourceNext = i;
                    break;
                }
            }


        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            for (int i = 0; i < audioSource_music.Length; i++)
            {
                if (audioSource_music[i].isPlaying == false)
                {
                    int s = Random.Range(0, audioClip.Length);

                    print("s=" + s);

                    audioSource_music[i].clip = audioClip[s];
                    audioSource_music[i].Play();
                    audioSource_music[i].volume = 0;
                    isFade = true;
                    audioSourceNext = i;
                    break;
                }
            }


        }

    }

    // void OnGUI()
    // {

    //     string text = audioSource_music[0].volume.ToString("0.0");
    //     text += "\n" + audioSource_music[1].volume.ToString("0.0");

    //     GUI.TextField(new Rect(10, 10, 200, 300), text);

    // }
}