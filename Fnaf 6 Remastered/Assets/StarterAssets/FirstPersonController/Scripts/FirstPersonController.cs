using UnityEngine;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		/*[Tooltip("Crouch Height")]
		public float CrouchHeight = 0.75f;
		public float NormalHeight = 2f;
		public float CrouchCenterY = 0.5f; // New center Y position for crouching
    	public float NormalCenterY = 1f; // Normal center Y position
		[Tooltip("Crouch Speed")]
		public float LerpSpeed = 5f;
		public Transform cameraTransform;*/

		[Header("Camera Tilt Settings")]
		public float tiltAmount = 2.5f; // Maximum tilt angle
		public float tiltSpeed = 3f; // Speed of tilting

		[Header("Camera Headbob Settings")]
		public float bobbingSpeed;
		public float bobbingAmount = 0.08f; // Adjust this to control the amount of bobbing
		public float bobbingWalk = 7.0f; // Adjust this to control the speed of bobbing
		public float bobbingSprint = 12.0f; // Adjust this to control the speed of bobbing
		public float bobbingVent = 5.0f; // Adjust this to control the speed of bobbing
		public float midpoint = 1.7f;
		public float returnSpeed = 2.0f; // Speed at which the camera returns to the midpoint when stopping
		public float bobbingHorizontalAmount = 0.05f; // Amount for horizontal bobbing
		private float currentBobbingAmount = 0.0f; // Current bobbing amount for vertical
		private float currentHorizontalBobbingAmount = 0.0f; // Current bobbing amount for horizontal
		private float targetBobbingAmount = 0.0f; // Target bobbing amount for vertical
		private float targetHorizontalBobbingAmount = 0.0f; // Target bobbing amount for horizontal
		//private float currentBobbingY = 0.0f; // Store the current bobbing y position // FOR BOBBInG TYPE 2

		private Vector3 originalPosition;
    	private Quaternion originalRotation;
		private Vector3 originalCameraLocalPosition;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;
		public LayerMask collisionLayers;

		public bool DEAD=false;

		[Header("Vent Controls")]
		private PlayerVentInteraction playerVentInteraction;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		//AUDIO

		private AudioSource StepSource;
		public AudioClip[] StepSounds; 

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		public float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;
		//private bool isCrouched = false;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private CapsuleCollider _hitbox;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.001f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			originalPosition = transform.localPosition;
			originalRotation = transform.localRotation;
			playerVentInteraction = GetComponent<PlayerVentInteraction>();
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			StepSource = GameObject.Find("PlayerCapsule").GetComponent<AudioSource>();

			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			#if ENABLE_INPUT_SYSTEM
						_playerInput = GetComponent<PlayerInput>();
			#else
						Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
			#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			if(DEAD==false)
			{
				if(playerVentInteraction.inVent==false && playerVentInteraction.isVenting==false)
				{
					JumpAndGravity();
				}
				if(playerVentInteraction.isVenting==false)
				{
					Move();
					ApplyHeadBobbing();
					ApplyTilt();
				}
				GroundedCheck();
			}
		}

		private void LateUpdate()
		{
			if(DEAD==false)
			{
				CameraRotation();
				ApplyHeadBobbing();
			}
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			float targetSpeed;
			if(playerVentInteraction.inVent==true)
				targetSpeed = _input.sprint ? SprintSpeed/4 : MoveSpeed/2;
			else
				targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
			// set target speed based on move speed, sprint speed and if sprint is pressed

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		/*private void Crouch()
		{
			if (Input.GetKeyDown(KeyCode.LeftControl) && !crouchingAnim)
			{
				if (isCrouched)
				{
					if (CanStandUp())
					{
						StartCoroutine(ChangeHeightAndCenter(NormalHeight, NormalCenterY, NormalCenterY + NormalHeight / 2));
						MoveSpeed *= 2;
						SprintSpeed *= 2;
						isCrouched = false;
					}
				}
				else
				{
					StartCoroutine(ChangeHeightAndCenter(CrouchHeight, CrouchCenterY, CrouchCenterY + CrouchHeight / 2));
					MoveSpeed /= 2;
					SprintSpeed /= 2;
					isCrouched = true;
				}
			}
		}

		private bool CanStandUp()
		{
			// Calculate the top position of the character when standing
			Vector3 start = _controller.bounds.center;
			Vector3 end = start + Vector3.up * (NormalHeight - CrouchHeight);

			// Check if there is anything obstructing the space where the character would stand up
			return !Physics.CheckCapsule(start, end, _controller.radius, collisionLayers);
		}

		private IEnumerator ChangeHeightAndCenter(float targetHeight, float targetCenterY, float targetCameraY)
		{
			crouchingAnim = true;
			float startHeight = _controller.height;
			Vector3 startCenter = _controller.center;
			Vector3 targetCenter = new Vector3(startCenter.x, targetCenterY, startCenter.z);
			float startCameraY = cameraTransform.localPosition.y;
			float elapsedTime = 0f;

			while (elapsedTime < 1f)
			{
				_controller.height = Mathf.Lerp(startHeight, targetHeight, elapsedTime);
				_controller.center = Vector3.Lerp(startCenter, targetCenter, elapsedTime);
				Vector3 cameraLocalPos = cameraTransform.localPosition;
				cameraLocalPos.y = Mathf.Lerp(startCameraY, targetCameraY, elapsedTime);
				cameraTransform.localPosition = cameraLocalPos;
				elapsedTime += Time.deltaTime * LerpSpeed;
				yield return null;
			}

			_controller.height = targetHeight;
			_controller.center = targetCenter;
			Vector3 finalCameraLocalPos = cameraTransform.localPosition;
			finalCameraLocalPos.y = targetCameraY;
			cameraTransform.localPosition = finalCameraLocalPos;
			crouchingAnim = false;
		}*/

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump MEEEEE
				if (_input.jump && _jumpTimeoutDelta <= 0.0f )
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
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

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private bool canPlayFootstep = true; // A flag to control when the footstep sound can play

		private void ApplyHeadBobbing()
		{
			Vector3 newLocalPos = CinemachineCameraTarget.transform.localPosition;

			// Adjust bobbing speed and amount based on player state
			if (playerVentInteraction.inVent)
			{
				bobbingSpeed = bobbingVent;           // Slower bobbing in vent
				bobbingAmount = 0.12f;                // Adjust bobbing for vents
			}
			else if (_speed > MoveSpeed - 1.0f && _speed < SprintSpeed - 0.9f)
			{
				bobbingSpeed = bobbingWalk;           // Regular walking bobbing
				bobbingAmount = 0.08f;
			}
			else if (_speed >= SprintSpeed)
			{
				bobbingSpeed = bobbingSprint * 2f;    // Faster bobbing when sprinting
				bobbingAmount = 0.08f;                // Keep the amount same but adjust the speed
			}

			if (Grounded && _input.move.magnitude > 0 && ((playerVentInteraction.inVent == false && _speed > 2f) || (playerVentInteraction.inVent == true && _speed > 1f)))
			{
				// Set target bobbing amounts when grounded and moving
				targetBobbingAmount = bobbingAmount;
				targetHorizontalBobbingAmount = bobbingHorizontalAmount;
			}
			else
			{
				// Stop bobbing when not moving
				targetBobbingAmount = 0.0f;
				targetHorizontalBobbingAmount = 0.0f;
			}

			// Smoothly interpolate the current bobbing amount
			currentBobbingAmount = Mathf.Lerp(currentBobbingAmount, targetBobbingAmount, Time.deltaTime * returnSpeed);
			currentHorizontalBobbingAmount = Mathf.Lerp(currentHorizontalBobbingAmount, targetHorizontalBobbingAmount, Time.deltaTime * returnSpeed);

			// Calculate bobbing wave
			float waveSlice = Mathf.Sin(Time.time * bobbingSpeed);
			float bobbingForce = waveSlice * currentBobbingAmount;
			float horizontalBobbingForce = Mathf.Sin(Time.time * bobbingSpeed * 0.5f) * currentHorizontalBobbingAmount;

			// Apply bobbing to camera's local position
			newLocalPos.y = midpoint + bobbingForce;
			newLocalPos.x = horizontalBobbingForce; // Horizontal bobbing

			// Check if player is stepping down (foot hitting ground) and play sound
			if (waveSlice < -0.9f && Grounded && _input.move.magnitude > 0 && canPlayFootstep)
			{
				StepSource.PlayOneShot(StepSounds[Random.Range(0, StepSounds.Length)]);
				canPlayFootstep = false; // Prevent multiple sounds during one step down
			}

			// Reset the flag when head moves up to allow the next footstep sound
			if (waveSlice > 0.9f)
			{
				canPlayFootstep = true; // Allow footstep sound again when head moves up
			}

			// Update the camera target's position
			CinemachineCameraTarget.transform.localPosition = newLocalPos;
		}

		/*private void ApplyHeadBobbing()
		{
			Vector3 newLocalPos = CinemachineCameraTarget.transform.localPosition;

			// Determine the appropriate bobbing speed based on player state
			if (playerVentInteraction.inVent == true)
			{
				bobbingSpeed = bobbingVent;
				bobbingAmount = 0.12f;
			}
			else if (_speed > MoveSpeed - 1.0f && _speed < SprintSpeed - 0.9f)
			{
				bobbingSpeed = bobbingWalk;
				bobbingAmount = 0.08f;
			}
			else if (_speed >= SprintSpeed)
			{
				bobbingSpeed = bobbingSprint;
				bobbingAmount = 0.08f;
			}

			if (Grounded && _input.move.magnitude > 0 && ((playerVentInteraction.inVent == false && _speed > 2f) || (playerVentInteraction.inVent == true && _speed > 1f))) // Only bob the head if grounded and moving
			{
				// Set target bobbing amounts
				targetBobbingAmount = bobbingAmount;
				targetHorizontalBobbingAmount = bobbingHorizontalAmount;
			}
			else
			{
				// Reduce the target bobbing amounts to zero
				targetBobbingAmount = 0.0f;
				targetHorizontalBobbingAmount = 0.0f;
			}

			// Smoothly interpolate the current bobbing amounts towards the target values
			currentBobbingAmount = Mathf.Lerp(currentBobbingAmount, targetBobbingAmount, Time.deltaTime * returnSpeed);
			currentHorizontalBobbingAmount = Mathf.Lerp(currentHorizontalBobbingAmount, targetHorizontalBobbingAmount, Time.deltaTime * returnSpeed);

			// Calculate the bobbing forces
			float waveSlice = Mathf.Sin(Time.time * bobbingSpeed);
			float bobbingForce = waveSlice * currentBobbingAmount;
			float horizontalBobbingForce = Mathf.Sin(Time.time * bobbingSpeed * 0.5f) * currentHorizontalBobbingAmount;

			// Apply bobbing to camera's local position
			newLocalPos.y = midpoint + bobbingForce;
			newLocalPos.x = horizontalBobbingForce; // Bobbing left and right

			// Check if the player is close to the ground (head bobbing downwards) and play sound if allowed
			if (waveSlice < -0.9f && Grounded && _input.move.magnitude > 0 && canPlayFootstep)
			{
				StepSource.PlayOneShot(StepSounds[Random.Range(0, StepSounds.Length)]);
				canPlayFootstep = false; // Disable further footstep sounds until the head bobs up again
			}

			// Reset the flag when the head moves back up (prepare for the next step sound)
			if (waveSlice > 0.9f)
			{
				canPlayFootstep = true; // Allow footstep sound again when the head bobs up
			}

			// Update the local position of the Cinemachine camera target
			CinemachineCameraTarget.transform.localPosition = newLocalPos;
		}*/
		
		void ApplyTilt()
		{
			// Calculate tilt based on player's input direction
			float targetTilt = -_input.move.x * tiltAmount;

			// Create a quaternion for the target rotation with only Z-axis tilt
			Quaternion targetRotation = originalRotation * Quaternion.Euler(0, 0, targetTilt);

			// Extract the current local Euler angles
			Vector3 currentEulerAngles = transform.localRotation.eulerAngles;

			// Apply the new Z-axis rotation, maintain X and Y axes
			Quaternion newRotation = Quaternion.Euler(currentEulerAngles.x, currentEulerAngles.y, targetRotation.eulerAngles.z);

			// Apply the tilt rotation with interpolation to smooth the movement
			transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, Time.deltaTime * tiltSpeed);
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}