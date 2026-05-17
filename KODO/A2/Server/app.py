from flask import Flask, request, jsonify
import os

app = Flask(__name__)

UPLOAD_FOLDER = 'uploaded_texts'
if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)

@app.route('/', methods=['POST'])
def save_text_file():
    try:
        text_data = request.data.decode('utf-8')
        text_filename = os.path.join(UPLOAD_FOLDER, 'text_file_{}.txt'.format(len(os.listdir(UPLOAD_FOLDER))))
        with open(text_filename, 'w', encoding='utf-8') as text_file:
            text_file.write(text_data)

        print(f"Text file saved as {text_filename}")
        return '', 200

    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
