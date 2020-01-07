import requests

url = "http://localhost:51343/api/upload"
fin = open('test_file.txt', 'rb')
files = {'file': fin}
try:
  r = requests.post(url, data={'host': 'test_host'}, files=files)
  print(r.text)
finally:
	fin.close()