docker build -t youtubedlui:dev .
docker run -it -p 5000:80 -e DOWNLOAD_DIR=/download -e OUTPUT_DIR=/output -e CONFIG_DIR=/config youtubedlui:dev