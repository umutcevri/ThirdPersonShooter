using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
	public Rig weaponAim;
	public Transform currentPlanet;
	public Transform[] planets;
	Animator animator;
	Quaternion lastTransform;
	PlayerCamera playerCamera;
	float cameraXAngle;
	public float rotationSpeed = 1f;
	public Transform camTransform;
	public Vector3 gravityUp;
    public float gravity = -9.81f;
	public float walkSpeed = 6;

    public float moveSpeed = 6;
	public float jumpForce = 220;
	public LayerMask groundedMask;

	float inputX;

	float inputY;
	
	// System vars
	bool grounded;
	Vector3 moveAmount;
	Rigidbody rb;

	RaycastWeapon weapon;
	
	bool desiredJump;

	bool isAiming;

	int targetJumpLayerWeight = 0;

	public float layerChangeSpeed = 5f;
	void Awake() {
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		weapon = GetComponentInChildren<RaycastWeapon>();
		rb = GetComponent<Rigidbody>();
		playerCamera = camTransform.gameObject.GetComponent<PlayerCamera>();
		animator = GetComponentInChildren<Animator>();
	}
	
	void Update() {
		
		isAiming = Input.GetButton("Fire2");

		if(isAiming)
		{
			//Time.timeScale = 0.25f;
			weaponAim.weight = 1;
		}
		else
		{
			//Time.timeScale = 1f;
			weaponAim.weight = 0;
		}

		animator.SetBool("isAiming", isAiming);

		if(Input.GetButtonDown("Fire1"))
		{
			weapon.StartFiring();
		}
		if(Input.GetButtonUp("Fire1"))
		{
			weapon.StopFiring();
		}
		if(weapon.isFiring)
		{
			weapon.UpdateFiring();
		}
		
		desiredJump |= Input.GetButtonDown("Jump");
        
		inputX = Input.GetAxisRaw("Horizontal");
		inputY = Input.GetAxisRaw("Vertical");

		animator.SetFloat("Vertical", Input.GetAxis("Vertical"));
		animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
		
		int groundLayerMask = 1 << LayerMask.NameToLayer("Ground");

        Ray ray = new Ray(transform.position + transform.up, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1.1f, groundLayerMask))
        {
            grounded = true;
			targetJumpLayerWeight = 0;
        }
        else
        {
            grounded = false;
			targetJumpLayerWeight = 1;
        }

		if(!grounded)
		{
			animator.SetBool("isRunning", false);
		}
		else if(inputX == 0f && inputY == 0f)
        {
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }
		
		//animator.SetLayerWeight(1, targetJumpLayerWeight);

		if(animator.GetLayerWeight(1) != targetJumpLayerWeight)
		{
			float newWeight = Mathf.Lerp(animator.GetLayerWeight(1), targetJumpLayerWeight, layerChangeSpeed * Time.deltaTime);
			animator.SetLayerWeight(1, newWeight);
		}

		
		Debug.DrawRay(ray.origin, ray.direction * 1.1f, grounded ? Color.green : Color.red);

        //Debug.Log(grounded);

		foreach(Transform planet in planets)
		{
			if(Mathf.Abs((rb.position - planet.position).magnitude) < Mathf.Abs((rb.position - currentPlanet.position).magnitude))
			{
				currentPlanet = planet;
				gravity = planet.GetComponent<Planet>().gravitationalPull;
				rb.velocity = Vector3.zero;
			}
		}

		gravityUp = (rb.position - currentPlanet.position).normalized;

		if(isAiming)
		{
			Vector3 lookDirection = camTransform.forward;
			if(lookDirection != Vector3.zero)
			{
				//rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(lookDirection, rb.transform.up), rotationSpeed * Time.deltaTime));
				rb.MoveRotation(Quaternion.LookRotation(lookDirection, rb.transform.up));
			}
			rb.MoveRotation(Quaternion.FromToRotation(rb.transform.up, gravityUp) * rb.rotation);
		}
		else if(rb.velocity != Vector3.zero)
		{
			Vector3 lookDirection = camTransform.forward * inputY + camTransform.right * inputX;
			if(lookDirection != Vector3.zero)
			{
				rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(lookDirection, rb.transform.up), rotationSpeed * Time.deltaTime));
			}
			rb.MoveRotation(Quaternion.FromToRotation(rb.transform.up, gravityUp) * rb.rotation);
		}
		
	}
	
	void FixedUpdate() {

		moveAmount = camTransform.forward * inputY + camTransform.right * inputX;
		
		// Apply downwards gravity to body
		rb.AddForce(gravityUp * gravity, ForceMode.Acceleration);
		
        Vector3 currentVelocity = transform.InverseTransformDirection(rb.velocity);
        currentVelocity = new Vector2(currentVelocity.x, currentVelocity.z);

        if(grounded && !(currentVelocity.magnitude > 6f || currentVelocity.magnitude < -6f))
            rb.AddForce(moveAmount * walkSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        
        //if(moveAmount == Vector3.zero && currentVelocity.magnitude != 0)
            //rb.AddRelativeForce(-new Vector3(currentVelocity.x, 0, currentVelocity.z), ForceMode.VelocityChange);

        if (desiredJump && grounded) {
            Debug.Log("jump");
			desiredJump = false;
			rb.AddForce(transform.up * jumpForce);
		}

        desiredJump = false;
	}

	void OnAnimatorMove() {}
}
