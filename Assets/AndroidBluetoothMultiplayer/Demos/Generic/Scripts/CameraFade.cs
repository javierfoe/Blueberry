using System;
using UnityEngine;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples {
    public class CameraFade : MonoBehaviour {
        public GUIStyle BackgroundStyle = new GUIStyle(); // Style for background tiling
        public Texture2D FadeTexture; // 1x1 pixel texture used for fading
        public Color CurrentScreenOverlayColor = new Color(0, 0, 0, 0); // default starting color: black and fully transparrent
        public Color TargetScreenOverlayColor = new Color(0, 0, 0, 0); // default target color: black and fully transparrent
        public Color DeltaColor = new Color(0, 0, 0, 0); // the delta-color is basically the "speed / second" at which the current color should change
        public int FadeGUIDepth = -1000; // make sure this texture is drawn on top of everything

        public float FadeDelay = 0;
        public Action OnFadeFinish = null;

        private static CameraFade _instance;

        private static CameraFade Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType(typeof(CameraFade)) as CameraFade ??
                                new GameObject("CameraFade").AddComponent<CameraFade>();
                }

                return _instance;
            }
        }

        private void Awake() {
            if (_instance == null) {
                _instance = this;
                _instance.Init();
            }
        }

        // Initialize the texture, background-style and initial color:
        private void Init() {
            Instance.FadeTexture = new Texture2D(1, 1);
            Instance.BackgroundStyle.normal.background = Instance.FadeTexture;
        }

        // Draw the texture and perform the fade:
        private void OnGUI() {
            // If delay is over...
            if (Time.time > Instance.FadeDelay) {
                // If the current color of the screen is not equal to the desired color: keep fading!
                if (Instance.CurrentScreenOverlayColor != Instance.TargetScreenOverlayColor) {
                    // If the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
                    if (Mathf.Abs(Instance.CurrentScreenOverlayColor.a - Instance.TargetScreenOverlayColor.a) < Mathf.Abs(Instance.DeltaColor.a) * Time.deltaTime) {
                        Instance.CurrentScreenOverlayColor = Instance.TargetScreenOverlayColor;
                        SetScreenOverlayColor(Instance.CurrentScreenOverlayColor);
                        Instance.DeltaColor = new Color(0, 0, 0, 0);

                        if (Instance.OnFadeFinish != null) {
                            Instance.OnFadeFinish();
                        }

                        Die();
                    } else {
                        // Fade!
                        SetScreenOverlayColor(Instance.CurrentScreenOverlayColor + Instance.DeltaColor * Time.deltaTime);
                    }
                }
            }
            // Only draw the texture when the alpha value is greater than 0:
            if (CurrentScreenOverlayColor.a > 0) {
                GUI.depth = Instance.FadeGUIDepth;
                GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), Instance.FadeTexture, Instance.BackgroundStyle);
            }
        }

        /// <summary>
        /// Sets the color of the screen overlay instantly.  Useful to start a fade.
        /// </summary>
        /// <param name='newScreenOverlayColor'>
        /// New screen overlay color.
        /// </param>
        private static void SetScreenOverlayColor(Color newScreenOverlayColor) {
            Instance.CurrentScreenOverlayColor = newScreenOverlayColor;
            Instance.FadeTexture.SetPixel(0, 0, Instance.CurrentScreenOverlayColor);
            Instance.FadeTexture.Apply();
        }

        /// <summary>
        /// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent.
        /// </summary>
        /// <param name='newScreenOverlayColor'>
        /// Target screen overlay Color.
        /// </param>
        /// <param name='isFadeIn'>
        /// Whether to fade in or out.
        /// </param>
        /// <param name='fadeDuration'>
        /// Fade duration.
        /// </param>
        public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration) {
            if (fadeDuration <= 0.0f) {
                SetScreenOverlayColor(newScreenOverlayColor);
            } else {
                if (isFadeIn) {
                    Instance.TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0);
                    SetScreenOverlayColor(newScreenOverlayColor);
                } else {
                    Instance.TargetScreenOverlayColor = newScreenOverlayColor;
                    SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0));
                }

                Instance.DeltaColor = (Instance.TargetScreenOverlayColor - Instance.CurrentScreenOverlayColor) / fadeDuration;
            }
        }

        /// <summary>
        /// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent, after a delay.
        /// </summary>
        /// <param name='newScreenOverlayColor'>
        /// New screen overlay color.
        /// </param>
        /// <param name='fadeDuration'>
        /// Fade duration.
        /// </param>
        /// <param name='isFadeIn'>
        /// Whether to fade in or out.
        /// </param>
        /// <param name='fadeDelay'>
        /// Fade delay.
        /// </param>
        public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay) {
            if (fadeDuration <= 0.0f) {
                SetScreenOverlayColor(newScreenOverlayColor);
            } else {
                Instance.FadeDelay = Time.time + fadeDelay;

                if (isFadeIn) {
                    Instance.TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0);
                    SetScreenOverlayColor(newScreenOverlayColor);
                } else {
                    Instance.TargetScreenOverlayColor = newScreenOverlayColor;
                    SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0));
                }

                Instance.DeltaColor = (Instance.TargetScreenOverlayColor - Instance.CurrentScreenOverlayColor) / fadeDuration;
            }
        }

        /// <summary>
        /// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent, after a delay, with Action OnFadeFinish.
        /// </summary>
        /// <param name='newScreenOverlayColor'>
        /// New screen overlay color.
        /// </param>
        /// <param name='isFadeIn'>
        /// Whether to fade in or out.
        /// </param>
        /// <param name='fadeDuration'>
        /// Fade duration.
        /// </param>
        /// <param name='fadeDelay'>
        /// Fade delay.
        /// </param>
        /// <param name='OnFadeFinish'>
        /// On fade finish, doWork().
        /// </param>
        public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay, Action OnFadeFinish) {
            if (fadeDuration <= 0.0f) {
                SetScreenOverlayColor(newScreenOverlayColor);
            } else {
                Instance.OnFadeFinish = OnFadeFinish;
                Instance.FadeDelay = Time.time + fadeDelay;

                if (isFadeIn) {
                    Instance.TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0);
                    SetScreenOverlayColor(newScreenOverlayColor);
                } else {
                    Instance.TargetScreenOverlayColor = newScreenOverlayColor;
                    SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0));
                }
                Instance.DeltaColor = (Instance.TargetScreenOverlayColor - Instance.CurrentScreenOverlayColor) / fadeDuration;
            }
        }

        private void Die() {
            _instance = null;
            Destroy(gameObject);
        }

        private void OnApplicationQuit() {
            _instance = null;
        }
    }
}