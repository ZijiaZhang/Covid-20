using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeBox : MonoBehaviour
{
    public AudioSource audioSource_A;
    public AudioSource audioSource_B;
    public bool onAudioSource_A = true;

    [SerializeField]
    AudioClip[] bgsAmbient;

    float audioLengthCurrent;
    int lastTrackHolder_BGS = 0;
    int tempRandomNum;
    bool shouldPlayNextTrack = true;

    private IEnumerator coroutine;

    private void Update()
    {
        if (shouldPlayNextTrack)
        {
            //aquire target
            tempRandomNum = Random.Range(0, bgsAmbient.Length);
            while (tempRandomNum == lastTrackHolder_BGS)
            {
                tempRandomNum = Random.Range(0, bgsAmbient.Length);
            }
            lastTrackHolder_BGS = tempRandomNum;

            //aquire timer length
            audioLengthCurrent = bgsAmbient[lastTrackHolder_BGS].length;
            shouldPlayNextTrack = false;
            if (onAudioSource_A)
            {
                audioSource_A.clip = bgsAmbient[lastTrackHolder_BGS];
                audioSource_A.Play();
                onAudioSource_A = false;
                Debug.Log("On playing A, track is: " + bgsAmbient[lastTrackHolder_BGS].name);
            }
            else
            {
                audioSource_B.clip = bgsAmbient[lastTrackHolder_BGS];
                audioSource_B.Play();
                onAudioSource_A = true;
                Debug.Log("On playing B, track is: " + bgsAmbient[lastTrackHolder_BGS].name);
            }
            coroutine = BgsPlayer(audioLengthCurrent - 3);
            StartCoroutine(coroutine);
        }
    }
    private IEnumerator BgsPlayer(float nextTrackTime)
    {
        yield return new WaitForSeconds(nextTrackTime);
        shouldPlayNextTrack = true;
    }
}
