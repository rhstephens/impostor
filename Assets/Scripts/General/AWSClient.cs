using UnityEngine;

using System.IO;

using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;

/// <summary>
/// Provides access to necessary AWS services without exposing my private credentials.
/// </summary>
public class AWSClient : MonoBehaviour {

	const string COGNITO_POOL_ID = "us-east-2:7d3d9a09-7eef-4d62-a5cf-ffda2a75491c";

	RegionEndpoint _region = RegionEndpoint.USEast2;
	CognitoAWSCredentials _creds = null;
	AmazonS3Client _client = null;

	CognitoAWSCredentials Creds {
		get {
			if (_creds == null) {
				_creds = new CognitoAWSCredentials(COGNITO_POOL_ID, _region);
			}
			return _creds;
		}
	}

	AmazonS3Client Client {
		get {
			if (_client == null) {
				_client = new AmazonS3Client(Creds, _region);
			}
			return _client;
		}
	}

	void Start () {
		UnityInitializer.AttachToGameObject(gameObject);

		// Workaround for https://github.com/aws/aws-sdk-net/issues/696
		Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;
	}

	//
	// Each public method maps to a corresponding AWS service.
	//

	// Get object from S3 with given bucket/file name. It is the CALLER'S responsibility to close the stream
	public StreamReader GetObject(string bucket, string key) {
		StreamReader sr = null;
		Client.GetObjectAsync(bucket, key, (cb) => {
			if (cb.Exception != null) {
				Debug.Log(cb.Exception);
				return;
			}
			GetObjectResponse resp = cb.Response;
			if (resp.ResponseStream != null) {
				sr = new StreamReader(resp.ResponseStream);
			}
		});
		return sr;
	}

	// Post object into S3 with given bucket/file name.
	public void PostObject(string bucket, string key, string tmpFile) {
		FileStream stream = new FileStream(tmpFile, FileMode.Open, FileAccess.Read, FileShare.Read);

		PostObjectRequest req = new PostObjectRequest() {
			Bucket = bucket,
			Key = key,
			InputStream = stream,
			CannedACL = S3CannedACL.Private,
			Region = _region
		};
		Client.PostObjectAsync(req, (cb) => {
			if (cb.Exception != null) {
				Debug.Log(cb.Exception);
				stream.Dispose();
				File.Delete(tmpFile);
				return;
			}
			Debug.Log("Successful save: " + key);
			File.Delete(tmpFile);
		});
	}
}
