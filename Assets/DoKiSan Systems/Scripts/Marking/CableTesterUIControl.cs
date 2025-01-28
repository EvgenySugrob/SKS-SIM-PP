using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableTesterUIControl : MonoBehaviour
{
    [Header("Main windows")]
    [SerializeField] CableTestChecker cableTestChecker;
    [SerializeField] GameObject loadScreen;
    [SerializeField] GameObject mainWindow;
    [SerializeField] SelectableImage[] selectableImage = new SelectableImage[2];
    private SelectableImage _currentSelectableImage = null;
    private SelectableImage _prevSelectableImage = null;
    private int _currentIndexMainImage = 1;

    [Header("Current and prev window show")]
    [SerializeField] GameObject _currentWindowShow;
    [SerializeField] GameObject _prevWindowShow;
    [SerializeField]private List<SelectableImage> _currentImageGroup = new List<SelectableImage>();
    [SerializeField]private List<SelectableImage> _prevImageGroup = new List<SelectableImage>();
    [SerializeField]private ICheckingMenu _checkingMenu;

    [Header("Mapping Menu")]
    [SerializeField] SelectableImage[] mappingSelectableImage= new SelectableImage[2];
    [SerializeField] GameObject testingWindow;
    [SerializeField] GameObject resultWindow;
    [SerializeField] GameObject resultError;
    [SerializeField] GameObject resultDone;
    [SerializeField] UILineRenderConection lineRenderConection;
    

    private void Start()
    {
        //_startScanIconPosition = scanIcon.position;
        _currentImageGroup.Clear();
        _currentWindowShow = mainWindow;
        _prevWindowShow = _currentWindowShow;

        for (int i = 0; i < selectableImage.Length; i++)
        {
            _currentImageGroup.Add(selectableImage[i]);
        }
    }

    public void SetCurrentImageGroup(SelectableImage[] images)
    {
        //Проверить заполнения листов 
        if(_prevImageGroup == null)
        {
            _prevImageGroup = _currentImageGroup;
        }

        if(_prevImageGroup!=_currentImageGroup)
        {
            _prevImageGroup.Clear();
            _prevImageGroup = _currentImageGroup;
        }

        _currentImageGroup.Clear();

        for (int i = 0; i < images.Length; i++)
        {
            _currentImageGroup.Add(images[i]);
        }
    }

    public void LeftRightNavigation(int index)
    {
        _currentIndexMainImage += index;

        if(_currentIndexMainImage<0)
        {
            _currentIndexMainImage = _currentImageGroup.Count - 1;
        }
        else if (_currentIndexMainImage>_currentImageGroup.Count-1)
        {
            _currentIndexMainImage = 0;
        }
        SelecetIconBackgroundActive(_currentImageGroup[_currentIndexMainImage]);
    }

    public void ConfirmCurrentSelectebleImage()
    {
        _checkingMenu.SetSelfCheckMenu();
    }

    public void OpenWindowSelectebleWindow(GameObject selectableMenu, string nameMenu)
    {
        if (selectableMenu == null)
            return;

        bool dontOpenNextWindow = false;

        switch (nameMenu)
        {
            case "mapping":
                if(cableTestChecker.GetIsMarkingProgress())
                {
                    dontOpenNextWindow = true;
                    break;
                }
                else
                {
                    StartCoroutine(WaitLoadTestingScreen());
                }
                
                //SetCurrentImageGroup(mappingSelectableImage);
                break;

            case "scan":
                if (cableTestChecker.GetIsNfrInSocket() || cableTestChecker.GetIsSearchSocketTermimnation())
                {
                    dontOpenNextWindow = true;
                    break;
                } 
                ScanWorkProgress();

                break;
            default: 
                break;
        }

        if(!dontOpenNextWindow)
        {
            _prevWindowShow = _currentWindowShow;
            _prevWindowShow.SetActive(false);
            _currentWindowShow = selectableMenu;
            _currentWindowShow.SetActive(true);
        }
    }

    public void OpenMainMenuAfterWork()
    {
        _prevWindowShow = _currentWindowShow;
        _prevWindowShow.SetActive(false);
        _currentWindowShow = mainWindow;
        _currentWindowShow.SetActive(true);
    }

    public void ReturnMenuBtClick()
    {
        cableTestChecker.ActiveReturnButtons(false);

        resultError.SetActive(false);
        resultWindow.SetActive(false);
        resultDone.SetActive(false);
        testingWindow.SetActive(true);

        _currentWindowShow = mainWindow;
        _currentWindowShow.SetActive(true);

        cableTestChecker.ReturnWorkPortsMode();
    }

    private void ScanWorkProgress()
    {
        cableTestChecker.StartSearch();
    }

    private void SelecetIconBackgroundActive(SelectableImage image)
    {
        _currentSelectableImage = image;
        _checkingMenu = image.GetComponent<ICheckingMenu>();

        if(_prevSelectableImage == null)
        {
            _prevSelectableImage = _currentSelectableImage;
        }
        if(_prevSelectableImage!=_currentSelectableImage)
        {
            _prevSelectableImage.FadeOutIcon();
            _prevSelectableImage = _currentSelectableImage;
        }

        if(_currentSelectableImage!=null)
        {
            _currentSelectableImage.FadeInIcon();
        }
    }

    private IEnumerator WaitLoadTestingScreen()
    {
        cableTestChecker.ActiveButtonsOnTool(false);
        PortConnectInfo port = cableTestChecker.GetCurrentPort();

        yield return new WaitForSeconds(2f);

        if (port.IsConnectingPair())
        {
            lineRenderConection.SetPortsInfo(port.GetArrayContact());

            resultDone.SetActive(true);
            resultWindow.SetActive(true);
            testingWindow.SetActive(false);
        }
        else
        {
            resultError.SetActive(true);
            resultWindow.SetActive(true);
            testingWindow.SetActive(false);
        }

        cableTestChecker.ActiveReturnButtons(true);
    }

    public void ForceReturnBtClick()
    {
        resultError.SetActive(false);
        resultWindow.SetActive(false);
        resultDone.SetActive(false);
        testingWindow.SetActive(true);

        _currentWindowShow = mainWindow;
        _currentWindowShow.SetActive(true);
    }
}
