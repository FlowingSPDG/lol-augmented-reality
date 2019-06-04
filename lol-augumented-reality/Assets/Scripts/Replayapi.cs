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

    CameraPosition cameraposition = new CameraPosition();
    CameraRotation camerarotation = new CameraRotation();

    [NonSerialized]
    public GameObject LoLCamera;

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
        LoLCamera = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(GetCamera());
    }

    IEnumerator GetCamera()
    {

        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;


        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:2999/replay/render");
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

        yield return 0;
    }
}
