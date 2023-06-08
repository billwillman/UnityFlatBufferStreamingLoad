using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTest : MonoBehaviour
{
    public AnimationCurve m_LocalRotCurve;
    public AnimationCurve m_LocalOffsetCurve;
    public AnimationCurve m_CircleTimeCurve;

    public float CirCleTime = 2f;

    // ������
    public float F = 1f;

    // ����
    public float M = 1f;

    public float RotSpeed = 0.1f;

    private float m_StartTime = -1;


    private void Update() {
        /*
         * Բ���˶���ʽ�����ٶȣ����ٶȵĹ�ϵʽ�ǣ�v=R�ء�W�ǽ��ٶ�
         * UpdateCirCleSpline
         * */
        float delta = Time.deltaTime;

        if (m_StartTime < 0) {
            m_StartTime = 0;
        } else
            m_StartTime += delta;

        UpdateCirCleSpline(m_StartTime);
    }

    // Բ���˶��飬�ʼǣ�����Ϸ����ŷ����Τ����������
    void UpdateCirCleSpline(float t) {
        t = t % CirCleTime;
        if (m_CircleTimeCurve.length > 0) {
            t = m_CircleTimeCurve.Evaluate(t) / m_CircleTimeCurve.length * CirCleTime;
        }

        float rot = 0f;
        if (m_LocalRotCurve.length > 0) {
            rot = m_LocalRotCurve.Evaluate(t);
        }
        float angle = rot * 2 * Mathf.PI * RotSpeed;
        //rot += this.transform.localRotation.eulerAngles.y;
        this.transform.RotateAroundLocal(Vector3.up, angle);
        float offset = m_LocalOffsetCurve.Evaluate(t);
        //Vector3 offsetX = this.transform.forward * offset;
        // ������ = ���� x  �ٶ�^2/�뾶
        float V = offset;

        // ����뾶
        float R = M * Mathf.Pow(V, 2f) / F;

        // Բ���˶����ķ���
        float sin = Mathf.Sin(angle / 2f);

        if (Mathf.Abs(sin) > float.Epsilon) {
            Vector3 rotAxis = Vector3.up;
            Vector3 n = Vector3.Cross(this.transform.forward, rotAxis).normalized;
            Vector3 O = this.transform.localPosition + n * R;
            // ������ٶ� ���ٶ�=R * W
            float W = V / R;
            Vector3 off = this.transform.localPosition - O;

            sin = Mathf.Sin(W / 2f);
            Quaternion q = new Quaternion(rotAxis.x * sin, rotAxis.y * sin, rotAxis.z * sin, Mathf.Cos(W / 2f));
            off = q * off;

            this.transform.localPosition = off + O;
        } else {
            // ˮƽ�˶�

        }
    }
}
