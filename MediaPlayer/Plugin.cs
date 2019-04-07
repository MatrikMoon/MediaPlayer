using CustomUI.MenuButton;
using IllusionPlugin;
using MediaPlayer.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MediaPlayer
{
    public class Plugin : IPlugin
    {
        public string Name => "MediaPlayer";
        public string Version => "0.0.1";

        private MainFlowCoordinator mainFlowCoordinator;
        private MediaPanelFlowCoordinator mediaPanelFlowCoordinator;

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "MenuCore")
            {
                SharedCoroutineStarter.instance.StartCoroutine(SetupUI());
            }
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        //Waits for menu scenes to be loaded then creates UI elements
        //Courtesy of BeatSaverDownloader
        private IEnumerator SetupUI()
        {
            List<Scene> menuScenes = new List<Scene>() { SceneManager.GetSceneByName("MenuCore"), SceneManager.GetSceneByName("MenuViewControllers"), SceneManager.GetSceneByName("MainMenu") };
            yield return new WaitUntil(() => { return menuScenes.All(x => x.isLoaded); });

            if (mainFlowCoordinator == null) mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            if (mediaPanelFlowCoordinator == null) mediaPanelFlowCoordinator = mainFlowCoordinator.gameObject.AddComponent<MediaPanelFlowCoordinator>();

            CreateUI();
        }

        private void CreateUI()
        {
            MenuButtonUI.AddButton("MediaPanel", () => mainFlowCoordinator.PresentFlowCoordinatorOrAskForTutorial(mediaPanelFlowCoordinator));
        }
    }
}
