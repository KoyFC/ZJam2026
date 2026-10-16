using Unity.Cinemachine;
using UnityEngine;

public class CameraHeightLimiter : CinemachineExtension
{
    [SerializeField] private float m_MinHeight = 12f;
    [SerializeField] private float m_MaxHeight = 48f;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Body)
            return;

        Vector3 position = state.RawPosition;
        position.y = Mathf.Clamp(position.y, m_MinHeight, m_MaxHeight);
        state.RawPosition = position;
    }
}
