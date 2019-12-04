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
        [Serializable]
        public class MarkerObject {
            public int markerId;
            public GameObject markerPrefab;
        }

        public class MarkerOnScene {
            public int bestMatchIndex = -1;
            public float destroyAt = -1f;
            public GameObject gameObject = null;
        }

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
        Size chessBoard = new Size(6, 9);

        double[] markerRvec, markerTvec;
        double[,] rotationMatrix;
        private List<Matrix4x4> markerTransforms = new List<Matrix4x4>();
        [SerializeField] GameObject obj;

        [SerializeField] private bool UseMinAspect = true;

        protected override void Awake() {
            base.Awake();
            rectTransform = gameObject.GetComponent<RectTransform>();
            rawImage = gameObject.GetComponent<RawImage>();
            if (File.Exists(Application.dataPath + "/CameraCalibration/data.json")) {
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
            double cx = Screen.width / 2.0d;
            double cy = Screen.height / 2.0d;
            double[,] cameraMatrix = new double[3, 3] {
                {fx, 0d, cx},
                {0d, fy, cy},
                {0d, 0d, 1d}
            };
            Mat grey = input.CvtColor(ColorConversionCodes.BGR2GRAY);
            Dictionary dictionary = CvAruco.GetPredefinedDictionary(Aruco.PredefinedDictionaryName.Dict4X4_50);
            CvAruco.DetectMarkers(grey, dictionary, out markerCorners, out markerIds, DetectorParameters.Create(), out rejectedCandidates);
            CvAruco.DrawDetectedMarkers(input, markerCorners, markerIds, new Scalar(0, 0, 255));

            List<int> result = new List<int>();
            for (int i = 0; i < markerIds.Length; i++) {

                Cv2.SolvePnP(markerPoints, markerCorners[i], cameraMatrix, calibrationData.distCoeffs.ToArray(), out markerRvec, out markerTvec, false, SolvePnPFlags.Iterative);
                Debug.Log(markerRvec);
                CvAruco.DrawAxis(input, cameraMatrix, calibrationData.distCoeffs, markerRvec, markerTvec, arucoSquareDim);
                obj.transform.position = new Vector3((float)markerTvec[0], (float)-markerTvec[1], (float)markerTvec[2]);

                Cv2.Rodrigues(markerRvec, out rotationMatrix);
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetRow(0, new Vector4((float)rotationMatrix[0, 0], (float)rotationMatrix[0, 1], (float)rotationMatrix[0, 2], (float)markerTvec[0]));
                matrix.SetRow(1, new Vector4((float)rotationMatrix[1, 0], (float)rotationMatrix[1, 1], (float)rotationMatrix[1, 2], (float)markerTvec[1]));
                matrix.SetRow(2, new Vector4((float)rotationMatrix[2, 0], (float)rotationMatrix[2, 1], (float)rotationMatrix[2, 2], (float)markerTvec[2]));
                matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));


                result.Add(markerIds[i]);
                markerTransforms.Add(matrix);
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


