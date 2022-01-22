using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurCamera : MonoBehaviour
{
    private Camera blurCamera;
    private Vector3 actPostion = new Vector3(-2f, 0, -8f);
    private Vector3 actRotation = new Vector3(0,10f,0);

    private Vector3 enemyActPostion = new Vector3(2f, 0, -8f);
    private Vector3 enemyActRotation = new Vector3(0, -10f, 0);

    private Vector3 originPostion = new Vector3(0, 0, -8f);
    private Vector3 originRotation = new Vector3(0, 0f, 0);
    private bool isRunning = false;

    void Start()
    {
        blurCamera = gameObject.GetComponent<Camera>();
    }

    public void CameraAction(bool isStart, bool isEnemy)
    {
        if (isStart)
        {
            isRunning = true;
            if (isEnemy)
                StartCoroutine(ActionStart(enemyActPostion, enemyActRotation, isStart));
            else
                StartCoroutine(ActionStart(actPostion, actRotation, isStart));
        }
        else
        {
            StartCoroutine(ActionStart(originPostion, originRotation,isStart));
        }
    }

    private IEnumerator ActionStart(Vector3 destPos, Vector3 destRot, bool isStart)
    {
        while (!isStart && isRunning )
            yield return null;

        Vector3 translatePos = new Vector3((destPos.x - blurCamera.transform.position.x)/4f, 0f, 0f);
        Vector3 rotatePos = new Vector3(0f,(destRot.y - blurCamera.transform.localEulerAngles.y)/4f,0f);
        
        while (( (translatePos.x <0 ) && blurCamera.transform.position.x > destPos.x)
            ||((translatePos.x > 0) && blurCamera.transform.position.x < destPos.x))
        {
            yield return new WaitForSeconds(0.02f);
            blurCamera.transform.position += translatePos;
            blurCamera.transform.Rotate(rotatePos);
        }

        blurCamera.transform.position = destPos;
        blurCamera.transform.localEulerAngles = destRot;
        isRunning = false;
    }

}
