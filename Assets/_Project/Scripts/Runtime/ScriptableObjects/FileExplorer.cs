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

using System.IO;
using UnityEngine;
#if !UNITY_EDITOR
using SimpleFileBrowser;
using System.Linq;
#endif

namespace Arcade
{
    [CreateAssetMenu(menuName = "3DArcade/Utils/FileExplorer", fileName = "FileExplorer")]
    public sealed class FileExplorer : ScriptableObject
    {
        [System.NonSerialized] private string _lastDirectory;
        [System.NonSerialized] private string _lastFileDirectory;
#if !UNITY_EDITOR
        [System.NonSerialized] private System.Action<string[]> _callback;
#endif
        public void OpenDirectoryDialog(System.Action<string[]> callback)
        {
#if UNITY_EDITOR
            string result = UnityEditor.EditorUtility.OpenFolderPanel("Select directory", _lastDirectory, null);
            if (string.IsNullOrEmpty(result))
            {
                callback.Invoke(null);
                return;
            }
            _lastDirectory = Path.GetDirectoryName(_lastDirectory);
            callback.Invoke(new[] { result.Replace('\\', '/') });
#else
            _callback = callback;
            _ = FileBrowser.ShowLoadDialog(OpenDirectoryDialogSuccessCallback,
                                           OpenDirectoryDialogCancelCallback,
                                           FileBrowser.PickMode.Folders,
                                           false,
                                           _lastDirectory,
                                           null,
                                           "Select directory",
                                           "Select");
#endif
        }

        public void OpenFileDialog(System.Action<string[]> callback)
        {
#if UNITY_EDITOR
            string result = UnityEditor.EditorUtility.OpenFilePanel("Select file", _lastFileDirectory, null);
            if (string.IsNullOrEmpty(result))
            {
                callback.Invoke(null);
                return;
            }
            _lastFileDirectory = Path.GetDirectoryName(result);
            callback.Invoke(new[] { result.Replace('\\', '/') });
#else
            _callback = callback;
            _ = FileBrowser.ShowLoadDialog(OpenFileDialogSuccessCallback,
                                           OpenFileDialogCancelCallback,
                                           FileBrowser.PickMode.Files,
                                           false,
                                           _lastFileDirectory,
                                           null,
                                           "Select file",
                                           "Select");
#endif
        }

#if !UNITY_EDITOR
        private void OpenDirectoryDialogSuccessCallback(string[] paths)
        {
            paths = paths.Select(x => x.Replace('\\', '/')).ToArray();
            if (paths.Length > 0)
                _lastDirectory = Path.GetDirectoryName(paths[0]);
            _callback?.Invoke(paths);
            _callback = null;
        }

        private void OpenDirectoryDialogCancelCallback()
        {
            _callback?.Invoke(null);
            _callback = null;
        }

        private void OpenFileDialogSuccessCallback(string[] paths)
        {
            paths = paths.Select(x => x.Replace('\\', '/')).ToArray();
            if (paths.Length > 0)
                _lastFileDirectory = Path.GetDirectoryName(paths[0]);
            _callback?.Invoke(paths);
            _callback = null;
        }

        private void OpenFileDialogCancelCallback()
        {
            _callback?.Invoke(null);
            _callback = null;
        }
#endif
    }
}
