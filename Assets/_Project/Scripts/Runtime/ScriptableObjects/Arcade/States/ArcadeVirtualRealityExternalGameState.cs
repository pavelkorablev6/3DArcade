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
    public sealed class ArcadeVirtualRealityExternalGameState : ArcadeExternalGameState
    {
        private ScreenNodeTag ScreenNode { get; set; }
        private Material SavedMaterial { get; set; }

        protected override void OnEnterState()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            ScreenNode = Context.InteractionControllers.NormalModeController.InteractionData.TargetPair.Current.GetComponentInChildren<ScreenNodeTag>();
            if (ScreenNode == null)
            {
                Context.TransitionToPrevious();
                return;
            }

            if (ScreenNode.TryGetComponent(out VideoPlayer videoPlayer))
                Context.VideoPlayerController.StopVideo(videoPlayer);

            if (ScreenNode.TryGetComponent(out DynamicArtworkComponent dynamicArtworkComponent))
                dynamicArtworkComponent.enabled = false;

            Renderer renderer = ScreenNode.GetComponent<Renderer>();
            SavedMaterial    = renderer.material;
            renderer.material = Context.GameControllers.External.ScreenMaterial;

            uDesktopDuplication.Texture uddTexture = ScreenNode.gameObject.AddComponent<uDesktopDuplication.Texture>();
            uddTexture.invertY = true;
        }

        protected override void OnExitState()
        {
            Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

            if (ScreenNode == null)
                return;

            if (ScreenNode.TryGetComponent(out uDesktopDuplication.Texture uddTexture))
                Object.Destroy(uddTexture);

            Renderer renderer = ScreenNode.GetComponent<Renderer>();
            renderer.material = SavedMaterial;

            if (ScreenNode.TryGetComponent(out DynamicArtworkComponent dynamicArtworkComponent))
                dynamicArtworkComponent.enabled = true;
        }
    }
}
