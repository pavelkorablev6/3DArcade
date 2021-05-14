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

using System;
using UnityEngine;

namespace Arcade
{
    [CreateAssetMenu(menuName = "Arcade/Interaction/NormalModeInteractionController", fileName = "NormalModeInteractionController")]
    public sealed class NormalModeInteractionController : InteractionController<NormalModeInteractionData>
    {
        [SerializeField] private ArcadeContext _arcadeContext;

        public void HandleInteraction()
        {
            if (InteractionData.Current == null)
                return;

            ModelConfiguration modelConfiguration = InteractionData.Current.Configuration;
            InteractionType interactionType       = modelConfiguration.InteractionType;

            switch (interactionType)
            {
                case InteractionType.Default:
                case InteractionType.GameInternal:
                case InteractionType.GameExternal:
                    HandleEmulatorInteraction(modelConfiguration);
                    break;
                case InteractionType.FpsArcadeConfiguration:
                case InteractionType.CylArcadeConfiguration:
                case InteractionType.FpsMenuConfiguration:
                case InteractionType.CylMenuConfiguration:
                    HandleArcadeTransition(modelConfiguration);
                    break;
                case InteractionType.None:
                case InteractionType.URL:
                    break;
                default:
                    throw new Exception($"Unhandled switch case for InteractionType: {modelConfiguration.InteractionType}");
            }

            InteractionData.Reset();
        }

        protected override Ray GetRay() => Camera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

        private void HandleEmulatorInteraction(ModelConfiguration modelConfiguration)
        {
            bool foundPlatform         = _arcadeContext.Databases.Platforms.TryGet(modelConfiguration.Platform, out PlatformConfiguration platform);
            bool foundEmulatorOverride = _arcadeContext.Databases.Emulators.TryGet(modelConfiguration.Overrides.Emulator, out EmulatorConfiguration emulator);
            if (foundPlatform && !foundEmulatorOverride)
                _ = _arcadeContext.Databases.Emulators.TryGet(platform.Emulator, out emulator);

            modelConfiguration.EmulatorConfiguration = emulator;
            if (modelConfiguration.EmulatorConfiguration == null)
                return;

            InteractionType interactionType = modelConfiguration.EmulatorConfiguration.InteractionType;

            switch (interactionType)
            {
                case InteractionType.GameInternal:
                {
                    if (_arcadeContext.GeneralConfiguration.Value.EnableVR)
                        _arcadeContext.TransitionTo<ArcadeVirtualRealityInternalGameState>();
                    else
                        _arcadeContext.TransitionTo<ArcadeStandardInternalGameState>();
                }
                break;
                case InteractionType.GameExternal:
                {
                    if (_arcadeContext.GeneralConfiguration.Value.EnableVR)
                        _arcadeContext.TransitionTo<ArcadeVirtualRealityExternalGameState>();
                    else
                        _arcadeContext.TransitionTo<ArcadeStandardExternalGameState>();
                }
                break;
                case InteractionType.FpsArcadeConfiguration:
                case InteractionType.CylArcadeConfiguration:
                case InteractionType.FpsMenuConfiguration:
                case InteractionType.CylMenuConfiguration:
                    HandleArcadeTransition(modelConfiguration);
                    break;
                case InteractionType.Default:
                case InteractionType.None:
                case InteractionType.URL:
                    Debug.LogError("This message should never appear!");
                    break;
                default:
                    throw new Exception($"Unhandled switch case for InteractionType: {interactionType}");
            }
        }

        private void HandleArcadeTransition(ModelConfiguration modelConfiguration)
        {
            InteractionType interactionType = modelConfiguration.InteractionType;

            switch (interactionType)
            {
                case InteractionType.FpsArcadeConfiguration:
                    _arcadeContext.StartArcade(modelConfiguration.Id, ArcadeType.Fps, ArcadeMode.Normal).Forget();
                    break;
                case InteractionType.CylArcadeConfiguration:
                    _arcadeContext.StartArcade(modelConfiguration.Id, ArcadeType.Cyl, ArcadeMode.Normal).Forget();
                    break;
                case InteractionType.FpsMenuConfiguration:
                    _arcadeContext.StartArcade(modelConfiguration.Id, ArcadeType.Fps, ArcadeMode.RenderTexture).Forget();
                    break;
                case InteractionType.CylMenuConfiguration:
                    _arcadeContext.StartArcade(modelConfiguration.Id, ArcadeType.Cyl, ArcadeMode.RenderTexture).Forget();
                    break;
                case InteractionType.Default:
                case InteractionType.None:
                case InteractionType.GameInternal:
                case InteractionType.GameExternal:
                case InteractionType.URL:
                    Debug.LogError("This message should never appear!");
                    break;
                default:
                    throw new Exception($"Unhandled switch case for InteractionType: {interactionType}");
            }
        }
    }
}
