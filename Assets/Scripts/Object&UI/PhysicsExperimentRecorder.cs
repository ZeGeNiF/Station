
using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class PhysicsExperimentRecorder : MonoBehaviour
{
    [Header("Объект для измерений")]
    [SerializeField] private Rigidbody targetBody;

    [Header("Параметры записи")]
    [SerializeField] private float sampleInterval = 0.05f;

    private StreamWriter writer;
    private float elapsedTime;
    private float nextSampleTime;
    private Vector3 startPosition;
    private Vector3 knockbackDirection;
    private bool recording;

    private string filePath;

    public void StartRecording()
    {
        if (targetBody == null)
        {
            Debug.LogWarning("Recorder: Rigidbody не назначен.", this);
            return;
        }

        // Закрываем предыдущую запись, если она ещё активна.
        StopRecording();

        elapsedTime = 0f;
        nextSampleTime = 0f;
        startPosition = targetBody.position;

        // Направление текущего движения фиксируется в начале опыта.
        knockbackDirection = targetBody.linearVelocity.normalized;

        if (knockbackDirection.sqrMagnitude < 0.001f)
            knockbackDirection = transform.forward;

        string fileName =
            "PhysicsExperiment_" +
            DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") +
            ".csv";

        filePath = Path.Combine(
            Application.persistentDataPath,
            fileName
        );

        try
        {
            writer = new StreamWriter(
                filePath,
                false,
                Encoding.UTF8
            );

            writer.WriteLine(
                "Time_s,X_m,Y_m,Z_m,Distance_m,Vx_m_s,Vy_m_s,Vz_m_s,Speed_m_s,VelocityAlongImpulse_m_s"
            );

            recording = true;

            Debug.Log("Запись физического опыта началась.");
        }
        catch (IOException exception)
        {
            Debug.LogError("Не удалось создать CSV: " + exception.Message);
            writer = null;
            recording = false;
        }
    }

    private void FixedUpdate()
    {
        if (!recording || targetBody == null || writer == null)
            return;

        elapsedTime += Time.fixedDeltaTime;

        if (elapsedTime + 0.00001f < nextSampleTime)
            return;

        nextSampleTime += Mathf.Max(sampleInterval, Time.fixedDeltaTime);

        Vector3 position = targetBody.position;
        Vector3 velocity = targetBody.linearVelocity;

        float distance = Vector3.Distance(startPosition, position);
        float speed = velocity.magnitude;
        float velocityAlongImpulse =
            Vector3.Dot(velocity, knockbackDirection);

        string line = string.Join(",",
            elapsedTime.ToString("F3", CultureInfo.InvariantCulture),
            position.x.ToString("F4", CultureInfo.InvariantCulture),
            position.y.ToString("F4", CultureInfo.InvariantCulture),
            position.z.ToString("F4", CultureInfo.InvariantCulture),
            distance.ToString("F4", CultureInfo.InvariantCulture),
            velocity.x.ToString("F4", CultureInfo.InvariantCulture),
            velocity.y.ToString("F4", CultureInfo.InvariantCulture),
            velocity.z.ToString("F4", CultureInfo.InvariantCulture),
            speed.ToString("F4", CultureInfo.InvariantCulture),
            velocityAlongImpulse.ToString("F4", CultureInfo.InvariantCulture)
        );

        writer.WriteLine(line);
    }

    public void StopRecording()
    {
        if (!recording && writer == null)
            return;

        recording = false;

        if (writer != null)
        {
            writer.Flush();
            writer.Close();
            writer = null;

            Debug.Log("Опыт сохранён в CSV: " + filePath);
        }
    }

    private void OnDisable()
    {
        StopRecording();
    }

    private void OnDestroy()
    {
        StopRecording();
    }
}
