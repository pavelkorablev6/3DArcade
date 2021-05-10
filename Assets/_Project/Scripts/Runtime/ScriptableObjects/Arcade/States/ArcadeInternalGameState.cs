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
    public abstract class ArcadeInternalGameState : ArcadeState
    {
        private ScreenNodeTag ScreenNode { get; set; }
        private Renderer ScreenRenderer { get; set; }
        private Material SavedMaterial { get; set; }
        private DynamicArtworkComponent DynamicArtworkComponent { get; set; }

        public sealed override void OnEnter()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            ModelConfigurationComponent currentTarget = Context.InteractionControllers.NormalModeController.InteractionData.TargetPair.Current;
            if (currentTarget == null)
            {
                Context.TransitionToPrevious();
                return;
            }

            ScreenNodeTag screenNodeTag = currentTarget.GetComponentInChildren<ScreenNodeTag>(true);
            if (screenNodeTag == null)
            {
                Context.TransitionToPrevious();
                return;
            }

            if (screenNodeTag.TryGetComponent(out VideoPlayer videoPlayer))
                Context.VideoPlayerController.StopVideo(videoPlayer);

            if (!Context.GameControllers.Internal.StartGame(screenNodeTag, currentTarget.Configuration))
            {
                Context.TransitionToPrevious();
                return;
            }

            ScreenNode = screenNodeTag;

            DynamicArtworkComponent = ScreenNode.GetComponent<DynamicArtworkComponent>();
            if (DynamicArtworkComponent != null)
                DynamicArtworkComponent.enabled = false;

            ScreenRenderer = ScreenNode.GetComponent<Renderer>();
            SavedMaterial  = ScreenRenderer.material;
            ScreenRenderer.material = Context.GameControllers.Internal.ScreenMaterial;

            OnStateEnter();
        }

        public sealed override void OnExit()
        {
            Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

            OnStateExit();

            Context.GameControllers.Internal.StopGame();

            if (ScreenRenderer != null)
                ScreenRenderer.material = SavedMaterial;

            if (DynamicArtworkComponent != null)
                DynamicArtworkComponent.enabled = true;
        }

        public sealed override void OnUpdate(float dt)
        {
            if (Context.InputActions.Global.Quit.triggered)
            {
                Context.TransitionToPrevious();
                return;
            }

            Context.GameControllers.Internal.UpdateGame();
        }

        protected virtual void OnStateEnter()
        {
        }

        protected virtual void OnStateExit()
        {
        }
    }
}
