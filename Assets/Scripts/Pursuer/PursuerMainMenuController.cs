using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pursuer
{
    public class PursuerMainMenuController : MonoBehaviour
    {
        public Button PlayButton;
        public InputField SeedInputField;
        public Dropdown MapDropdown;

        private void Start()
        {
            populateMapDropdown();
            SeedInputField.onValueChanged.AddListener(setSeed);
            MapDropdown.onValueChanged.AddListener(setMap);
            PlayButton.onClick.AddListener(startGame);
        }

        private void startGame()
        {
            PursuerGameManager.Instance.LoadGameScene();
        }

        void populateMapDropdown()
        {
            var options = new List<Dropdown.OptionData>();

            List<string> types = PursuerGameManager.Instance.GetMapTypes();
            for (int n = 0; n < types.Count; n++)
            {
                options.Add(new Dropdown.OptionData(types[n]));
            }

            MapDropdown.options = options;
        }

        private void setSeed(string seed)
        {
            PursuerGameManager.Instance?.SetSeed(seed);
        }

        private void setMap(int index)
        {
            PursuerGameManager.Instance?.SetMap(index);
        }
    }
}