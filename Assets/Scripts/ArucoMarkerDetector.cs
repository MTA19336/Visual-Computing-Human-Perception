using UnityEngine;
using OpenCvSharp.Aruco;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

public class ArucoMarkerDetector : WebCameraHandler {

	public static ArucoMarkerDetector instance = null;
	private List<MarkerObject> markerObjects = new List<MarkerObject>();

	double[,] cameraMatrix;
	DetectorParameters detectorParameter = DetectorParameters.Create();
	TermCriteria criteria = new TermCriteria(CriteriaType.Eps | CriteriaType.Count, 30, 0.001);
	Point3f[] markerPoints;
	Dictionary markerDictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_50);
	private bool FirstImage = true;

	[Header("Camera settings")]
	[SerializeField] private List<double> distCoeffs = new List<double>(5);

	[Header("Aruco settings")]
	[SerializeField] private bool debug;
	[SerializeField] private float arucoSquareDim = 0.0273f; //in Meters

	[Header("Marker options")]
	[SerializeField] private GameObject markerPrefab;
	[SerializeField] private List<MarkerModels> markerModels = new List<MarkerModels>();

	protected override void Awake() {
		if(instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}

		base.Awake();

		markerPoints = new Point3f[] {
				new Point3f(-arucoSquareDim / 2f,  arucoSquareDim / 2f, 0f),
				new Point3f( arucoSquareDim / 2f,  arucoSquareDim / 2f, 0f),
				new Point3f( arucoSquareDim / 2f, -arucoSquareDim / 2f, 0f),
				new Point3f(-arucoSquareDim / 2f, -arucoSquareDim / 2f, 0f)};

		detectorParameter.DoCornerRefinement = true;
		detectorParameter.CornerRefinementWinSize = 3;
		detectorParameter.ErrorCorrectionRate = .001;
	}

	protected override unsafe Mat ImageProcessing(Mat input) {
		if(FirstImage) {
			double max_wh = Math.Max(input.Cols, input.Rows);
			double fx = max_wh;
			double fy = max_wh;
			double cx = input.Cols / 2.0d;
			double cy = input.Rows / 2.0d;
			cameraMatrix = new double[3, 3] {
				{fx, 0d, cx},
				{0d, fy, cy},
				{0d, 0d, 1d}
			};
			FirstImage = false;
		}

		Mat greyImage = new Mat();
		Cv2.CvtColor(input, greyImage, ColorConversionCodes.BGR2GRAY);

		int[] markerIds;
		Point2f[][] markerCorners, rejectedCandidates;
		CvAruco.DetectMarkers(greyImage, markerDictionary, out markerCorners, out markerIds, detectorParameter, out rejectedCandidates);

		for(int i = 0; i < markerIds.Length; i++) {

			double[] markerRvec, markerTvec;
			Cv2.SolvePnP(markerPoints, markerCorners[i], cameraMatrix, distCoeffs, out markerRvec, out markerTvec, false, SolvePnPFlags.Iterative);

			if(debug) {
				CvAruco.DrawDetectedMarkers(input, markerCorners, markerIds, new Scalar(0, 0, 255));
				CvAruco.DrawAxis(input, cameraMatrix, distCoeffs, markerRvec, markerTvec, arucoSquareDim);
			}

			MarkerObject m = markerObjects.FirstOrDefault(x => x.id == markerIds[i]);
			if(m == null) {
				m = Instantiate(markerPrefab).GetComponent<MarkerObject>();
				m.SetId(markerIds[i]);
				m.SetModel(markerModels.FirstOrDefault(x => x.Id == markerIds[i]).Model, arucoSquareDim);
				markerObjects.Add(m);
			}

			Vector3 positionVector = new Vector3((float)markerTvec[0], -(float)markerTvec[1], (float)markerTvec[2]);
			Vector3 rotationVector = new Vector3(-(float)markerRvec[0], (float)markerRvec[1], -(float)markerRvec[2]);
			m.UpdateMarkerTransform(positionVector, Quaternion.AngleAxis(rotationVector.magnitude * Mathf.Rad2Deg, rotationVector) * Quaternion.Euler(90, 0, 0));
		}

		return input;
	}

	[Serializable]
	public struct MarkerModels {
		public GameObject Model;
		public uint Id { get { return id; } }
		[SerializeField] uint id;
	}
}


