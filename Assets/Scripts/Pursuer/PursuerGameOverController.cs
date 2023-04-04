using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pursuer
{
    public class PursuerGameOverController : MonoBehaviour
    {
        public UnityEvent OnLoseGame;
        public UnityEvent OnWinGame;

        public Button[] ReturnButtons;

        private void Start()
        {
            PursuerGameManager.Instance.OnGameOver += onGameOver;

            foreach(Button b in ReturnButtons)
            {
                b.onClick.AddListener(onReturnClicked);
            }
        }

        private void onReturnClicked()
        {
            PursuerGameManager.Instance?.LoadMenuScene();
        }

        private void onGameOver(object sender, bool e)
        {
            if (e)
            {
                OnWinGame?.Invoke();
            }
            else
            {
                OnLoseGame?.Invoke();
            }
        }
    }
}
