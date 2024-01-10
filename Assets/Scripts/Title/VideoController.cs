using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;

    void Start()
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true; // 비디오를 무한 반복 재생하도록 설정
        videoPlayer.Prepare();
        videoPlayer.Play();
    }

    void Update()
    {
        rawImage.texture = videoPlayer.texture;
    }
}
