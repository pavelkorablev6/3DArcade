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
    public sealed class FpsArcadeController : ArcadeController
    {
        //public override float AudioMinDistance { get; protected set; }
        //public override float AudioMaxDistance { get; protected set; }
        //public override AnimationCurve VolumeCurve { get; protected set; }

        protected override string ArcadeName => !string.IsNullOrEmpty(_currentArcadeConfiguration.FpsArcadeProperties.Scene) ? _currentArcadeConfiguration.FpsArcadeProperties.Scene : null;
        //protected override bool UseModelTransforms => true;
        //protected override PlayerControls PlayerControls => _main.PlayerFpsControls;
        //protected override CameraSettings CameraSettings => _currentArcadeConfiguration.FpsArcadeProperties.CameraSettings;

        /*
        public FpsArcadeController(ObjectsHierarchy normalHierarchy,
                                   Database<EmulatorConfiguration> emulatorDatabase,
                                   PlatformDatabase platformDatabase,
                                   AssetCache<GameObject> gameObjectCache,
                                   NodeController<MarqueeNodeTag> marqueeNodeController,
                                   NodeController<ScreenNodeTag> screenNodeController,
                                   NodeController<GenericNodeTag> genericNodeController)
        : base(normalHierarchy, emulatorDatabase, platformDatabase, gameObjectCache, marqueeNodeController, screenNodeController, genericNodeController)
        {
            AudioMinDistance = 1f;
            AudioMaxDistance = 3f;

            VolumeCurve = new AnimationCurve(new Keyframe[]
            {
                 new Keyframe(0.8f,                     1.0f, -2.6966875f,  -2.6966875f,  0.2f,        0.10490462f),
                 new Keyframe(AudioMaxDistance * 0.5f, 0.3f, -0.49866775f, -0.49866775f, 0.28727788f, 0.2f),
                 new Keyframe(AudioMaxDistance,        0.0f, -0.08717632f, -0.08717632f, 0.5031141f,  0.2f)
            });
        }
        protected override void PreSetupPlayer()
        {
            _main.PlayerFpsControls.gameObject.SetActive(true);
            _main.PlayerCylControls.gameObject.SetActive(false);
        }
        */
    }
}
