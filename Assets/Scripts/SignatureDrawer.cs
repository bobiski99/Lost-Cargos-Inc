
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class SignatureDrawer : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private MeshCollider targetCollider;
    [SerializeField] private Transform drawingRoot;
    [SerializeField] private Material signatureMaterial;
    [SerializeField] private float lineWidth = 0.004f;
    [SerializeField] private float surfaceOffset = 0.002f;
    [SerializeField] private float minPointDistance = 0.003f;
    private bool drawingEnabled;
    private bool isDrawing;
    private LineRenderer currentLine;
    private Vector3 lastPoint;
    private readonly List<LineRenderer> createdLines = new List<LineRenderer>();
    public Action OnFirstStroke;
    public void SetDrawingEnabled(bool value)
    {
        drawingEnabled = value;
        isDrawing = false;
        currentLine = null;
    }

    private void Update()
    {
        if (!drawingEnabled)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (TryGetSurfacePoint(out RaycastHit hit))
            {
                StartStroke(hit);
            }
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            if (TryGetSurfacePoint(out RaycastHit hit))
            {
                AddPoint(hit);
            }
            else
            {
                isDrawing = false;
                currentLine = null;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            currentLine = null;
        }
    }

    private bool TryGetSurfacePoint(out RaycastHit hit)
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        return targetCollider.Raycast(ray, out hit, 100f);
    }

    private void StartStroke(RaycastHit hit)
    {
        bool firstStroke = createdLines.Count == 0;
        GameObject lineObject = new GameObject("SignatureStroke");
        lineObject.transform.SetParent(drawingRoot, false);
        currentLine = lineObject.AddComponent<LineRenderer>();
        currentLine.useWorldSpace = false;
        currentLine.positionCount = 0;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.material = signatureMaterial;
        currentLine.numCapVertices = 4;
        currentLine.numCornerVertices = 4;
        currentLine.textureMode = LineTextureMode.Stretch;
        createdLines.Add(currentLine);
        isDrawing = true;
        AddPoint(hit);
        if (firstStroke)
        {
            OnFirstStroke?.Invoke();
        }
    }

    private void AddPoint(RaycastHit hit)
    {
        Vector3 worldPoint = hit.point + hit.normal * surfaceOffset;

        if (currentLine.positionCount > 0)
        {
            float distance = Vector3.Distance(lastPoint,worldPoint);

            if (distance < minPointDistance)
            {
                return;
            }
        }

        Vector3 localPoint = drawingRoot.InverseTransformPoint(worldPoint);
        currentLine.positionCount++;
        currentLine.SetPosition(currentLine.positionCount - 1, localPoint);
        lastPoint = worldPoint;
    }

    public void ClearSignature()
    {
        isDrawing = false;
        currentLine = null;
        foreach (LineRenderer line in createdLines)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }
        createdLines.Clear();
    }

    public bool HasSignature()
    {
        return createdLines.Count > 0;
    }
}
