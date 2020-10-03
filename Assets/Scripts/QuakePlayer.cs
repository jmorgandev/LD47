using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuakePlayer : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private Camera m_camera;
    private bool m_grounded;

    private Vector3 m_accel_direction;
    private Vector3 m_input_abs;
    private float m_accel = 100.0f;
    private float m_friction_factor = 0.8f;
    private float m_speed_limit = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
        m_rigidbody = GetComponent<Rigidbody>();
        m_accel_direction = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float x_input = Input.GetAxis("Horizontal");
        float y_input = Input.GetAxis("Vertical");
        Vector3 input3d = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        m_accel_direction.Set(x_input, 0.0f, y_input);
        m_accel_direction.Normalize();
    }

    Vector3 GetHorizontalVelocity(Vector3 vel)
    {
        return new Vector3(vel.x, 0.0f, vel.z);
    }
    void FixedUpdate()
    {
        Vector3 v = m_accel_direction * m_accel;
        m_rigidbody.AddForce(m_accel_direction * m_accel, ForceMode.Acceleration);
        float temp_y = m_rigidbody.velocity.y;
        Vector3 hvel = GetHorizontalVelocity(m_rigidbody.velocity);
        Vector3 new_velocity = Vector3.ClampMagnitude(hvel, m_speed_limit);
        new_velocity.y = temp_y;
        if (m_accel_direction.x == 0.0f)
            new_velocity.x *= m_friction_factor;
        if (m_accel_direction.z == 0.0f)
            new_velocity.z *= m_friction_factor;
        m_rigidbody.velocity = new_velocity;
    }
}
