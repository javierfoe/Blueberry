using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace javierfoe.Blueberry.Examples.MultipleAdditiveScenes
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : Examples.PlayerController
    {
        protected override void OnDisable()
        {
            if (isLocalPlayer && Camera.main != null)
            {
                Camera.main.orthographic = true;
                Camera.main.transform.SetParent(null);
                SceneManager.MoveGameObjectToScene(Camera.main.gameObject, SceneManager.GetActiveScene());
                Camera.main.transform.localPosition = new Vector3(0f, 70f, 0f);
                Camera.main.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }
        }
    }
}
