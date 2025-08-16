using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class BikeRider : MonoBehaviour
    {
        private RGSKMotorbike motorbike;
        public IKRacer ikRacer { get; set; }
        public Animator anim { get; set; }
        public bool isAlive { get; set; }

        void Start()
        {
            DisableRagdoll();
            motorbike = GetComponentInParent<RGSKMotorbike>();
            ikRacer = GetComponent<IKRacer>();
            anim = GetComponent<Animator>();

            isAlive = true;
        }


        void Update()
        {
            //Update animator values
            if(anim != null && motorbike != null)
            {
                anim.SetFloat("Speed", motorbike.currentSpeedKPH);
                anim.SetFloat("Steer", Mathf.Lerp(anim.GetFloat("Steer"), motorbike.steerInput, Time.deltaTime * 2));
				anim.SetBool("Reverse", motorbike.currentGear == 0 && motorbike.throttleInput > 0);
            }    
        }


        public void EnableRagdoll()
        {
            if (!isAlive)
                return;

            isAlive = false;

            foreach (Rigidbody rigid in GetComponentsInChildren<Rigidbody>())
            {
                rigid.isKinematic = false;
                if (rigid.GetComponent<Collider>())
                    rigid.GetComponent<Collider>().enabled = true;
            }

            if (anim != null)
            {
                anim.enabled = false;
            }

            if (ikRacer != null)
            {
                ikRacer.enabled = false;
            }
        }


        public void DisableRagdoll()
        {
            if (isAlive)
                return;

            isAlive = true;

            foreach (Rigidbody rigid in GetComponentsInChildren<Rigidbody>())
            {
                rigid.isKinematic = true;
                if (rigid.GetComponent<Collider>())
                    rigid.GetComponent<Collider>().enabled = false;
            }

            if (anim != null)
            {
                anim.enabled = true;
            }

            if (ikRacer != null)
            {
                ikRacer.enabled = true;
            }
        }
    }
}