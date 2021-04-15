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

using UnityEngine;
using UnityEngine.Video;

namespace Arcade
{
    [DisallowMultipleComponent, RequireComponent(typeof(Renderer))]
    public sealed class DynamicArtworkComponent : MonoBehaviour
    {
        private const float DEFAULT_MIN_DELAY = 0.8f;
        private const float DEFAULT_MAX_DELAY = 1.8f;

        [SerializeField] private bool _enableCycling                   = true;
        [SerializeField] private bool _useRandomDelay                  = true;
        [SerializeField] private bool _changeRandomDelayAfterTimerEnds = true;

        public bool EnableCycling
        {
            get => _enableCycling;
            set => _enableCycling = value;
        }

        public bool UseRandomDelay
        {
            get => _useRandomDelay;
            set
            {
                _useRandomDelay    = value;
                _imageCyclingDelay = Random.Range(DEFAULT_MIN_DELAY, DEFAULT_MAX_DELAY);
            }
        }

        public bool ChangeRandomDelayAfterTimerEnds
        {
            get => _changeRandomDelayAfterTimerEnds;
            set => _changeRandomDelayAfterTimerEnds = value;
        }

        private bool VideoIsPlaying => _videoPlayer != null && _videoPlayer.enabled && _videoPlayer.isPlaying;

        private Texture[] _imageCyclingTextures;
        private float _imageCyclingDelay = DEFAULT_MIN_DELAY;
        private float _emissionIntensity = 1f;

        private int _imageCyclingIndex   = 0;
        private float _imageCyclingTimer = 0f;

        private Renderer _renderer;
        private VideoPlayer _videoPlayer;

        private void Awake()
        {
            _renderer    = GetComponent<Renderer>();
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        private void OnEnable()
        {
            ArtworkController.OnVideoPlayerAdded += OnVideoPlayerAdded;

            _imageCyclingIndex = 0;
            _imageCyclingTimer = 0f;

            if (_useRandomDelay)
                _imageCyclingDelay = Random.Range(DEFAULT_MIN_DELAY, DEFAULT_MAX_DELAY);
        }

        private void OnDisable() => ArtworkController.OnVideoPlayerAdded -= OnVideoPlayerAdded;

        private void Update()
        {
            if (VideoIsPlaying || !_enableCycling)
                return;

            if (_imageCyclingTextures.Length < 2)
                return;

            if ((_imageCyclingTimer += Time.deltaTime) >= _imageCyclingDelay)
            {
                CycleTexture();
                if (_changeRandomDelayAfterTimerEnds)
                    _imageCyclingDelay = Random.Range(DEFAULT_MIN_DELAY, DEFAULT_MAX_DELAY);
                _imageCyclingTimer = 0f;
            }
        }

        public void SetImageCyclingTextures(Texture[] textures, float emissionIntensity)
        {
            _emissionIntensity = emissionIntensity;

            if (textures != null)
                _imageCyclingTextures = textures;

            if (_renderer.enabled && _imageCyclingTextures != null && _imageCyclingTextures.Length > 0)
                SwapTexture();
        }

        public void SetImageCyclingDelay(float delay) => _imageCyclingDelay = Mathf.Max(0.01f, delay);

        private void CycleTexture()
        {
            if (_imageCyclingIndex++ >= _imageCyclingTextures.Length - 1)
                _imageCyclingIndex = 0;

            SwapTexture();
        }

        private void SwapTexture()
        {
            if (_imageCyclingTextures.Length < 1)
                return;

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(block);
            if (_renderer.material.IsKeywordEnabled("_EMISSION"))
            {
                block.SetColor("_BaseColor", Color.black);
                block.SetColor("_EmissionColor", Color.white * _emissionIntensity);
                block.SetTexture("_EmissionMap", _imageCyclingTextures[_imageCyclingIndex]);
            }
            else
            {
                block.SetColor("_BaseColor", Color.white);
                block.SetTexture("_BaseMap", _imageCyclingTextures[_imageCyclingIndex]);
            }
            _renderer.SetPropertyBlock(block);
        }

        private void OnVideoPlayerAdded() => _videoPlayer = GetComponent<VideoPlayer>();
    }
}
