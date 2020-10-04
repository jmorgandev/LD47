using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuakePlayer : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private Camera m_camera;
    private bool m_grounded;
    private CapsuleCollider m_capsule;
    private float m_last_jump_input;
    private float m_jump_input_fudge = 0.2f; // 0.4 seconds
    private bool m_jump_request;

    private Vector3 m_accel_direction;
    private Vector3 m_input_abs;
    private float m_accel = 100.0f;
    
    private float m_jump_height = 1.0f;
    private float m_jump_force = 0.0f;
    private Vector3 m_unbounded_jump_normal = Vector3.zero;
    private float m_fall_speed; // per second
    private float m_friction_factor = 0.8f;
    private float m_speed_limit = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
        m_rigidbody = GetComponent<Rigidbody>();
        m_accel_direction = Vector3.zero;
        m_capsule = GetComponentInChildren<CapsuleCollider>();
        m_jump_force = Mathf.Sqrt(-2f * Physics.gravity.y * m_jump_height);
    }

    // Update is called once per frame
    void Update()
    {
        float x_input = Input.GetAxis("Horizontal");
        float y_input = Input.GetAxis("Vertical");
        Vector3 input3d = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        m_accel_direction.Set(x_input, 0.0f, y_input);
        m_accel_direction.Normalize();

        if (Input.GetButtonDown("Jump"))
        {
            m_jump_request = true;
            m_last_jump_input = Time.time;
        }
        if (Time.time - m_last_jump_input > m_jump_input_fudge)
        {
            m_jump_request = false;
        }
    }

    Vector3 GetHorizontalVelocity(Vector3 vel)
    {
        return new Vector3(vel.x, 0.0f, vel.z);
    }

    void HorizontalMovement(Rigidbody rb)
    {
        rb.AddForce(m_accel_direction * m_accel, ForceMode.Acceleration);
        float temp_y = rb.velocity.y;
        Vector3 hvel = GetHorizontalVelocity(rb.velocity);
        Vector3 new_velocity = Vector3.ClampMagnitude(hvel, m_speed_limit);
        new_velocity.y = temp_y;
        if (m_grounded)
        {
            if (m_accel_direction.x == 0.0f)
                new_velocity.x *= m_friction_factor;
            if (m_accel_direction.z == 0.0f)
                new_velocity.z *= m_friction_factor;
        }
        rb.velocity = new_velocity;
    }

    void VerticalMovement(Rigidbody rb)
    {
        if (m_jump_request && m_grounded)
        {
            // normalize jump normal
            Vector3 jump_normal = m_unbounded_jump_normal.normalized;
            rb.AddForce(jump_normal * m_jump_force, ForceMode.Impulse);
            m_jump_request = false;
        }
    }
    void FixedUpdate()
    {
        HorizontalMovement(m_rigidbody);
        VerticalMovement(m_rigidbody);
        m_grounded = false;
        m_unbounded_jump_normal = Vector3.zero;
    }

    void ContactPointEvaluation(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.0f) 
            {
                m_grounded = true;
                // accumulate contact point normals to influence jumping direction
                m_unbounded_jump_normal += contact.normal;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPointEvaluation(collision);
    }
    void OnCollisionStay(Collision collision)
    {
        ContactPointEvaluation(collision);
    }
}
