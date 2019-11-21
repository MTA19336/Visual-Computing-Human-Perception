using OpenCvSharp;
using UnityEngine;

public class orbImage {
	public Scalar color { get; private set; }
	public Mat imageMat { get; private set; }
	public Mat imageMatGray { get; private set; }
	public OutputArray descriptors { get; private set; }
	public KeyPoint[] KeyPoints { get => keyPoints; }
	private KeyPoint[] keyPoints;

	public orbImage(Mat img, ORB o) {
		imageMat = img;
		imageMatGray = imageMat.CvtColor(ColorConversionCodes.BGR2GRAY);
		descriptors = new OutputArray(new Mat());
		o.DetectAndCompute(imageMatGray, null, out keyPoints, descriptors);
		color = new Scalar(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
	}
}
