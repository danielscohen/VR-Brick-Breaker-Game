using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabHandPose : MonoBehaviour
{
    public HandData _rightHandPose;
    Vector3 startingHandPosition;
    Vector3 finalHandPosition;
    Quaternion startingHandRotation;
    Quaternion finalHandRotation;
    Quaternion[] startingFingerRotation;
    Quaternion[] finalFingerRotation;
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(SetupPose);
        _rightHandPose.gameObject.SetActive(false);
        
    }

    public void SetupPose(BaseInteractionEventArgs arg){
        if(arg.interactorObject is XRDirectInteractor){
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;
            SetHandDataValues(handData, _rightHandPose);
        }

    }

    public void SetHandDataValues(HandData h1, HandData h2){
        startingHandPosition = h1.root.localPosition;
        finalHandPosition = h2.root.localPosition;

        startingHandRotation = h1.root.localRotation;
        finalHandRotation = h2.root.localRotation;

        startingFingerRotation = new Quaternion[h1.fingerBones.Length];
        finalFingerRotation = new Quaternion[h1.fingerBones.Length];

        for(int i = 0; i < h1.fingerBones.Length; i++){
            startingFingerRotation[i] = h1.fingerBones[i].localRotation;
            finalFingerRotation[i] = h2.fingerBones[i].localRotation;
        }
    }
}
