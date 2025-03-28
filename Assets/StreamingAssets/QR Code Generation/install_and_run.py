import subprocess
import sys
import os

# Define the main script file to run
SCRIPT_FILE = "imageUpload.py"

def install_packages():
    """Install required packages listed in requirements.txt"""
    try:
        # Attempt to install packages listed in requirements.txt
        print("Installing required packages...")
        subprocess.check_call([sys.executable, "-m", "pip", "install", "-r", "requirements.txt"])
        print("All packages installed successfully.")
    except subprocess.CalledProcessError as e:
        print(f"Failed to install packages: {e}")
        sys.exit(1)

def run_app():
    """Run the main Flask application script"""
    print("Starting the Flask application...")
    subprocess.call([sys.executable, SCRIPT_FILE])

if __name__ == "__main__":
    # Check if the requirements.txt file exists
    if not os.path.isfile("requirements.txt"):
        print("Error: requirements.txt not found.")
        sys.exit(1)

    install_packages()
    run_app()
