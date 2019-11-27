//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using OpenCvSharp;

//public class Marcus_Trash_Ref_script : MonoBehaviour
//{

//    public unsafe Mat GreyScale(Mat input) {
//        float weightR = 0.299f;
//        float weightG = 0.114f;
//        float weightB = 0.587f;
//        float grey;
//        Mat[] channel = new Mat[3];
//        channel = input.Split();
//        Debug.Log(input.Width);
//        Debug.Log(input.Height);
//        Debug.Log(channel[1].Width);
//        Debug.Log(channel[1].Height);


//        for (int x = 0; x < 480; x++) {
//            for (int y = 0; y < 640; y++) {
//                grey = (weightR * (*channel[0].Row[x].Col[y].DataPointer)) + (weightG * *channel[1].Row[x].Col[y].DataPointer) + (weightB * *channel[2].Row[x].Col[y].DataPointer);
//                for (int i = 0; i < 3; i++) {
//                    channel[i].Row[x].Col[y].SetTo(grey);
//                }
//            }
//        }
//        OpenCvSharp.Cv2.Merge(channel, input);
//        return input;
//    }

//    public Mat RedTracking(Mat input) {

//        Mat imgThreshold = new Mat();
//        Mat imgHSV = new Mat();
//        Mat red = new Mat();

//        OpenCvSharp.Cv2.CvtColor(input, imgHSV, ColorConversionCodes.BGR2HSV);
//        OpenCvSharp.Cv2.InRange(imgHSV, new Scalar(LowH, LowS, LowV), new Scalar(HighH, HighS, HighV), imgThreshold);

//        OpenCvSharp.Cv2.BitwiseAnd(input, input, red, imgThreshold);
//        ////Opening
//        //OpenCvSharp.Cv2.Erode(red, red, OpenCvSharp.Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5)));
//        //OpenCvSharp.Cv2.Dilate(red, red, OpenCvSharp.Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5)));
//        ////Closing
//        //OpenCvSharp.Cv2.Erode(red, red, OpenCvSharp.Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5)));
//        //OpenCvSharp.Cv2.Dilate(red, red, OpenCvSharp.Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5)));

//        return red;
//    }

//    void CreateMarkers() {
//        Mat outputMarker = new Mat();

//        Aruco.Dictionary markerDictionary = Aruco.CvAruco.GetPredefinedDictionary(Aruco.PredefinedDictionaryName.Dict4X4_50);

//        for (int i = 0; i < 50; i++) {
//            Aruco.CvAruco.DrawMarker(markerDictionary, i, 500, outputMarker, 1);

//            string imageName = "4x4Marker_";
//            string convert = imageName + i.ToString() + ".jpg";
//            OpenCvSharp.Cv2.ImWrite(convert, outputMarker);
//            Debug.Log(i.ToString());

//        }
//    }

//    void createKnownBoardPosition(Size boardSize, float squareEdgeLength, out List<List<Point3f>> corners) {
//        List<Point3f> temp = new List<Point3f>();
//        for (int i = 0; i < boardSize.Height; i++) {
//            for (int j = 0; j < boardSize.Width; j++) {
//                temp.Add(new Point3f(j * squareEdgeLength, i * squareEdgeLength, 0.0f));
//            }
//        }
//        corners = new List<List<Point3f>>() { temp };
//    }

//    void getChessboardCorners(List<Mat> images, List<List<Point2f>> foundCorners, bool showResults = false) {

//        foreach (Mat item in images) {
//            Point2f[] pointBuf;
//            bool isFoundChessboard = Cv2.FindChessboardCorners(item, chessBoard, out pointBuf);

//            if (isFoundChessboard) {
//                foundCorners.Add(new List<Point2f>(pointBuf));
//            }
//            if (showResults) {
//                Cv2.DrawChessboardCorners(item, chessBoard, pointBuf, isFoundChessboard);
//            }
//        }
//    }


//    void cameraCalibration(List<Mat> calibrationImages, Size boardSize, float squareEdgeLength, double[,] cameraMatix, List<double> distCoef) {
//        List<List<Point2f>> chessboardImagePoints = new List<List<Point2f>>();
//        getChessboardCorners(calibrationImages, chessboardImagePoints, false);

//        List<List<Point3f>> worldCornerPoints = new List<List<Point3f>>();
//        Debug.Log(worldCornerPoints.Count);

//        if (worldCornerPoints != null) {
//            createKnownBoardPosition(boardSize, squareEdgeLength, out worldCornerPoints);
//        }
//        int size = worldCornerPoints.Count;
//        int newSize = chessboardImagePoints.Count;
//        if (newSize < size) {
//            worldCornerPoints.RemoveRange(newSize, size - newSize);
//        } else if (newSize > size) {
//            if (newSize > worldCornerPoints.Capacity) {
//                worldCornerPoints.Capacity = size;
//            }

//            worldCornerPoints.AddRange(Enumerable.Repeat(worldCornerPoints[0], newSize - size));
//        }

//        Vec3d tempRvec, tempTvec;

//        tempRvec.Item0 = rvec[0]; tempRvec.Item1 = rvec[1]; tempRvec.Item2 = rvec[2];
//        tempTvec.Item0 = tvec[0]; tempTvec.Item1 = tvec[1]; tempTvec.Item2 = tvec[2];

//        Cv2.CalibrateCamera(worldCornerPoints, chessboardImagePoints, chessBoard, cameraMatrix, distCoef.ToArray(), out tempRvec, out tempTvec);
//    }

//    bool saveCameraCalibration(string name, double[,] cameraMatrix, List<double> distCoef) {
//        string path = "Assets/Bin/";

//        StreamWriter writer = new StreamWriter(path + name);

//        Debug.Log("I tried!");
//        for (int i = 0; i < cameraMatrix.GetLength(0); i++) {
//            for (int j = 0; j < cameraMatrix.GetLength(1); j++) {
//                double value = cameraMatrix[i, j];
//                writer.WriteLine(value.ToString() + "\n");
//            }
//        }

//        for (int j = 0; j < distCoef.Count; j++) {
//            double value = distCoef[j];
//            writer.WriteLine(value.ToString() + "\n");

//        }
//        Debug.Log("Camera Calibration saved!");
//        writer.Close();
//        return true;
//    }

//}
//}
