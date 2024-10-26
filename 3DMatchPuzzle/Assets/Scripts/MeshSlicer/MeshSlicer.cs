using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.XR;
using System.Linq;
using TMPro;

public class MeshSlicer : MonoBehaviour
{
    private GameObject targetMesh;
    [SerializeField] private Material capMaterial;

    private class SlicedMeshInfo
    {
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector3> normals;
        public List<Vector2> uv;
        public List<int> subMeshTriangles;

        public SlicedMeshInfo()
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            normals = new List<Vector3>();
            uv = new List<Vector2>();
            subMeshTriangles = new List<int>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        targetMesh = other.gameObject;
    }

    public void Slice()
    {
        Plane plane = new Plane(targetMesh.transform.InverseTransformDirection(-transform.forward),
                          targetMesh.transform.InverseTransformPoint(transform.position));

        SlicedMeshInfo sideA = new SlicedMeshInfo();
        SlicedMeshInfo sideB = new SlicedMeshInfo();
        SlicedMeshInfo CreatedSideA = new SlicedMeshInfo();

        SlicedMeshInfo CreatedSideB = new SlicedMeshInfo();

        Mesh target = targetMesh.GetComponent<MeshFilter>().sharedMesh;
        int polygonCount = target.triangles.Length / 3;

        bool[] side = { false, false, false };
        for (int i = 0; i < polygonCount; i++)
        {
            int[] vertIndexs = { target.triangles[i * 3] ,
                target.triangles[i * 3 + 1],
                target.triangles[i * 3 + 2] };

            Vector3[] verts = { target.vertices[vertIndexs[0]],
                target.vertices[vertIndexs[1]],
                target.vertices[vertIndexs[2]] };

            Vector3[] normals = { target.normals[vertIndexs[0]],
                target.normals[vertIndexs[1]],
                target.normals[vertIndexs[2]] };

            Vector2[] uvs = { target.uv[vertIndexs[0]],
                target.uv[vertIndexs[1]],
                target.uv[vertIndexs[2]] };

            side[0] = plane.GetSide(verts[0]);
            side[1] = plane.GetSide(verts[1]);
            side[2] = plane.GetSide(verts[2]);

            // 모두 단면의 같은 쪽에 있음
            if (side[0] == side[1] && side[0] == side[2])
            {
                SlicedMeshInfo whichSide = side[0] ? sideA : sideB;

                whichSide.vertices.Add(verts[0]);
                whichSide.vertices.Add(verts[1]);
                whichSide.vertices.Add(verts[2]);

                whichSide.uv.Add(uvs[0]);
                whichSide.uv.Add(uvs[1]);
                whichSide.uv.Add(uvs[2]);

                whichSide.normals.Add(normals[0]);
                whichSide.normals.Add(normals[1]);
                whichSide.normals.Add(normals[2]);

                whichSide.triangles.Add(whichSide.triangles.Count);
                whichSide.triangles.Add(whichSide.triangles.Count);
                whichSide.triangles.Add(whichSide.triangles.Count);
            }
            else
            {
                // 따로 떨어져 있는 버텍스 세팅
                int otherSidePoint = -1,
                    sameSidePoint0 = -1,
                    sameSidePoint1 = -1;

                if (side[0] == side[1]) { otherSidePoint = 2; sameSidePoint0 = 1; sameSidePoint1 = 0; }
                else if (side[0] == side[2]) { otherSidePoint = 1; sameSidePoint0 = 0; sameSidePoint1 = 2; }
                else { otherSidePoint = 0; sameSidePoint0 = 2; sameSidePoint1 = 1; }

                SlicedMeshInfo otherSide, sameSide;
                if (side[otherSidePoint] == true) { otherSide = sideA; sameSide = sideB; }
                else { otherSide = sideB; sameSide = sideA; }

                ///////////////////////////////////////////////////////////////////

                // 새롭게 생성된 버텍스 정보 세팅
                float distFromOtherSide = Mathf.Abs(plane.GetDistanceToPoint(verts[otherSidePoint]));
                float distFromSameSide0 = Mathf.Abs(plane.GetDistanceToPoint(verts[sameSidePoint0]));
                float distFromSameSide1 = Mathf.Abs(plane.GetDistanceToPoint(verts[sameSidePoint1]));

                float other2Point0Ratio = distFromOtherSide / (distFromOtherSide + distFromSameSide0);
                float other2Point1Ratio = distFromOtherSide / (distFromOtherSide + distFromSameSide1);

                Vector3 newPoint0 = Vector3.Lerp(verts[otherSidePoint], verts[sameSidePoint0], other2Point0Ratio);
                Vector3 newPoint1 = Vector3.Lerp(verts[otherSidePoint], verts[sameSidePoint1], other2Point1Ratio);

                Vector3 newNormal0 = Vector3.Lerp(normals[otherSidePoint], normals[sameSidePoint0], other2Point0Ratio);
                Vector3 newNormal1 = Vector3.Lerp(normals[otherSidePoint], normals[sameSidePoint1], other2Point1Ratio);

                Vector2 newUV0 = Vector2.Lerp(uvs[otherSidePoint], uvs[sameSidePoint0], other2Point0Ratio);
                Vector2 newUV1 = Vector2.Lerp(uvs[otherSidePoint], uvs[sameSidePoint1], other2Point1Ratio);

                ///////////////////////////////////////////////////////////////////

                // 생성된 버텍스 이후 빈 공간을 체우기 위해 저장
                SlicedMeshInfo CreatedOtherSide, CreatedSameSide;
                if (side[otherSidePoint] == true)
                {
                    CreatedOtherSide = CreatedSideA;
                    CreatedSameSide = CreatedSideB;
                }
                else
                {
                    CreatedOtherSide = CreatedSideB;
                    CreatedSameSide = CreatedSideA;
                }
                CreatedOtherSide.vertices.Add(newPoint0);
                CreatedOtherSide.vertices.Add(newPoint1);

                CreatedOtherSide.normals.Add(newNormal0);
                CreatedOtherSide.normals.Add(newNormal1);

                CreatedOtherSide.uv.Add(newUV0);
                CreatedOtherSide.uv.Add(newUV1);



                CreatedSameSide.vertices.Add(newPoint1);
                CreatedSameSide.vertices.Add(newPoint0);

                CreatedSameSide.normals.Add(newNormal1);
                CreatedSameSide.normals.Add(newNormal0);

                CreatedSameSide.uv.Add(newUV1);
                CreatedSameSide.uv.Add(newUV0);

                ////////////////////////////////////////////////////////////////////

                // 각 사이드에 추가하기
                otherSide.vertices.Add(verts[otherSidePoint]);
                otherSide.vertices.Add(newPoint1);
                otherSide.vertices.Add(newPoint0);

                otherSide.normals.Add(normals[otherSidePoint]);
                otherSide.normals.Add(newNormal1);
                otherSide.normals.Add(newNormal0);

                otherSide.uv.Add(uvs[otherSidePoint]);
                otherSide.uv.Add(newUV1);
                otherSide.uv.Add(newUV0);

                otherSide.triangles.Add(otherSide.triangles.Count);
                otherSide.triangles.Add(otherSide.triangles.Count);
                otherSide.triangles.Add(otherSide.triangles.Count);



                sameSide.vertices.Add(verts[sameSidePoint1]);
                sameSide.vertices.Add(verts[sameSidePoint0]);
                sameSide.vertices.Add(newPoint0);

                sameSide.normals.Add(normals[sameSidePoint1]);
                sameSide.normals.Add(normals[sameSidePoint0]);
                sameSide.normals.Add(newNormal0);

                sameSide.uv.Add(uvs[sameSidePoint1]);
                sameSide.uv.Add(uvs[sameSidePoint0]);
                sameSide.uv.Add(newUV0);

                sameSide.triangles.Add(sameSide.triangles.Count);
                sameSide.triangles.Add(sameSide.triangles.Count);
                sameSide.triangles.Add(sameSide.triangles.Count);



                sameSide.vertices.Add(verts[sameSidePoint1]);
                sameSide.vertices.Add(newPoint0);
                sameSide.vertices.Add(newPoint1);

                sameSide.normals.Add(normals[sameSidePoint1]);
                sameSide.normals.Add(newNormal0);
                sameSide.normals.Add(newNormal1);

                sameSide.uv.Add(uvs[sameSidePoint1]);
                sameSide.uv.Add(newUV0);
                sameSide.uv.Add(newUV1);

                sameSide.triangles.Add(sameSide.triangles.Count);
                sameSide.triangles.Add(sameSide.triangles.Count);
                sameSide.triangles.Add(sameSide.triangles.Count);
            }
        }

        // 빈 공간 체우기
        Vector3 midPointA = GetMidPoint(ref CreatedSideA);
        Vector2 midUVA = GetUV(ref CreatedSideA);
        CreateTriangles(in CreatedSideA, ref sideA,
            in midPointA, transform.forward);

        Vector3 midPointB = GetMidPoint(ref CreatedSideB);
        Vector2 midUVB = GetUV(ref CreatedSideB);
        CreateTriangles(in CreatedSideB, ref sideB,
            in midPointB, -transform.forward);

        // 각 게임 오브젝트 생성
        GameObject sideAObj, sideBObj;
        CreateNewObj(out sideAObj, out sideBObj);

        Mesh sideAMesh = new Mesh();
        sideAMesh.vertices = sideA.vertices.ToArray();
        sideAMesh.subMeshCount = 2;
        sideAMesh.SetTriangles(sideA.subMeshTriangles, 1);
        sideAMesh.SetTriangles(sideA.triangles, 0);
        sideAMesh.normals = sideA.normals.ToArray();
        sideAMesh.uv = sideA.uv.ToArray();
        sideAObj.GetComponent<MeshFilter>().mesh = sideAMesh;
        sideAObj.GetComponent<MeshCollider>().sharedMesh = sideAMesh;
        sideAObj.GetComponent<MeshRenderer>().materials = new Material[]
            { targetMesh.GetComponent<MeshRenderer>().material, capMaterial};
        sideAObj.transform.position = targetMesh.transform.position;
        sideAObj.transform.rotation = targetMesh.transform.rotation;

        Mesh sideBMesh = new Mesh();
        sideBMesh.vertices = sideB.vertices.ToArray();
        sideBMesh.subMeshCount = 2;
        sideBMesh.SetTriangles(sideB.subMeshTriangles, 1);
        sideBMesh.SetTriangles(sideB.triangles, 0);
        sideBMesh.normals = sideB.normals.ToArray();
        sideBMesh.uv = sideB.uv.ToArray();
        sideBObj.GetComponent<MeshFilter>().mesh = sideBMesh;
        sideBObj.GetComponent<MeshCollider>().sharedMesh = sideBMesh;
        sideBObj.GetComponent<MeshRenderer>().materials = new Material[]
            { targetMesh.GetComponent<MeshRenderer>().material, capMaterial};
        sideBObj.transform.position = targetMesh.transform.position;
        sideBObj.transform.rotation = targetMesh.transform.rotation;

        targetMesh.SetActive(false);
    }

    private void CreateNewObj(out GameObject topObj, out GameObject bottomObj)
    {
        topObj = new GameObject();
        topObj.AddComponent<MeshFilter>();
        topObj.AddComponent<MeshRenderer>();
        topObj.AddComponent<MeshCollider>();

        bottomObj = Instantiate(topObj);
    }

    private void SortVertices(SlicedMeshInfo CreatedPoints)
    {
        List<Vector3> newVertices = new List<Vector3>();
        int verCount = CreatedPoints.vertices.Count;
        for (int i = 0; i < verCount; ++i)
        {
            bool isTherePoint = false;
            for (int j = 0; j < newVertices.Count; ++j)
            {
                if (newVertices[j] == CreatedPoints.vertices[i])
                {
                    isTherePoint = true;
                    break;
                }
            }

            if (isTherePoint)
            {
                continue;
            }

            newVertices.Add(CreatedPoints.vertices[i]);
        }

        CreatedPoints.vertices = newVertices;
    }

    private Vector3 GetMidPoint(ref SlicedMeshInfo CreatedPoints)
    {
        Vector3 resultPoint = Vector3.zero;

        foreach (var point in CreatedPoints.vertices)
        {
            resultPoint += point;
        }
        resultPoint /= CreatedPoints.vertices.Count;

        return resultPoint;
    }

    private Vector2 GetUV(ref SlicedMeshInfo CreatedPoints)
    {
        Vector2 resultUV = Vector2.zero;

        foreach (var uv in CreatedPoints.uv)
        {
            resultUV += uv;
        }
        resultUV /= CreatedPoints.uv.Count;

        return resultUV;
    }

    private void CreateTriangles(in SlicedMeshInfo CreatedPointsInfo,
        ref SlicedMeshInfo sideInfo, in Vector3 midPoint, Vector3 faceNormal)
    {
        int edgeCount = CreatedPointsInfo.vertices.Count / 2;
        int vertStartIndex = sideInfo.vertices.Count;

        for (int i = 0; i < CreatedPointsInfo.vertices.Count; ++i)
        {
            sideInfo.vertices.Add(CreatedPointsInfo.vertices[i]);
            sideInfo.normals.Add(faceNormal);
            sideInfo.uv.Add(new Vector2(0f, 0f));
        }

        sideInfo.vertices.Add(midPoint);
        sideInfo.normals.Add(faceNormal);
        sideInfo.uv.Add(new Vector2(0f, 0f));

        int midPointIndex = sideInfo.vertices.Count - 1;
        for (int i = 0; i < edgeCount - 1; i++)
        {
            int pointIndex0 = i * 2;
            int pointIndex1 = i * 2 + 1;

            sideInfo.subMeshTriangles.Add(midPointIndex);
            sideInfo.subMeshTriangles.Add(vertStartIndex + pointIndex0);
            sideInfo.subMeshTriangles.Add(vertStartIndex + pointIndex1);
        }
    }
}
