"""
Reads the text of a receipt photo. POST /ocr with the image as the request body answers with
every piece of text found and the four corners of its box (top-left, top-right, bottom-right,
bottom-left). Making sense of the text is up to the API; this service only reads it.
"""

import io
import json
import threading
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

from PIL import Image, ImageOps, UnidentifiedImageError
from rapidocr import RapidOCR

PORT = 8000
MAX_IMAGE_BYTES = 20 * 1024 * 1024

engine = RapidOCR(params={"Global.log_level": "warning"})
# One photo at a time: the model already uses every core, and the Pi has little memory to spare.
engine_lock = threading.Lock()


def read_photo(data: bytes) -> Image.Image:
    # Phones store the photo sideways and say how to turn it in EXIF.
    return ImageOps.exif_transpose(Image.open(io.BytesIO(data))).convert("RGB")


def recognize(photo: Image.Image) -> list[dict]:
    with engine_lock:
        result = engine(photo)
    if result.boxes is None:
        return []
    return [
        {"text": text, "score": round(float(score), 3), "box": [[round(float(x)), round(float(y))] for x, y in box]}
        for box, text, score in zip(result.boxes, result.txts, result.scores)
    ]


class Handler(BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == "/health":
            self.answer(200, {"status": "ok"})
        else:
            self.answer(404, {"error": "not found"})

    def do_POST(self):
        if self.path != "/ocr":
            self.answer(404, {"error": "not found"})
            return
        length = int(self.headers.get("Content-Length") or 0)
        if length <= 0 or length > MAX_IMAGE_BYTES:
            self.answer(413 if length > 0 else 400, {"error": "image missing or too large"})
            return
        try:
            photo = read_photo(self.rfile.read(length))
        except (UnidentifiedImageError, OSError):
            self.answer(415, {"error": "not an image"})
            return
        self.answer(200, {"lines": recognize(photo)})

    def answer(self, status: int, body: dict):
        data = json.dumps(body, ensure_ascii=False).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(data)))
        self.end_headers()
        self.wfile.write(data)

    def log_message(self, format, *args):
        pass


if __name__ == "__main__":
    ThreadingHTTPServer(("0.0.0.0", PORT), Handler).serve_forever()
