using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CovidOmega
{
    public class ThridPersonController : MonoBehaviour
    {
        public Animator animator;
        public CharacterController controller;
        public float speed = 6f;
        public float turnSmoothTime = 0.1f;
        float turnSmoothVelocity;
        private void LateUpdate()
        {
            //animator.SetFloat("speed", controller.velocity.magnitude);
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                controller.Move(direction * speed * Time.deltaTime);
            }
        }

        //public static ThridPersonController instance;

        //public float speed;

        //private Rigidbody myRigidbody;
        //private Vector3 change;
        //public Animator animator;

        //[HideInInspector]
        //public bool directorMode = false;

        //public enum charState
        //{
        //    idle,
        //    move,
        //    death
        //}

        //private void Start()
        //{
        //    if (instance == null)
        //    {
        //        DontDestroyOnLoad(gameObject);
        //        instance = this;
        //    }
        //    else
        //    {
        //        Destroy(gameObject);
        //    }
        //    myRigidbody = GetComponent<Rigidbody>();
        //}

        //private void FixedUpdate()
        //{
        //    if (!directorMode)
        //    {
        //        change = Vector3.zero;
        //        change.x = Input.GetAxisRaw("Horizontal");
        //        change.z = Input.GetAxisRaw("Vertical");

        //        animator.SetFloat("speed", change.magnitude);

        //        if (change != Vector3.zero)
        //        {
        //            MoveCharactor();

        //            //setting player direction synchronize with their movement
        //            if ((change.x > 0 && transform.localScale.x > 0) || (change.x < 0 && transform.localScale.x < 0))
        //            {
        //                Vector3 scaleTemp = transform.localScale;
        //                scaleTemp.x *= -1;
        //                transform.localScale = scaleTemp;
        //            }

        //        }
        //    }
        //}

        //void MoveCharactor()
        //{
        //    myRigidbody.MovePosition(transform.position + change * speed * Time.deltaTime);
        //}
    }
}

