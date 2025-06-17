using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components;

namespace Wacky612.PortalLibrarySystem2
{
    [RequireComponent(typeof(VRCUnityVideoPlayer))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class JsonWorldData : BaseWorldData
    {
        [SerializeField] private VRCUrl _jsonUrl;
        [SerializeField] private VRCUrl _thumbnailUrl = null;
        [SerializeField] private float  _worldDataLoadDelaySeconds;
        [SerializeField] private float  _thumbnailsLoadDelaySeconds;
        
        [SerializeField] private RenderTexture _renderTexture;
        
        private VRCUnityVideoPlayer _videoPlayer;
        private float               _videoPlayerTime;

        public override float WorldDataLoadDelaySeconds  { get { return _worldDataLoadDelaySeconds; } }
        public override float ThumbnailsLoadDelaySeconds { get { return _thumbnailsLoadDelaySeconds; } }

        public override void Initialize()
        {
            base.Initialize();
            _videoPlayer = GetComponent<VRCUnityVideoPlayer>();
            _videoPlayer.Loop = false;
            _videoPlayer.EnableAutomaticResync = false;
        }

        public override void LoadWorldData()
        {
            VRCStringDownloader.LoadUrl(_jsonUrl, (IUdonEventReceiver) this);
        }

        public override void LoadThumbnails()
        {
            if (_thumbnailUrl != null) _videoPlayer.LoadURL(_thumbnailUrl);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            if (result.Url == _jsonUrl && VRCJson.TryDeserializeFromJson(result.Result, out DataToken data))
            {
                _worldDataProcessor.OnWorldDataLoadSuccess(data);
            }
            else
            {
                _worldDataProcessor.OnWorldDataLoadFailed();
            }
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            _worldDataProcessor.OnWorldDataLoadFailed();
        }

        public override void OnVideoReady()
        {
            _videoPlayer.Pause();
            _worldDataProcessor.OnThumbnailsLoadFinish();
        }

        public override void OnVideoError(VideoError videoError)
        {
            _worldDataProcessor.OnThumbnailsLoadFinish();
        }

        public override Texture GetThumbnail(int index)
        {
            if (_videoPlayer.IsReady)
            {
                _videoPlayerTime = index + 1;
                _UpdateRenderTexture();
                return (Texture) _renderTexture;
            }
            else
            {
                return null;
            }
        }

        public void _UpdateRenderTexture()
        {
            _videoPlayer.SetTime(_videoPlayerTime);
            _videoPlayer.Play();
            SendCustomEventDelayedFrames(nameof(_CheckRenderTexture), 10);
        }

        public void _CheckRenderTexture()
        {
            _videoPlayer.Pause();
            var time = _videoPlayer.GetTime();

            if (time < _videoPlayerTime || time >= _videoPlayerTime + 1)
            {
                // まれに、SetTime() しても動画プレイヤーの再生時間が目的の時間にならないことがある
                // 一旦、SetTime(0) した後、_UpdateRenderTexture を再度実行することで対処している

                _videoPlayer.SetTime(0);
                SendCustomEventDelayedFrames(nameof(_UpdateRenderTexture), 10);
            }
        }
    }
}
