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

namespace Arcade
{
    public sealed class MarqueeArtworkDirectoryNamesProvider : IArtworkDirectoryNamesProvider
    {
        public ArtworkDirectories DefaultImageDirectories { get; private set; }
        public ArtworkDirectories DefaultVideoDirectories { get; private set; }

        private const string IMAGES_DIRECTORY_NAME = "MarqueesImages";
        private const string VIDEOS_DIRECTORY_NAME = "MarqueesVideos";

        private readonly ArtworkController _artworkController;

        public MarqueeArtworkDirectoryNamesProvider(ArtworkController artworkController) => _artworkController = artworkController;

        public void Initialize()
        {
            DefaultImageDirectories ??= new ArtworkDirectories($"{_artworkController.DefaultMediaDirectory}/{IMAGES_DIRECTORY_NAME}");
            DefaultVideoDirectories ??= new ArtworkDirectories($"{_artworkController.DefaultMediaDirectory}/{VIDEOS_DIRECTORY_NAME}");
        }

        public ArtworkDirectories GetModelImageDirectories(ModelConfiguration modelConfiguration)
            => ArtworkDirectories.GetCorrectedDirectories(modelConfiguration.Overrides.ArtworkDirectories.MarqueeImageDirectories);

        public ArtworkDirectories GetModelVideoDirectories(ModelConfiguration modelConfiguration)
            => ArtworkDirectories.GetCorrectedDirectories(modelConfiguration.Overrides.ArtworkDirectories.MarqueeVideoDirectories);

        public ArtworkDirectories GetPlatformImageDirectories(PlatformConfiguration platform)
            => ArtworkDirectories.GetCorrectedDirectories(platform?.MarqueeImagesDirectories);

        public ArtworkDirectories GetPlatformVideoDirectories(PlatformConfiguration platform)
            => ArtworkDirectories.GetCorrectedDirectories(platform?.MarqueeVideosDirectories);
    }
}
