using CustomUI.BeatSaber;
using MediaPlayer.Helpers;
using MediaPlayer.UI.ViewControllers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VRUI;

namespace MediaPlayer.UI
{
    class MediaPanelFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator mainFlowCoordinator;
        private GeneralNavigationController mainModNavigationController;
        private FileListViewController fileListViewController;
        private MediaPanel panel;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (activationType == ActivationType.AddedToHierarchy)
            {
                title = "Media Panel";

                if (mainFlowCoordinator == null) mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();

                mainModNavigationController = BeatSaberUI.CreateViewController<GeneralNavigationController>();
                mainModNavigationController.didFinishEvent += (_) => mainFlowCoordinator.InvokeMethod("DismissFlowCoordinator", this, null, false);

                ProvideInitialViewControllers(mainModNavigationController, null, null);
                OpenFileList();
            }
        }

        private void OpenFileList()
        {
            if (fileListViewController == null) fileListViewController = BeatSaberUI.CreateViewController<FileListViewController>();
            if (panel == null) panel = MediaPanel.Create();
            if (mainModNavigationController.GetField<List<VRUIViewController>>("_viewControllers").IndexOf(fileListViewController) < 0)
            {
                SetViewControllersToNavigationConctroller(mainModNavigationController, new VRUIViewController[] { fileListViewController });

                fileListViewController.SetFiles(LoadMediaFiles());
                fileListViewController.FileSelected += (file) => {
                    panel.SetMediaSource(file.Path);
                    panel.StartVideo();
                };
            }
        }

        private List<MediaFile> LoadMediaFiles()
        {
            List<MediaFile> ret = new List<MediaFile>();
            var directory = Directory.CreateDirectory("./MediaPanel/");
            foreach (var file in directory.GetFiles())
            {
                ret.Add(MediaFile.LoadFromFile(file));
            }
            return ret.ToList();
        }
    }
}
