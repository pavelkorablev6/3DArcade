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

using UnityEditor;

namespace Arcade.UnityEditor
{
    internal abstract class DatabaseEditorWindow<T, U> : EditorWindow, IDatabaseEditorWindow<T>
        where T : DatabaseEntry, new()
        where U : ConfigurationSO<T>
    {
        public abstract MultiFileDatabase<T> Database { get; }

        protected const float MIN_WINDOW_WIDTH  = 468f;
        protected const float MIN_WINDOW_HEIGHT = 468f;

        private DatabaseEditorWindowContext<T> _context;
        private U _tempCfg;

        private void OnEnable() => _context = new DatabaseEditorWindowContext<T>(this);

        private void OnGUI() => _context.OnUpdate(0f);

        public SerializedObject GetSerializedObject(T cfg)
        {
            _tempCfg = CreateInstance<U>();
            _tempCfg.Value = cfg ?? new T();
            return new SerializedObject(_tempCfg);
        }

        public void ClearConfiguration()
        {
            if (_tempCfg == null)
                return;

            DestroyImmediate(_tempCfg);
            _tempCfg = null;
        }

        public virtual void DrawInlineButtons(T entry)
        {
        }
    }
}
