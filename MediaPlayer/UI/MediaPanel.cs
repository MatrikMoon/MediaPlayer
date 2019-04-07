using MediaPlayer.Misc;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VRUIControls;

/**
 * Created by Moon on 4/4/2019
 * The Panel for the media to play on
 */

namespace MediaPlayer
{
    public class MediaPanel : MonoBehaviour
    {
        private Canvas mainCanvas;
        private RawImage rawImage;
        private VideoPlayer videoPlayer;
        private AudioSource audioSource;
        private VRPointer vrPointer;

        /*
        private GameObject dragger;
        private bool grabbed = false;
        private Vector3 grabbedOrigin;
        */

        private static MediaPanel instance;

        public static MediaPanel Create()
        {
            if (instance == null) instance = new GameObject("Media Panel").AddComponent<MediaPanel>();
            return instance;
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        /*
        protected void Update()
        {
            Logger.Warning($"{vrPointer.vrController.horizontalAxisValue} {vrPointer.vrController.verticalAxisValue}");

            grabbed = false;
            if (vrPointer.vrController != null && vrPointer.vrController.triggerValue > 0.9f || Input.GetMouseButton(0))
            {
                if (Physics.Raycast(vrPointer.vrController.position, vrPointer.vrController.forward, out var hit))
                {
                    //Logger.Warning($"POINTERINFO: {vrPointer.name} {vrPointer.vrController?.node} {vrPointer.cursorPosition}");
                    //Logger.Warning($"CASTING TO TRANSFORM: {hit.transform}");
                    if (hit.transform == dragger.transform)
                    {
                        grabbed = true;
                        Logger.Warning("CASTING TO PROPER");

                        grabbedOrigin = vrPointer.vrController.transform.InverseTransformPoint(dragger.transform.position);
                        var newRot = Quaternion.Inverse(vrPointer.vrController.transform.rotation) * dragger.transform.rotation;

                        //mainCanvas.transform.rotation = newRot;
                    }
                }
            }
        }

        protected void LateUpdate()
        {
            if (grabbed)
            {
                var delta = vrPointer.vrController.verticalAxisValue * Time.unscaledDeltaTime;
                if (grabbedOrigin.magnitude > 0.25f)
                {
                    grabbedOrigin -= Vector3.forward * delta;
                }
                else
                {
                    grabbedOrigin -= Vector3.forward * Mathf.Clamp(delta, float.MinValue, 0);
                }

                Logger.Warning($"GRABBED: {delta} {vrPointer.vrController.verticalAxisValue} {Time.unscaledDeltaTime}");

                dragger.transform.position = vrPointer.vrController.transform.TransformPoint(grabbedOrigin);
            }
        }
        */

        private void Awake()
        {
            Config.LoadConfig();
            gameObject.transform.position = Config.Position;
            gameObject.transform.eulerAngles = Config.Rotation;
            gameObject.transform.localScale = Config.Scale;

            vrPointer = Resources.FindObjectsOfTypeAll<VRPointer>().First();

            /*
            dragger = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dragger.transform.localScale = new Vector3(0.15f, 0.15f, 0.22f);
            dragger.transform.position = Position + new Vector3(0, 1, 0);
            dragger.transform.eulerAngles = Rotation;
            DebugTools.LogComponents(dragger.transform, includeScipts: true);
            DontDestroyOnLoad(dragger);
            */

            mainCanvas = gameObject.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.WorldSpace;
            var canvasTransform = mainCanvas.transform as RectTransform;
            canvasTransform.sizeDelta = Config.Size;

            rawImage = mainCanvas.gameObject.AddComponent<RawImage>();
            var imageTransform = rawImage.transform as RectTransform;
            imageTransform.SetParent(mainCanvas.transform, false);
            rawImage.color = Color.clear;
            rawImage.material = Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault(x => x.name == "UINoGlow");

            audioSource = gameObject.AddComponent<AudioSource>();

            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.transform.SetParent(mainCanvas.transform, false);
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);

            DontDestroyOnLoad(gameObject);
        }

        public void SetMediaSource(string url)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = url;
        }

        public void StartVideo() => SharedCoroutineStarter.instance.StartCoroutine(StartVideoCoroutine());
        IEnumerator StartVideoCoroutine()
        {
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);

            rawImage.color = Config.Color;
            rawImage.texture = videoPlayer.texture;

            videoPlayer.Play();
            audioSource.Play();
        }
    }
}
