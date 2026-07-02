using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelSelectManager : MonoBehaviour
{
    public Transform LevelParent;
    public GameObject LevelButtonPrefab;
    public LineRenderer LinePrefab;
    public TextMeshProUGUI AreaHeaderText;
    public TextMeshProUGUI LevelHeaderText;
    public AreaData CurrentArea;

    [Header("Player References")]
    public GameObject PlayerUIPrefab;
    public RectTransform WorldSpaceCanvasRect;
    public Vector2 PlayerPositionOffsetPerLevel = new Vector2(0.02f, -0.5f);

    [Header("Level Detail")]
    public LevelDetailPanel DetailPanel;

    public HashSet<string> UnlockedLevelIDs = new HashSet<string>();

    private LevelSelectEventSystemHandler _eventSystemHandler;

    private Camera _camera;

    private List<GameObject> _buttonObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> _buttonPositions = new Dictionary<GameObject, Vector3>();

    public GameObject PlayerObj { get; set; }
    private bool _playerIsFacingRight;

    private void Awake()
    {
        _camera = Camera.main;
        _eventSystemHandler = GetComponentInChildren<LevelSelectEventSystemHandler>(true);
    }

    private void Start()
    {
        AssignAreaText();
        LoadUnlockedLevels();
        CreateLevelButtons();
    }

    private void AssignAreaText()
    {
        AreaHeaderText.SetText(CurrentArea.AreaName);
    }

    private void LoadUnlockedLevels()
    {
        foreach (var level in CurrentArea.Levels)
        {
            if (level.IsUnlockedByDefault)
            {
                Debug.Log("added defaultLevel");
                UnlockedLevelIDs.Add(level.LevelID);
                GameManager.Instance.AddUnlockedLevel(level);
            }
        }

        foreach (var item in GameManager.Instance.UnlockedLevel)
        {
            if (!UnlockedLevelIDs.Contains(item.LevelID))
                UnlockedLevelIDs.Add(item.LevelID);
        }
    }

    private void CreateLevelButtons()
    {
        for (int i = 0; i < CurrentArea.Levels.Count; i++)
        {
            GameObject buttonGO = Instantiate(LevelButtonPrefab, LevelParent);
            _buttonObjects.Add(buttonGO);

            RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();

            buttonGO.name = CurrentArea.Levels[i].LevelID;
            CurrentArea.Levels[i].LevelButtonObj = buttonGO;

            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();
            levelButton.Setup(CurrentArea.Levels[i], UnlockedLevelIDs.Contains(CurrentArea.Levels[i].LevelID), DetailPanel);

            // populate the selectables for the event system
            Selectable selectable = buttonGO.GetComponent<Selectable>();
            _eventSystemHandler.AddSelectable(selectable);

            StartCoroutine(AddLocationAfterDelay(buttonGO, buttonRect));

            if (i > 0)
            {
                LineRenderer line = Instantiate(LinePrefab, LevelParent);

                line.transform.SetSiblingIndex(0);
                LineRendererConnector lineConnector = line.GetComponent<LineRendererConnector>();
                lineConnector.StartRectTrans = CurrentArea.Levels[i - 1].LevelButtonObj.GetComponent<RectTransform>();
                lineConnector.EndRectTrans = buttonGO.GetComponent<RectTransform>();

                StartCoroutine(DelayedLineSetup(lineConnector));
            }
            else
            {
                StartCoroutine(SpawnPlayerAfterDelay(buttonRect, WorldSpaceCanvasRect));
            }
        }

        StartCoroutine(SetupButtonNavigation());

        LevelParent.gameObject.SetActive(true);
        _eventSystemHandler.InitSelectables();
        _eventSystemHandler.SetFirstSelected();
    }

    private IEnumerator DelayedLineSetup(LineRendererConnector lineConnector)
    {
        yield return null;
        lineConnector.UpdateLinePosition();
    }

    private IEnumerator AddLocationAfterDelay(GameObject buttonGo, RectTransform buttonRect)
    {
        yield return null;

        Vector2 buttonScreenPoint = RectTransformUtility.WorldToScreenPoint(_camera, buttonRect.position);
        Vector3 buttonWorldPos = _camera.ScreenToWorldPoint(new Vector3(buttonScreenPoint.x, buttonScreenPoint.y, _camera.nearClipPlane));
        buttonWorldPos.z = 0f;

        _buttonPositions.Add(buttonGo, buttonWorldPos);
    }

    #region Navigation
    private IEnumerator SetupButtonNavigation()
    {
        yield return null;

        for (int i = 0; i < _buttonObjects.Count; i++)
        {
            GameObject currentButton = _buttonObjects[i];
            Vector3 currentPos = _buttonPositions[currentButton];
            Selectable currentSelectable = currentButton.GetComponent<Selectable>();
            Navigation nav = new Navigation { mode = Navigation.Mode.Explicit };

            // check if previous button exists
            if (i > 0 && UnlockedLevelIDs.Contains(CurrentArea.Levels[i].LevelID))
            {
                GameObject prevButton = _buttonObjects[i - 1];
                Vector3 prevPos = _buttonPositions[prevButton];
                Vector3 dirToPrev = (prevPos - currentPos).normalized;

                if (Vector3.Dot(dirToPrev, Vector3.right) > 0.7f)
                    nav.selectOnRight = prevButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToPrev, Vector3.left) > 0.7f)
                    nav.selectOnLeft = prevButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToPrev, Vector3.up) > 0.7f)
                    nav.selectOnUp = prevButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToPrev, Vector3.down) > 0.7f)
                    nav.selectOnDown = prevButton.GetComponent<Selectable>();
            }

            // check if future button exists
            if (i < _buttonObjects.Count - 1 && UnlockedLevelIDs.Contains(CurrentArea.Levels[i + 1].LevelID))
            {
                GameObject nextButton = _buttonObjects[i + 1];
                Vector3 nextPos = _buttonPositions[nextButton];
                Vector3 dirToNext = (nextPos - currentPos).normalized;

                if (Vector3.Dot(dirToNext, Vector3.right) > 0.7f)
                    nav.selectOnRight = nextButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToNext, Vector3.left) > 0.7f)
                    nav.selectOnLeft = nextButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToNext, Vector3.up) > 0.7f)
                    nav.selectOnUp = nextButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToNext, Vector3.down) > 0.7f)
                    nav.selectOnDown = nextButton.GetComponent<Selectable>();
            }

            currentSelectable.navigation = nav;
        }
    }
    #endregion

    #region Helper methods
    public void UnlockLevel(string levelID, LevelButton levelButton)
    {
        UnlockedLevelIDs.Add(levelID);
        levelButton.Unlock();

        StartCoroutine(SetupButtonNavigation());
    }

    [ContextMenu("Test Level Unlock")]
    public void UnlockLevelTwoExample()
    {
        LevelButton levelButton = _buttonObjects[1].GetComponent<LevelButton>();
        string levelToUnlock = levelButton.LevelData.LevelID;

        UnlockLevel(levelToUnlock, levelButton);
    }
    #endregion

    // #region Player
    // private IEnumerator SpawnPlayerAfterDelay(RectTransform screenSpaceUIObject, RectTransform worldSpaceUIObject)
    // {
    //     yield return null;
    //     SpawnInPlayer(screenSpaceUIObject, worldSpaceUIObject);
    // }

    // private void SpawnInPlayer(RectTransform screenSpaceUIObject, RectTransform worldSpaceUIObject)
    // {
    //     _playerIsFacingRight = true;
    //     PlayerObj = Instantiate(PlayerUIPrefab, worldSpaceUIObject);

    //     Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, screenSpaceUIObject.position);
    //     Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint   .y, _camera.nearClipPlane));
    //     worldPosition.z = worldSpaceUIObject.position.z;

    //     Vector3 offsetPosition = worldPosition + (Vector3)PlayerPositionOffsetPerLevel;

    //     PlayerObj.transform.position = offsetPosition;

    //     if (_buttonObjects.Count > 1)
    //     {
    //         Vector2 secondScreenPoint = RectTransformUtility.WorldToScreenPoint(_camera, _buttonObjects[1].GetComponent<RectTransform>().position);
    //         Vector3 secondWorldPosition = _camera.ScreenToWorldPoint(new Vector3(secondScreenPoint.x, secondScreenPoint.y, _camera.nearClipPlane));
    //         secondWorldPosition.z = worldSpaceUIObject.position.z;

    //         CheckForRightOrLeftTurn(PlayerObj, ref _playerIsFacingRight, secondWorldPosition);
    //     }
    // }

    // private void CheckForRightOrLeftTurn(GameObject player, ref bool isFacingRight, Vector3 targetWorldPosition)
    // {
    //     if (isFacingRight)
    //     {
    //         if (targetWorldPosition.x < player.transform.position.x)
    //         {
    //             player.transform.Rotate(0f, 180f, 0f);
    //             isFacingRight = false;
    //         }
    //     }
    //     else
    //     {
    //         if (targetWorldPosition.x > player.transform.position.x)
    //         {
    //             player.transform.Rotate(0f, -180f, 0f);
    //             isFacingRight = true;
    //         }
    //     }
    // }

    // public void MovePlayerToButton(GameObject playerUI, RectTransform targetButton, RectTransform worldSpaceUIObject)
    // {
    //     Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, targetButton.position);
    //     Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, _camera.nearClipPlane));
    //     worldPosition.z = worldSpaceUIObject.position.z;

    //     Vector3 endPosition = worldPosition + (Vector3)PlayerPositionOffsetPerLevel;    

    //     CheckForRightOrLeftTurn(playerUI, ref _playerIsFacingRight, worldPosition);

    //     playerUI.transform.DOMove(endPosition, 0.11f);
    // }
    // #endregion

    #region Player
    private IEnumerator SpawnPlayerAfterDelay(RectTransform screenSpaceUIObject, RectTransform worldSpaceUIObject)
    {
        yield return new WaitForEndOfFrame();
        SpawnInPlayer(screenSpaceUIObject, worldSpaceUIObject);
    }

    private Vector3 UIToWorldPosition(RectTransform screenSpaceRect, RectTransform worldSpaceCanvasRect)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, screenSpaceRect.position);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(worldSpaceCanvasRect, screenPoint, _camera, out Vector3 worldPoint);
        return worldPoint;
    }

    private void SpawnInPlayer(RectTransform screenSpaceUIObject, RectTransform worldSpaceUIObject)
    {
        _playerIsFacingRight = true;
        PlayerObj = Instantiate(PlayerUIPrefab, worldSpaceUIObject);

        Vector3 worldPosition = UIToWorldPosition(screenSpaceUIObject, worldSpaceUIObject);

        Vector3 offsetPosition = worldPosition + new Vector3(
            PlayerPositionOffsetPerLevel.x,
            PlayerPositionOffsetPerLevel.y,
            0f
        );
        PlayerObj.transform.position = offsetPosition;

        if (_buttonObjects.Count > 1)
        {
            Vector3 secondWorldPosition = UIToWorldPosition(_buttonObjects[1].GetComponent<RectTransform>(), worldSpaceUIObject);
            CheckForRightOrLeftTurn(PlayerObj, ref _playerIsFacingRight, secondWorldPosition);
        }
    }

    private void CheckForRightOrLeftTurn(GameObject player, ref bool isFacingRight, Vector3 targetWorldPosition)
    {
        if (isFacingRight)
        {
            if (targetWorldPosition.x < player.transform.position.x)
            {
                player.transform.Rotate(0f, 180f, 0f);
                isFacingRight = false;
            }
        }
        else
        {
            if (targetWorldPosition.x > player.transform.position.x)
            {
                player.transform.Rotate(0f, -180f, 0f);
                isFacingRight = true;
            }
        }
    }

    public void MovePlayerToButton(GameObject playerUI, RectTransform targetButton, RectTransform worldSpaceUIObject)
    {
        Vector3 worldPosition = UIToWorldPosition(targetButton, worldSpaceUIObject);

        Vector3 endPosition = worldPosition + new Vector3(
            PlayerPositionOffsetPerLevel.x,
            PlayerPositionOffsetPerLevel.y,
            0f
        );

        CheckForRightOrLeftTurn(playerUI, ref _playerIsFacingRight, worldPosition);
        playerUI.transform.DOMove(endPosition, 0.11f);
    }
    #endregion
}