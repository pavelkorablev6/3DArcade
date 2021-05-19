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

using Cysharp.Threading.Tasks;
using SK.Utilities.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIEdit : MonoBehaviour
    {
        [SerializeField] private ArcadeContext _arcadeContext;
        [SerializeField] private UISelectionText _selectionText;
        [SerializeField] private UIStandardEditModeEditPanel _editModePanel;
        [SerializeField] private UIStandardEditModeActionBar _editModeActionBar;

        private readonly List<GameObject> _addedItems   = new List<GameObject>();
        private readonly List<GameObject> _removedItems = new List<GameObject>();

        private ModelConfigurationComponent _target;

        public void Show()
        {
            gameObject.SetActive(true);

            _arcadeContext.Databases.Initialize();

            _addedItems.Clear();
            _removedItems.Clear();

            _editModePanel.Show(_arcadeContext.Databases.Platforms);
            _editModeActionBar.Show();

            if (_target != null)
            {
                _selectionText.SetValue(_target);
                _editModePanel.SetUIData(_target.Configuration);
            }
        }

        public void Hide()
        {
            _addedItems.Clear();
            _removedItems.Clear();

            _editModePanel.Hide();
            _editModeActionBar.Hide();

            gameObject.SetActive(false);
        }

        public void SetUIData(ModelConfigurationComponent target)
        {
            if (target == null)
            {
                _selectionText.ResetValue();
                _editModePanel.ResetUIData();
                return;
            }

            _target = target;

            _selectionText.SetValue(_target);
            _editModePanel.SetUIData(_target.Configuration);
        }

        public void ResetUIData()
        {
            _target = null;
            _selectionText.ResetValue();
            _editModePanel.ResetUIData();
        }

        public void ApplyChanges()
        {
            if (_target != null)
                SpawnGameAsync(_target).Forget();
        }

        public void SpawnGame() => SpawnGameAsync(null).Forget();

        public void RemoveModel(bool resetUIData)
        {
            if (_target == null)
                return;

            GameObject targetObject = _target.gameObject;

            if (_addedItems.Contains(targetObject))
            {
                _ = _addedItems.Remove(targetObject);
                ObjectUtils.DestroyObject(targetObject);
            }
            else
            {
                _removedItems.Add(targetObject);
                targetObject.SetActive(false);
            }

            if (resetUIData)
                ResetUIData();
        }

        public void RestoreCurrentArcadeModels()
        {
            foreach (GameObject item in _addedItems)
                ObjectUtils.DestroyObject(item);
            _addedItems.Clear();

            foreach (GameObject item in _removedItems)
                item.SetActive(true);
            _removedItems.Clear();

            //_arcadeContext.ArcadeController.Value.RestoreModelPositions();
        }

        public void SaveCurrentArcadeModels()
        {
            foreach (GameObject item in _removedItems)
                ObjectUtils.DestroyObject(item);

            _removedItems.Clear();
            _ = _arcadeContext.SaveCurrentArcade(true);
        }

        private async UniTaskVoid SpawnGameAsync(ModelConfigurationComponent existing)
        {
            ModelConfiguration modelConfiguration;
            Vector3 spawnPosition;
            Quaternion spawnRotation;

            if (existing == null)
            {
                modelConfiguration = new ModelConfiguration();

                Transform playerTransform = _arcadeContext.Player.ActiveTransform;
                Vector3 playerPosition    = playerTransform.position;
                Vector3 playerDirection   = playerTransform.forward;
                float spawnDistance       = 2f;
                Vector3 verticalOffset    = Vector3.up * 0.4f;
                spawnPosition             = playerPosition + verticalOffset + playerDirection * spawnDistance;
                spawnRotation             = Quaternion.LookRotation(-playerDirection);
            }
            else
            {
                modelConfiguration = ClassUtils.DeepCopy(existing.Configuration);

                spawnPosition = existing.transform.position;
                spawnRotation = existing.transform.localRotation;

                RemoveModel(false);
            }

            modelConfiguration = _editModePanel.UpdatedModelConfigurationValues(modelConfiguration);

            GameObject spawnedGame = await _arcadeContext.ArcadeController.Value.ModelSpawner.SpawnGameAsync(modelConfiguration, spawnPosition, spawnRotation);
            _addedItems.Add(spawnedGame);
        }
    }
}
