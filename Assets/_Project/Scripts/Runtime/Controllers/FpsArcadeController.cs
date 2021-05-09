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
        public override float AudioMinDistance { get; protected set; }
        public override float AudioMaxDistance { get; protected set; }
        public override AnimationCurve VolumeCurve { get; protected set; }

        protected override CameraSettings CameraSettings => _arcadeContext.ArcadeConfiguration.FpsArcadeProperties.CameraSettings;
        protected override RenderSettings RenderSettings => _arcadeContext.ArcadeConfiguration.FpsArcadeProperties.RenderSettings;
        protected override bool GameModelsSpawnAtPositionWithRotation => true;

        //protected override PlayerControls PlayerControls => _main.PlayerFpsControls;

        public FpsArcadeController(ArcadeContext arcadeContext)
        : base(arcadeContext)
        {
            AudioMinDistance = 1f;
            AudioMaxDistance = 3f;

            VolumeCurve = new AnimationCurve(new Keyframe[]
            {
                 new Keyframe(0.8f,                    1.0f, -2.6966875f,  -2.6966875f,  0.2f,        0.10490462f),
                 new Keyframe(AudioMaxDistance * 0.5f, 0.3f, -0.49866775f, -0.49866775f, 0.28727788f, 0.2f),
                 new Keyframe(AudioMaxDistance,        0.0f, -0.08717632f, -0.08717632f, 0.5031141f,  0.2f)
            });
        }

        protected override void SetupPlayer()
        {
            if (_arcadeContext.GeneralConfiguration.Value.EnableVR)
                _arcadeContext.Player.TransitionTo<PlayerVirtualRealityFpsState>();
            else
                _arcadeContext.Player.TransitionTo<PlayerStandardFpsState>();

            //PlayerControls.transform.SetPositionAndRotation(CameraSettings.Position, Quaternion.Euler(0f, CameraSettings.Rotation.y, 0f));

            //PlayerControls.Camera.rect = CameraSettings.ViewportRect;

            //CinemachineNewVirtualCamera vCam = PlayerControls.VirtualCamera;
            //vCam.transform.eulerAngles    = new Vector3(0f, CameraSettings.Rotation.y, 0f);
            //vCam.m_Lens.Orthographic      = CameraSettings.Orthographic;
            //vCam.m_Lens.FieldOfView       = CameraSettings.FieldOfView;
            //vCam.m_Lens.OrthographicSize  = CameraSettings.AspectRatio;
            //vCam.m_Lens.NearClipPlane     = CameraSettings.NearClipPlane;
            //vCam.m_Lens.FarClipPlane      = CameraSettings.FarClipPlane;

            //CinemachineTransposer transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
            //transposer.m_FollowOffset.y      = CameraSettings.Height;
        }
    }
}
