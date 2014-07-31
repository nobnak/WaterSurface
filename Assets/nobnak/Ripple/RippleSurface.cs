using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace nobnak.Ripple {

	public class RippleSurface : MonoBehaviour {
		public const string SHADER_PROP_RIPPLES = "_Ripples";

		public Material rippleSurfaceMat;
		public Material[] outputMats;
		public int size = 256;
		public int fps = 30;
		public int maxRipples = 128;

		private RenderTexture _rtex0, _rtex1;
		private float _dt;
		private float _tPrev = 0f;
		private ComputeBuffer _ripplesBuffer;
		private Ripple[] _ripples0 = new Ripple[0], _ripples1 = new Ripple[0];

		void Start() {
			_rtex0 = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
			_rtex1 = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
			_rtex0.wrapMode = TextureWrapMode.Clamp;
			_rtex1.wrapMode = TextureWrapMode.Clamp;
			RenderTexture.active = _rtex0;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = _rtex1;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = null;

			_dt = 1f / fps;
			_tPrev = Time.timeSinceLevelLoad;
		}

		void OnDestroy() {
			if (_ripplesBuffer != null)
				_ripplesBuffer.Release();
		}

		void Update() {
			var n = Mathf.FloorToInt((Time.timeSinceLevelLoad - _tPrev) / _dt);
			if (n == 0)
				return;
			_tPrev += n * _dt;

			if (Input.GetMouseButton(0)) {
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (collider.Raycast(ray, out hit, float.MaxValue)) {
					if (_ripples0.Length >= maxRipples) {
						if (_ripples1.Length < maxRipples)
							System.Array.Resize(ref _ripples1, maxRipples);
						System.Array.Copy(_ripples0, 1, _ripples1, 0, maxRipples-1);
					} else {
						System.Array.Resize(ref _ripples1, _ripples0.Length + 1);
						System.Array.Copy(_ripples0, _ripples1, _ripples0.Length);
					}
					_ripples1[_ripples1.Length - 1] = new Ripple(hit.textureCoord, Time.timeSinceLevelLoad);
					var tmpRipples = _ripples0; _ripples0 = _ripples1; _ripples1 = tmpRipples;

					if (_ripplesBuffer != null)
						_ripplesBuffer.Release();
					_ripplesBuffer = new ComputeBuffer(_ripples0.Length, Marshal.SizeOf(typeof(Ripple)));
					_ripplesBuffer.SetData(_ripples0);
					rippleSurfaceMat.SetBuffer(SHADER_PROP_RIPPLES, _ripplesBuffer);
				}
			}

			for (var i = 0; i < n; i++) {
				_rtex1.DiscardContents();
				Graphics.Blit(_rtex0, _rtex1, rippleSurfaceMat);
				var tmpRtex = _rtex0; _rtex0 = _rtex1; _rtex1 = tmpRtex;
			}
			foreach (var mat in outputMats)
				mat.mainTexture = _rtex0;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct Ripple {
			public Vector2 center;
			public float tStart;

			public Ripple(Vector2 center, float tStart) {
				this.center = center;
				this.tStart = tStart;
			}
		}
	}
}