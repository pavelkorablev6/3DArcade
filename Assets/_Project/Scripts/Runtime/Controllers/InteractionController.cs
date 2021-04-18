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
    public sealed class InteractionController
    {
        public ModelConfiguration CurrentTarget { get; private set; }

        private Camera _camera;
        private ModelConfigurationComponent _modelConfiguration;

        public void Initialize(Camera camera) => _camera = camera;

        public void FindInteractable(LayerMask raycastLayers, float raycastMaxDistance)
        {
            if (_camera == null)
                return;

            Ray ray = _camera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, raycastMaxDistance, raycastLayers))
            {
                _modelConfiguration = null;
                return;
            }

            ModelConfigurationComponent currentTarget = hitInfo.transform.GetComponent<ModelConfigurationComponent>();
            if (currentTarget != _modelConfiguration)
                _modelConfiguration = currentTarget;
        }

        public void HandleInteraction(ArcadeContext arcadeContext)
        {
            if (_modelConfiguration == null)
                return;

            CurrentTarget = _modelConfiguration.ToModelConfiguration();

            switch (CurrentTarget.InteractionType)
            {
                case InteractionType.Default:
                case InteractionType.GameInternal:
                case InteractionType.GameExternal:
                {
                    bool foundPlatform         = arcadeContext.Databases.Platforms.TryGet(CurrentTarget.Platform, out PlatformConfiguration platform);
                    bool foundEmulatorOverride = arcadeContext.Databases.Emulators.TryGet(CurrentTarget.Overrides.Emulator, out EmulatorConfiguration emulator);
                    if (foundPlatform && !foundEmulatorOverride)
                        _ = arcadeContext.Databases.Emulators.TryGet(platform.Emulator, out emulator);

                    CurrentTarget.EmulatorConfiguration = emulator;

                    HandleEmulatorInteraction(arcadeContext);
                }
                break;
                case InteractionType.FpsArcadeConfiguration:
                    arcadeContext.StartArcade(CurrentTarget.Id, ArcadeType.Fps, ArcadeMode.Normal);
                    break;
                case InteractionType.CylArcadeConfiguration:
                    arcadeContext.StartArcade(CurrentTarget.Id, ArcadeType.Cyl, ArcadeMode.Normal);
                    break;
                case InteractionType.FpsMenuConfiguration:
                    arcadeContext.StartArcade(CurrentTarget.Id, ArcadeType.Fps, ArcadeMode.RenderTexture);
                    break;
                case InteractionType.CylMenuConfiguration:
                    arcadeContext.StartArcade(CurrentTarget.Id, ArcadeType.Cyl, ArcadeMode.RenderTexture);
                    break;
                case InteractionType.URL:
                default:
                    break;
            }

            CurrentTarget = null;
        }

        private void HandleEmulatorInteraction(ArcadeContext arcadeContext)
        {
            if (CurrentTarget.EmulatorConfiguration == null)
                return;

            switch (CurrentTarget.EmulatorConfiguration.InteractionType)
            {
                case InteractionType.GameInternal:
                    arcadeContext.TransitionTo<ArcadeInternalGameState>();
                    break;
                case InteractionType.GameExternal:
                    arcadeContext.TransitionTo<ArcadeExternalGameState>();
                    break;
                case InteractionType.FpsArcadeConfiguration:
                    arcadeContext.StartArcade(CurrentTarget.Id, ArcadeType.Fps, ArcadeMode.Normal);
                    break;
                case InteractionType.CylArcadeConfiguration:
                    arcadeContext.StartArcade(CurrentTarget.Id, ArcadeType.Cyl, ArcadeMode.Normal);
                    break;
                case InteractionType.FpsMenuConfiguration:
                    arcadeContext.StartArcade(CurrentTarget.Id, ArcadeType.Fps, ArcadeMode.RenderTexture);
                    break;
                case InteractionType.CylMenuConfiguration:
                    arcadeContext.StartArcade(CurrentTarget.Id, ArcadeType.Cyl, ArcadeMode.RenderTexture);
                    break;
                case InteractionType.Default:
                case InteractionType.URL:
                default:
                    break;
            }

            CurrentTarget = null;
        }
    }
}
