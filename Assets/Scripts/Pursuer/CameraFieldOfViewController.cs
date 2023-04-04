using System;
using UnityEngine;

namespace Pursuer
{
    [RequireComponent(typeof(Camera))]
    public class CameraFieldOfViewController : MonoBehaviour
    {
        Camera camera;
        private void Start()
        {
            camera = GetComponent<Camera>();
            camera.fieldOfView = PursuerGameManager.Instance.FieldOfViewSetting.Value;
            PursuerGameManager.Instance.FieldOfViewSetting.OnChanged += OnFieldOfViewSettingChanged;
        }

        private void OnFieldOfViewSettingChanged(object sender, GameSettingChangedEventArgs e)
        {
            camera.fieldOfView = e.FinalValue;
        }
    }
}
