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
using UnityEngine;
using UnityEngine.Video;

namespace Arcade
{
    public sealed class ArtworkController
    {
        public static event System.Action OnVideoPlayerAdded;

        public static readonly string ShaderEmissionKeyword = "_EMISSION";
        public static readonly int ShaderBaseColorId        = Shader.PropertyToID("_BaseColor");
        public static readonly int ShaderBaseMapId          = Shader.PropertyToID("_BaseMap");
        public static readonly int ShaderEmissionColorId    = Shader.PropertyToID("_EmissionColor");
        public static readonly int ShaderEmissionMapId      = Shader.PropertyToID("_EmissionMap");

        public static string DefaultMediaDirectory { get; private set; }

        private static readonly string[] _imageExtensions = new string[] { "png", "jpg", "jpeg" };
        private static readonly string[] _videoExtensions = new string[] { "mp4", "mkv", "avi" };

        private readonly VirtualFileSystem _virtualFileSystem;
        private readonly AssetCache<Texture> _textureCache;

        public ArtworkController(VirtualFileSystem virtualFileSystem, AssetCache<Texture> textureCache)
        {
            _virtualFileSystem = virtualFileSystem;
            _textureCache      = textureCache;
        }

        public async UniTask SetupImages(IArtworkDirectoriesProvider directoryNamesProvider, ModelConfiguration modelConfiguration, string[] fileNamesToTry, Renderer[] renderers, float emissionIntensity)
        {
            if (modelConfiguration == null || fileNamesToTry == null || renderers == null)
                return;

            if (DefaultMediaDirectory == null)
            {
                if (!_virtualFileSystem.TryGetDirectory("medias", out string defaultMediaDirectory))
                    throw new System.Exception("Directory 'medias' not mapped in VirtualFileSystem.");
                DefaultMediaDirectory = defaultMediaDirectory;
            }

            string[] gameDirectories     = directoryNamesProvider.GetModelImageDirectories(modelConfiguration);
            string[] platformDirectories = directoryNamesProvider.GetPlatformImageDirectories(modelConfiguration.PlatformConfiguration);
            Directories directories      = new Directories(gameDirectories, platformDirectories);

            Files files = new Files(directories, fileNamesToTry, _imageExtensions);

            if (files.Count == 0)
            {
                directories = new Directories(directoryNamesProvider.DefaultImageDirectories);
                files       = new Files(directories, fileNamesToTry, _imageExtensions);
            }

            if (files.Count == 0)
                return;

            Texture[] textures = await _textureCache.LoadMultipleAsync(files);
            if (textures == null || textures.Length == 0)
            {
                if (directoryNamesProvider is GenericArtworkDirectoriesProvider)
                    SetRandomColors(renderers);
                return;
            }

            SetupDynamicArtworkComponents(renderers, textures, emissionIntensity);
        }

        public async UniTask SetupVideos(IArtworkDirectoriesProvider directoryNamesProvider, ModelConfiguration modelConfiguration, string[] fileNamesToTry, Renderer[] renderers, float audioMinDistance, float audioMaxDistance, AnimationCurve volumeCurve)
        {
            if (modelConfiguration == null || fileNamesToTry == null || renderers == null)
                return;

            string[] gameDirectories     = directoryNamesProvider.GetModelVideoDirectories(modelConfiguration);
            string[] platformDirectories = directoryNamesProvider.GetPlatformVideoDirectories(modelConfiguration.PlatformConfiguration);
            Directories directories      = new Directories(gameDirectories, platformDirectories);

            Files files = new Files(directories, fileNamesToTry, _videoExtensions);

            if (files.Count == 0)
            {
                directories = new Directories(directoryNamesProvider.DefaultVideoDirectories);
                files       = new Files(directories, fileNamesToTry, _videoExtensions);
            }

            if (files.Count == 0)
                return;

            foreach (Renderer renderer in renderers)
            {
                if (!renderer.gameObject.TryGetComponent(out AudioSource audioSource))
                    audioSource = renderer.gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake  = false;
                audioSource.dopplerLevel = 0f;
                audioSource.spatialBlend = 1f;
                audioSource.minDistance  = audioMinDistance;
                audioSource.maxDistance  = audioMaxDistance;
                audioSource.volume       = 1f;
                audioSource.rolloffMode  = AudioRolloffMode.Custom;
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeCurve);

                if (!renderer.gameObject.TryGetComponent(out VideoPlayer videoPlayer))
                    videoPlayer = renderer.gameObject.AddComponent<VideoPlayer>();
                videoPlayer.errorReceived            -= OnVideoPlayerErrorReceived;
                videoPlayer.errorReceived            += OnVideoPlayerErrorReceived;
                videoPlayer.playOnAwake               = false;
                videoPlayer.waitForFirstFrame         = true;
                videoPlayer.isLooping                 = true;
                videoPlayer.skipOnDrop                = true;
                videoPlayer.source                    = VideoSource.Url;
                videoPlayer.url                       = files[0];
                videoPlayer.renderMode                = VideoRenderMode.MaterialOverride;
                videoPlayer.targetMaterialProperty    = "_EmissionMap";
                videoPlayer.audioOutputMode           = VideoAudioOutputMode.AudioSource;
                videoPlayer.controlledAudioTrackCount = 1;
                videoPlayer.Stop();

                OnVideoPlayerAdded?.Invoke();

                await UniTask.Yield();
            }
        }

        private static void SetRandomColors(Renderer[] renderers)
        {
            Color color = new Color(Random.value, Random.value, Random.value);

            foreach (Renderer renderer in renderers)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor(ShaderBaseColorId, color);
                renderer.SetPropertyBlock(block);
            }
        }

        private static void SetupDynamicArtworkComponents(Renderer[] renderers, Texture[] textures, float emissionIntensity)
        {
            foreach (Renderer renderer in renderers)
            {
                if (!renderer.gameObject.TryGetComponent(out DynamicArtworkComponent dynamicArtworkComponent))
                    dynamicArtworkComponent = renderer.gameObject.AddComponent<DynamicArtworkComponent>();
                dynamicArtworkComponent.Construct(textures, emissionIntensity);
            }
        }

        private static void OnVideoPlayerErrorReceived(VideoPlayer _, string message)
            => Debug.LogError($"OnVideoPlayerErrorReceived: {message}");
    }
}
