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

namespace Arcade
{
    public sealed class ArtworkController
    {
        public static event System.Action OnVideoPlayerAdded;

        public string DefaultMediaDirectory { get; private set; }

        private readonly IVirtualFileSystem _virtualFileSystem;
        private readonly AssetCache<Texture> _textureCache;
        //private readonly AssetCache<string> _videoCache;

        public ArtworkController(IVirtualFileSystem virtualFileSystem, AssetCache<Texture> textureCache)
        {
            _virtualFileSystem = virtualFileSystem;
            _textureCache      = textureCache;
            //_videoCache        = videoCache;
        }

        public void Initialize() => DefaultMediaDirectory ??= _virtualFileSystem.GetDirectory("medias");

        public void SetupImages(IArtworkDirectoriesProvider directoryNamesProvider, ModelConfiguration modelConfiguration, string[] fileNamesToTry, Renderer[] renderers, float emissionIntensity)
        {
            if (modelConfiguration == null || fileNamesToTry == null || renderers == null)
                return;

            string[] gameDirectories     = directoryNamesProvider.GetModelImageDirectories(modelConfiguration);
            string[] platformDirectories = directoryNamesProvider.GetPlatformImageDirectories(modelConfiguration.PlatformConfiguration);
            Directories directories      = new Directories(gameDirectories, platformDirectories);

            Files files = new Files(new[] { "png", "jpg", "jpeg" }, directories, fileNamesToTry);

            if (files.Count == 0)
            {
                directories = new Directories(directoryNamesProvider.DefaultImageDirectories);
                files       = new Files(new[] { "png", "jpg", "jpeg" }, directories, fileNamesToTry);
            }

            if (files.Count == 0)
                return;

            Texture[] textures = _textureCache.LoadMultiple(files.ToArray());
            if (textures == null || textures.Length == 0)
            {
                if (directoryNamesProvider is GenericArtworkDirectoriesProvider)
                    SetRandomColors(renderers);
                return;
            }

            if (textures.Length == 1)
            {
                SetupStaticImage(renderers, textures[0], emissionIntensity);
                return;
            }

            SetupImageCycling(renderers, textures, emissionIntensity);
        }

        private static void SetupStaticImage(Renderer[] renderers, Texture texture, float emissionIntensity)
        {
            for (int i = 0; i < renderers.Length; ++i)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", Color.black);
                block.SetColor("_EmissionColor", Color.white * emissionIntensity);
                block.SetTexture("_EmissionMap", texture);
                renderers[i].SetPropertyBlock(block);
            }
        }

        private static void SetupImageCycling(Renderer[] renderers, Texture[] textures, float emissionIntensity)
        {
            foreach (Renderer renderer in renderers)
            {
                if (!renderer.gameObject.TryGetComponent(out DynamicArtworkComponent dynamicArtworkComponent))
                    dynamicArtworkComponent = renderer.gameObject.AddComponent<DynamicArtworkComponent>();
                dynamicArtworkComponent.SetImageCyclingTextures(textures, emissionIntensity);
            }
        }

        //public static void SetupVideos(IEnumerable<string> directories, IEnumerable<string> namesToTry, Renderer[] renderers, float audioMinDistance, float audioMaxDistance, AnimationCurve volumeCurve)
        //{
        //    string videopath = _videoCache.Load(directories, namesToTry);
        //    if (string.IsNullOrEmpty(videopath))
        //        return;

        //    foreach (Renderer renderer in renderers)
        //    {
        //        AudioSource audioSource  = renderer.gameObject.AddComponentIfNotFound<AudioSource>();
        //        audioSource.playOnAwake  = false;
        //        audioSource.dopplerLevel = 0f;
        //        audioSource.spatialBlend = 1f;
        //        audioSource.minDistance  = audioMinDistance;
        //        audioSource.maxDistance  = audioMaxDistance;
        //        audioSource.volume       = 1f;
        //        audioSource.rolloffMode  = AudioRolloffMode.Custom;
        //        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeCurve);

        //        VideoPlayer videoPlayer               = renderer.gameObject.AddComponentIfNotFound<VideoPlayer>();
        //        videoPlayer.errorReceived            -= OnVideoPlayerErrorReceived;
        //        videoPlayer.errorReceived            += OnVideoPlayerErrorReceived;
        //        videoPlayer.playOnAwake               = false;
        //        videoPlayer.waitForFirstFrame         = true;
        //        videoPlayer.isLooping                 = true;
        //        videoPlayer.skipOnDrop                = true;
        //        videoPlayer.source                    = VideoSource.Url;
        //        videoPlayer.url                       = videopath;
        //        videoPlayer.renderMode                = VideoRenderMode.MaterialOverride;
        //        videoPlayer.targetMaterialProperty    = MaterialUtils.SHADER_EMISSIVE_TEXTURE_NAME;
        //        videoPlayer.audioOutputMode           = VideoAudioOutputMode.AudioSource;
        //        videoPlayer.controlledAudioTrackCount = 1;
        //        videoPlayer.Stop();

        //        OnVideoPlayerAdded?.Invoke();
        //    }
        //}

        //private static void OnVideoPlayerErrorReceived(VideoPlayer _, string message)
        //    => Debug.LogError($"OnVideoPlayerErrorReceived: {message}");

        private static void SetRandomColors(Renderer[] renderers)
        {
            Color color = new Color(Random.value, Random.value, Random.value);

            foreach (Renderer renderer in renderers)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", color);
                renderer.SetPropertyBlock(block);
            }
        }
    }
}
