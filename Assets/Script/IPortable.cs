using UnityEngine;

public interface IPortable {
    float GetCurTime();
    void SetCurTime(float time);

    float GetMaxTime();
    void SetMaxTime(float time);

    Vector3 GetPortTarget();
    void SetPortTarget(Vector3 target);
}