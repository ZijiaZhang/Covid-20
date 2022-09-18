using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CovidOmega
{
    public class FollowCamera : MonoBehaviour
    {
        public Transform target;
        public float smoothing;

        //public Vector2 maxPosition;
        //public Vector2 minPosition;

        public float zOffset = 2.5f;

        // Update is called once per frame
        void LateUpdate()
        {
            if (transform.position != target.position)
            {
                Vector3 end = new Vector3(target.position.x, transform.position.y, (target.position.z-zOffset));

                //end.x = Mathf.Clamp(end.x, minPosition.x, maxPosition.x);
                //end.y = Mathf.Clamp(end.y, minPosition.y, maxPosition.y);

                transform.position = Vector3.Lerp(transform.position, end, smoothing);
            }
        }
    }
}

