from webcam import Webcam
import cv2
import numpy as np
import json

webcam = Webcam()
webcam.start()

criteria = (cv2.TERM_CRITERIA_EPS + cv2.TERM_CRITERIA_MAX_ITER, 30, 0.001)
objp = np.zeros((6 * 9, 3), np.float32)
objp[:, :2] = np.mgrid[0:9, 0:6].T.reshape(-1, 2)
objpoints = []
imgpoints = []
i = 0

file1 = open("cameraCalibration.txt", "w")

while i < 10:
    image = webcam.get_current_frame()
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    ret, corners = cv2.findChessboardCorners(gray, (9, 6), None)

    if ret:
        cv2.cornerSubPix(gray, corners, (11, 11), (-1, -1), criteria)
        imgpoints.append(corners)
        objpoints.append(objp)
        cv2.drawChessboardCorners(image, (9, 6), corners, ret)
        i += 1

    cv2.imshow('grid', image)
    cv2.waitKey(1000)

cv2.destroyAllWindows()
ret, mtx, dist, rvecs, tvecs = cv2.calibrateCamera(objpoints, imgpoints, gray.shape[::-1], None, None)

# np.savez("webcam_calibration_output", ret=ret, mtx=mtx, dist=dist, rvecs=rvecs, tvecs=tvecs)
rrvecs = []
data = {}
data["return"] = ret
data["cameraMatrix"] = mtx.tolist()
data["distCoeffs"] = dist.tolist()
for i in rvecs:
    rrvecs.append(i.tolist())
data["radialVectors"] = rrvecs

tvecsList = [[[]]]
for i in tvecs:
    tempList = [[]]
    for j in i:
        tempList.append(j.tolist())
    tvecsList.append(tempList)
data["translationVectors"] = tvecsList
with open("data.json", "w") as write_file:
    json.dump(data, write_file, indent=2)
