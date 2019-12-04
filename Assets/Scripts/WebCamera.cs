using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp.Aruco;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace OpenCvSharp.Demo {
	[RequireComponent(typeof(RawImage))]
	public class WebCamera : WebCameraHandler {

		private RawImage rawImage;
		[Range(0, 179)] public int LowH = 0, HighH = 179;
		[Range(0, 255)] public int LowS = 42, HighS = 255, LowV = 0, HighV = 255;
		[SerializeField] GameObject obj;
		[SerializeField] private bool UseMinAspect = true;
		[SerializeField] public bool debug;

		private RectTransform rectTransform;
		public CalibrationData calibrationData = new CalibrationData();
		Vector3 oldPos = Vector3.zero, oldRot = Vector3.zero;
		public int i = 0;

		//Aruco Markers
		int[] markerIds;
		private Point2f[][] markerCorners, rejectedCandidates;
		Dictionary dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_50);
		const float arucoSquareDim = 0.0273f; //in Meters
		private double[] markerRvec, markerTvec;
		private List<Matrix4x4> markerTransforms = new List<Matrix4x4>();


		//mutiple marker testing
		[SerializeField] private GameObject markerPrefab;
		[SerializeField] private List<MarkerModels> markerModels = new List<MarkerModels>();
		private List<Marker> markerObjects = new List<Marker>();

		protected override void Awake() {
			base.Awake();
			rectTransform = gameObject.GetComponent<RectTransform>();
			rawImage = gameObject.GetComponent<RawImage>();
			if(File.Exists(Application.dataPath + "/CameraCalibration/data.json")) {
				calibrationData = JsonConvert.DeserializeObject<CalibrationData>(File.ReadAllText(Application.dataPath + "/CameraCalibration/data.json"));
			} else {
				Debug.Log("Camera Calibration data not found");
			}
		}

		protected override unsafe Mat ImageProcessing(Mat input) {

			//This should be some inistilazation stuff
			Point3f[] markerPoints = new Point3f[] {
				new Point3f(-arucoSquareDim / 2f,  arucoSquareDim / 2f, 0f),
				new Point3f( arucoSquareDim / 2f,  arucoSquareDim / 2f, 0f),
				new Point3f( arucoSquareDim / 2f, -arucoSquareDim / 2f, 0f),
				new Point3f(-arucoSquareDim / 2f, -arucoSquareDim / 2f, 0f)
			};
			double max_wh = Math.Max(input.Cols, input.Rows);
			double fx = max_wh;
			double fy = max_wh;
			double cx = input.Cols / 2.0d;
			double cy = input.Rows / 2.0d;
			double[,] cameraMatrix = new double[3, 3] {
				{fx, 0d, cx},
				{0d, fy, cy},
				{0d, 0d, 1d}
			};
			double[] distCoeffs = new double[4] { 0d, 0d, 0d, 0d };
			//This should be some inistilazation stuff



			CvAruco.DetectMarkers(input.CvtColor(ColorConversionCodes.BGR2GRAY), CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_50), out markerCorners, out markerIds, DetectorParameters.Create(), out rejectedCandidates);

			for(int i = 0; i < markerIds.Length; i++) {
				Cv2.SolvePnP(markerPoints, markerCorners[i], cameraMatrix, distCoeffs, out markerRvec, out markerTvec, false, SolvePnPFlags.Iterative);

				if(debug) {
					CvAruco.DrawDetectedMarkers(input, markerCorners, markerIds, new Scalar(0, 0, 255));
					CvAruco.DrawAxis(input, cameraMatrix, distCoeffs, markerRvec, markerTvec, arucoSquareDim);
				}

				Marker m = markerObjects.FirstOrDefault(x => x.id == markerIds[i]);
				if(m == null) {
					m = Instantiate(markerPrefab).GetComponent<Marker>();
					m.SetId(markerIds[i]);
					m.SetModel(markerModels.FirstOrDefault(x => x.Id == i).Model);
					markerObjects.Add(m);
				}

				Vector3 positionVector = new Vector3((float)markerTvec[0], (float)-markerTvec[1], (float)markerTvec[2]);
				Vector3 rotationVector = new Vector3(-(float)markerRvec[0], (float)markerRvec[1], -(float)markerRvec[2]);
				m.UpdateMarkerTransform(positionVector, rotationVector);
			}

			return input;
		}

		protected override void CameraUpdate(Texture2D output) {
			float aspect = UseMinAspect ? Mathf.Min(Screen.width / (float)output.width, Screen.height / (float)output.height) : Mathf.Max(Screen.width / (float)output.width, Screen.height / (float)output.height);
			rectTransform.sizeDelta = new Vector2(output.width * aspect, output.height * aspect);
			rawImage.texture = output;
		}
	}

	[Serializable]
	public class CalibrationData {
		public float ret;
		public double[,] cameraMatrix;
		public List<double> distCoeffs;
		public double[][] radialVectors;
		public double[][] translationVectors;
	}

	[Serializable]
	public struct MarkerModels {
		public GameObject Model;
		public uint Id { get { return id; } }
		[SerializeField] uint id;
	}
}


