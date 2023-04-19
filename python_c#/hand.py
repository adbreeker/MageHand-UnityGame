import mediapipe as mp
import cv2


class HandLandmarkerDetector:
    def __init__(self):
        self.mp = mp
        self.cap = None
        self.landmarker = None
        self.options = None
        
        self.base_options = mp.tasks.BaseOptions
        self.hand_landmarker = mp.tasks.vision.HandLandmarker
        self.hand_landmarker_options = mp.tasks.vision.HandLandmarkerOptions
        self.hand_landmarker_result = mp.tasks.vision.HandLandmarkerResult
        self.vision_running_mode = mp.tasks.vision.RunningMode

        self.x , self.y, self.z = 0, 0, 0

    def run(self):
        if self.landmarker is None:
            self._initialize()

        while self.cap.isOpened():
            success, image = self.cap.read()
            if not success:
                print("Ignoring empty camera frame.")
                continue

            timestamp_ms = self.cap.get(cv2.CAP_PROP_POS_MSEC)

            image.flags.writeable = False
            mp_image = self.mp.Image(
                image_format=self.mp.ImageFormat.SRGB, data=image)
            self.landmarker.detect_async(
                mp_image, timestamp_ms=int(timestamp_ms))
            
            print(f"x = {self.x:.3f}, y = {self.y:.3f}, z = {self.z:.3f}")

            image = cv2.circle(image, (int(self.x * 640), int(self.y * 480)), 5, (0, 0, 255), -1)

            cv2.imshow('MediaPipe Hands', cv2.flip(image, 1))
            if cv2.waitKey(5) & 0xFF == 27:
                break

        self.cap.release()
        self.cv2.destroyAllWindows()

    def _callback(self, result, output_image: mp.Image, timestamp_ms: int):
        if result.hand_landmarks != []:
            self.x = result.hand_landmarks[0][7].x
            self.y = result.hand_landmarks[0][7].y
            self.z = result.hand_landmarks[0][7].z

    def _initialize(self):
        self.cap = cv2.VideoCapture(0)

        self.options = self.hand_landmarker_options(
            base_options=self.base_options(
                model_asset_path='hand_landmarker.task'),
            running_mode=self.vision_running_mode.LIVE_STREAM,
            result_callback=self._callback)

        self.landmarker = self.hand_landmarker.create_from_options(
            self.options)



detector = HandLandmarkerDetector()
detector.run()
