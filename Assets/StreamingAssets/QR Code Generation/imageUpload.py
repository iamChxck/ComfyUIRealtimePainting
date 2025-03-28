from flask import Flask, send_from_directory, request, jsonify
import os
import qrcode

app = Flask(__name__)
UPLOAD_FOLDER = 'C:\\Users\\User\\Documents\\Uploads'
QR_FOLDER = 'C:\\Users\\User\\Documents\\Uploads\\QRcodes'
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER

if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)
if not os.path.exists(QR_FOLDER):
    os.makedirs(QR_FOLDER)

@app.route('/upload', methods=['POST'])
def upload_file():
    if 'file' not in request.files:
        return 'No file part', 400
    file = request.files['file']
    if file.filename == '':
        return 'No selected file', 400
    if file:
        file_path = os.path.join(app.config['UPLOAD_FOLDER'], file.filename)
        file.save(file_path)

        qr_code_path = generate_qr_code(file.filename)
        return jsonify({
            "url": f"http://192.168.1.246:5000/uploads/{file.filename}",  # Update with your local IP
            "qr_code": qr_code_path
        })

@app.route('/uploads/<filename>')
def uploaded_file(filename):
    return send_from_directory(app.config['UPLOAD_FOLDER'], filename)

def generate_qr_code(filename):
    url = f"http://192.168.1.246:5000/uploads/{filename}"  # Update with your local IP
    qr_code_path = os.path.join(QR_FOLDER, f"{filename}.png")

    qr = qrcode.QRCode(
        version=1,
        error_correction=qrcode.constants.ERROR_CORRECT_L,
        box_size=30,
        border=2,
    )
    qr.add_data(url)
    qr.make(fit=True)

    img = qr.make_image(fill_color='white', back_color='black')
    img.save(qr_code_path)

    return qr_code_path

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)
