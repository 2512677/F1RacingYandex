using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class IKRacer : MonoBehaviour
    {
        private Animator anim;
        private Rigidbody rigid;

        public bool enableIK = true;

        //Targets
        [Header("IK Targets")]
        public Transform leftHandTarget;
        public Transform rightHandTarget;
        public Transform leftFootTarget;
        public Transform rightFootTarget;
        public Transform lookTarget;

        private Vector3 rightHandPositon;
        private Quaternion rightHandRotation;
        private Vector3 leftHandPositon;
        private Quaternion leftHandRotation;

        public Vector3 sitPosition;
        public Vector3 sitRotation;
        public float headLookSpeed = 2;
        private float initalLookTargetX;

        //Finger rotation
        public Transform leftHandReference;
        public Transform rightHandReference;
        private Transform[] Lfingers;
        private Transform[] Rfingers;
        private Transform[] LfingersReference;
        private Transform[] RfingersReference;

        void Start()
        {
            anim = GetComponent<Animator>();
            rigid = GetComponentInParent<Rigidbody>();

            if (lookTarget != null)
            {
                //Set the original look position
                initalLookTargetX = lookTarget.localPosition.x;
            }

            if(anim != null && anim.avatar != null)
            {
                //Get the left / right hand child bones
                Lfingers = anim.GetBoneTransform(HumanBodyBones.LeftHand).GetComponentsInChildren<Transform>();
                Rfingers = anim.GetBoneTransform(HumanBodyBones.RightHand).GetComponentsInChildren<Transform>();
            }

            if (rightHandReference != null && leftHandReference != null)
            {
                //Store the left / right hand finger rotations from the references
                LfingersReference = leftHandReference.GetComponentsInChildren<Transform>();
                RfingersReference = rightHandReference.GetComponentsInChildren<Transform>();
            }
        }


        void Update()
        {
            //Update the racer's sitting position
            transform.localPosition = sitPosition;
            transform.localEulerAngles = sitRotation;

            if (lookTarget != null)
            {
                //Move the look target based on the steer input
                float steer = rigid ? Mathf.Clamp(rigid.angularVelocity.y,-1, 1) : 0;
                Vector3 newLook = lookTarget.localPosition;
                newLook.x = steer + initalLookTargetX;
                lookTarget.localPosition = Vector3.Lerp(lookTarget.localPosition, newLook, Time.deltaTime * headLookSpeed);
            }
        }


        void LateUpdate()
        {
            if (anim == null || !enableIK)
                return;

            if (rightHandReference == null || leftHandReference == null)
                return;

            //Set the hand rotations according to the reference hand rotations
            for (int i = 1; i < Lfingers.Length; i++)
            {
                Lfingers[i].localRotation = LfingersReference[i].localRotation;
                Rfingers[i].localRotation = RfingersReference[i].localRotation;
            }
        }


        void OnAnimatorIK()
        {
            if (!enableIK || !anim.enabled)
                return;

            //Look
            if (lookTarget != null)
            {
                anim.SetLookAtWeight(1);
                anim.SetLookAtPosition(lookTarget.position);
            }


            //Right Hand
            if (rightHandTarget != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }


            //Left Hand
            if (leftHandTarget != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }


            //Left Foot
            if (leftFootTarget != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
                anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
            }


            //Right Foot
            if (rightFootTarget != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
                anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
            }
        }
    }
}
