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
    public sealed class ScreenArtworkDirectoryNamesProvider : IArtworkDirectoryNamesProvider
    {
        private const string SNAPS_DIRECTORY_NAME  = "ScreensImages";
        private const string TITLES_DIRECTORY_NAME = "TitlesImages";
        private const string VIDEOS_DIRECTORY_NAME = "ScreensVideos";

        string[] IArtworkDirectoryNamesProvider.DefaultImageDirectories { get; } = new string[]
        {
            $"{ArtworkController.DefaultMediaDirectory}/{SNAPS_DIRECTORY_NAME}",
            $"{ArtworkController.DefaultMediaDirectory}/{TITLES_DIRECTORY_NAME}"
        };
        string[] IArtworkDirectoryNamesProvider.DefaultVideoDirectories { get; } = new string[] { $"{ArtworkController.DefaultMediaDirectory}/{VIDEOS_DIRECTORY_NAME}" };

        string[] IArtworkDirectoryNamesProvider.GetModelImageDirectories(ModelConfiguration modelConfiguration)
            => ArtworkUtilities.GetDirectories(modelConfiguration.Overrides.ArtworkDirectories.ScreenSnapDirectories, modelConfiguration.Overrides.ArtworkDirectories.ScreenTitleDirectories);

        string[] IArtworkDirectoryNamesProvider.GetModelVideoDirectories(ModelConfiguration modelConfiguration)
            => ArtworkUtilities.GetDirectories(modelConfiguration.Overrides.ArtworkDirectories.ScreenVideoDirectories);

        string[] IArtworkDirectoryNamesProvider.GetPlatformImageDirectories(PlatformConfiguration platform)
            => ArtworkUtilities.GetDirectories(platform?.ScreenSnapsDirectories, platform?.ScreenTitlesDirectories);

        string[] IArtworkDirectoryNamesProvider.GetPlatformVideoDirectories(PlatformConfiguration platform)
            => ArtworkUtilities.GetDirectories(platform?.ScreenVideosDirectories);
    }
}
