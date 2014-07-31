using UnityEngine;
using System.Collections;

namespace nobnak.Diffusion {

	public class DiffusionSurface : MonoBehaviour {
		public const string SHADER_PROP_DT = "_Dt";
		public const string SHADER_PROP_DX = "_Dx";
		public const string SHADER_PROP_D = "_D";

		public Material diffusionSurfaceMat;
		public Material[] outputMats;
		public Material brushMat;
		public Texture2D brushTex;
		public int size = 256;
		public int fps = 30;

		private RenderTexture _rtex0, _rtex1;
		private float _dt;
		private float _tPrev = 0f;

		void Start() {
			_rtex0 = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
			_rtex1 = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
			_rtex0.wrapMode = TextureWrapMode.Repeat;
			_rtex1.wrapMode = TextureWrapMode.Repeat;
			RenderTexture.active = _rtex0;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = _rtex1;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = null;

			_dt = 1f / fps;
			_tPrev = Time.timeSinceLevelLoad;
			diffusionSurfaceMat.SetFloat(SHADER_PROP_DT, _dt);
			var dx = diffusionSurfaceMat.GetFloat(SHADER_PROP_DX);
			var d = diffusionSurfaceMat.GetFloat(SHADER_PROP_D);
			var lambda = d * _dt / (dx * dx);
			if (lambda > 0.25f)
				diffusionSurfaceMat.SetFloat(SHADER_PROP_D, 0.25f * (dx * dx) / _dt);
		}

		void Update() {
			var n = Mathf.FloorToInt((Time.timeSinceLevelLoad - _tPrev) / _dt);
			_tPrev += n * _dt;

			if (Input.GetMouseButton(0)) {
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (collider.Raycast(ray, out hit, float.MaxValue)) {
					var pxPos = size * hit.textureCoord;
					var projMat = Matrix4x4.Ortho(0f, size, 0f, size, -1f, 1f);
					GL.PushMatrix();
					GL.LoadIdentity();
					GL.LoadProjectionMatrix(projMat);
					RenderTexture.active = _rtex0;
					var paintRect = new Rect(pxPos.x - 0.5f * brushTex.width, pxPos.y - 0.5f * brushTex.height, brushTex.width, brushTex.height);
					Graphics.DrawTexture(paintRect, brushTex, brushMat);
					RenderTexture.active = null;
					GL.PopMatrix();
				}
			}

			for (var i = 0; i < n; i++) {
				Graphics.Blit(_rtex0, _rtex1, diffusionSurfaceMat);
				var tmpRtex = _rtex0; _rtex0 = _rtex1; _rtex1 = tmpRtex;
			}
			foreach (var mat in outputMats)
				mat.mainTexture = _rtex0;
		}
	}
}