using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMT.Gameplay
{
    public enum MaterialEnum
    {
        Wood,
        Metal,
        Tile,
        Grass,
        Concrete
    }

    public class MaterialType : MonoBehaviour
    {
        [SerializeField] MaterialEnum _material;

        public MaterialEnum Material => _material;
    }
}