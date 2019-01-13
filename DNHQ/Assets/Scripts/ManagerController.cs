using UnityEngine;

/// <summary>
/// An empty gameobject for holding and organizing other gameobjects.
/// PositionLocked prevents object from being moved in the x and y axiis,
///		which would mess with the child gameobjects.
/// </summary>
[RequireComponent(typeof(PositionLocked))]
public class ManagerController : MonoBehaviour
{
}