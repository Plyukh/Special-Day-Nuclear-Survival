using UnityEngine;
using System.Collections;

public enum RoomType
{
    Metal,
    Grass,
    Wood,
    Rock,
    Dirty
}

public class Room : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh;
    public RoomType roomType;
    public bool find;
    public bool light;
    public bool radiactive;

    public int quest = -1;
    public int questPart = -1;

    public CubeObject CubeObject
    {
        get
        {
            return mesh.GetComponent<CubeObject>();
        }
    }

    public bool Mesh()
    {
        if(mesh != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        if (find)
        {
            Find();
        }
    }

    public void Find()
    {
        if(mesh != null)
        {
            mesh.material.color -= new Color32(0, 0, 0, 2);
        }
    }

    public void MeshDisabled()
    {
        mesh.GetComponent<Outline>().enabled = false;
        mesh.GetComponent<BoxCollider>().enabled = false;
    }
}
