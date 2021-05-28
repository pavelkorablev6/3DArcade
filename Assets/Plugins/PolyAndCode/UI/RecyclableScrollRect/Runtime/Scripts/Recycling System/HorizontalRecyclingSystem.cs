//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyAndCode.UI
{
    /// <summary>
    /// Recyling system for horizontal type.
    /// </summary>
    public class HorizontalRecyclingSystem : RecyclingSystem
    {
        //Assigned by constructor
        private readonly int _rows;

        //Cell dimensions
        private float _cellWidth, _cellHeight;

        //Pool Generation
        private List<RectTransform> _cellPool;
        private List<ICell> _cachedCells;
        private Bounds _recyclableViewBounds;

        //Temps, Flags
        private readonly Vector3[] _corners = new Vector3[4];
        private bool _recycling;

        //Trackers
        private int _currentItemCount; //item count corresponding to the datasource.
        private int _leftMostCellIndex, _rightMostCellIndex; //Topmost and bottommost cell in the List
        private int _leftMostCellRow, _rightMostCellRow; // used for recyling in Grid layout. leftmost and rightmost row

        //Cached zero vector
        private Vector2 _zeroVector = Vector2.zero;
        #region INIT
        public HorizontalRecyclingSystem(RectTransform prototypeCell, RectTransform viewport, RectTransform content, IRecyclableScrollRectDataSource dataSource, bool isGrid, int rows)
        {
            _prototypeCell = prototypeCell;
            _viewport = viewport;
            _content = content;
            DataSource = dataSource;
            _isGrid = isGrid;
            _rows = isGrid ? rows : 1;
            _recyclableViewBounds = new Bounds();
        }

        /// <summary>
        /// Corotuine for initiazation.
        /// Using coroutine for init because few UI stuff requires a frame to update
        /// </summary>
        /// <param name="onInitialized">callback when init done</param>
        /// <returns></returns>
        public override IEnumerator InitCoroutine(Action onInitialized)
        {
            //Setting up container and bounds
            SetLeftAnchor(_content);
            _content.anchoredPosition = Vector3.zero;
            yield return null;
            SetRecyclingBounds();

            //Cell Poool
            CreateCellPool();
            _currentItemCount = _cellPool.Count;
            _leftMostCellIndex = 0;
            _rightMostCellIndex = _cellPool.Count - 1;

            //Set content width according to no of coloums
            int coloums = Mathf.CeilToInt((float)_cellPool.Count / _rows);
            float contentXSize = coloums * _cellWidth;
            _content.sizeDelta = new Vector2(contentXSize, _content.sizeDelta.y);
            SetLeftAnchor(_content);

            onInitialized?.Invoke();
        }

        /// <summary>
        /// Sets the uppper and lower bounds for recycling cells.
        /// </summary>
        private void SetRecyclingBounds()
        {
            _viewport.GetWorldCorners(_corners);
            float threshHold = _recyclingThreshold * (_corners[2].x - _corners[0].x);
            _recyclableViewBounds.min = new Vector3(_corners[0].x - threshHold, _corners[0].y);
            _recyclableViewBounds.max = new Vector3(_corners[2].x + threshHold, _corners[2].y);
        }

        /// <summary>
        /// Creates cell Pool for recycling, Caches ICells
        /// </summary>
        private void CreateCellPool()
        {
            //Reseting Pool
            if (_cellPool != null)
            {
                _cellPool.ForEach((RectTransform item) => UnityEngine.Object.Destroy(item.gameObject));
                _cellPool.Clear();
                _cachedCells.Clear();
            }
            else
            {
                _cachedCells = new List<ICell>();
                _cellPool = new List<RectTransform>();
            }

            //Set the prototype cell active and set cell anchor as top
            _prototypeCell.gameObject.SetActive(true);
            SetLeftAnchor(_prototypeCell);

            //set new cell size according to its aspect ratio
            _cellHeight = _content.rect.height / _rows;
            _cellWidth = _prototypeCell.sizeDelta.x / _prototypeCell.sizeDelta.y * _cellHeight;

            //Reset
            _leftMostCellRow = _rightMostCellRow = 0;

            //Temps
            float currentPoolCoverage = 0;
            int poolSize = 0;
            float posX = 0;
            float posY = 0;

            //Get the required pool coverage and mininum size for the Cell pool
            float requriedCoverage = _minPoolCoverage * _viewport.rect.width;
            int minPoolSize = Math.Min(_minPoolSize, DataSource.GetItemCount());

            //create cells untill the Pool area is covered and pool size is the minimum required
            while ((poolSize < minPoolSize || currentPoolCoverage < requriedCoverage) && poolSize < DataSource.GetItemCount())
            {
                //Instantiate and add to Pool
                RectTransform item = UnityEngine.Object.Instantiate(_prototypeCell.gameObject).GetComponent<RectTransform>();
                item.name = "Cell";
                item.sizeDelta = new Vector2(_cellWidth, _cellHeight);
                _cellPool.Add(item);
                item.SetParent(_content, false);

                if (_isGrid)
                {
                    posY = -_rightMostCellRow * _cellHeight;
                    item.anchoredPosition = new Vector2(posX, posY);
                    if (++_rightMostCellRow >= _rows)
                    {
                        _rightMostCellRow = 0;
                        posX += _cellWidth;
                        currentPoolCoverage += item.rect.width;
                    }
                }
                else
                {
                    item.anchoredPosition = new Vector2(posX, 0);
                    posX = item.anchoredPosition.x + item.rect.width;
                    currentPoolCoverage += item.rect.width;
                }

                //Setting data for Cell
                _cachedCells.Add(item.GetComponent<ICell>());
                DataSource.SetCell(_cachedCells[_cachedCells.Count - 1], poolSize);

                //Update the Pool size
                poolSize++;
            }

            if (_isGrid)
            {
                _rightMostCellRow = (_rightMostCellRow - 1 + _rows) % _rows;
            }

            //Deactivate prototype cell if it is not a prefab(i.e it's present in scene)
            if (_prototypeCell.gameObject.scene.IsValid())
                _prototypeCell.gameObject.SetActive(false);
        }
        #endregion

        #region RECYCLING
        /// <summary>
        /// Recyling entry point
        /// </summary>
        /// <param name="direction">scroll direction </param>
        /// <returns></returns>
        public override Vector2 OnValueChangedListener(Vector2 direction)
        {
            if (_recycling || _cellPool == null || _cellPool.Count == 0)
                return _zeroVector;

            //Updating Recyclable view bounds since it can change with resolution changes.
            SetRecyclingBounds();

            if (direction.x < 0 && _cellPool[_rightMostCellIndex].MinX() < _recyclableViewBounds.max.x)
                return RecycleLeftToRight();
            else if (direction.x > 0 && _cellPool[_leftMostCellIndex].MaxX() > _recyclableViewBounds.min.x)
                return RecycleRightToleft();
            return _zeroVector;
        }

        /// <summary>
        /// Recycles cells from Left to Right in the List heirarchy
        /// </summary>
        private Vector2 RecycleLeftToRight()
        {
            _recycling = true;

            int n = 0;
            float posX = _isGrid ? _cellPool[_rightMostCellIndex].anchoredPosition.x : 0;
            float posY = 0;

            //to determine if content size needs to be updated
            int additionalColoums = 0;

            //Recycle until cell at left is avaiable and current item count smaller than datasource
            while (_cellPool[_leftMostCellIndex].MaxX() < _recyclableViewBounds.min.x && _currentItemCount < DataSource.GetItemCount())
            {
                if (_isGrid)
                {
                    if (++_rightMostCellRow >= _rows)
                    {
                        n++;
                        _rightMostCellRow = 0;
                        posX = _cellPool[_rightMostCellIndex].anchoredPosition.x + _cellWidth;
                        additionalColoums++;
                    }

                    //Move Left most cell to right
                    posY = -_rightMostCellRow * _cellHeight;
                    _cellPool[_leftMostCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (++_leftMostCellRow >= _rows)
                    {
                        _leftMostCellRow = 0;
                        additionalColoums--;
                    }
                }
                else
                {
                    //Move Left most cell to right
                    posX = _cellPool[_rightMostCellIndex].anchoredPosition.x + _cellPool[_rightMostCellIndex].sizeDelta.x;
                    _cellPool[_leftMostCellIndex].anchoredPosition = new Vector2(posX, _cellPool[_leftMostCellIndex].anchoredPosition.y);
                }

                //Cell for row at
                DataSource.SetCell(_cachedCells[_leftMostCellIndex], _currentItemCount);

                //set new indices
                _rightMostCellIndex = _leftMostCellIndex;
                _leftMostCellIndex = (_leftMostCellIndex + 1) % _cellPool.Count;

                _currentItemCount++;
                if (!_isGrid)
                    n++;
            }

            //Content size adjustment
            if (_isGrid)
            {
                _content.sizeDelta += _cellWidth * additionalColoums * Vector2.right;
                if (additionalColoums > 0)
                    n -= additionalColoums;
            }

            //Content anchor position adjustment.
            _cellPool.ForEach((RectTransform cell) => cell.anchoredPosition -= _cellPool[_leftMostCellIndex].sizeDelta.x * n * Vector2.right);
            _content.anchoredPosition += _cellPool[_leftMostCellIndex].sizeDelta.x * n * Vector2.right;
            _recycling = false;
            return _cellPool[_leftMostCellIndex].sizeDelta.x * n * Vector2.right;
        }

        /// <summary>
        /// Recycles cells from Right to Left in the List heirarchy
        /// </summary>
        private Vector2 RecycleRightToleft()
        {
            _recycling = true;

            int n = 0;
            float posX = _isGrid ? _cellPool[_leftMostCellIndex].anchoredPosition.x : 0;
            float posY = 0;

            //to determine if content size needs to be updated
            int additionalColoums = 0;
            //Recycle until cell at Right end is avaiable and current item count is greater than cellpool size
            while (_cellPool[_rightMostCellIndex].MinX() > _recyclableViewBounds.max.x && _currentItemCount > _cellPool.Count)
            {
                if (_isGrid)
                {
                    if (--_leftMostCellRow < 0)
                    {
                        n++;
                        _leftMostCellRow = _rows - 1;
                        posX = _cellPool[_leftMostCellIndex].anchoredPosition.x - _cellWidth;
                        additionalColoums++;
                    }

                    //Move Right most cell to left
                    posY = -_leftMostCellRow * _cellHeight;
                    _cellPool[_rightMostCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (--_rightMostCellRow < 0)
                    {
                        _rightMostCellRow = _rows - 1;
                        additionalColoums--;
                    }
                }
                else
                {
                    //Move Right most cell to left
                    posX = _cellPool[_leftMostCellIndex].anchoredPosition.x - _cellPool[_leftMostCellIndex].sizeDelta.x;
                    _cellPool[_rightMostCellIndex].anchoredPosition = new Vector2(posX, _cellPool[_rightMostCellIndex].anchoredPosition.y);
                    n++;
                }

                _currentItemCount--;
                //Cell for row at
                DataSource.SetCell(_cachedCells[_rightMostCellIndex], _currentItemCount - _cellPool.Count);

                //set new indices
                _leftMostCellIndex = _rightMostCellIndex;
                _rightMostCellIndex = (_rightMostCellIndex - 1 + _cellPool.Count) % _cellPool.Count;
            }

            //Content size adjustment
            if (_isGrid)
            {
                _content.sizeDelta += _cellWidth * additionalColoums * Vector2.right;
                if (additionalColoums > 0)
                    n -= additionalColoums;
            }

            //Content anchor position adjustment.
            _cellPool.ForEach((RectTransform cell) => cell.anchoredPosition += _cellPool[_leftMostCellIndex].sizeDelta.x * n * Vector2.right);
            _content.anchoredPosition -= _cellPool[_leftMostCellIndex].sizeDelta.x * n * Vector2.right;
            _recycling = false;
            return _cellPool[_leftMostCellIndex].sizeDelta.x * -n * Vector2.right;
        }
        #endregion

        #region  HELPERS
        /// <summary>
        /// Anchoring cell and content rect transforms to top preset. Makes repositioning easy.
        /// </summary>
        /// <param name="rectTransform"></param>
        private void SetLeftAnchor(RectTransform rectTransform)
        {
            //Saving to reapply after anchoring. Width and height changes if anchoring is change.
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            Vector2 pos = _isGrid ? new Vector2(0, 1) : new Vector2(0, 0.5f);

            //Setting top anchor
            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;
            rectTransform.pivot = pos;

            //Reapply size
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        #endregion

        #region  TESTING
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_recyclableViewBounds.min - new Vector3(0, 2000), _recyclableViewBounds.min + new Vector3(0, 2000));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_recyclableViewBounds.max - new Vector3(0, 2000), _recyclableViewBounds.max + new Vector3(0, 2000));
        }
        #endregion

    }
}
