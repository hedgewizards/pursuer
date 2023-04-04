using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pursuer
{
    public class PursuerPauseMenuController : MonoBehaviour
    {
        public GameObject PauseScreen;

        public Button QuitButton;
        public Button ResumeButton;

        bool gameOver = false;

        private void Start()
        {
            PursuerGameManager.Instance.OnPauseStateChanged += setPaused;
            PursuerGameManager.Instance.OnGameOver += onGameOver;
            SensitivitySlider?.onValueChanged.AddListener(onSensitivitySliderChanged);
            FOVSlider?.onValueChanged.AddListener(onFOVChanged);
            ResumeButton?.onClick.AddListener(onResumeClicked);
            QuitButton?.onClick.AddListener(onQuitClicked);
        }
        private void setPaused(object sender, bool nowPaused)
        {
            if (gameOver) return;

            PauseScreen?.SetActive(nowPaused);
            QuitButton?.gameObject.SetActive(PursuerGameManager.Instance?.InGame ?? false);

            if (nowPaused)
            {
                fov = PursuerGameManager.Instance.FieldOfViewSetting.Value;
                FOVSlider.value = fov;
                sensitivity = PursuerGameManager.Instance.SensitivitySetting.Value;
                SensitivitySlider.value = sensitivity * SENSITIVITY_SLIDER_CONVERSION;
            }
        }
        private void onGameOver(object sender, bool e)
        {
            setPaused(this, false);
            gameOver = true;
        }


        private void onResumeClicked()
        {
            PursuerGameManager.Paused = false;
        }

        private void onQuitClicked()
        {
            PursuerGameManager.Instance?.LoadMenuScene();
        }


        #region FOV

        float fov;
        float FOV
        {
            get => fov;
            set
            {
                PursuerGameManager.Instance.FieldOfViewSetting.Value = value;
                fov = value;
            }
        }


        public Slider FOVSlider;

        private void onFOVChanged(float displayValue)
        {
            FOV = displayValue;
        }
        #endregion

        #region Sensitivity
        const float SENSITIVITY_SLIDER_CONVERSION = 100;

        const float SENSITIVITY_MINIMUM = 0.01f;
        const float SENSITIVITY_MAXIMUM = 2f;

        float sensitivity;
        float Sensitivity
        {
            get => sensitivity;
            set
            {
                sensitivity = Mathf.Clamp(value, SENSITIVITY_MINIMUM, SENSITIVITY_MAXIMUM);
                PursuerGameManager.Instance.SensitivitySetting.Value = sensitivity;
            }
        }

        public Text SensitivityDisplayLabel;

        public Slider SensitivitySlider;

        private void onSensitivitySliderChanged(float displayValue)
        {
            Sensitivity = displayValue / SENSITIVITY_SLIDER_CONVERSION;

            SensitivityDisplayLabel.text = Sensitivity.ToString("N2");
        }
        #endregion
    }

}
