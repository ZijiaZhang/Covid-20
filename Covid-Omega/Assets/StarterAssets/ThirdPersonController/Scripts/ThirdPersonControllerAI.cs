using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StarterAssets
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class ThirdPersonControllerAI : MonoBehaviour
    {
        [Header("State")]
        [SerializeField]
        protected UnitData _unitData;

        [Tooltip("Move speed of the AI in m/s, controls NavMeshAgent's speed")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the AI in m/s, controls NavMeshAgent's speed")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the AI turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        public float SmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The AI uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the AI is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;

        [Range(0, 1)] public float FootstepAudioVolume = 0.1f;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator _animator;
        private CharacterController _controller;

        private const float _threshold = 0.01f;

        // AI variables
        [Tooltip("If the AI is sprinting or not.")]
        public bool Sprinting = false;

        [Tooltip("If the AI will start a Jump or not.")]
        public bool Jump = false;

        private NavMeshAgent _thisAgent;
        private ThirdPersonController _player;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();

            _thisAgent = GetComponent<NavMeshAgent>();
            _thisAgent.updateRotation = false;

            if (Sprinting) _thisAgent.speed = SprintSpeed;
            else _thisAgent.speed = MoveSpeed;

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj.TryGetComponent<ThirdPersonController>(out var thirdPersonController))
            {
                _player = thirdPersonController;
            }

            _thisAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (_player != null)
            {
                _thisAgent.SetDestination(_player.transform.position);
            }

            JumpAndGravity();
            GroundedCheck();

            if (Sprinting) _thisAgent.speed = SprintSpeed;
            else _thisAgent.speed = MoveSpeed;

            if (_thisAgent.remainingDistance > _thisAgent.stoppingDistance)
                Move(_thisAgent.desiredVelocity.normalized, _thisAgent.desiredVelocity.magnitude);
            else
                Move(_thisAgent.desiredVelocity.normalized, 0f);
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                                                 transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                                           QueryTriggerInteraction.Ignore);

            // update animator if using character
            _animator.SetBool(_animIDGrounded, Grounded);
        }

        private void Move(Vector3 AgentDestination, float AgentSpeed)
        {
            if (AgentSpeed > 0f)
            {
                // a reference to the players current horizontal velocity
                float currentHorizontalSpeed =
                    new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

                float speedOffset = 0.1f;

                // accelerate or decelerate to target speed
                if (currentHorizontalSpeed < AgentSpeed - speedOffset ||
                    currentHorizontalSpeed > AgentSpeed + speedOffset)
                {
                    // creates curved result rather than a linear one giving a more organic speed change
                    // note T in Lerp is clamped, so we don't need to clamp our speed
                    _speed = Mathf.Lerp(currentHorizontalSpeed, AgentSpeed, Time.deltaTime * SpeedChangeRate);

                    // round speed to 3 decimal places
                    _speed = Mathf.Round(_speed * 1000f) / 1000f;
                }
                else
                {
                    _speed = AgentSpeed;
                }

                _animationBlend = Mathf.Lerp(_animationBlend, AgentSpeed, Time.deltaTime * SpeedChangeRate);

                // rotate player when the player is moving
                if (_speed != 0f)
                {
                    _targetRotation = Mathf.Atan2(AgentDestination.x, AgentDestination.z) * Mathf.Rad2Deg;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
                                                           ref _rotationVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

                // update animator if using character
                float theMagnitude = 1f;
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, theMagnitude);
            }
            else
            {
                _animationBlend = Mathf.Lerp(_animationBlend, 0f, Time.deltaTime * SpeedChangeRate);
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, 1f);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                var verticalDis = _player.transform.position.y - gameObject.transform.position.y;
                Jump = verticalDis >= 0.2f;
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);

                // Jump
                if (Jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _thisAgent.velocity =
                        new Vector3(_thisAgent.desiredVelocity.x, _verticalVelocity, _thisAgent.desiredVelocity.z);
                    // update animator if using character
                    _animator.SetBool(_animIDJump, true);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    _animator.SetBool(_animIDFreeFall, true);
                }

                // if we are not grounded, do not jump
                Jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            // Trigger Jump Once
            Jump = false;
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                              GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center),
                                                FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center),
                                            FootstepAudioVolume);
            }
        }
    }
}