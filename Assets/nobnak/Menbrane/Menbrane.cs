using UnityEngine;
using System.Collections;

namespace nobnak.Menbrane {

	public class Menbrane : MonoBehaviour {
		public const string SHADER_DT = "_Dt";
		public const string SHADER_DX = "_Dx";
		public const string SHADER_T = "_T";

		public Material sim;
		public Material[] outputMats;
		public Material brushMat;
		public Texture2D brushTex;
		public int nGrids = 256;
		public float l = 4;
		public int fps = 30;
		public float tension = 3;
		public float density = 10;

		private RenderTexture _vhTex0, _vhTex1;
		private float _dt;
		private float _tPrev = 0f;

		void Start() {
			_vhTex0 = new RenderTexture(nGrids, nGrids, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
			_vhTex1 = new RenderTexture(nGrids, nGrids, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
			_vhTex0.wrapMode = _vhTex1.wrapMode = TextureWrapMode.Repeat;
			_vhTex0.Create();
			_vhTex1.Create();

			RenderTexture.active = _vhTex0;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = _vhTex1;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = null;

			_tPrev = Time.timeSinceLevelLoad;
		}

		void Update() {
			var dx = l / nGrids;
			var m = density * dx * dx;
			_dt = 1f / fps;
			var nSteps = Mathf.FloorToInt((Time.timeSinceLevelLoad - _tPrev) / _dt);
			_tPrev += nSteps * _dt;

			sim.SetFloat(SHADER_DX, dx);
			sim.SetFloat(SHADER_DT, _dt);
			sim.SetFloat(SHADER_T, tension / m);

			if (Input.GetMouseButton(0)) {
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (collider.Raycast(ray, out hit, float.MaxValue)) {
					var pxPos = nGrids * hit.textureCoord;
					var projMat = Matrix4x4.Ortho(0f, nGrids, 0f, nGrids, -1f, 1f);
					GL.PushMatrix();
					GL.LoadIdentity();
					GL.LoadProjectionMatrix(projMat);
					RenderTexture.active = _vhTex0;
					var paintRect = new Rect(pxPos.x - 0.5f * brushTex.width, pxPos.y - 0.5f * brushTex.height, brushTex.width, brushTex.height);
					Graphics.DrawTexture(paintRect, brushTex, brushMat);
					RenderTexture.active = null;
					GL.PopMatrix();
				}
			}


			for (var i = 0; i < nSteps; i++) {
				Graphics.Blit(_vhTex0, _vhTex1, sim);
				Swap();
			}
			foreach (var mat in outputMats) {
				mat.mainTexture = _vhTex0;
				mat.SetFloat(SHADER_DX, dx);
			}
		}
		void Swap() {
			var tmpRtex = _vhTex0; _vhTex0 = _vhTex1; _vhTex1 = tmpRtex;
		}
	}
}