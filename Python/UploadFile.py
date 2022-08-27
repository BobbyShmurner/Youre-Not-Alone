import json
import sys
import requests
from typing_extensions import Required

from apiclient.discovery import build
from oauth2client.service_account import ServiceAccountCredentials
from apiclient.http import MediaFileUpload, MediaIoBaseDownload

def get_service(api_name, api_version, scopes, service_key):
	credentials = ServiceAccountCredentials.from_json_keyfile_dict(json.loads(service_key), scopes=scopes)

	# Build the service object.
	service = build(api_name, api_version, credentials=credentials)

	return service

def main(file_name, file_loc, description, service_key):
	scope = 'https://www.googleapis.com/auth/drive'

	service = get_service(api_name='drive', api_version='v3', scopes=[scope], service_key=service_key)

	file_metadata = {
		'name': file_name,
		'mimeType': '*/*',
		'parents': ['1o62lg9CPiwMabvMklxkiyllO7n0OdksH'],
		'description': description
	}
	media = MediaFileUpload(file_loc, mimetype='*/*', resumable=True)

	file_id = service.files().create(body=file_metadata, media_body=media, fields='id', supportsAllDrives=True).execute().get('id')

	new_permission = {
	  'role': 'reader',
	  'type': 'anyone'
  	}
	service.permissions().create(fileId=file_id, body=new_permission).execute()

	file = service.files().get(fileId=file_id, fields='webViewLink').execute()
	
	print(file['webViewLink'])

if __name__ == '__main__':
	main(*sys.argv[1:])
