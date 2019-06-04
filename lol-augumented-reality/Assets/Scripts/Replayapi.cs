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
    // 信頼できないSSL証明書を「問題なし」にするメソッド
    // allow SSL connection without cert
    private bool OnRemoteCertificateValidationCallback(
      System.Object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetCamera());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GetCamera()
    {

        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;


        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://192.168.11.2:2999/replay/render");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        Stream dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        string json = reader.ReadToEnd();
        Debug.Log("RAW DATA : " + json);
        //var data = new ReplayAPIData();

        //一階層目
        var data = Json.Deserialize(json) as Dictionary<string, object>;
        
        //二階層目
        var CameraPosition = data["cameraPosition"] as Dictionary<string, object>;
        var CameraRotation = data["cameraRotation"] as Dictionary<string, object>;
        var DepthFogColor = data["depthFogColor"] as Dictionary<string, object>;
        var HeightFogColor = data["heightFogColor"] as Dictionary<string, object>;
        var SunDirection = data["sunDirection"] as Dictionary<string, object>;

        Debug.Log("CAMERA Xpos : " + CameraPosition["x"]);
        Debug.Log("CAMERA Ypos : " + CameraPosition["y"]);
        Debug.Log("CAMERA Zpos : " + CameraPosition["z"]);

        Debug.Log("CAMERA Xrot : " + CameraRotation["x"]);
        Debug.Log("CAMERA Yrot : " + CameraRotation["y"]);
        Debug.Log("CAMERA Zrot : " + CameraRotation["z"]);



        yield return 0;
    }

    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // all Certificates are accepted
        return true;
    }

}
