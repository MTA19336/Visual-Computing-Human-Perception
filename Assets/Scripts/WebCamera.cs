using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebCamera : WebCameraHandler {

	private RawImage rawImage;
	private RectTransform rectTransform;
	private ORB orb;
	private BFMatcher bfMatcher;
	private FlannBasedMatcher flannMatcher;
	private List<orbImage> orbImagePatterns;

	[Header("Camera Settings")]
	[SerializeField] private bool useMinAspect = true;
	[SerializeField] private List<Texture2D> patterns = new List<Texture2D>();

	[Header("ORB Settings")]
	[SerializeField] private int nFeatures = 500;
	[SerializeField] private float scalefactor = 1.2f;
	[SerializeField] private int nLevels = 8;
	[SerializeField] private int edgeThreshold = 31;
	[SerializeField] private int firstLevel = 0;
	[SerializeField] private int wtaK = 2;
	[SerializeField] private ORBScore scoreType = ORBScore.Harris;
	[SerializeField] private int patchsize = 31;

	[Header("FlannMatcher settings")]
	[SerializeField] private int checks = 32;
	[SerializeField] private float eps = 0;
	[SerializeField] private bool sorted = true;
	[Header("BruteForceMatcher settings")]
	[SerializeField] private NormTypes normType = NormTypes.Hamming;
	[Header("Matcher Filter settings")]
	[SerializeField] [Range(0.1f, 1f)] private float ratioThresh = 0.75f;
	[SerializeField] private bool drawMatches = true;
	[SerializeField] private int minMatches = 10;
	

	[Header("FindHomography settings")]
	[SerializeField] HomographyMethods homographyMethod = HomographyMethods.Ransac;
	[SerializeField] double ransacReprojThreshold = 5;

	protected override void Awake() {
		base.Awake();
		rectTransform = gameObject.GetComponent<RectTransform>();
		rawImage = gameObject.GetComponent<RawImage>();

		ReloadOrb();
	}

	protected override unsafe Mat ImageProcessing(Mat input) {
		///Detect keypoints of the input image
		orbImage orbImageInput = new orbImage(input, orb);


		///Check if any features are detected in the input image
		if(orbImageInput.KeyPoints.Length == 0) {
			Debug.Log("No features found with camera");
			return input;
		}

		Mat output = input;
		for(int i = 0; i < orbImagePatterns.Count; i++) {

			///Feature matching using brute force feature matcher, should use flann
			List<DMatch> bestDMatches = new List<DMatch>();
			DMatch[][] KnnMatch = bfMatcher.KnnMatch(orbImagePatterns[i].descriptors.GetMat(), orbImageInput.descriptors.GetMat(), 2);

			///Filter matches using the Lowe's ratio test
			List<DMatch> listOfGoodMatches = new List<DMatch>();
			for(int j = 0; j < KnnMatch.Length; j++) {
				if(KnnMatch[j][0].Distance < ratioThresh * KnnMatch[j][1].Distance) {
					bestDMatches.Add(KnnMatch[j][0]);
				}
			}


			///Draw matches
			if(drawMatches) {
				Cv2.DrawMatches(orbImagePatterns[i].imageMat, orbImagePatterns[i].KeyPoints, orbImageInput.imageMat, orbImageInput.KeyPoints, bestDMatches, output);
			}


			///Disregard if not enough matches
			if (bestDMatches.Count < minMatches) {
				continue;
			}

			///Localize the object
			List<Point2d> obj = new List<Point2d>();
			List<Point2d> scene = new List<Point2d>();
			for(int j = 0; j < bestDMatches.Count; j++) {
				///Get the keypoints from the good matches
				obj.Add(new Point2d(orbImagePatterns[i].KeyPoints[bestDMatches.ElementAt(j).QueryIdx].Pt.X, orbImagePatterns[i].KeyPoints[bestDMatches.ElementAt(j).QueryIdx].Pt.Y));
				scene.Add(new Point2d(orbImageInput.KeyPoints[bestDMatches.ElementAt(j).TrainIdx].Pt.X, orbImageInput.KeyPoints[bestDMatches.ElementAt(j).TrainIdx].Pt.Y));
			}
			Mat H = Cv2.FindHomography(obj, scene, homographyMethod, ransacReprojThreshold);


			///Check if Homography mat isnt empty
			if(H.Empty()) {
				continue;
			} else {
				Debug.Log("Found" + patterns[i].name);
			}


			///Get the corners from the input image
			List<Point2f> objCorners = new List<Point2f>();
			objCorners.Add(new Point2f(0, 0));
			objCorners.Add(new Point2f((float)orbImagePatterns[i].imageMatGray.Cols, 0));
			objCorners.Add(new Point2f((float)orbImagePatterns[i].imageMatGray.Cols, (float)orbImagePatterns[i].imageMatGray.Rows));
			objCorners.Add(new Point2f(0, (float)orbImagePatterns[i].imageMatGray.Rows));
			Point2f[] sceneCorners = Cv2.PerspectiveTransform(objCorners, H);


			///Draw lines between the corners of the object in the input image
			int offset = drawMatches ? orbImagePatterns[i].imageMatGray.Cols : 0;
			Cv2.Line(output, sceneCorners[0] + new Point2f(offset, 0), sceneCorners[1] + new Point2f(offset, 0), orbImagePatterns[i].color, 4);
			Cv2.Line(output, sceneCorners[1] + new Point2f(offset, 0), sceneCorners[2] + new Point2f(offset, 0), orbImagePatterns[i].color, 4);
			Cv2.Line(output, sceneCorners[2] + new Point2f(offset, 0), sceneCorners[3] + new Point2f(offset, 0), orbImagePatterns[i].color, 4);
			Cv2.Line(output, sceneCorners[3] + new Point2f(offset, 0), sceneCorners[0] + new Point2f(offset, 0), orbImagePatterns[i].color, 4);
		}

		return output;
	}
	protected override void CameraUpdate(Texture2D output) {
		//Make the rawImage gameobject fit the screen based on the min or max aspect
		float aspect = useMinAspect ? Mathf.Min(Screen.width / (float)output.width, Screen.height / (float)output.height) : Mathf.Max(Screen.width / (float)output.width, Screen.height / (float)output.height);
		rectTransform.sizeDelta = new Vector2(output.width * aspect, output.height * aspect);

		//Update rawImage texture with output
		rawImage.texture = output;
	}

	[ContextMenu("Reload Orb")]
	protected void ReloadOrb() {
		orb = ORB.Create(nFeatures, scalefactor, nLevels, edgeThreshold, firstLevel, wtaK, scoreType, patchsize);

		flannMatcher = new FlannBasedMatcher(new OpenCvSharp.Flann.IndexParams(), new OpenCvSharp.Flann.SearchParams(checks, eps, sorted));
		bfMatcher = new BFMatcher(normType, false);
		orbImagePatterns = new List<orbImage>();
		foreach(Texture2D pattern in patterns) {
			orbImagePatterns.Add(new orbImage(OpenCvSharp.Unity.TextureToMat(pattern), orb));
		}
	}
}