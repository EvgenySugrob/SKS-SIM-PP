using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableTesterUIControl : MonoBehaviour
{
    [Header("Main windows")]
    [SerializeField] GameObject loadScreen;
    [SerializeField] GameObject mainWindow;
    [SerializeField] SelectableImage[] selectableImage = new SelectableImage[2];
    private SelectableImage _currentSelectableImage = null;
    private SelectableImage _prevSelectableImage= null;
    private int _currentIndexMainImage = 1;

    [Header("Mapping window")]
    [SerializeField] GameObject mappingWindow;

    [Header("Scan window")]
    [SerializeField] GameObject scanningWindow;
    [SerializeField] RectTransform scanIcon;
    [SerializeField] RectTransform[] scanIconPath = new RectTransform[4];
    private Vector3 _startScanIconPosition;

    private void Start()
    {
        _startScanIconPosition = scanIcon.position;
    }

    public void LeftRightNavigation(int index)
    {
        _currentIndexMainImage += index;

        if(_currentIndexMainImage<0)
        {
            _currentIndexMainImage = selectableImage.Length - 1;
        }
        else if (_currentIndexMainImage>selectableImage.Length-1)
        {
            _currentIndexMainImage = 0;
        }
        SelecetIconBackgroundActive(selectableImage[_currentIndexMainImage]);
    }

    private void SelecetIconBackgroundActive(SelectableImage image)
    {
        _currentSelectableImage = image;

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
}
