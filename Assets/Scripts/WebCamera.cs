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
		public int i = 0;
		[Range(0, 179)] public int LowH = 0, HighH = 179;
		[Range(0, 255)] public int LowS = 42, HighS = 255, LowV = 0, HighV = 255;
		private RectTransform rectTransform;

		public CalibrationData calibrationData = new CalibrationData();

		//Aruco Markers
		int[] markerIds;
		private Point2f[][] markerCorners, rejectedCandidates;
		Dictionary dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_50);

		const float arucoSquareDim = 0.0273f; //in Meters

		double[] markerRvec, markerTvec;
		double[,] rotationMatrix;
		private List<Matrix4x4> markerTransforms = new List<Matrix4x4>();
		[SerializeField] GameObject obj;

		[SerializeField] private bool UseMinAspect = true;
		[SerializeField] public bool debug;

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

			List<int> markerIdList = Detector(input);

			//int count = 0;
			//foreach (MarkerObject markerObject in markers) {
			//    List<int> foundMarkers = new List<int>();
			//    for (int i = 0; i < markerIdList; i++) {
			//        if (markerIdList[i] == markerObject.markerId) {
			//            foundMarkers.Add(i);
			//            count++;
			//        }
			//    }
			//    int index;

			//    index = gameObjects.Count - 1;
			//    while (index >= 0) {
			//        MarkerOnScene markerOnScene = gameObject[index];
			//        markerOnScene.bestMatchIndex = -1;
			//        if (markerOnScene.destroyAt > 0 && markerOnScene.destroyAt < Time.fixedTime) {
			//            Destroy(markerOnScene.gameObject);
			//            gameObjects.RemoveAt(index);
			//        }

			//    }
			//}

			return input;
		}

		protected override void CameraUpdate(Texture2D output) {
			float aspect = UseMinAspect ? Mathf.Min(Screen.width / (float)output.width, Screen.height / (float)output.height) : Mathf.Max(Screen.width / (float)output.width, Screen.height / (float)output.height);
			rectTransform.sizeDelta = new Vector2(output.width * aspect, output.height * aspect);
			rawImage.texture = output;
		}
		protected List<int> Detector(Mat input) {
			Point3f[] markerPoints = new Point3f[] {
				new Point3f(-arucoSquareDim / 2f,  arucoSquareDim / 2f, 0f),
				new Point3f( arucoSquareDim / 2f,  arucoSquareDim / 2f, 0f),
				new Point3f( arucoSquareDim / 2f, -arucoSquareDim / 2f, 0f),
				new Point3f(-arucoSquareDim / 2f, -arucoSquareDim / 2f, 0f)
			};
			double max_wh = (double)Math.Max(input.Cols, input.Rows);
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
			Mat grey = input.CvtColor(ColorConversionCodes.BGR2GRAY);
			Dictionary dictionary = CvAruco.GetPredefinedDictionary(Aruco.PredefinedDictionaryName.Dict4X4_50);
			CvAruco.DetectMarkers(grey, dictionary, out markerCorners, out markerIds, DetectorParameters.Create(), out rejectedCandidates);




			List<int> result = new List<int>();
			for(int i = 0; i < markerIds.Length; i++) {
				//Cv2.Find4QuadCornerSubpix(grey, markerCorners[i], new Size(1, 1));
				Cv2.SolvePnP(markerPoints, markerCorners[i], cameraMatrix, distCoeffs, out markerRvec, out markerTvec, false, SolvePnPFlags.Iterative);

				if(debug) {
					CvAruco.DrawDetectedMarkers(input, markerCorners, markerIds, new Scalar(0, 0, 255));
					CvAruco.DrawAxis(input, cameraMatrix, distCoeffs, markerRvec, markerTvec, arucoSquareDim);
				}
				
				Vector3 rotationVector = new Vector3(-(float)markerRvec[0], (float)markerRvec[1], -(float)markerRvec[2]);
				obj.transform.rotation = Quaternion.AngleAxis(rotationVector.magnitude * Mathf.Rad2Deg, rotationVector);
				obj.transform.Rotate(Vector3.right*90);

				obj.transform.position = new Vector3((float)markerTvec[0], (float)-markerTvec[1], (float)markerTvec[2]) + (obj.transform.up * (obj.GetComponent<Renderer>().bounds.size.y / 2));
			}
			return result;
		}
	}

	[System.Serializable]
	public class CalibrationData {
		public float ret;
		public double[,] cameraMatrix;
		public List<double> distCoeffs;
		public double[][] radialVectors;
		public double[][] translationVectors;
	}
}


