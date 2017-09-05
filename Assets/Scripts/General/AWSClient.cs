using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

/// <summary>
/// Provides access to necessary AWS services without exposing my private credentials.
/// </summary>
public class AWSClient : MonoBehaviour {

	public const string BUCKET_NAME = "codetroopa-imposter";
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

	void Awake () {
		UnityInitializer.AttachToGameObject(gameObject);

		// Workaround for https://github.com/aws/aws-sdk-net/issues/696
		Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;
	}

	//
	// Each public method maps to a corresponding AWS service.
	//

	// Get object from S3 with given bucket/file name. It is the CALLER'S responsibility to close the stream
	public void GetObject(string bucket, string key, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback) {
		Client.GetObjectAsync(bucket, key, callback);
	}

	// Post object into S3 with given bucket/file name
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

	// Returns a list of keys corresponding to all objects in the given bucket with the given prefix
	public void ListObjects(string bucket, string prefix, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback) {
		ListObjectsRequest req = new ListObjectsRequest() {
			BucketName = bucket,
			Prefix = prefix
		};
				
		Client.ListObjectsAsync(req, callback);
	}
}
