using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp.Aruco;
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
        Mat image = new Mat();
        Mat poseEstimation = new Mat();
        public Texture2D treatedImg;

		public CalibrationData calibrationData = new CalibrationData();

		//Camera calibration
		bool cameraCalibrated;
        //Mat cameraMatrix = new Mat();
        //Mat distCoeffs = new Mat();
        
        //Aruco Markers
        int[] markerIds;
        Point2f[][] markerCorners, rejectedCandidates;
        Dictionary dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_50);
        const float calibSquareDim = 0.024f; //in Meters
        const float arucoSquareDim = 0.0246f; //in Meters
        Size chessBoard = new Size(6, 9);

        [SerializeField] private bool UseMinAspect = true;

        //JSON Variables
        //private string jsonString;
        CameraInfo myCamInfo = new CameraInfo();
        


    protected override void Awake() {
            base.Awake();
            rectTransform = gameObject.GetComponent<RectTransform>();
            rawImage = gameObject.GetComponent<RawImage>();
            //(jsonString = File.ReadAllText(Application.dataPath + "/CameraCalibration/data.json");
            //Debug.Log(jsonString);
            //myCamInfo = JsonUtility.FromJson<CameraInfo>(File.ReadAllText(Application.dataPath + "/CameraCalibration/data.json"));
            //Debug.Log(myCamInfo.cameraMatrix);
			//string[] strings = jsonString.Split(',');
			//foreach (var word in strings) {
			//    Debug.Log($"<{word}>");
			//}
			if (File.Exists(Application.dataPath + "/CameraCalibration/data.json")) {
				calibrationData = JsonConvert.DeserializeObject<CalibrationData>(File.ReadAllText(Application.dataPath + "/CameraCalibration/data.json"));
			} else {
				Debug.Log("Camera Calibration data not found");
			}
			
        }


        protected override unsafe void CamUpdate(WebCamTexture input) {
            float aspect = UseMinAspect ? Mathf.Min(Screen.width / (float)input.width, Screen.height / (float)input.height) : Mathf.Max(Screen.width / (float)input.width, Screen.height / (float)input.height);
            rectTransform.sizeDelta = new Vector2(input.width * aspect, input.height * aspect);

            image = Unity.TextureToMat(input);
            image.CvtColor(ColorConversionCodes.BGR2GRAY);
            Dictionary dictionary = Aruco.CvAruco.GetPredefinedDictionary(Aruco.PredefinedDictionaryName.Dict4X4_50);
            CvAruco.DetectMarkers(image, dictionary, out markerCorners, out markerIds, DetectorParameters.Create(), out rejectedCandidates);
            CvAruco.DrawDetectedMarkers(image, markerCorners, markerIds, new Scalar(0, 0, 255));

            List<Point2f> foundPoints = new List<Point2f>();

            Point2f[] foundPointsArray;
            bool foundPatterns = false;

            foundPatterns = Cv2.FindChessboardCorners(image, chessBoard, out foundPointsArray);
            foundPointsArray.ToList();
            //Cv2.DrawChessboardCorners(image, chessBoard, foundPoints, foundPatterns);
            Point2f[] corners2 = Cv2.CornerSubPix(image, foundPointsArray, new Size(11, 11), new Size(-1, -1), TermCriteria.Both(30, 0.001));
            //DrawOnInput(image, corners2);
            //if (foundPatterns) {
            //    treatedImg = Unity.MatToTexture((image), treatedImg);
            //    rawImage.texture = treatedImg;
            //} else {
            //    treatedImg = Unity.MatToTexture(image), treatedImg);
            //    rawImage.texture = treatedImg;
            //}
            treatedImg = Unity.MatToTexture((image), treatedImg);
            rawImage.texture = treatedImg;


            if (Input.GetKeyDown(KeyCode.Space)) {
                //get images

            }
            if (cameraCalibrated = false && Input.GetKeyDown(KeyCode.Return)) {
                //calibrate
                //CalibrateTheWebCamera();
            }
            if (Input.GetKey("escape")) {
                //exit
                return;
            }

        }
        public void CalibrateTheWebCamera(List<Mat> calibrationImages) {

            Size imgSize; imgSize.Height = calibrationImages[0].Height; imgSize.Width = calibrationImages[0].Width;
            Size regionSize = new Size(4, 4);
            Size rectangleSize = new Size(20f, 20f);

            List<Mat> imgPoints = new List<Mat>();
            List<Mat> objPoints = new List<Mat>();

            Mat tempMat = new Mat();

            Mat objPoint = new Mat();
            for (int i = 0; i < chessBoard.Height; i++) {
                for (int j = 0; j < chessBoard.Width; j++) {

                    //objPoint.Add(new Point3f(j * rectangleSize.Width, i * rectangleSize.Height, 0.0f);
                }
            }

            Mat[] radVector, transVector = new Mat[3];

            //Cv2.CalibrateCamera(objPoints, imgPoints, imgSize, cameraMatrix, distCoeffs, out radVector, out transVector);
        }

        public void DrawOnInput(Mat image, Point2f[] corners) {

            Cv2.Line(image, corners[0], corners[0], new Scalar(255, 0, 0));
            Cv2.Line(image, corners[2], corners[2], new Scalar(255, 0, 0));
            Cv2.Line(image, corners[3], corners[3], new Scalar(255, 0, 0));
        }
    }

	[System.Serializable]
	public class CalibrationData {
		public float ret;
		public float[][] cameraMatrix;
		public List<List<float>> distCoeffs;
		public List<List<List<float>>> radialVectors;
		public List<List<List<float>>> translationVectors;
	}
}


