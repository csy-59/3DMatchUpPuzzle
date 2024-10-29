using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSlicer : SingletonBehaviour<MeshSlicer>
{
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

    public GameObject[] Slice(GameObject _targetMesh, Vector3 _anchorPoint, Vector3 _normalDirection, Material _capMaterial)
    {
        Plane plane = new Plane(_targetMesh.transform.InverseTransformDirection(-_normalDirection),
                          _targetMesh.transform.InverseTransformPoint(_anchorPoint));

        SlicedMeshInfo sideA = new SlicedMeshInfo();
        SlicedMeshInfo sideB = new SlicedMeshInfo();
        SlicedMeshInfo createdSideA = new SlicedMeshInfo();

        SlicedMeshInfo createdSideB = new SlicedMeshInfo();

        Mesh target = _targetMesh.GetComponent<MeshFilter>().sharedMesh;
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
                // Triangle이 잘린 상황
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
                
                // 버텍스 위치 계산
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

                // 생성한 버텍스 저장하기
                SlicedMeshInfo CreatedOtherSide, CreatedSameSide;
                if (side[otherSidePoint] == true)
                {
                    CreatedOtherSide = createdSideA;
                    CreatedSameSide = createdSideB;
                }
                else
                {
                    CreatedOtherSide = createdSideB;
                    CreatedSameSide = createdSideA;
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

                // Triangle 자르기
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

        // 면체우기
        Vector3 midPointA = GetMidPoint(ref createdSideA);
        Vector2 midUVA = GetUV(ref createdSideA);
        CreateTriangles(in createdSideA, ref sideA,
            in midPointA, _normalDirection);

        Vector3 midPointB = GetMidPoint(ref createdSideB);
        Vector2 midUVB = GetUV(ref createdSideB);
        CreateTriangles(in createdSideB, ref sideB,
            in midPointB, -_normalDirection);

        // 오브젝트 생성
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
        sideAObj.GetComponent<MeshCollider>().convex = true;
        sideAObj.GetComponent<MeshRenderer>().materials = new Material[]
            { _targetMesh.GetComponent<MeshRenderer>().material, _capMaterial};
        sideAObj.transform.position = _targetMesh.transform.position;
        sideAObj.transform.rotation = _targetMesh.transform.rotation;

        Mesh sideBMesh = new Mesh();
        sideBMesh.vertices = sideB.vertices.ToArray();
        sideBMesh.subMeshCount = 2;
        sideBMesh.SetTriangles(sideB.subMeshTriangles, 1);
        sideBMesh.SetTriangles(sideB.triangles, 0);
        sideBMesh.normals = sideB.normals.ToArray();
        sideBMesh.uv = sideB.uv.ToArray();
        sideBObj.GetComponent<MeshFilter>().mesh = sideBMesh;
        sideBObj.GetComponent<MeshCollider>().sharedMesh = sideBMesh;
        sideBObj.GetComponent<MeshCollider>().convex = true;
        sideBObj.GetComponent<MeshRenderer>().materials = new Material[]
            { _targetMesh.GetComponent<MeshRenderer>().material, _capMaterial};
        sideBObj.transform.position = _targetMesh.transform.position;
        sideBObj.transform.rotation = _targetMesh.transform.rotation;

        _targetMesh.SetActive(false);

        return new GameObject[] { sideAObj, sideBObj };
    }

    private void CreateNewObj(out GameObject _topObj, out GameObject _bottomObj)
    {
        _topObj = new GameObject();
        _topObj.AddComponent<MeshFilter>();
        _topObj.AddComponent<MeshRenderer>();
        _topObj.AddComponent<MeshCollider>();

        _bottomObj = Instantiate(_topObj);
    }

    private Vector3 GetMidPoint(ref SlicedMeshInfo _createdPoints)
    {
        Vector3 resultPoint = Vector3.zero;

        foreach (var point in _createdPoints.vertices)
        {
            resultPoint += point;
        }
        resultPoint /= _createdPoints.vertices.Count;

        return resultPoint;
    }

    private Vector2 GetUV(ref SlicedMeshInfo _createdPoints)
    {
        Vector2 resultUV = Vector2.zero;

        foreach (var uv in _createdPoints.uv)
        {
            resultUV += uv;
        }
        resultUV /= _createdPoints.uv.Count;

        return resultUV;
    }

    private void CreateTriangles(in SlicedMeshInfo _createdPointsInfo,
        ref SlicedMeshInfo _sideInfo, in Vector3 _midPoint, Vector3 _faceNormal)
    {
        int edgeCount = _createdPointsInfo.vertices.Count / 2;
        int vertStartIndex = _sideInfo.vertices.Count;

        for (int i = 0; i < _createdPointsInfo.vertices.Count; ++i)
        {
            _sideInfo.vertices.Add(_createdPointsInfo.vertices[i]);
            _sideInfo.normals.Add(_createdPointsInfo.normals[i]);
            _sideInfo.uv.Add(_createdPointsInfo.uv[i]);
        }

        _sideInfo.vertices.Add(_midPoint);
        _sideInfo.normals.Add(_faceNormal);
        _sideInfo.uv.Add(new Vector2(0f, 0f));

        int midPointIndex = _sideInfo.vertices.Count - 1;
        for (int i = 0; i < edgeCount - 1; i++)
        {
            int pointIndex0 = i * 2;
            int pointIndex1 = i * 2 + 1;

            _sideInfo.subMeshTriangles.Add(midPointIndex);
            _sideInfo.subMeshTriangles.Add(vertStartIndex + pointIndex0);
            _sideInfo.subMeshTriangles.Add(vertStartIndex + pointIndex1);
        }
    }
}
