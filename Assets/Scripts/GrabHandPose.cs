using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrabHandPose : MonoBehaviour
{
    public HandData _rightHandPose;
    public HandData _leftHandPose;
    public float poseTransitionDuration = 1f;
    public Transform _attachPoint;
    Vector3 startingHandPosition;
    Vector3 finalHandPosition;
    Quaternion startingHandRotation;
    Quaternion finalHandRotation;
    Quaternion[] startingFingerRotation;
    Quaternion[] finalFingerRotation;
    // Start is called before the first frame update
    void OnEnable() {
    }

    void OnDisable() {
    }
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);
        _rightHandPose.gameObject.SetActive(false);
        _leftHandPose.gameObject.SetActive(false);
        
    }

    public void SetupPose(BaseInteractionEventArgs arg){
        if(arg.interactorObject is XRDirectInteractor){
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;
            if(handData.handType == HandData.HandModelType.Right){
                Quaternion origRot = _attachPoint.localRotation;
                origRot.y = Mathf.Abs(origRot.y);
                _attachPoint.localRotation = origRot;
                SetHandDataValues(handData, _rightHandPose);
            }
            else{
                Quaternion origRot = _attachPoint.localRotation;
                origRot.y = Mathf.Abs(origRot.y) * -1;
                _attachPoint.localRotation = origRot;
                
                SetHandDataValues(handData, _leftHandPose);
            }
            StartCoroutine(SetHandDataRoutine(handData, finalHandPosition, finalHandRotation, finalFingerRotation, startingHandPosition, startingHandRotation, startingFingerRotation));
        }

    }
    public void UnSetPose(BaseInteractionEventArgs arg){
        if(arg.interactorObject is XRDirectInteractor){
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = true;
            StartCoroutine(SetHandDataRoutine(handData, startingHandPosition, startingHandRotation, startingFingerRotation, finalHandPosition, finalHandRotation, finalFingerRotation));
        }

    }



    public void SetHandDataValues(HandData h1, HandData h2){
        startingHandPosition = new Vector3(h1.root.localPosition.x / h1.root.localScale.x, h1.root.localPosition.y / h1.root.localScale.y, h1.root.localPosition.z / h1.root.localScale.z );
        finalHandPosition = new Vector3(h2.root.localPosition.x / h2.root.localScale.x, h2.root.localPosition.y / h2.root.localScale.y, h2.root.localPosition.z / h2.root.localScale.z );


        startingHandRotation = h1.root.localRotation;
        finalHandRotation = h2.root.localRotation;

        startingFingerRotation = new Quaternion[h1.fingerBones.Length];
        finalFingerRotation = new Quaternion[h1.fingerBones.Length];

        for(int i = 0; i < h1.fingerBones.Length; i++){
            startingFingerRotation[i] = h1.fingerBones[i].localRotation;
            finalFingerRotation[i] = h2.fingerBones[i].localRotation;
        }
    }

    public IEnumerator SetHandDataRoutine(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation, Vector3 startingPosition, Quaternion startingRotation, Quaternion[] startingBonesRotation){
        float timer = 0;
        while(timer < poseTransitionDuration){
            Vector3 p = Vector3.Lerp(startingPosition, newPosition, timer / poseTransitionDuration);
            Quaternion r = Quaternion.Lerp(startingRotation, newRotation, timer / poseTransitionDuration);
            h.root.localPosition = p;
            h.root.localRotation = r;

            for(int i = 0; i < newBonesRotation.Length; i++){
                h.fingerBones[i].localRotation = Quaternion.Lerp(startingBonesRotation[i], newBonesRotation[i], timer / poseTransitionDuration);
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
    
#if UNITY_EDITOR

    [MenuItem("Tools/Mirror Selected Right Grab Pose")]
    public static void MirrorRightPose(){
        GrabHandPose handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();
        handPose.MirrorPose(handPose._leftHandPose, handPose._rightHandPose);
    }
#endif
    public void MirrorPose(HandData poseToMirror, HandData poseUsedToMirror){
        Vector3 mirroredPosition = poseUsedToMirror.root.localPosition;
        mirroredPosition.x *= -1;

        Quaternion mirroredQuaternion = poseUsedToMirror.root.localRotation;
        mirroredQuaternion.y *= -1;
        mirroredQuaternion.z *= -1;
        poseToMirror.root.localPosition = mirroredPosition;
        poseToMirror.root.localRotation = mirroredQuaternion;

        for(int i = 0; i < poseUsedToMirror.fingerBones.Length; i++){
            poseToMirror.fingerBones[i].localRotation = poseUsedToMirror.fingerBones[i].localRotation;
        }

    }
}
