using HMUI;
using MediaPlayer.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

/**
 * Created by andruzzzhka, from the BeatSaverMultiplayer plugin,
 * modified for the DiscordCommunityPlugin then subsequently trimmed
 * down for the InstantReplayPlugin
 * 
 * Represents and controls a song list
 */

namespace MediaPlayer.UI.ViewControllers
{
    class FileListViewController : VRUIViewController, TableView.IDataSource
    {
        public TableView songsTableView;
        public Action<MediaFile> FileSelected;

        private Button _pageUpButton;
        private Button _pageDownButton;
        
        LevelListTableCell _songTableCellInstance;
        List<MediaFile> availableFiles = new List<MediaFile>();

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            if (firstActivation && type == ActivationType.AddedToHierarchy)
            {
                _songTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));

                _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), rectTransform, false);
                (_pageUpButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 1f);
                (_pageUpButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 1f);
                (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -3f);
                (_pageUpButton.transform as RectTransform).sizeDelta = new Vector2(40f, 6f);
                _pageUpButton.onClick.AddListener(() =>
                {
                    songsTableView.PageScrollUp();
                });
                _pageUpButton.interactable = false;

                _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), rectTransform, false);
                (_pageDownButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0f);
                (_pageDownButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0f);
                (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 8f);
                (_pageDownButton.transform as RectTransform).sizeDelta = new Vector2(40f, 6f);
                _pageDownButton.onClick.AddListener(() =>
                {
                    songsTableView.PageScrollDown();
                });
                _pageDownButton.interactable = false;

                //Courtesy of andruzzzhka's scroll overlap fix
                RectTransform container = new GameObject("Content", typeof(RectTransform)).transform as RectTransform;
                container.SetParent(rectTransform, false);
                container.anchorMin = new Vector2(0.3f, 0.5f);
                container.anchorMax = new Vector2(0.7f, 0.5f);
                container.sizeDelta = new Vector2(0f, 60f);

                songsTableView = new GameObject("CustomTableView").AddComponent<TableView>();
                songsTableView.gameObject.AddComponent<RectMask2D>();
                songsTableView.transform.SetParent(container, false);

                songsTableView.SetField("_isInitialized", false);
                songsTableView.SetField("_preallocatedCells", new TableView.CellsGroup[0]);
                songsTableView.Init();

                (songsTableView.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
                (songsTableView.transform as RectTransform).anchorMax = new Vector2(1f, 1f);
                (songsTableView.transform as RectTransform).sizeDelta = new Vector2(0f, 0f);
                (songsTableView.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f);

                songsTableView.SetField("_pageUpButton", _pageUpButton);
                songsTableView.SetField("_pageDownButton", _pageDownButton);

                songsTableView.didSelectCellWithIdxEvent += SongsTableView_didSelectCellWithIdxEvent;
                songsTableView.dataSource = this;

                //Set to view "Downloading songs..." until the songs are set
                songsTableView.gameObject.SetActive(false);
                _pageUpButton.gameObject.SetActive(false);
                _pageDownButton.gameObject.SetActive(false);
            }
            else
            {
                songsTableView.ReloadData();
            }
        }

        private void SongsTableView_didSelectCellWithIdxEvent(TableView sender, int row)
        {
            FileSelected?.Invoke(availableFiles[row]);
        }

        public void SetFiles(List<MediaFile> files)
        {
            StartCoroutine(SetFilesCoroutine(files));
        }

        private IEnumerator SetFilesCoroutine(List<MediaFile> files)
        {
            yield return new WaitUntil(() => songsTableView != null && _pageUpButton != null && _pageDownButton != null);

            songsTableView.gameObject.SetActive(true);
            _pageUpButton.gameObject.SetActive(true);
            _pageDownButton.gameObject.SetActive(true);

            availableFiles = files;

            if (songsTableView.dataSource != (TableView.IDataSource)this)
            {
                songsTableView.dataSource = this;
            }
            else
            {
                songsTableView.ReloadData();
            }

            songsTableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, false);
        }

        public bool HasSongs()
        {
            return availableFiles.Count > 0;
        }

        public TableCell CellForIdx(int row)
        {
            LevelListTableCell cell = Instantiate(_songTableCellInstance);

            cell.reuseIdentifier = "SongCell";
            cell.GetField<UnityEngine.UI.Image>("_coverImage").sprite = availableFiles[row].CoverImage;
            cell.GetField<TextMeshProUGUI>("_songNameText").text = availableFiles[row].FileName;
            cell.GetField<TextMeshProUGUI>("_authorText").text = "";

            cell.SetField("_beatmapCharacteristicAlphas", new float[0]);
            cell.SetField("_beatmapCharacteristicImages", new UnityEngine.UI.Image[0]);
            cell.SetField("_bought", true);

            return cell;
        }

        public int NumberOfCells()
        {
            return availableFiles.Count;
        }

        public float CellSize()
        {
            return 10f;
        }
    }
}
