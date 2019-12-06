using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp.Aruco;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

[RequireComponent(typeof(RawImage))]
public class WebCamera : WebCameraHandler {

	public static WebCamera instance = null;
	private RawImage rawImage;
	private RectTransform rectTransform;
	private List<Marker> markerObjects = new List<Marker>();

	[Header("Camera settings")]
	[SerializeField] private bool UseMinAspect = true;
	[SerializeField] private List<double> distCoeffs = new List<double>(5);

	[Header("Aruco settings")]
	[SerializeField] private bool debug;
	[SerializeField] private float arucoSquareDim = 0.0273f; //in Meters
	public float ArucoSquareDim { get { return arucoSquareDim; } }

	[Header("Marker options")]
	[SerializeField] private GameObject markerPrefab;
	[SerializeField] private List<MarkerModels> markerModels = new List<MarkerModels>();

	DetectorParameters detectorParameter = DetectorParameters.Create();
	TermCriteria criteria = new TermCriteria(CriteriaType.Eps | CriteriaType.Count, 30, 0.001);
	Point3f[] markerPoints;


	protected override void Awake() {
		if(instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}

		base.Awake();
		rectTransform = gameObject.GetComponent<RectTransform>();
		rawImage = gameObject.GetComponent<RawImage>();

		markerPoints = new Point3f[] {
				new Point3f(-ArucoSquareDim / 2f,  ArucoSquareDim / 2f, 0f),
				new Point3f( ArucoSquareDim / 2f,  ArucoSquareDim / 2f, 0f),
				new Point3f( ArucoSquareDim / 2f, -ArucoSquareDim / 2f, 0f),
				new Point3f(-ArucoSquareDim / 2f, -ArucoSquareDim / 2f, 0f)};

		detectorParameter.DoCornerRefinement = true;
		detectorParameter.CornerRefinementWinSize = 2;
		detectorParameter.ErrorCorrectionRate = .001;

	}

	protected override unsafe Mat ImageProcessing(Mat input) {

		//This should be some inistilazation stuff
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
		//This should be some inistilazation stuff

		Mat greyImage = new Mat();
		Cv2.CvtColor(input, greyImage, ColorConversionCodes.BGR2GRAY);

		int[] markerIds;
		Point2f[][] markerCorners, rejectedCandidates;
		CvAruco.DetectMarkers(greyImage, CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_50), out markerCorners, out markerIds, detectorParameter, out rejectedCandidates);

		for(int i = 0; i < markerIds.Length; i++) {

			double[] markerRvec, markerTvec;

			Cv2.CornerSubPix(greyImage, markerCorners[i], new Size(2, 2), new Size(-1, -1), criteria);
			Cv2.SolvePnP(markerPoints, markerCorners[i], cameraMatrix, distCoeffs, out markerRvec, out markerTvec, false, SolvePnPFlags.Iterative);

			if(debug) {
				CvAruco.DrawDetectedMarkers(input, markerCorners, markerIds, new Scalar(0, 0, 255));
				CvAruco.DrawAxis(input, cameraMatrix, distCoeffs, markerRvec, markerTvec, ArucoSquareDim);
			}

			Marker m = markerObjects.FirstOrDefault(x => x.id == markerIds[i]);
			if(m == null) {
				m = Instantiate(markerPrefab).GetComponent<Marker>();
				m.SetId(markerIds[i]);
				m.SetModel(markerModels.FirstOrDefault(x => x.Id == markerIds[i]).Model);
				markerObjects.Add(m);
			}

			Vector3 positionVector = new Vector3((float)markerTvec[0], -(float)markerTvec[1], (float)markerTvec[2]);
			Vector3 rotationVector = new Vector3(-(float)markerRvec[0], (float)markerRvec[1], -(float)markerRvec[2]);
			m.UpdateMarkerTransform(positionVector, Quaternion.AngleAxis(rotationVector.magnitude * Mathf.Rad2Deg, rotationVector) * Quaternion.Euler(90, 0, 0));
		}

		return input;
	}

	protected override void CameraUpdate(Texture2D output) {
		float aspect = UseMinAspect ? Mathf.Min(Screen.width / (float)output.width, Screen.height / (float)output.height) : Mathf.Max(Screen.width / (float)output.width, Screen.height / (float)output.height);
		rectTransform.sizeDelta = new Vector2(output.width * aspect, output.height * aspect);
		rawImage.texture = output;
	}

	[Serializable]
	public struct MarkerModels {
		public GameObject Model;
		public uint Id { get { return id; } }
		[SerializeField] uint id;
	}
}


