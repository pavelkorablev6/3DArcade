/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using DG.Tweening;
using PolyAndCode.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIGameList : MonoBehaviour, IRecyclableScrollRectDataSource
    {
        [SerializeField] private FloatVariable _animationDuration;
        [SerializeField] private Databases _databases;
        [SerializeField] private TMP_InputField _idInputField;

        private readonly Dictionary<int, GameConfiguration[]> _allGames = new Dictionary<int, GameConfiguration[]>();

        private List<GameConfiguration> _filteredGames = new List<GameConfiguration>();

        private RectTransform _transform;
        private float _startPositionX;
        private float _endPositionX;

        private RecyclableScrollRect _recyclableScrollRect;
        private int _platformIndex;

        private void Awake()
        {
            _transform      = transform as RectTransform;
            _startPositionX = _transform.anchoredPosition.x;
            _endPositionX   = 0f;

            _recyclableScrollRect = GetComponentInChildren<RecyclableScrollRect>();
            _recyclableScrollRect.DataSource = this;
        }

        public void SetVisibility(bool visible)
        {
            _recyclableScrollRect.ReloadData();
            if (visible)
                Show();
            else
                Hide();
        }

        public void Show() => _transform.DOAnchorPosX(_endPositionX, _animationDuration.Value);

        public void Hide() => _transform.DOAnchorPosX(_startPositionX, _animationDuration.Value);

        public void Refresh(int index)
        {
            if (index == 0)
            {
                _filteredGames.Clear();
                _recyclableScrollRect.ReloadData();
                return;
            }

            _platformIndex = index - 1;
            if (_allGames.ContainsKey(_platformIndex))
            {
                if (_allGames[_platformIndex] is null)
                    _filteredGames.Clear();
                else
                    _filteredGames = _allGames[_platformIndex].ToList();
                _recyclableScrollRect.ReloadData();
                return;
            }

            //RefreshAsync().Forget();
            _databases.Games.Initialize();
            GameConfiguration[] games = _databases.Games.GetGames(_databases.Platforms[_platformIndex].MasterList);
            _allGames.Add(_platformIndex, games);
            if (!(games is null) && games.Length > 0)
                _filteredGames = games.ToList();
            _recyclableScrollRect.ReloadData();
        }

        public void Search(string lookUp)
        {
            _filteredGames = _allGames[_platformIndex].Where(x => x.Description.IndexOf(lookUp, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            _recyclableScrollRect.ReloadData();
        }

        public void ResetLists()
        {
            _allGames.Clear();
            _filteredGames.Clear();
        }

        //private async UniTaskVoid RefreshAsync() => _games.Add(_platformIndex, await UniTask.Run(() =>
        //{
        //    _databases.Games.Initialize();
        //    return _databases.Games.GetGames(_databases.Platforms[_platformIndex].MasterList);
        //}));

        public int GetItemCount() => _filteredGames.Count;

        public void SetCell(ICell cell, int index)
        {
            if (_filteredGames.Count == 0)
                return;

            UIGameButton item = cell as UIGameButton;
            item.ConfigureCell(_idInputField, _filteredGames[index]);
        }
    }
}
