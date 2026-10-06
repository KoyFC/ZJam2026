using UnityEngine;

[ExecuteInEditMode]
public class WorldCurver : MonoBehaviour
{
	[SerializeField] private float m_BaseCurveStrength = 1f;
	[SerializeField] private float m_BaseCurveStrength2 = 1f;

	private float curveStrength;
	private float curveStrength2;

	int m_CurveStrengthID;
	int m_CurveStrengthID2;

	private void OnEnable()
	{
		// curveStrength = Random.Range(0, 1f) * 0.004f;
		// curveStrength2 = Random.Range(-1f, 1f) * 0.003f;
		curveStrength = m_BaseCurveStrength * 0.004f;
		curveStrength2 = m_BaseCurveStrength2 * 0.003f;

		m_CurveStrengthID = Shader.PropertyToID("_CurveStrength");
		m_CurveStrengthID2 = Shader.PropertyToID("_CurveStrength2");
	}

	void Update()
	{
		Shader.SetGlobalFloat(m_CurveStrengthID, curveStrength);
		Shader.SetGlobalFloat(m_CurveStrengthID2, curveStrength2);

		// Use time to create a dynamic curve effect
		// curveStrength = Mathf.Sin(Time.time) * m_BaseCurveStrength * 0.004f;
		// curveStrength2 = Mathf.Cos(Time.time) * m_BaseCurveStrength2 * 0.003f;
	}

	void OnValidate()
	{
		curveStrength = m_BaseCurveStrength * 0.004f;
		curveStrength2 = m_BaseCurveStrength2 * 0.003f;
	}
}
