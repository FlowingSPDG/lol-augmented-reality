using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MiniJSON;
using System.Text;

public class Replayapi : MonoBehaviour
{
    // Camera Datas
    public class CameraPosition
    {
        public float x;
        public float y;
        public float z;
    }
    public class CameraRotation
    {
        public float x;
        public float y;
        public float z;
    }
    //public float fieldOfView = 40.0f;
    private class PostCameraData
    {
        public CameraPosition cameraPosition;
        public CameraRotation cameraRotation;
    }

    const string endpoint = "https://localhost:2999/replay/render";

    CameraPosition cameraposition = new CameraPosition();
    CameraRotation camerarotation = new CameraRotation();

    [NonSerialized]
    public Camera LoLCamera;

    public bool OverrideCamera;

    // allow un-signed SSL(https)
    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // all Certificates are accepted
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(GetCamera());
        LoLCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (OverrideCamera) // allow camera override when its true
        {
            StartCoroutine(PostCamera());
        }
        else
        {
            StartCoroutine(GetCamera());
        }
    }

    IEnumerator GetCamera()
    {
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
        request.Method = "GET";
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        Stream dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        string json = reader.ReadToEnd();
        //Debug.Log("RAW DATA : " + json);
        //var data = new ReplayAPIData();

        //一階層目
        var data = Json.Deserialize(json) as Dictionary<string, object>;
        
        //二階層目
        var RAW_CameraPosition = data["cameraPosition"] as Dictionary<string, object>;
        var RAW_CameraRotation = data["cameraRotation"] as Dictionary<string, object>;
        var RAW_DepthFogColor = data["depthFogColor"] as Dictionary<string, object>;
        var RAW_HeightFogColor = data["heightFogColor"] as Dictionary<string, object>;
        var RAW_SunDirection = data["sunDirection"] as Dictionary<string, object>;

        //Camera pos
        cameraposition.x = Convert.ToSingle(RAW_CameraPosition["x"]);
        cameraposition.y = Convert.ToSingle(RAW_CameraPosition["y"]);
        cameraposition.z = Convert.ToSingle(RAW_CameraPosition["z"]);
        
        Debug.Log("CAMERA Xpos : " + cameraposition.x);
        Debug.Log("CAMERA Ypos : " + cameraposition.y);
        Debug.Log("CAMERA Zpos : " + cameraposition.z);
        // Camera position has different property between Unity and LoL
        LoLCamera.transform.position = new Vector3(cameraposition.x, cameraposition.y, cameraposition.z);

        //Camera rot
        camerarotation.x = Convert.ToSingle(RAW_CameraRotation["x"]);
        camerarotation.y = Convert.ToSingle(RAW_CameraRotation["y"]);
        camerarotation.z = Convert.ToSingle(RAW_CameraRotation["z"]);

        Debug.Log("CAMERA Xrot : " + camerarotation.x);
        Debug.Log("CAMERA Yrot : " + camerarotation.y);
        Debug.Log("CAMERA Zrot : " + camerarotation.z);
        // Camera rotation has different property between Unity and LoL
        LoLCamera.transform.eulerAngles = new Vector3(camerarotation.y, camerarotation.x, camerarotation.z);

        // FOV
        var fieldOfView = Convert.ToSingle(data["fieldOfView"]);
        LoLCamera.fieldOfView = fieldOfView;

        yield return 0;
    }

    IEnumerator PostCamera()
    {
        var camerapos = this.LoLCamera.transform.position;
        var camerarot = this.LoLCamera.transform.eulerAngles;

        CameraPosition cameraPosition = new CameraPosition();
        cameraPosition.x = camerapos.x;
        cameraPosition.y = camerapos.y;
        cameraPosition.z = camerapos.z;

        CameraRotation cameraRotation = new CameraRotation();
        cameraRotation.x = camerarot.y; // XとYが入れ替わっている
        cameraRotation.y = camerarot.x;
        cameraRotation.z = camerarot.z;

        string json = "{\"cameraPosition\":";
        json += JsonUtility.ToJson(cameraPosition);

        json += ",\"cameraRotation\":";
        json += JsonUtility.ToJson(cameraRotation);

        json += ",\"fieldOfView\":";
        json += LoLCamera.fieldOfView;

        json += ",\"cameraMode\":";
        json += "\"fps\"";

        json += "}";

        Debug.Log("RAW : " + cameraPosition);
        Debug.Log("JSON : " + json);
        WebClient webClient = new WebClient();
        webClient.Headers[HttpRequestHeader.ContentType] = "application/json;charset=UTF-8";
        webClient.Headers[HttpRequestHeader.Accept] = "application/json";
        webClient.Encoding = Encoding.UTF8;
        string response = webClient.UploadString(new Uri(endpoint), json);

        yield return 0;
    }
}
